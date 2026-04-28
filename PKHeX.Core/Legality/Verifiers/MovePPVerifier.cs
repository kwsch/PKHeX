using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.MoveHealState;

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
                var value = ups[i];
                if (value != 0)
                    data.AddLine(GetInvalid(MovePPUpsTooHigh_01, (ushort)(i + 1), (ushort)value));
            }
        }
        else // Check specific move indexes
        {
            for (int i = 0; i < ups.Length; i++)
            {
                var value = ups[i];
                if (!Legal.IsPPUpAvailable(moves[i]) && value != 0)
                    data.AddLine(GetInvalid(MovePPUpsTooHigh_01, (ushort)(i + 1), (ushort)value));
            }
        }

        var allowedStates = GetPermittedStatePP(data, pk);

        for (int i = 0; i < pp.Length; i++)
        {
            // Sometimes the PP count will exceed (such as VC=>Bank); just flag it as invalid so the user knows they need to heal them.
            // Technically that case is legal (game bug) only if they never move it from the box, but we want to inform the user.
            var healed = pk.GetMovePP(moves[i], ups[i]);
            var value = pp[i];
            if (value > healed)
            {
                data.AddLine(GetInvalid(MovePPTooHigh_01, (ushort)(i + 1), (ushort)value));
                continue;
            }

            if (allowedStates == Any)
                continue;
            if (value == healed && (allowedStates == OnlyHealed || allowedStates.HasFlag(AllowHealed)))
                continue;
            if (value == 0 && (allowedStates == Only0 || allowedStates.HasFlag(Allow0)))
                continue;

            // Not Valid. Add a flag.
            var (message, expect) = allowedStates switch
            {
                OnlyHealed => (MovePPExpectHealed_01, healed),
                Only0 => (MovePPTooHigh_01, 0),
                _ => (MovePPExpectHealed_01, healed), // just pick one of the expected states; heal is safe default.
            };
            data.AddLine(GetInvalid(message, (ushort)(i + 1), (ushort)expect));
        }
    }

    private static MoveHealState GetPermittedStatePP(LegalityAnalysis data, PKM pk)
    {
        if (Legal.IsPPUnused(pk))
        {
            if (pk is PA9 pa9 && HomeQuirks.HasEnteredReachingZA(pa9, data.EncounterOriginal.Context))
                return AllowHealedOr0; // HOME sets 0 PP for all moves. Healing / reassigning moves in ZA will heal individual indexes.

            return OnlyHealed;
        }

        if (data.IsStoredSlot(StorageSlotType.Party))
            return Any;

        return data.SlotOrigin switch
        {
            StorageSlotType.Box or StorageSlotType.GTS or StorageSlotType.BattleBox => GetIsStoredHealed(pk, data.EncounterOriginal),
            _ => Any, // Deposited slots pass through party.
        };
    }

    /// <summary>
    /// Checks if the format is expected to have the Pokémon healed to full PP.
    /// </summary>
    private static MoveHealState GetIsStoredHealed(PKM pk, IEncounterTemplate enc) => pk switch
    {
        // Boxes accessible from anywhere; retain HP and PP
        PK9 => Any,
        PK8 or PA8 or PB8 => Any,
        PB7 => Any,
        // Don't heal PP when deposited
        PK1 or PK2 => Any,
        PK6 or PK7 => Any,

        // Do heal after capture/deposit
        SK2 => OnlyHealed,
        CK3 or XK3 => OnlyHealed,
        PK4 or RK4 or BK4 or PK5 => OnlyHealed,

        // Check if the encounter has left the boxes after being acquired by the player
        // only reachable by PK3?
        _ => HasLeftBoxAfterAcquisition(pk, enc),
    };

    private static MoveHealState HasLeftBoxAfterAcquisition(PKM pk, IEncounterTemplate enc)
    {
        if (enc.Context != pk.Context)
            return OnlyHealed; // Different context, assume it was traded and thus is not a wild->box
        if (pk.EVTotal != 0)
            return OnlyHealed; // EVs are not possible direct from wild encounters

        if (!Experience.IsAtLevelThreshold(pk.EXP, pk.PersonalInfo.EXPGrowth, out var current))
            return OnlyHealed; // gained experience

        // Only scenario is if it was leveled up AND matches that exp threshold
        if (pk.Format >= 3) // has met level
            return pk.MetLevel != current ? OnlyHealed : Any;
        return !enc.IsLevelWithinRange(current) ? OnlyHealed : Any;
    }
}

[Flags]
public enum MoveHealState
{
    None, // Invalid result.

    // Intermixed states for individual move indexes:
    Allow0 = 1 << 0, // A zero PP value is allowed.
    AllowHealed = 1 << 1, // Expect PP to be fully healed.
    AllowUsed = 1 << 2, // Any value in-between is allowed.

    AllowHealedOr0 = Allow0 | AllowHealed, // Expect PP to be either fully healed or 0.

    // Overall states for all move indexes:
    OnlyHealed = 1 << 3, // Expect PP of all indexes to be fully healed.
    Only0 = 1 << 4, // Expect PP of all indexes to be 0.

    Any = Allow0 | AllowHealed | AllowUsed, // No expectation on PP values.
}
