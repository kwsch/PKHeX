namespace PKHeX.Core;

internal static class Encounters1GBEra
{
    private static readonly IndividualValueSet Yoshira = new(5, 10, 1, 12, 5, 5);
    private static readonly string[] YoshiOT = ["YOSHIRA", "YOSHIRB", "YOSHIBA", "YOSHIBB"];
    private static readonly string[] TourOT = ["LINKE", "LINKW", "LUIGE", "LUIGW", "LUIGIC", "YOSHIC"];
    private static readonly string[] StadiumOT_Int = ["STADIUM", "STADE", "STADIO", "ESTADIO"];
    private const string StadiumOT_JPN = "スタジアム";

    internal static readonly EncounterGift1[] Gifts =
    [
        // Stadium 1 (International)
        new(001, 05, GameVersion.Stadium) {Moves = new(033, 045), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Bulbasaur
        new(004, 05, GameVersion.Stadium) {Moves = new(010, 043), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Charmander
        new(007, 05, GameVersion.Stadium) {Moves = new(033, 045), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Squirtle
        new(106, 20, GameVersion.Stadium) {Moves = new(024, 096), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Hitmonlee
        new(107, 20, GameVersion.Stadium) {Moves = new(004, 097), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Hitmonchan
        new(133, 25, GameVersion.Stadium) {Moves = new(033, 039), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Eevee
        new(138, 20, GameVersion.Stadium) {Moves = new(055, 110), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Omanyte
        new(140, 20, GameVersion.Stadium) {Moves = new(010, 106), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Kabuto
        new(054, 15, GameVersion.Stadium) {Moves = new(133, 010), TID16 = 2000, OT_Names = StadiumOT_Int, Language = EncounterGBLanguage.International}, // Psyduck (Amnesia)

        // Stadium 2 (Japan)
        new(001, 05, GameVersion.Stadium) {Moves = new(033, 045), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Bulbasaur
        new(004, 05, GameVersion.Stadium) {Moves = new(010, 043), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Charmander
        new(007, 05, GameVersion.Stadium) {Moves = new(033, 045), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Squirtle
        new(106, 20, GameVersion.Stadium) {Moves = new(024, 096), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Hitmonlee
        new(107, 20, GameVersion.Stadium) {Moves = new(004, 097), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Hitmonchan
        new(133, 25, GameVersion.Stadium) {Moves = new(033, 039), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Eevee
        new(138, 20, GameVersion.Stadium) {Moves = new(055, 110), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Omanyte
        new(140, 20, GameVersion.Stadium) {Moves = new(010, 106), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Kabuto
        new(054, 15, GameVersion.Stadium) {Moves = new(133, 010), TID16 = 1999, OT_Name = StadiumOT_JPN}, // Psyduck (Amnesia)

        new(151, 5, GameVersion.RB) {IVs = Yoshira, OT_Names = YoshiOT, Language = EncounterGBLanguage.International }, // Yoshira Mew Events
        new(151, 5, GameVersion.RB) {IVs = Yoshira, OT_Names = TourOT, Language = EncounterGBLanguage.International }, // Pokémon 2000 Stadium Tour Mew
    ];
}
