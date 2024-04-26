using System;
using static PKHeX.Core.LegalityCheckStrings;

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

    private void VerifyEgg(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Move1_PPUps != 0 || pk.Move2_PPUps != 0 || pk.Move3_PPUps != 0 || pk.Move4_PPUps != 0)
            data.AddLine(GetInvalid(LEggPPUp, CheckIdentifier.Egg));
        if (!IsZeroMovePP(pk))
            data.AddLine(GetInvalid(LEggPP, CheckIdentifier.Egg));
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
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, i + 1)));
            }
        }
        else // Check specific move indexes
        {
            for (int i = 0; i < ups.Length; i++)
            {
                if (!Legal.IsPPUpAvailable(moves[i]) && ups[i] != 0)
                    data.AddLine(GetInvalid(string.Format(LMovePPUpsTooHigh_0, i + 1)));
            }
        }

        for (int i = 0; i < pp.Length; i++)
        {
            if (pp[i] > pk.GetMovePP(moves[i], ups[i]))
                data.AddLine(GetInvalid(string.Format(LMovePPTooHigh_0, i + 1)));
        }
    }
}
