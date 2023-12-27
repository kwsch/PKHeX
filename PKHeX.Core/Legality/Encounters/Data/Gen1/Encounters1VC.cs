namespace PKHeX.Core;

internal static class Encounters1VC
{
    private static readonly IndividualValueSet Flawless15 = new(15, 15, 15, 15, 15, 15);

    internal static readonly EncounterGift1[] Gifts =
    [
        // Event Mew
        new(151, 5, GameVersion.RBY) { IVs = Flawless15, TID16 = 22796, OT_Name = "GF", Language = EncounterGBLanguage.International },
        new(151, 5, GameVersion.RBY) { IVs = Flawless15, TID16 = 22796, OT_Name = "ゲーフリ" },
    ];
}
