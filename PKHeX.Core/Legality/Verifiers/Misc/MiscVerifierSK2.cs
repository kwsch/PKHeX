using System;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierSK2 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is SK2 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, SK2 pk)
    {
        CheckMovesetValidInStadium(data, pk);
    }

    private static void CheckMovesetValidInStadium(LegalityAnalysis data, SK2 sk2)
    {
        // Stadium has hardcoded move compatibility tables.
        // The tables do not consider event specific moves (even if the encounter allows it).
        // For it to be able to be used in Stadium, all moves must be within the compatibility tables.
        Span<ushort> moves = stackalloc ushort[4];
        sk2.GetMoves(moves);
        Span<bool> flaggedIndex = stackalloc bool[4];

        if (!CheckMoves(sk2, moves, flaggedIndex))
            UpdateMoveParse(data, flaggedIndex);
    }

    private static bool CheckMoves(SK2 sk2, Span<ushort> moves, Span<bool> flaggedIndex)
    {
        if (sk2.Species is (ushort)Species.Smeargle)
        {
            // Smeargle checks only if the move can be sketched (basically, only flag Baton Pass).
            // Any other un-sketchable move is already flagged.
            return LearnsetStadium.ValidateSmeargle(moves, flaggedIndex);
        }

        var learn = LearnSource2Stadium.Instance.GetLearnsetStadium(sk2.Species, sk2.Form);
        return learn.Validate(moves, sk2.CurrentLevel, flaggedIndex);

    }

    private static void UpdateMoveParse(LegalityAnalysis data, Span<bool> flaggedIndex)
    {
        var parse = data.Info.Moves;
        for (int i = 0; i < flaggedIndex.Length; i++)
        {
            if (!flaggedIndex[i])
                continue;

            // Mark as invalid, only if it's not already flagged (more detailed?).
            ref var m = ref parse[i];
            if (!m.Valid)
                continue;
            m = m with { Info = m.Info with { Method = LearnMethod.Unobtainable, Environment = LearnEnvironment.Stadium2 } };
        }
    }
}
