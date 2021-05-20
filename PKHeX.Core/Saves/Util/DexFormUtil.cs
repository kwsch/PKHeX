using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for interacting with Pokédex Form flags
    /// </summary>
    public static class DexFormUtil
    {
        public static int GetDexFormIndexSM(int species, int formCount, int start) => GetDexFormBitIndex(species, formCount, start, formtable_SM);
        public static int GetDexFormIndexUSUM(int species, int formCount, int start) => GetDexFormBitIndex(species, formCount, start, formtable_USUM);
        public static int GetDexFormIndexGG(int species, int formCount, int start) => GetDexFormBitIndex(species, formCount, start, formtable_GG);
        public static int GetDexFormCountSM(int species) => GetDexFormCount(species, formtable_SM);
        public static int GetDexFormCountUSUM(int species) => GetDexFormCount(species, formtable_USUM);
        public static int GetDexFormCountGG(int species) => GetDexFormCount(species, formtable_GG);

        private static readonly ushort[] formtable_SM = // u16 species, u16 formcount
        {
            0x0003, 0x0002, 0x0006, 0x0003, 0x0009, 0x0002, 0x000F, 0x0002,
            0x0012, 0x0002, 0x0013, 0x0002, 0x0014, 0x0003, 0x0019, 0x0007,
            0x001A, 0x0002, 0x001B, 0x0002, 0x001C, 0x0002, 0x0025, 0x0002,
            0x0026, 0x0002, 0x0032, 0x0002, 0x0033, 0x0002, 0x0034, 0x0002,
            0x0035, 0x0002, 0x0041, 0x0002, 0x004A, 0x0002, 0x004B, 0x0002,
            0x004C, 0x0002, 0x0050, 0x0002, 0x0058, 0x0002, 0x0059, 0x0002,
            0x005E, 0x0002, 0x0067, 0x0002, 0x0069, 0x0002, 0x0073, 0x0002,
            0x007F, 0x0002, 0x0082, 0x0002, 0x008E, 0x0002, 0x0096, 0x0003,
            0x00B5, 0x0002, 0x00C9, 0x001C, 0x00D0, 0x0002, 0x00D4, 0x0002,
            0x00D6, 0x0002, 0x00E5, 0x0002, 0x00F8, 0x0002, 0x00FE, 0x0002,
            0x0101, 0x0002, 0x0104, 0x0002, 0x011A, 0x0002, 0x012E, 0x0002,
            0x012F, 0x0002, 0x0132, 0x0002, 0x0134, 0x0002, 0x0136, 0x0002,
            0x013F, 0x0002, 0x0143, 0x0002, 0x014E, 0x0002, 0x015F, 0x0004,
            0x0162, 0x0002, 0x0167, 0x0002, 0x016A, 0x0002, 0x0175, 0x0002,
            0x0178, 0x0002, 0x017C, 0x0002, 0x017D, 0x0002, 0x017E, 0x0002,
            0x017F, 0x0002, 0x0180, 0x0002, 0x0182, 0x0004, 0x019C, 0x0003,
            0x019D, 0x0003, 0x01A5, 0x0002, 0x01A6, 0x0002, 0x01A7, 0x0002,
            0x01AC, 0x0002, 0x01BD, 0x0002, 0x01C0, 0x0002, 0x01CC, 0x0002,
            0x01DB, 0x0002, 0x01DF, 0x0006, 0x01E7, 0x0002, 0x01EC, 0x0002,
            0x01ED, 0x0012, 0x0213, 0x0002, 0x0226, 0x0002, 0x022B, 0x0002,
            0x0249, 0x0004, 0x024A, 0x0004, 0x0281, 0x0002, 0x0282, 0x0002,
            0x0285, 0x0002, 0x0286, 0x0003, 0x0287, 0x0002, 0x0288, 0x0002,
            0x0289, 0x0005, 0x0292, 0x0003, 0x029A, 0x0014, 0x029D, 0x0005,
            0x029E, 0x0006, 0x029F, 0x0005, 0x02A4, 0x000A, 0x02A6, 0x0002,
            0x02A9, 0x0002, 0x02C6, 0x0004, 0x02C7, 0x0004, 0x02CC, 0x0002,
            0x02CE, 0x0005, 0x02CF, 0x0002, 0x02D0, 0x0002, 0x02DF, 0x0002,
            0x02E2, 0x0002, 0x02E5, 0x0004, 0x02E9, 0x0002, 0x02EA, 0x0002,
            0x02F2, 0x0002, 0x02F6, 0x0002, 0x0305, 0x0012, 0x0306, 0x000E,
            0x030A, 0x0004, 0x0310, 0x0002, 0x0321, 0x0002,
        };

        private static readonly ushort[] formtable_USUM = // u16 species, u16 formcount
        {
            0x0003, 0x0002, 0x0006, 0x0003, 0x0009, 0x0002, 0x000F, 0x0002,
            0x0012, 0x0002, 0x0013, 0x0002, 0x0014, 0x0003, 0x0019, 0x0008,
            0x001A, 0x0002, 0x001B, 0x0002, 0x001C, 0x0002, 0x0025, 0x0002,
            0x0026, 0x0002, 0x0032, 0x0002, 0x0033, 0x0002, 0x0034, 0x0002,
            0x0035, 0x0002, 0x0041, 0x0002, 0x004A, 0x0002, 0x004B, 0x0002,
            0x004C, 0x0002, 0x0050, 0x0002, 0x0058, 0x0002, 0x0059, 0x0002,
            0x005E, 0x0002, 0x0067, 0x0002, 0x0069, 0x0003, 0x0073, 0x0002,
            0x007F, 0x0002, 0x0082, 0x0002, 0x008E, 0x0002, 0x0096, 0x0003,
            0x00B5, 0x0002, 0x00C9, 0x001C, 0x00D0, 0x0002, 0x00D4, 0x0002,
            0x00D6, 0x0002, 0x00E5, 0x0002, 0x00F8, 0x0002, 0x00FE, 0x0002,
            0x0101, 0x0002, 0x0104, 0x0002, 0x011A, 0x0002, 0x012E, 0x0002,
            0x012F, 0x0002, 0x0132, 0x0002, 0x0134, 0x0002, 0x0136, 0x0002,
            0x013F, 0x0002, 0x0143, 0x0002, 0x014E, 0x0002, 0x015F, 0x0004,
            0x0162, 0x0002, 0x0167, 0x0002, 0x016A, 0x0002, 0x0175, 0x0002,
            0x0178, 0x0002, 0x017C, 0x0002, 0x017D, 0x0002, 0x017E, 0x0002,
            0x017F, 0x0002, 0x0180, 0x0002, 0x0182, 0x0004, 0x019C, 0x0003,
            0x019D, 0x0003, 0x019E, 0x0003, 0x01A5, 0x0002, 0x01A6, 0x0002,
            0x01A7, 0x0002, 0x01AC, 0x0002, 0x01BD, 0x0002, 0x01C0, 0x0002,
            0x01CC, 0x0002, 0x01DB, 0x0002, 0x01DF, 0x0006, 0x01E7, 0x0002,
            0x01EC, 0x0002, 0x01ED, 0x0012, 0x0213, 0x0002, 0x0226, 0x0002,
            0x022B, 0x0002, 0x0249, 0x0004, 0x024A, 0x0004, 0x0281, 0x0002,
            0x0282, 0x0002, 0x0285, 0x0002, 0x0286, 0x0003, 0x0287, 0x0002,
            0x0288, 0x0002, 0x0289, 0x0005, 0x0292, 0x0003, 0x0298, 0x0014,
            0x0299, 0x0014, 0x029A, 0x0014, 0x029D, 0x0005, 0x029E, 0x0006,
            0x029F, 0x0005, 0x02A4, 0x000A, 0x02A6, 0x0002, 0x02A9, 0x0002,
            0x02C6, 0x0004, 0x02C7, 0x0004, 0x02CC, 0x0002, 0x02CE, 0x0005,
            0x02CF, 0x0002, 0x02D0, 0x0002, 0x02DF, 0x0002, 0x02E2, 0x0002,
            0x02E5, 0x0004, 0x02E7, 0x0002, 0x02E8, 0x0002, 0x02E9, 0x0003,
            0x02EA, 0x0002, 0x02F0, 0x0002, 0x02F2, 0x0002, 0x02F6, 0x0002,
            0x0305, 0x0012, 0x0306, 0x000E, 0x0309, 0x0002, 0x030A, 0x0004,
            0x0310, 0x0002, 0x0320, 0x0004, 0x0321, 0x0002
        };

        private static readonly ushort[] formtable_GG = // u16 species, u16 formcount
        {
            0x0003, 0x0002, 0x0006, 0x0003, 0x0009, 0x0002, 0x000F, 0x0002,
            0x0012, 0x0002, 0x0013, 0x0002, 0x0014, 0x0003, 0x0019, 0x0009,
            0x001A, 0x0002, 0x001B, 0x0002, 0x001C, 0x0002, 0x0025, 0x0002,
            0x0026, 0x0002, 0x0032, 0x0002, 0x0033, 0x0002, 0x0034, 0x0002,
            0x0035, 0x0002, 0x0041, 0x0002, 0x004A, 0x0002, 0x004B, 0x0002,
            0x004C, 0x0002, 0x0050, 0x0002, 0x0058, 0x0002, 0x0059, 0x0002,
            0x005E, 0x0002, 0x0067, 0x0002, 0x0069, 0x0003, 0x0073, 0x0002,
            0x007F, 0x0002, 0x0082, 0x0002, 0x008E, 0x0002, 0x0096, 0x0003,
        };

        private static int GetDexFormBitIndex(int species, int formCount, int start, IReadOnlyList<ushort> formTable)
        {
            int formIndex = start;
            for (int i = 0; i < formTable.Count; i += 2)
            {
                int s = formTable[i];
                int f = formTable[i + 1];
                if (s == species)
                    return f > formCount ? -1 : formIndex;

                formIndex += f - 1;
            }
            return -1;
        }

        private static int GetDexFormCount(int species, IReadOnlyList<ushort> formTable)
        {
            for (int i = 0; i < formTable.Count; i += 2)
            {
                if (formTable[i] == species)
                    return formTable[i + 1];
            }
            return 0;
        }

        public static int GetDexFormIndexBW(int species, int formCount)
        {
            if (formCount < 1 || species < 0)
                return -1; // invalid
            return species switch
            {
                201 => 000, // 28 Unown
                386 => 028, // 4 Deoxys
                492 => 032, // 2 Shaymin
                487 => 034, // 2 Giratina
                479 => 036, // 6 Rotom
                422 => 042, // 2 Shellos
                423 => 044, // 2 Gastrodon
                412 => 046, // 3 Burmy
                413 => 049, // 3 Wormadam
                351 => 052, // 4 Castform
                421 => 056, // 2 Cherrim
                585 => 058, // 4 Deerling
                586 => 062, // 4 Sawsbuck
                648 => 066, // 2 Meloetta
                555 => 068, // 2 Darmanitan
                550 => 070, // 2 Basculin
                _ => -1
            };
        }

        public static int GetDexFormIndexB2W2(int species, int formCount)
        {
            if (formCount < 1 || species < 0)
                return -1; // invalid
            return species switch
            {
                646 => 072, // 3 Kyurem
                647 => 075, // 2 Keldeo
                642 => 077, // 2 Thundurus
                641 => 079, // 2 Tornadus
                645 => 081, // 2 Landorus
                _ => GetDexFormIndexBW(species, formCount)
            };
        }

        public static int GetDexFormIndexXY(int species, int formCount)
        {
            if (formCount < 1 || species < 0)
                return -1; // invalid
            return species switch
            {
                666 => 083, // 20 Vivillion
                669 => 103, // 5 Flabébé
                670 => 108, // 6 Floette
                671 => 114, // 5 Florges
                710 => 119, // 4 Pumpkaboo
                711 => 123, // 4 Gourgeist
                681 => 127, // 2 Aegislash
                716 => 129, // 2 Xerneas
                003 => 131, // 2 Venusaur
                006 => 133, // 3 Charizard
                009 => 136, // 2 Blastoise
                065 => 138, // 2 Alakazam
                094 => 140, // 2 Gengar
                115 => 142, // 2 Kangaskhan
                127 => 144, // 2 Pinsir
                130 => 146, // 2 Gyarados
                142 => 148, // 2 Aerodactyl
                150 => 150, // 3 Mewtwo
                181 => 153, // 2 Ampharos
                212 => 155, // 2 Scizor
                214 => 157, // 2 Heracros
                229 => 159, // 2 Houndoom
                248 => 161, // 2 Tyranitar
                257 => 163, // 2 Blaziken
                282 => 165, // 2 Gardevoir
                303 => 167, // 2 Mawile
                306 => 169, // 2 Aggron
                308 => 171, // 2 Medicham
                310 => 173, // 2 Manetric
                354 => 175, // 2 Banette
                359 => 177, // 2 Absol
                380 => 179, // 2 Latias
                381 => 181, // 2 Latios
                445 => 183, // 2 Garchomp
                448 => 185, // 2 Lucario
                460 => 187, // 2 Abomasnow
                _ => GetDexFormIndexB2W2(species, formCount)
            };
        }

        public static int GetDexFormIndexORAS(int species, int formCount)
        {
            if (formCount < 1 || species < 0)
                return -1; // invalid
            return species switch
            {
                025 => 189, // 7 Pikachu
                720 => 196, // 2 Hoopa
                015 => 198, // 2 Beedrill
                018 => 200, // 2 Pidgeot
                080 => 202, // 2 Slowbro
                208 => 204, // 2 Steelix
                254 => 206, // 2 Sceptile
                260 => 208, // 2 Swampert
                302 => 210, // 2 Sableye
                319 => 212, // 2 Sharpedo
                323 => 214, // 2 Camerupt
                334 => 216, // 2 Altaria
                362 => 218, // 2 Glalie
                373 => 220, // 2 Salamence
                376 => 222, // 2 Metagross
                384 => 224, // 2 Rayquaza
                428 => 226, // 2 Lopunny
                475 => 228, // 2 Gallade
                531 => 230, // 2 Audino
                719 => 232, // 2 Diancie
                382 => 234, // 2 Kyogre
                383 => 236, // 2 Groudon
                493 => 238, // 18 Arceus
                649 => 256, // 5 Genesect
                676 => 261, // 10 Furfrou
                _ => GetDexFormIndexXY(species, formCount)
            };
        }
    }
}
