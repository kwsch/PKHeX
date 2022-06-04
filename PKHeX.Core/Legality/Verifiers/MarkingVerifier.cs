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
        var pkm = data.pkm;
        VerifyFavoriteMark(data, pkm);
        VerifyMarkValue(data, pkm);
    }

    private void VerifyFavoriteMark(LegalityAnalysis data, PKM pkm)
    {
        // Can only be toggled on in LGP/E, and is retained via transfer to HOME and into other games.
        if (pkm is IFavorite { Favorite: true } && !pkm.GG)
            data.AddLine(GetInvalid(LFavoriteMarkingUnavailable));
    }

    private void VerifyMarkValue(LegalityAnalysis data, PKM pkm)
    {
        var mv = pkm.MarkValue;
        if (mv == 0)
            return;

        // Eggs can have markings applied.
        //if (pkm.IsEgg)
        //{
        //    data.AddLine(GetInvalid(LMarkValueShouldBeZero));
        //    return;
        //}

        switch (pkm.Format)
        {
            case <= 2:
                return;
            case <= 6:
                VerifyMarkValueSingle(data, pkm, mv);
                return;
            default:
                VerifyMarkValueDual(data, pkm, mv);
                return;
        }
    }

    private const int Single4 = 0b_1111;
    private const int Single6 = 0b_111111;
    private const int Dual6 = 0b_1111_1111_1111;

    private void VerifyMarkValueDual(LegalityAnalysis data, PKM pkm, int mv)
    {
        if (mv > Dual6)
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));

        var count = pkm.MarkingCount;
        for (int i = 0; i < count; i++)
        {
            var value = pkm.GetMarking(i);
            if (value is not (0 or 1 or 2))
                data.AddLine(GetInvalid(string.Format(LMarkValueOutOfRange_0, i)));
        }
    }

    private void VerifyMarkValueSingle(LegalityAnalysis data, PKM pkm, int mv)
    {
        if (!IsMarkValueValid3456(pkm, mv))
            data.AddLine(GetInvalid(LMarkValueUnusedBitsPresent));
    }

    private static bool IsMarkValueValid3456(PKM pkm, int value)
    {
        var max = pkm.Format is 3 ? Single4 : Single6;
        return value <= max;
    }
}
