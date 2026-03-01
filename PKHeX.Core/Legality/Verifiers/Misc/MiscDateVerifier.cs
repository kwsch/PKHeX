using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscDateVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data) => Verify(data, data.Entity);

    internal void Verify(LegalityAnalysis data, PKM pk)
    {
        if (pk.Format <= 3)
            return; // Gen1-3 do not have date values.

        var expect0 = pk is PB8 ? Locations.Default8bNone : 0;
        if (pk.MetLocation != expect0)
        {
            if (pk.MetDate is null)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidMet));
        }
        else if (data.EncounterMatch is not EncounterInvalid)
        {
            if (pk.MetMonth != 0 || pk.MetDay != 0 || pk.MetYear != 0)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidMet));
        }

        if (pk.EggLocation != expect0)
        {
            if (pk.EggMetDate is null)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidEgg));
        }
        else
        {
            if (pk.EggMonth != 0 || pk.EggDay != 0 || pk.EggYear != 0)
                data.AddLine(GetInvalid(Memory, DateCalendarInvalidEgg));
        }
    }
}
