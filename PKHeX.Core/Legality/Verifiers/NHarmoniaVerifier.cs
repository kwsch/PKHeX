using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PK5.NSparkle"/> data.
/// </summary>
public sealed class NHarmoniaVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        bool checksRequired = data.EncounterMatch is EncounterStatic5N;
        if (pk is PK5 pk5)
        {
            bool has = pk5.NSparkle;
            if (checksRequired && !has)
                data.AddLine(GetInvalid(LG5SparkleRequired, CheckIdentifier.Fateful));
            if (!checksRequired && has)
                data.AddLine(GetInvalid(LG5SparkleInvalid, CheckIdentifier.Fateful));
        }

        if (!checksRequired)
            return;

        if (pk.OT_Gender != 0)
            data.AddLine(GetInvalid(LG5OTGenderN, CheckIdentifier.Trainer));
        if (!VerifyNsPKMIVsValid(pk))
            data.AddLine(GetInvalid(LG5IVAll30, CheckIdentifier.IVs));
        if (!VerifyNsPKMOTValid(pk))
            data.AddLine(GetInvalid(LG5ID_N, CheckIdentifier.Trainer));
        if (pk.IsShiny)
            data.AddLine(GetInvalid(LG5PIDShinyN, CheckIdentifier.Shiny));
    }

    private static bool VerifyNsPKMIVsValid(PKM pk)
    {
        // All are 30.
        return pk.IV_HP == 30 && pk.IV_ATK == 30 && pk.IV_DEF == 30 && pk.IV_SPA == 30 && pk.IV_SPD == 30 && pk.IV_SPE == 30;
    }

    private static bool VerifyNsPKMOTValid(PKM pk)
    {
        if (pk.TID != 00002 || pk.SID != 00000)
            return false;
        var ot = pk.OT_Name;
        if (ot.Length != 1)
            return false;
        var c = EncounterStatic5N.GetOT(pk.Language);
        return c == ot;
    }
}
