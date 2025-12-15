using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;

namespace PKHeX.Core;

internal static class Encounters9a
{
    internal static readonly EncounterArea9a[] Slots = EncounterArea9a.GetAreas(Get16("za", "za"u8), SlotType9a.Standard);
    internal static readonly EncounterArea9a[] Hyperspace = EncounterArea9a.GetAreas(Get16("hyperspace_za", "za"u8), SlotType9a.Hyperspace);

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

        #region Sankaku
        new(0801,0,80,128) { Location = 30035, Nature = Nature.Modest, FlawlessIVCount = 3, Trainer = TrainerGift9a.Magearna }, // Magearna (sub_addpoke_magiana)
        new(0809,0,80,128) { Location = 30034, Nature = Nature.Adamant, FlawlessIVCount = 3 }, // Melmetal (sub_addpoke_merumetal)
        new(0720,0,80,128) { Location = 30034, Nature = Nature.Lonely, FlawlessIVCount = 3, Moves = new(593,094,417,566) }, // Hoopa (sub_addpoke_huupa)
        new(0999,0,05,128) { Location = 30033, Nature = Nature.Bold, FlawlessIVCount = 3, Trainer = TrainerGift9a.Gimmighoul }, // Gimmighoul (sub_addpoke_korekure)
        #endregion
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
        new(0150,0,70,128) { Location = 00234, FlawlessIVCount = 3, Moves = new(094,396,427,133) }, // Mewtwo (sub_120_mewtwo)
        new(0703,0,66,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(444,430,605,157) }, // Carbink (sub_119_melecie_01)
        new(0703,0,66,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(444,446,408,605) }, // Carbink (sub_119_melecie_02)
        new(0719,0,70,128) { Location = 00063, FlawlessIVCount = 3, Moves = new(591,585,446,577) }, // Diancie (sub_119_diancie)
        
        #region Sankaku
        // Rogue Mega Evolution
        new(0359,0,75,255) { Location = 00275, Gender = 1, Nature = Nature.Quiet,   IVs = new(31,31,20,28,31,18), Moves = new(282,400,555,425) }, // Absol (btl_ect_boss_0359_z_01)
        new(0384,0,85,128) { Location = 00283,             Nature = Nature.Brave,   IVs = new(31,31,29,20,31,29), Moves = new(620,304,800,200) }, // Rayquaza (ect_boss_0384_01)
        new(0398,0,75,255) { Location = 00276, Gender = 0, Nature = Nature.Adamant, IVs = new(31,31,15,24,31,20), Moves = new(411,814,038,370) }, // Staraptor (btl_ect_boss_0398_01)
        new(0485,0,80,128) { Location = 00279, Gender = 0, Nature = Nature.Bold,    IVs = new(31,31,28,08,31,25), Moves = new(463,430,523,315) }, // Heatran (btl_ect_boss_0485_01)
        new(0491,0,85,128) { Location = 00280,             Nature = Nature.Careful, IVs = new(31,31,18,26,31,21), Moves = new(464,248,138,555) }, // Darkrai (btl_ect_boss_0491_01)
        new(0678,0,75,255) { Location = 00278, Gender = 0, Nature = Nature.Calm,    IVs = new(31,31,22,24,31,25), Moves = new(094,115,113,060) }, // Meowstic (btl_ect_boss_0678_01)
        new(0807,0,85,128) { Location = 00273,             Nature = Nature.Brave,   IVs = new(31,31,19,27,31,15), Moves = new(721,223,527,528) }, // Zeraora (btl_ect_boss_0807_01)
        new(0978,0,75,255) { Location = 00277, Gender = 0, Nature = Nature.Timid,   IVs = new(31,31,16,30,31,28), Moves = new(127,058,407,250) }, // Tatsugiri (btl_ect_boss_0952_01)

        // Rogue Primal Reversion
        new(0382,0,80,128) { Location = 00281, Nature = Nature.Modest, IVs = new(31,31,12,22,31,28), Moves = new(618,058,057,087) }, // Kyogre (ect_boss_0382_01)
        new(0383,0,80,128) { Location = 00282, Nature = Nature.Impish, IVs = new(31,31,24,25,31,18), Moves = new(619,126,076,815) }, // Groudon (ect_boss_0383_01)

        // Legendary Pokémon
        new(0380,0,60,128) { Location = 00273, Shiny = Random, Gender = 1, IVs = new(12,07,31,16,31,31) }, // Latias (ect_zdm404_sp06_380_sp)
        new(0381,0,60,128) { Location = 00273, Shiny = Random, Gender = 0, IVs = new(31,31,31,16,16,07) }, // Latios (ect_zdm404_sp07_381_sp)
        new(0638,0,60,128) { Location = 00273, Shiny = Random, IVs = new(12,31,31,31,16,07) }, // Cobalion (ect_zdm406_sp01_638_sp)
        new(0639,0,60,128) { Location = 00273, Shiny = Random, IVs = new(31,31,16,31,12,07) }, // Terrakion (ect_zdm406_sp02_639_sp)
        new(0640,0,60,128) { Location = 00273, Shiny = Random, IVs = new(16,31,12,31,31,07) }, // Virizion (ect_zdm406_sp03_640_sp)

        // Mythical Pokémon
        new(0647,0,60,128) { Location = 00273, IVs = new(07,31,31,31,16,12) }, // Keldeo (ect_zdm402_sp04_647_sp)
        new(0648,0,60,128) { Location = 00273, IVs = new(07,31,12,31,31,16) }, // Meloetta (ect_zdm402_sp05_648_sp)
        new(0649,0,60,128) { Location = 00273, FlawlessIVCount = 3, Moves = new(546,430,679,405) }, // Genesect (Ev_sub_196_030)
        new(0721,0,80,128) { Location = 00230, FlawlessIVCount = 3, Moves = new(592,056,114,315) }, // Volcanion (Ev_sub_201_011)
        new(0802,0,80,128) { Location = 00070, FlawlessIVCount = 3, Moves = new(712,370,325,425) }, // Marshadow (Ev_sub_197_010)
        new(0808,0,70,128) { Location = 00014, FlawlessIVCount = 3, Moves = new(430,151,693,000) }, // Meltan (Ev_sub_200_010)

        // Side Missions
        new(0551,0,38,128) { Location = 00093, Nature = Nature.Adamant, Moves = new(044,043,242,091) }, // Sandile (sub_152_020_1)
        new(0552,0,50,128) { Location = 00093, Nature = Nature.Adamant, Moves = new(044,043,242,328) }, // Krokorok (sub_152_020_3)
        new(0769,0,40,128) { Location = 00211, Nature = Nature.Impish, Moves = new(328,523,334,202) }, // Sandygast (Ev_sub_157_010_1)
        new(0827,0,15,128) { Location = 00045, Gender = 1, Nature = Nature.Naughty, Moves = new(046,555,098,039) }, // Nickit (Ev_sub_160_040)
        new(0932,0,40,128) { Location = 00030, Gender = 1, Nature = Nature.Impish, Moves = new(446,334,088,105) }, // Nacli (sub_153_010_kojio)
        #endregion
    ];

    private const string tradeZA = "tradeza";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings11(tradeZA);
    internal static readonly EncounterTrade9a[] Trades =
    [
        new(TradeNames,0,0214,0,12) { ID32 = 797394, OTGender = 1, Gender = 0, Nature = Nature.Brave, IVs = new(31,31,15,31,15,15), Moves = new(033,043,203,042) }, // Heracross (sub_tradepoke_heracros)
        new(TradeNames,1,0447,0,25) { ID32 = 348226, OTGender = 0, Gender = 0, Nature = Nature.Rash,  IVs = new(15,31,15,31,31,15), Moves = new(418,098,249,197) }, // Riolu (sub_tradepoke_riolu)
        new(TradeNames,2,0079,1,30) { ID32 = 934764, OTGender = 0, Gender = 0,                                 FlawlessIVCount = 3, Moves = new(352,133,428,029) }, // Slowpoke-1 (sub_addpoke_gyadon)
        new(TradeNames,3,0026,1,64) { ID32 = 693489, OTGender = 1, Gender = 0, Nature = Nature.Jolly, IVs = new(20,20,20,20,20,20), Moves = new(094,087,057,583) }, // Raichu-1 (sub_addpoke_araichu)
        new(TradeNames,4,0233,0,50) { ID32 = 065536, OTGender = 0, Gender = 2, Nature = Nature.Modest                                                            }, // Porygon2 (sub_tradepoke_poligon2)
    ];
}
