using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.MarkValue"/>.
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
        if (pk is IFavorite { Favorite: true } && !pk.GG)
            data.AddLine(GetInvalid(LFavoriteMarkingUnavailable));
    }

    private void VerifyMarkValue(LegalityAnalysis data, PKM pk)
    {
        var mv = pk.MarkValue;
        if (mv == 0)
            return;

        // Eggs can have markings applied.
        //if (pk.IsEgg)
        //{
        //    data.AddLine(GetInvalid(LMarkValueShouldBeZero));
        //    return;
        //}

        switch (pk.Format)
        {
            case <= 2:
                return;
            case <= 6:
                VerifyMarkValueSingle(data, pk, mv);
                return;
            default:
                VerifyMarkValueDual(data, pk, mv);
                return;
        }
    }

    private const int Single4 = 0b_1111;
    private const int Single6 = 0b_111111;
    private const int Dual6 = 0b_1111_1111_1111;

    private void VerifyMarkValueDual(LegalityAnalysis data, PKM pk, int mv)
    {
        if (mv > Dual6)
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));

        var count = pk.MarkingCount;
        for (int i = 0; i < count; i++)
        {
            var value = pk.GetMarking(i);
            if (value is not (0 or 1 or 2))
                data.AddLine(GetInvalid(string.Format(LMarkValueOutOfRange_0, i)));
        }
    }

    private void VerifyMarkValueSingle(LegalityAnalysis data, PKM pk, int mv)
    {
        if (!IsMarkValueValid3456(pk, mv))
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));
    }

    private static bool IsMarkValueValid3456(PKM pk, int value)
    {
        var max = pk.Format is 3 ? Single4 : Single6;
        return value <= max;
    }
}
