using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public enum GameVersion
    {
        Invalid = -2,
        Any = -1,
        Unknown = 0,
        S = 1, R = 2, E = 3, FR = 4, LG = 5, CXD = 15,
        D = 10, P = 11, Pt = 12, HG = 7, SS = 8, 
        W = 20, B = 21, W2 = 22, B2 = 23,
        X = 24, Y = 25, AS = 26, OR = 27,
        SN = 28, MN = 29,

        // Game Groupings (SaveFile type)
        RS = 100,
        FRLG = 101,
        DP = 102,
        HGSS = 103,
        BW = 104,
        B2W2 = 105,
        XY = 106,
        ORASDEMO = 107,
        ORAS = 108,
        SM = 109,
    }

    public static class SaveUtil
    {
        internal const int BEEF = 0x42454546;

        internal const int SIZE_G6XY = 0x65600;
        internal const int SIZE_G6ORAS = 0x76000;
        internal const int SIZE_G6ORASDEMO = 0x5A00;
        internal const int SIZE_G5RAW = 0x80000;
        internal const int SIZE_G5BW = 0x24000;
        internal const int SIZE_G5B2W2 = 0x26000;
        internal const int SIZE_G4RAW = 0x80000;
        internal const int SIZE_G3RAW = 0x20000;
        internal const int SIZE_G3RAWHALF = 0x10000;

        internal static readonly byte[] FOOTER_DSV = Encoding.ASCII.GetBytes("|-DESMUME SAVE-|");

        /// <summary>Determines the generation of the given save data.</summary>
        /// <param name="data">Save data of which to determine the generation</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static int getSAVGeneration(byte[] data)
        {
            if (getIsG3SAV(data) != GameVersion.Invalid)
                return 3;
            if (getIsG4SAV(data) != GameVersion.Invalid)
                return 4;
            if (getIsG5SAV(data) != GameVersion.Invalid)
                return 5;
            if (getIsG6SAV(data) != GameVersion.Invalid)
                return 6;
            return -1;
        }
        /// <summary>Determines the type of 3th gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG3SAV(byte[] data)
        {
            if (data.Length != SIZE_G3RAW && data.Length != SIZE_G3RAWHALF)
                return GameVersion.Invalid;

            int[] BlockOrder = new int[14];
            for (int i = 0; i < 14; i++)
                BlockOrder[i] = BitConverter.ToInt16(data, i * 0x1000 + 0xFF4);

            if (BlockOrder.Any(i => i > 0xD || i < 0))
                return GameVersion.Invalid;

            // Detect RS/E/FRLG
            // Section 0 stores Game Code @ 0x00AC; 0 for RS, 1 for FRLG, else for Emerald

            int Block0 = Array.IndexOf(BlockOrder, 0);
            uint GameCode = BitConverter.ToUInt32(data, Block0 * 0x1000 + 0xAC);
            if (GameCode == uint.MaxValue)
                return GameVersion.Unknown; // what a hack
            
            switch (GameCode)
            {
                case 0: return GameVersion.RS;
                case 1: return GameVersion.FRLG;
                default: return GameVersion.E;
            }
        }
        /// <summary>Determines the type of 4th gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG4SAV(byte[] data)
        {
            if (data.Length != SIZE_G4RAW)
                return GameVersion.Invalid;
            
            // General Block Checksum
            if (BitConverter.ToUInt16(data, 0xC0FE) == ccitt16(data.Take(0xC0EC).ToArray()))
                return GameVersion.DP;
            if (BitConverter.ToUInt16(data, 0xCF2A) == ccitt16(data.Take(0xCF18).ToArray()))
                return GameVersion.Pt;
            if (BitConverter.ToUInt16(data, 0xF626) == ccitt16(data.Take(0xF618).ToArray()))
                return GameVersion.HGSS;

            // General Block Checksum is invalid, check for block identifiers
            if (data.Skip(0xC0F4).Take(10).SequenceEqual(new byte[] { 0x00, 0xC1, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.DP;
            if (data.Skip(0xCF20).Take(10).SequenceEqual(new byte[] { 0x2C, 0xCF, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.Pt;
            if (data.Skip(0xF61C).Take(10).SequenceEqual(new byte[] { 0x28, 0xF6, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.HGSS;

            return GameVersion.Invalid;
        }
        /// <summary>Determines the type of 5th gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG5SAV(byte[] data)
        {
            if (data.Length != SIZE_G5RAW)
                return GameVersion.Invalid;

            ushort chk1 = BitConverter.ToUInt16(data, SIZE_G5BW - 0x100 + 0x8C + 0xE);
            ushort actual1 = ccitt16(data.Skip(SIZE_G5BW - 0x100).Take(0x8C).ToArray());
            if (chk1 == actual1)
                return GameVersion.BW;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE_G5B2W2 - 0x100 + 0x94 + 0xE);
            ushort actual2 = ccitt16(data.Skip(SIZE_G5B2W2 - 0x100).Take(0x94).ToArray());
            if (chk2 == actual2)
                return GameVersion.B2W2;
            return GameVersion.Invalid;
        }
        /// <summary>Determines the type of 6th gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG6SAV(byte[] data)
        {
            if (!new []{SIZE_G6XY, SIZE_G6ORAS, SIZE_G6ORASDEMO}.Contains(data.Length))
                return GameVersion.Invalid;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return GameVersion.Invalid;

            switch (data.Length)
            {
                case SIZE_G6XY:
                    return GameVersion.XY;
                case SIZE_G6ORASDEMO:
                    return GameVersion.ORASDEMO;
                case SIZE_G6ORAS:
                    return GameVersion.ORAS;
            }
            return GameVersion.Invalid;
        }

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="data">Save data from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile getVariantSAV(byte[] data)
        {
            switch (getSAVGeneration(data))
            {
                case 3:
                    return new SAV3(data);
                case 4:
                    return new SAV4(data);
                case 5:
                    return new SAV5(data);
                case 6:
                    return new SAV6(data);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Detects a save file.
        /// </summary>
        /// <returns>Full path of a save file. Returns null if unable to find any.</returns>
        public static string detectSaveFile()
        {
            string path;
            string path3DS = Path.GetPathRoot(Util.get3DSLocation());
            List<string> possiblePaths = new List<string>();

            // save_manager
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "saveDataBackup")))
                possiblePaths.AddRange(getSavesFromFolder(path, false));

            // SaveDataFiler
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "filer", "UserSaveData")))
                possiblePaths.AddRange(getSavesFromFolder(path, true));

            // JKSV
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "JKSV", "Saves")))
                possiblePaths.AddRange(getSavesFromFolder(path, true));

            // TWL Save Tool
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "TWLSaveTool")))
                possiblePaths.AddRange(getSavesFromFolder(path, false));

            // CyberGadget (Cache)
            string pathCache = Util.GetCacheFolder();
            if (Directory.Exists(pathCache))
                possiblePaths.AddRange(getSavesFromFolder(Path.Combine(pathCache), false));

            // return newest save file path that is valid (oh man)
            return possiblePaths.OrderByDescending(f => new FileInfo(f).LastWriteTime).FirstOrDefault(p => getVariantSAV(File.ReadAllBytes(p)).ChecksumsValid);
        }
        /// <summary>
        /// Retrieves the full path of the most recent file based on LastWriteTime.
        /// </summary>
        /// <param name="folderPath">Folder to look within</param>
        /// <param name="deep">Search all subfolders</param>
        /// <returns>Full path of all save files that match criteria.</returns>
        public static IEnumerable<string> getSavesFromFolder(string folderPath, bool deep)
        {
            if (!Directory.Exists(folderPath))
                return null;
            return Directory.GetFiles(folderPath, "*", deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(f => SizeValidSAV((int)new FileInfo(f).Length));
        }

        /// <summary>
        /// Determines whether the save data size is valid for 6th generation saves.
        /// </summary>
        /// <param name="size">Size in bytes of the save data</param>
        /// <returns>A boolean indicating whether or not the save data size is valid.</returns>
        public static bool SizeValidSAV(int size)
        {
            switch (size)
            {
                case SIZE_G6XY:
                case SIZE_G6ORASDEMO:
                case SIZE_G6ORAS:
                case SIZE_G5B2W2:
                case SIZE_G4RAW:
                case SIZE_G3RAW:
                case SIZE_G3RAWHALF:
                    return true;
                default:
                    return false;
            }
        }

        // SAV Manipulation
        /// <summary>Calculates the CRC16-CCITT checksum over an input byte array.</summary>
        /// <param name="data">Input byte array</param>
        /// <returns>Checksum</returns>
        public static ushort ccitt16(byte[] data)
        {
            const ushort init = 0xFFFF;
            const ushort poly = 0x1021;

            ushort crc = init;
            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);
                for (int j = 0; j < 8; j++)
                {
                    bool flag = (crc & 0x8000) > 0;
                    crc <<= 1;
                    if (flag)
                        crc ^= poly;
                }
            }
            return crc;
        }
        /// <summary>Calculates the 32bit checksum over an input byte array. Used in GBA save files.</summary>
        /// <param name="data">Input byte array</param>
        /// <returns>Checksum</returns>
        internal static ushort check32(byte[] data)
        {
            uint val = 0;
            for (int i = 0; i < data.Length; i += 4)
                val += BitConverter.ToUInt32(data, i);
            return (ushort)((val & 0xFFFF) + (val >> 16));
        }

        public static int getDexFormIndexXY(int species, int formct)
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
                case 646: return 072; // 3 Kyurem
                case 647: return 075; // 2 Keldeo
                case 642: return 077; // 2 Thundurus
                case 641: return 079; // 2 Tornadus
                case 645: return 081; // 2 Landorus
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
                default: return -1;
            }
        }
        public static int getDexFormIndexORAS(int species, int formct)
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
                case 360: return 208; // 2 Swampert
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
                default: return getDexFormIndexXY(species, formct);
            }
        }
    }
}
