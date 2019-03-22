using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for interacting with Pokedex AltForm flags
    /// </summary>
    public static class DexFormUtil
    {
        public static int GetDexFormIndexSM(int species, int formct, int start) => GetDexFormBitIndex(species, formct, start, formtable_SM);
        public static int GetDexFormIndexUSUM(int species, int formct, int start) => GetDexFormBitIndex(species, formct, start, formtable_USUM);
        public static int GetDexFormIndexGG(int species, int formct, int start) => GetDexFormBitIndex(species, formct, start, formtable_GG);
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

        private static int GetDexFormBitIndex(int species, int formct, int start, IReadOnlyList<ushort> formtable)
        {
            int formindex = start;
            int f = 0;
            for (int i = 0; i < formtable.Count; i += 2)
            {
                int s = formtable[i];
                f = formtable[i + 1];
                if (s == species)
                    break;

                formindex += f - 1;
            }
            if (f > formct)
                return -1;
            return formindex;
        }

        private static int GetDexFormCount(int species, IReadOnlyList<ushort> formtable)
        {
            for (int i = 0; i < formtable.Count; i += 2)
            {
                if (formtable[i] == species)
                    return formtable[i + 1];
            }
            return 0;
        }

        public static int GetDexFormIndexBW(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 201: return 000; // 28 Unown
                case 386: return 028; // 4 Deoxys
                case 492: return 032; // 2 Shaymin
                case 487: return 034; // 2 Giratina
                case 479: return 036; // 6 Rotom
                case 422: return 042; // 2 Shellos
                case 423: return 044; // 2 Gastrodon
                case 412: return 046; // 3 Burmy
                case 413: return 049; // 3 Wormadam
                case 351: return 052; // 4 Castform
                case 421: return 056; // 2 Cherrim
                case 585: return 058; // 4 Deerling
                case 586: return 062; // 4 Sawsbuck
                case 648: return 066; // 2 Meloetta
                case 555: return 068; // 2 Darmanitan
                case 550: return 070; // 2 Basculin
                default: return -1;
            }
        }

        public static int GetDexFormIndexB2W2(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 646: return 072; // 3 Kyurem
                case 647: return 075; // 2 Keldeo
                case 642: return 077; // 2 Thundurus
                case 641: return 079; // 2 Tornadus
                case 645: return 081; // 2 Landorus
                default: return GetDexFormIndexBW(species, formct);
            }
        }

        public static int GetDexFormIndexXY(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 666: return 083; // 20 Vivillion
                case 669: return 103; // 5 Flabébé
                case 670: return 108; // 6 Floette
                case 671: return 114; // 5 Florges
                case 710: return 119; // 4 Pumpkaboo
                case 711: return 123; // 4 Gourgeist
                case 681: return 127; // 2 Aegislash
                case 716: return 129; // 2 Xerneas
                case 003: return 131; // 2 Venusaur
                case 006: return 133; // 3 Charizard
                case 009: return 136; // 2 Blastoise
                case 065: return 138; // 2 Alakazam
                case 094: return 140; // 2 Gengar
                case 115: return 142; // 2 Kangaskhan
                case 127: return 144; // 2 Pinsir
                case 130: return 146; // 2 Gyarados
                case 142: return 148; // 2 Aerodactyl
                case 150: return 150; // 3 Mewtwo
                case 181: return 153; // 2 Ampharos
                case 212: return 155; // 2 Scizor
                case 214: return 157; // 2 Heracros
                case 229: return 159; // 2 Houndoom
                case 248: return 161; // 2 Tyranitar
                case 257: return 163; // 2 Blaziken
                case 282: return 165; // 2 Gardevoir
                case 303: return 167; // 2 Mawile
                case 306: return 169; // 2 Aggron
                case 308: return 171; // 2 Medicham
                case 310: return 173; // 2 Manetric
                case 354: return 175; // 2 Banette
                case 359: return 177; // 2 Absol
                case 380: return 179; // 2 Latias
                case 381: return 181; // 2 Latios
                case 445: return 183; // 2 Garchomp
                case 448: return 185; // 2 Lucario
                case 460: return 187; // 2 Abomasnow
                default: return GetDexFormIndexB2W2(species, formct);
            }
        }

        public static int GetDexFormIndexORAS(int species, int formct)
        {
            if (formct < 1 || species < 0)
                return -1; // invalid
            switch (species)
            {
                case 025: return 189; // 7 Pikachu
                case 720: return 196; // 2 Hoopa
                case 015: return 198; // 2 Beedrill
                case 018: return 200; // 2 Pidgeot
                case 080: return 202; // 2 Slowbro
                case 208: return 204; // 2 Steelix
                case 254: return 206; // 2 Sceptile
                case 260: return 208; // 2 Swampert
                case 302: return 210; // 2 Sableye
                case 319: return 212; // 2 Sharpedo
                case 323: return 214; // 2 Camerupt
                case 334: return 216; // 2 Altaria
                case 362: return 218; // 2 Glalie
                case 373: return 220; // 2 Salamence
                case 376: return 222; // 2 Metagross
                case 384: return 224; // 2 Rayquaza
                case 428: return 226; // 2 Lopunny
                case 475: return 228; // 2 Gallade
                case 531: return 230; // 2 Audino
                case 719: return 232; // 2 Diancie
                case 382: return 234; // 2 Kyogre
                case 383: return 236; // 2 Groudon
                case 493: return 238; // 18 Arceus
                case 649: return 256; // 5 Genesect
                case 676: return 261; // 10 Furfrou
                default: return GetDexFormIndexXY(species, formct);
            }
        }
    }
}
