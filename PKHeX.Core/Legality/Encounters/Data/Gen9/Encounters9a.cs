using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;

namespace PKHeX.Core;

internal static class Encounters9a
{
    internal static readonly EncounterArea9a[] Slots = EncounterArea9a.GetAreas(Get16("za", "za"u8));

    internal static readonly EncounterGift9a[] Gifts =
    [
        // Z-A Starters
        new(0152,0,05,128) { Location = 30026, FlawlessIVCount = 3, Moves = new(033,039,000,000) }, // Chikorita (test_encount_init_poke_0)
        new(0498,0,05,128) { Location = 30026, FlawlessIVCount = 3, Moves = new(033,039,000,000) }, // Tepig (test_encount_init_poke_1)
        new(0158,0,05,128) { Location = 30026, FlawlessIVCount = 3, Moves = new(033,043,000,000) }, // Totodile (test_encount_init_poke_2)

        // Kanto Starters
        new(0001,0,05,000) { Location = 30027, FlawlessIVCount = 3 }, // Bulbasaur (sub_addpoke_fushigidane)
        new(0004,0,05,000) { Location = 30027, FlawlessIVCount = 3 }, // Charmander (sub_addpoke_hitokage)
        new(0007,0,05,000) { Location = 30027, FlawlessIVCount = 3 }, // Squirtle (sub_addpoke_zenigame)

        // Kalos Starters
        new(0650,0,14,128) { Location = 30030, Gender = 1, Nature = Nature.Impish, IVs = new(31,31,31,15,15,15) }, // Chespin (sub_addpoke_harimaron)
        new(0653,0,14,128) { Location = 30031, Gender = 1, Nature = Nature.Lonely, IVs = new(15,15,15,31,31,31) }, // Fennekin (sub_addpoke_fokko)
        new(0656,0,14,128) { Location = 00069, Gender = 0, Nature = Nature.Sassy,  IVs = new(15,31,15,31,31,15) }, // Froakie (sub_addpoke_keromatsu)

        // Side Missions
        new(0618,1,38,128) { Location = 30028, Gender = 0, FlawlessIVCount = 3, Moves = new(319,334,089,340), Trainer = TrainerGift9a.Stunfisk }, // Stunfisk-1 (sub_addpoke_gmaggyo)
        new(0665,8,09,128) { Location = 30029, Gender = 0, Nature = Nature.Naive, FlawlessIVCount = 3 }, // Spewpa-8 (sub_addpoke_kofuurai)

        // Eternal Flower Floette
        new(0670,5,72,128) { Location = 30026, Gender = 1, Nature = Nature.Modest, FlawlessIVCount = 3, Moves = new(617,412,202,235), Trainer = TrainerGift9a.Floette }, // Floette-5 (addpoke_floette_eien)

        // Korrina's Lucario
        new(0448,0,50,128) { Location = 30026, Gender = 0, Nature = Nature.Hardy, IVs = new(31,20,31,20,20,31), Moves = new(396,418,249,182), Trainer = TrainerGift9a.Lucario }, // Lucario (sub_addpoke_rukario)

        // Rogue Mega Absol
        new(0359,0,30,128) { Location = 30026, FlawlessIVCount = 3, Moves = new(400,163,098,014) }, // Absol (vsmega_init_abusoru)

        // Fossils (Tutorial Revival)
        new(0696,0,20,128) { Location = 30027, FlawlessIVCount = 3, Moves = new(046,088,204,044) }, // Tyrunt (sub_addpoke_chigoras)
        new(0698,0,20,128) { Location = 30027, FlawlessIVCount = 3, Moves = new(088,196,036,054) }, // Amaura (sub_addpoke_amarus)

        // Fossils
        new(0142,0,20,000) { Location = 30027, Shiny = Random }, // Aerodactyl (restoration_ptera)
        new(0696,0,20,000) { Location = 30027, Shiny = Random }, // Tyrunt (restoration_chigoras)
        new(0698,0,20,000) { Location = 30027, Shiny = Random }, // Amaura (restoration_amarus)

        // Alpha Fossils
        new(0142,0,35,255) { Location = 30027, Shiny = Random, IsAlpha = true, FlawlessIVCount = 3 }, // Aerodactyl (restoration_ptera)
        new(0696,0,35,255) { Location = 30027, Shiny = Random, IsAlpha = true, FlawlessIVCount = 3 }, // Tyrunt (restoration_chigoras)
        new(0698,0,35,255) { Location = 30027, Shiny = Random, IsAlpha = true, FlawlessIVCount = 3 }, // Amaura (restoration_amarus)
    ];

    internal static readonly EncounterStatic9a[] Static =
    [
        // Legendary Pokémon
        new(0716,0,75,128) { Location = 00210, FlawlessIVCount = 3, Moves = new(224,585,532,601) }, // Xerneas (m10_x)
        new(0717,0,75,128) { Location = 00075, FlawlessIVCount = 3, Moves = new(542,399,094,613) }, // Yveltal (m10_y)
        new(0718,2,84,128) { Location = 00212, Nature = Nature.Quiet, IVs = new(31,31,15,19,31,28), Moves = new(687,614,615,616) }, // Zygarde-2 (ect_boss_0718_01)

        // Main Missions / Side Missions
        new(0013,0,45,255) { Location = 00055, Gender = 0, Nature = Nature.Naive, IsAlpha = true, FlawlessIVCount = 3 }, // Weedle (sub_023_oybn)
        new(0092,0,40,128) { Location = 00231 }, // Gastly (ect_d03_01_z092_ev)
        new(0094,0,42,128) { Location = 00231 }, // Gengar (ect_d03_01_z094_ev)
        new(0302,0,10,128) { Location = 00205, Gender = 0, Nature = Nature.Naive, Moves = new(033,043,425,000) }, // Sableye (sub_003_ghost)
        new(0303,0,52,255) { Location = 00057, Gender = 1, Nature = Nature.Hardy, FlawlessIVCount = 3 }, // Mawile (sub_107_kucheat)
        new(0505,0,50,255) { Location = 00014, Gender = 0, Nature = Nature.Bold, IsAlpha = true, FlawlessIVCount = 3 }, // Watchog (sub_056_evoybn)
        new(0569,0,50,128) { Location = 00030, Gender = 0, Nature = Nature.Naughty, Moves = new(398,693,205,232) }, // Garbodor (sub_027_predator)
        new(0587,0,53,128) { Location = 00089, Gender = 1, Nature = Nature.Naive, Moves = new(113,521,403,209) }, // Emolga (sub_115_emonga)
        new(0659,0,10,255) { Location = 00079, Gender = 0, Nature = Nature.Mild, IsAlpha = true, FlawlessIVCount = 3 }, // Bunnelby (sub_002_oybn)
        new(0660,0,28,128) { Location = 00012, Gender = 0, Nature = Nature.Adamant }, // Diggersby (sub_055_evpoke)
        new(0707,0,40,128) { Location = 00235, Gender = 0, Nature = Nature.Timid, Moves = new(583,430,577,319) }, // Klefki (m05_cleffy)

        // Side Missions EX
        // new(0150,0,70,128) { Location = 00234, FlawlessIVCount = 3, Moves = new(094,396,427,133) }, // Mewtwo (sub_120_mewtwo)
        new(0703,0,66,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(444,430,605,157) }, // Carbink (sub_119_melecie_01)
        new(0703,0,66,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(444,446,408,605) }, // Carbink (sub_119_melecie_02)
        new(0719,0,70,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(591,585,446,577) }, // Diancie (sub_119_diancie)
    ];

    private const string tradeZA = "tradeza";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings11(tradeZA);
    internal static readonly EncounterTrade9a[] Trades =
    [
        new(TradeNames,0,0214,0,12) { ID32 = 797394, OTGender = 1, Gender = 0, Nature = Nature.Brave, IVs = new(31,31,15,31,15,15), Moves = new(033,043,203,042) }, // Heracross (sub_tradepoke_heracros)
        new(TradeNames,1,0447,0,25) { ID32 = 348226, OTGender = 0, Gender = 0, Nature = Nature.Rash,  IVs = new(15,31,15,31,31,15), Moves = new(418,098,249,197) }, // Riolu (sub_tradepoke_riolu)
        new(TradeNames,2,0079,1,30) { ID32 = 934764, OTGender = 0, Gender = 0,                                 FlawlessIVCount = 3, Moves = new(352,133,428,029) }, // Slowpoke-1 (sub_addpoke_gyadon)
        new(TradeNames,3,0026,1,64) { ID32 = 693489, OTGender = 1, Gender = 0, Nature = Nature.Jolly, IVs = new(20,20,20,20,20,20), Moves = new(094,087,057,583) }, // Raichu-1 (sub_addpoke_araichu)
    ];
}
