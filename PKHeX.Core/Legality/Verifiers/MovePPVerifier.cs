using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

public sealed class MovePPVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.CurrentMove;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity.IsEgg)
        {
            VerifyEgg(data);
            return;
        }

        VerifyEntity(data);
    }

    private static void VerifyEgg(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Move1_PPUps != 0 || pk.Move2_PPUps != 0 || pk.Move3_PPUps != 0 || pk.Move4_PPUps != 0)
            data.AddLine(GetInvalid(CheckIdentifier.Egg, EggPPUp));
        if (!IsZeroMovePP(pk))
            data.AddLine(GetInvalid(CheckIdentifier.Egg, EggPP));
    }

    private static bool IsZeroMovePP(PKM pk)
    {
        if (pk.Move1_PP != pk.GetBasePP(pk.Move1))
            return false;
        if (pk.Move2_PP != pk.GetBasePP(pk.Move2))
            return false;
        if (pk.Move3_PP != pk.GetBasePP(pk.Move3))
            return false;
        if (pk.Move4_PP != pk.GetBasePP(pk.Move4))
            return false;
        return true;
    }

    private void VerifyEntity(LegalityAnalysis data)
    {
        var pk = data.Entity;
        ReadOnlySpan<int> ups = [pk.Move1_PPUps, pk.Move2_PPUps, pk.Move3_PPUps, pk.Move4_PPUps];
        ReadOnlySpan<ushort> moves = [pk.Move1, pk.Move2, pk.Move3, pk.Move4];
        ReadOnlySpan<int> pp = [pk.Move1_PP, pk.Move2_PP, pk.Move3_PP, pk.Move4_PP];

        if (!Legal.IsPPUpAvailable(pk)) // No PP Ups for format
        {
            for (int i = 0; i < ups.Length; i++)
            {
                if (ups[i] != 0)
                    data.AddLine(GetInvalid(MovePPUpsTooHigh_0, (ushort)(i + 1)));
            }
        }
        else // Check specific move indexes
        {
            for (int i = 0; i < ups.Length; i++)
            {
                if (!Legal.IsPPUpAvailable(moves[i]) && ups[i] != 0)
                    data.AddLine(GetInvalid(MovePPUpsTooHigh_0, (ushort)(i + 1)));
            }
        }

        var expectHeal = Legal.IsPPUnused(pk) || IsPPHealed(data, pk);
        for (int i = 0; i < pp.Length; i++)
        {
            var expect = pk.GetMovePP(moves[i], ups[i]);
            if (pp[i] > expect)
                data.AddLine(GetInvalid(MovePPTooHigh_0, (ushort)(i + 1)));
            else if (expectHeal && pp[i] != expect)
                data.AddLine(GetInvalid(MovePPExpectHealed_0, (ushort)(i + 1)));
        }
    }

    private static bool IsPPHealed(LegalityAnalysis data, PKM pk)
    {
        if (data.IsStoredSlot(StorageSlotType.Party))
            return false;

        return data.SlotOrigin switch
        {
            StorageSlotType.Box or StorageSlotType.GTS or StorageSlotType.BattleBox => GetIsStoredHealed(pk, data.EncounterOriginal),
            _ => false, // Deposited slots pass through party.
        };
    }

    /// <summary>
    /// Checks if the format is expected to have the Pokémon healed to full PP.
    /// </summary>
    private static bool GetIsStoredHealed(PKM pk, IEncounterTemplate enc) => pk switch
    {
        // Boxes accessible from anywhere; retain HP and PP
        PK9 => false,
        PK8 or PA8 or PB8 => false,
        PB7 => false,
        // Don't heal PP when deposited
        PK1 or PK2 => false,
        PK6 or PK7 => false,

        // Do heal after capture/deposit
        SK2 => true,
        CK3 or XK3 => true,
        PK4 or RK4 or BK4 or PK5 => true,

        // Check if the encounter has left the boxes after being acquired by the player
        // only reachable by PK3?
        _ => HasLeftBoxAfterAcquisition(pk, enc),
    };

    private static bool HasLeftBoxAfterAcquisition(PKM pk, IEncounterTemplate enc)
    {
        if (enc.Context != pk.Context)
            return true; // Different context, assume it was traded and thus is not a wild->box
        if (pk.EVTotal != 0)
            return true; // EVs are not possible direct from wild encounters

        if (!Experience.IsAtLevelThreshold(pk.EXP, pk.PersonalInfo.EXPGrowth, out var current))
            return true; // gained experience

        // Only scenario is if it was leveled up AND matches that exp threshold
        if (pk.Format >= 3) // has met level
            return pk.MetLevel != current;
        return !enc.IsLevelWithinRange(current);
    }
}
