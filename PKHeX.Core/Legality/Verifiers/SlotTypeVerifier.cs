using static PKHeX.Core.LegalityCheckStrings;

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
        StorageSlotType.FusedKyurem => pk.Species is (int)Species.Reshiram or (int)Species.Zekrom,
        StorageSlotType.FusedNecrozmaS => pk.Species is (int)Species.Solgaleo,
        StorageSlotType.FusedNecrozmaM => pk.Species is (int)Species.Lunala,
        StorageSlotType.FusedCalyrex => pk.Species is (int)Species.Glastrier or (int)Species.Spectrier,

        StorageSlotType.Ride => pk.Species is (int)Species.Koraidon or (int)Species.Miraidon
                                && pk is PK9 {FormArgument: EncounterStatic9.RideLegendFormArg },
        _ => true,
    };

    public static bool IsSourceValidEgg(PKM pk, StorageSlotType source) => source switch
    {
        // Eggs should normally only be in Box or Party.
        StorageSlotType.Box or StorageSlotType.Party => true,
        StorageSlotType.Resort => true, // Poké Pelago can incubate eggs
        StorageSlotType.Daycare when pk.Format == 2 => true, // ignore the "current egg" slot
        _ => false,
    };
}
