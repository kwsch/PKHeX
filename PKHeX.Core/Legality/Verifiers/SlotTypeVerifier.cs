using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.StorageSlotType;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed class SlotTypeVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.SlotType;

    public override void Verify(LegalityAnalysis data)
    {
        var source = data.SlotOrigin;
        if (source == 0)
            return; // not provided, ignore

        var pk = data.Entity;
        if (pk.IsEgg)
        {
            if (!IsSourceValidEgg(pk, source))
                data.AddLine(GetInvalid(LStoredSourceEgg));
        }
        else
        {
            if (!IsSourceValid(pk, source))
                data.AddLine(GetInvalid(string.Format(LStoredSourceInvalid_0, source)));
        }
    }

    public static bool IsSourceValid(PKM pk, StorageSlotType source) => source switch
    {
        FusedKyurem => pk.Species is (int)Reshiram or (int)Zekrom,
        FusedNecrozmaS => pk.Species is (int)Solgaleo,
        FusedNecrozmaM => pk.Species is (int)Lunala,
        FusedCalyrex => pk.Species is (int)Glastrier or (int)Spectrier,

        Ride => pk.Species is (int)Koraidon or (int)Miraidon
                                && pk is PK9 {FormArgument: EncounterStatic9.RideLegendFormArg },
        _ => true,
    };

    public static bool IsSourceValidEgg(PKM pk, StorageSlotType source) => source switch
    {
        // Eggs should normally only be in Box or Party.
        Box or Party => true,
        Resort => true, // Poké Pelago can incubate eggs
        Daycare when pk.Format == 2 => true, // ignore the "current egg" slot
        _ => false,
    };
}
