using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscPokerusVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data) => Verify(data, data.Entity);

    internal void Verify(LegalityAnalysis data, PKM pk)
    {
        if (pk.Format == 1) // not stored in Gen1 format
            return;

        if (pk.Format == 3 && !ParseSettings.AllowGBACrossTransferRSE(pk))
        {
            VerifyNone(data, pk);
            return;
        }

        var strain = pk.PokerusStrain;
        var days = pk.PokerusDays;
        var enc = data.Info.EncounterMatch;
        if (!Pokerus.IsStrainValid(pk, enc, strain, days))
            data.AddLine(GetInvalid(PokerusStrainUnobtainable_0, (ushort)strain));
        if (!Pokerus.IsDurationValid(strain, days, out var max))
            data.AddLine(GetInvalid(PokerusDaysLEQ_0, (ushort)max));
    }

    private void VerifyNone(LegalityAnalysis data, PKM pk)
    {
        var strain = pk.PokerusStrain;
        var days = pk.PokerusDays;
        if (strain != 0)
            data.AddLine(GetInvalid(PokerusStrainUnobtainable_0, (ushort)strain));
        if (days != 0)
            data.AddLine(GetInvalid(PokerusDaysLEQ_0, 0));
    }
}
