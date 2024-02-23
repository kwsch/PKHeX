using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="IAppliedMarkings"/>.
/// </summary>
public sealed class MarkingVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Marking;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        VerifyFavoriteMark(data, pk);
        VerifyMarkValue(data, pk);
    }

    private void VerifyFavoriteMark(LegalityAnalysis data, PKM pk)
    {
        // Can only be toggled on in LGP/E, and is retained via transfer to HOME and into other games.
        if (pk is IFavorite { IsFavorite: true } && !data.Info.EvoChainsAllGens.HasVisitedLGPE)
            data.AddLine(GetInvalid(LFavoriteMarkingUnavailable));
    }

    private void VerifyMarkValue(LegalityAnalysis data, PKM pk)
    {
        // Eggs can have markings applied.
        switch (pk)
        {
            case IAppliedMarkings3 m4:
                VerifyMarkValueSingle(data, m4, m4.MarkingValue);
                return;
            case IAppliedMarkings7 m7:
                VerifyMarkValueDual(data, m7, m7.MarkingValue);
                return;
        }
    }

    private const int Single4 = 0b_1111;
    private const int Single6 = 0b_111111;
    private const int Dual6 = 0b_1111_1111_1111;

    private void VerifyMarkValueDual(LegalityAnalysis data, IAppliedMarkings7 pk, ushort mv)
    {
        if (mv == 0)
            return;
        if (mv > Dual6)
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));

        var count = pk.MarkingCount;
        for (int i = 0; i < count; i++)
        {
            var value = pk.GetMarking(i);
            if (value is not (0 or MarkingColor.Blue or MarkingColor.Pink))
                data.AddLine(GetInvalid(string.Format(LMarkValueOutOfRange_0, i)));
        }
    }

    private void VerifyMarkValueSingle(LegalityAnalysis data, IAppliedMarkings3 pk, byte mv)
    {
        if (mv == 0)
            return;
        if (!IsMarkValueValid3456(pk, mv))
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));
    }

    private static bool IsMarkValueValid3456(IAppliedMarkings3 pk, int value)
    {
        var max = pk is IAppliedMarkings4 ? Single6 : Single4;
        return value <= max;
    }
}
