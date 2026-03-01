using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPK5 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PK5 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PK5 pk)
    {
        var enc = data.EncounterOriginal;
        VerifyFame(data, pk, enc);
        VerifyNSparkle(data, pk, enc);
    }

    private static void VerifyFame(LegalityAnalysis data, PK5 pk, IEncounterTemplate enc)
    {
        var fame = pk.PokeStarFame;
        if (fame == 0)
            return;

        // Cannot participate in Pokestar Studios as Egg
        if (pk.IsEgg)
            data.AddLine(GetInvalid(Egg, EggShinyPokeStar));
        else if (enc.Species == (ushort)Species.Ditto) // Having Transform is not allowed; Smeargle can change moves.
            data.AddLine(GetInvalid(Misc, G5PokeStarMustBeZero));
        else if (fame % 25 is not (0 or 5)) // all values via +25, -50 for [0,255]
            data.AddLine(GetInvalid(Misc, G5PokeStarImpossibleValue));
    }

    private static void VerifyNSparkle(LegalityAnalysis data, PK5 pk, IEncounterTemplate enc)
    {
        // Ensure NSparkle is only present on N's encounters.
        if (enc is EncounterStatic5N)
        {
            if (!pk.NSparkle)
                data.AddLine(GetInvalid(Fateful, G5SparkleRequired));
        }
        else
        {
            if (pk.NSparkle)
                data.AddLine(GetInvalid(Fateful, G5SparkleInvalid));
        }
    }
}
