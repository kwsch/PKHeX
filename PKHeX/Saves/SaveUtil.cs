using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public enum GameVersion
    {
        /* I don't want to assign Gen I/II... */
        XD = -11,
        COLO = -10,
        BATREV = -7,
        RSBOX = -5,
        GS = -4,
        C = -3,
        Invalid = -2,
        Any = -1,
        Unknown = 0,
        S = 1, R = 2, E = 3, FR = 4, LG = 5, CXD = 15,
        D = 10, P = 11, Pt = 12, HG = 7, SS = 8, 
        W = 20, B = 21, W2 = 22, B2 = 23,
        X = 24, Y = 25, AS = 26, OR = 27,
        SN = 28, MN = 29,

        // Game Groupings (SaveFile type)
        RBY = 98,
        GSC = 99,
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
        internal const int SIZE_G4BR = 0x380000;
        internal const int SIZE_G4RAW = 0x80000;
        internal const int SIZE_G3BOX = 0x76000;
        internal const int SIZE_G3BOXGCI = 0x76040; // +64 if has GCI data
        internal const int SIZE_G3COLO = 0x60000;
        internal const int SIZE_G3COLOGCI = 0x60040; // +64 if has GCI data
        internal const int SIZE_G3XD = 0x56000;
        internal const int SIZE_G3XDGCI = 0x56040; // +64 if has GCI data
        internal const int SIZE_G3RAW = 0x20000;
        internal const int SIZE_G3RAWHALF = 0x10000;
        internal const int SIZE_G2RAW_U = 0x8000;
        internal const int SIZE_G2BAT_U = 0x802C;
        internal const int SIZE_G2EMU = 0x8030;
        internal const int SIZE_G2RAW_J = 0x10000;
        internal const int SIZE_G2BAT_J = 0x1002C;
        internal const int SIZE_G1RAW = 0x8000;
        internal const int SIZE_G1BAT = 0x802C;

        internal static readonly byte[] FOOTER_DSV = Encoding.ASCII.GetBytes("|-DESMUME SAVE-|");
        internal static readonly byte[] HEADER_BOX = Encoding.ASCII.GetBytes("GPX");
        internal static readonly byte[] HEADER_COLO = Encoding.ASCII.GetBytes("GC6");
        internal static readonly byte[] HEADER_XD = Encoding.ASCII.GetBytes("GXX");

        /// <summary>Determines the generation of the given save data.</summary>
        /// <param name="data">Save data of which to determine the generation</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static int getSAVGeneration(byte[] data)
        {
            if (getIsG1SAV(data) != GameVersion.Invalid)
                return 1;
            if (getIsG2SAV(data) != GameVersion.Invalid)
                return 2;
            if (getIsG3SAV(data) != GameVersion.Invalid)
                return 3;
            if (getIsG3COLOSAV(data) != GameVersion.Invalid)
                return (int)GameVersion.COLO;
            if (getIsG3XDSAV(data) != GameVersion.Invalid)
                return (int)GameVersion.XD;
            if (getIsG3BOXSAV(data) != GameVersion.Invalid)
                return (int)GameVersion.RSBOX;
            if (getIsG4SAV(data) != GameVersion.Invalid)
                return 4;
            if (getIsG4BRSAV(data) != GameVersion.Invalid)
                return (int) GameVersion.BATREV;
            if (getIsG5SAV(data) != GameVersion.Invalid)
                return 5;
            if (getIsG6SAV(data) != GameVersion.Invalid)
                return 6;
            return -1;
        }
        /// <summary>Determines the type of 1st gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG1SAV(byte[] data)
        {
            if (data.Length != SIZE_G1RAW && data.Length != SIZE_G1BAT)
                return GameVersion.Invalid;

            // Check if it's not an american save or a japanese save
            if (!(getIsG1SAVU(data) || getIsG1SAVJ(data)))
                return GameVersion.Invalid;
            // I can't actually detect which game version, because it's not stored anywhere.
            // If you can think of anything to do here, please implement :)
            return GameVersion.RBY;
        }
        /// <summary>Determines if 1st gen save is non-japanese</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid non-japanese save, False otherwise.</returns>
        public static bool getIsG1SAVU(byte[] data)
        {
            foreach (int ofs in new[] { 0x2F2C, 0x30C0 })
            {
                byte num_entries = data[ofs];
                if (num_entries > 20 || data[ofs + 1 + num_entries] != 0xFF)
                    return false;
            }
            return true;
        }
        /// <summary>Determines if 1st gen save is japanese</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid japanese save, False otherwise.</returns>
        public static bool getIsG1SAVJ(byte[] data)
        {
            foreach (int ofs in new[] { 0x2ED5, 0x302D })
            {
                byte num_entries = data[ofs];
                if (num_entries > 30 || data[ofs + 1 + num_entries] != 0xFF)
                    return false;
            }
            return true;
        }
        /// <summary>Determines the type of 2nd gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG2SAV(byte[] data)
        {
            if (data.Length != SIZE_G2RAW_U && data.Length != SIZE_G2BAT_U && data.Length != SIZE_G2RAW_J && data.Length != SIZE_G2BAT_J && data.Length != SIZE_G2EMU)
                return GameVersion.Invalid;

            // Check if it's not an american save or a japanese save
            if (getIsG2SAVU(data) != GameVersion.Invalid)
                return getIsG2SAVU(data);
            if (getIsG2SAVJ(data) != GameVersion.Invalid)
                return getIsG2SAVJ(data);
            return GameVersion.Invalid;
        }
        /// <summary>Determines if 2nd gen save is non-japanese</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid non-japanese save, False otherwise.</returns>
        public static GameVersion getIsG2SAVU(byte[] data)
        {
            bool gs = true;
            bool c = true;
            foreach (int ofs in new[] { 0x288A, 0x2D6C })
            {
                byte num_entries = data[ofs];
                if (num_entries > 20 || data[ofs + 1 + num_entries] != 0xFF)
                    gs = false;
            }
            foreach (int ofs in new[] { 0x2865, 0x2D10 })
            {
                byte num_entries = data[ofs];
                if (num_entries > 20 || data[ofs + 1 + num_entries] != 0xFF)
                    c = false;
            }
            if (gs)
                return GameVersion.GS;
            if (c)
                return GameVersion.C;
            return GameVersion.Invalid;
        }
        /// <summary>Determines if 2nd gen save is japanese</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid japanese save, False otherwise.</returns>
        public static GameVersion getIsG2SAVJ(byte[] data)
        {
            bool gs = true;
            bool c = true;
            foreach (int ofs in new[] { 0x283E, 0x2D10 })
            {
                byte num_entries = data[ofs];
                if (num_entries > 30 || data[ofs + 1 + num_entries] != 0xFF)
                    gs = false;
            }
            foreach (int ofs in new[] { 0x281A, 0x2D10 })
            {
                byte num_entries = data[ofs];
                if (num_entries > 30 || data[ofs + 1 + num_entries] != 0xFF)
                    c = false;
            }
            if (gs)
                return GameVersion.GS;
            if (c)
                return GameVersion.C;
            return GameVersion.Invalid;
        }
        /// <summary>Determines the type of 3rd gen save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG3SAV(byte[] data)
        {
            if (data.Length != SIZE_G3RAW && data.Length != SIZE_G3RAWHALF)
                return GameVersion.Invalid;

            // check the save file(s)
            int count = data.Length/SIZE_G3RAWHALF;
            for (int s = 0; s < count; s++)
            {
                int ofs = 0xE000*s;
                int[] BlockOrder = new int[14];
                for (int i = 0; i < 14; i++)
                    BlockOrder[i] = BitConverter.ToInt16(data, i * 0x1000 + 0xFF4 + ofs);

                if (BlockOrder.Any(i => i > 0xD || i < 0))
                    continue;

                // Detect RS/E/FRLG
                // Section 0 stores Game Code @ 0x00AC; 0 for RS, 1 for FRLG, else for Emerald

                int Block0 = Array.IndexOf(BlockOrder, 0);
                uint GameCode = BitConverter.ToUInt32(data, Block0 * 0x1000 + 0xAC + ofs);
                if (GameCode == uint.MaxValue)
                    return GameVersion.Unknown; // what a hack

                switch (GameCode)
                {
                    case 0: return GameVersion.RS;
                    case 1: return GameVersion.FRLG;
                    default: return GameVersion.E;
                }
            }
            return GameVersion.Invalid;
        }
        /// <summary>Determines the type of 3rd gen Box RS</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG3BOXSAV(byte[] data)
        {
            if (!new[] { SIZE_G3BOX, SIZE_G3BOXGCI }.Contains(data.Length))
                return GameVersion.Invalid;

            byte[] sav = data.Skip(data.Length - SIZE_G3BOX).Take(SIZE_G3BOX).ToArray();

            // Verify first checksum
            uint chk = 0; // initial value
            for (int j = 0x4; j < 0x1FFC; j += 2)
            {
                chk += (ushort)(sav[0x2000 + j] << 8);
                chk += sav[0x2000 + j + 1];
            }
            ushort chkA = (ushort)chk;
            ushort chkB = (ushort)(0xF004 - chkA);

            ushort CHK_A = (ushort)((sav[0x2000] << 8) | sav[0x2001]);
            ushort CHK_B = (ushort)((sav[0x2002] << 8) | sav[0x2003]);

            return CHK_A == chkA && CHK_B == chkB ? GameVersion.RSBOX : GameVersion.Invalid;
        }
        /// <summary>Determines the type of 3rd gen Colosseum</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG3COLOSAV(byte[] data)
        {
            if (!new[] { SIZE_G3COLO, SIZE_G3COLOGCI }.Contains(data.Length))
                return GameVersion.Invalid;

            // Check the intro bytes for each save slot
            byte[] slotintroColo = {0x01, 0x01, 0x00, 0x00};
            int offset = data.Length - SIZE_G3COLO;
            for (int i = 0; i < 3; i++)
            {
                var ident = data.Skip(0x6000 + offset + 0x1E000*i).Take(4).ToArray();
                if (!ident.SequenceEqual(slotintroColo))
                    return GameVersion.Invalid;
            }
            return GameVersion.COLO;
        }
        /// <summary>Determines the type of 3rd gen XD</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG3XDSAV(byte[] data)
        {
            if (!new[] {  SIZE_G3XD, SIZE_G3XDGCI }.Contains(data.Length))
                return GameVersion.Invalid;

            // Check the intro bytes for each save slot
            byte[] slotintroXD = { 0x01, 0x01, 0x01, 0x00 };
            int offset = data.Length - SIZE_G3XD;
            for (int i = 0; i < 2; i++)
            {
                var ident = data.Skip(0x6000 + offset + 0x28000 * i).Take(4).ToArray();
                if (!ident.SequenceEqual(slotintroXD))
                    return GameVersion.Invalid;
            }
            return GameVersion.XD;
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

            // Check the other save
            if (data.Skip(0xC0F4 + 0x40000).Take(10).SequenceEqual(new byte[] { 0x00, 0xC1, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.DP;
            if (data.Skip(0xCF20 + 0x40000).Take(10).SequenceEqual(new byte[] { 0x2C, 0xCF, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.Pt;
            if (data.Skip(0xF61C + 0x40000).Take(10).SequenceEqual(new byte[] { 0x28, 0xF6, 0x00, 0x00, 0x23, 0x06, 0x06, 0x20, 0x00, 0x00 }))
                return GameVersion.HGSS;

            return GameVersion.Invalid;
        }
        /// <summary>Determines the type of 4th gen Battle Revolution</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getIsG4BRSAV(byte[] data)
        {
            if (data.Length != SIZE_G4BR)
                return GameVersion.Invalid;

            byte[] sav = SAV4BR.DecryptPBRSaveData(data);

            bool valid = SAV4BR.VerifyChecksum(sav, 0, 0x1C0000, 0x1BFF80);
            valid &= SAV4BR.VerifyChecksum(sav, 0, 0x100, 8);
            valid &= SAV4BR.VerifyChecksum(sav, 0x1C0000, 0x1C0000, 0x1BFF80 + 0x1C0000);
            valid &= SAV4BR.VerifyChecksum(sav, 0x1C0000, 0x100, 0x1C0008);

            return  valid ? GameVersion.BATREV : GameVersion.Invalid;
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

        /// <summary>Determines the Version Grouping of an input Version ID</summary>
        /// <param name="Version">Version of which to determine the group</param>
        /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getVersionGroup(GameVersion Version)
        {
            switch (Version)
            {
                case GameVersion.CXD:
                    return GameVersion.CXD;

                case GameVersion.RBY:
                    return GameVersion.RBY;

                case GameVersion.GS:
                case GameVersion.C:
                    return GameVersion.GSC;

                case GameVersion.R:
                case GameVersion.S:
                    return GameVersion.RS;

                case GameVersion.E:
                    return GameVersion.E;

                case GameVersion.FR:
                case GameVersion.LG:
                    return GameVersion.FR;

                case GameVersion.D:
                case GameVersion.P:
                    return GameVersion.DP;

                case GameVersion.Pt:
                    return GameVersion.Pt;

                case GameVersion.HG:
                case GameVersion.SS:
                    return GameVersion.HGSS;

                case GameVersion.B:
                case GameVersion.W:
                    return GameVersion.BW;

                case GameVersion.B2:
                case GameVersion.W2:
                    return GameVersion.B2W2;

                case GameVersion.X:
                case GameVersion.Y:
                    return GameVersion.XY;

                case GameVersion.OR:
                case GameVersion.AS:
                    return GameVersion.ORAS;

                default:
                    return GameVersion.Invalid;
            }
        }

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="data">Save data from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile getVariantSAV(byte[] data)
        {
            switch (getSAVGeneration(data))
            {
                case 1:
                    return new SAV1(data);
                case 2:
                    return new SAV2(data);
                case 3:
                    return new SAV3(data);
                case (int)GameVersion.RSBOX:
                    return new SAV3RSBox(data);
                case (int)GameVersion.COLO:
                    return new SAV3Colosseum(data);
                case (int)GameVersion.XD:
                    return new SAV3XD(data);
                case 4:
                    return new SAV4(data);
                case (int)GameVersion.BATREV:
                    return new SAV4BR(data);
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

            // FBI
            if (path3DS != null && Directory.Exists(path = Path.Combine(path3DS, "fbi", "save")))
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
        internal static void CheckHeaderFooter(ref byte[] input, ref byte[] header, ref byte[] footer)
        {
            if (input.Length > SIZE_G4RAW) // DeSmuME Gen4/5 DSV
            {
                bool dsv = FOOTER_DSV.SequenceEqual(input.Skip(input.Length - FOOTER_DSV.Length));
                if (dsv)
                {
                    footer = input.Skip(SIZE_G4RAW).ToArray();
                    input = input.Take(footer.Length).ToArray();
                }
            }
            if (input.Length == SIZE_G3BOXGCI)
            {
                bool gci = HEADER_BOX.SequenceEqual(input.Take(HEADER_BOX.Length));
                if (gci)
                {
                    header = input.Take(SIZE_G3BOXGCI - SIZE_G3BOX).ToArray();
                    input = input.Skip(header.Length).ToArray();
                }
            }
            if (input.Length == SIZE_G3COLOGCI)
            {
                bool gci = HEADER_COLO.SequenceEqual(input.Take(HEADER_COLO.Length));
                if (gci)
                {
                    header = input.Take(SIZE_G3COLOGCI - SIZE_G3COLO).ToArray();
                    input = input.Skip(header.Length).ToArray();
                }
            }
            if (input.Length == SIZE_G3XDGCI)
            {
                bool gci = HEADER_XD.SequenceEqual(input.Take(HEADER_XD.Length));
                if (gci)
                {
                    header = input.Take(SIZE_G3XDGCI - SIZE_G3XD).ToArray();
                    input = input.Skip(header.Length).ToArray();
                }
            }
        }

        public static int getDexFormIndexBW(int species, int formct)
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
        public static int getDexFormIndexB2W2(int species, int formct)
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
                default: return getDexFormIndexBW(species, formct);
            }
        }
        public static int getDexFormIndexXY(int species, int formct)
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
                default: return getDexFormIndexB2W2(species, formct);
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

        public static int getCXDVersionID(int gen3version)
        {
            switch ((GameVersion)gen3version)
            {
                case GameVersion.FR: return 1;
                case GameVersion.LG: return 2;
                case GameVersion.S: return 8;
                case GameVersion.R: return 9;
                case GameVersion.E: return 10;
                case GameVersion.CXD: return 11;
                default: return 0;
            }
        }
        public static int getG3VersionID(int CXDversion)
        {
            switch (CXDversion)
            {
                case 1: return (int)GameVersion.FR;
                case 2: return (int)GameVersion.LG;
                case 8: return (int)GameVersion.S;
                case 9: return (int)GameVersion.R;
                case 10: return (int)GameVersion.E;
                case 11: return (int)GameVersion.CXD;
                default: return (int)GameVersion.Unknown;
            }
        }


        internal static byte[] DecryptGC(byte[] input, int start, int end, ushort[] keys)
        {
            byte[] output = (byte[])input.Clone();
            for (int ofs = start; ofs < end; ofs += 8)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    ushort val = BigEndian.ToUInt16(input, ofs + i * 2);
                    val -= keys[i];
                    BigEndian.GetBytes(val).CopyTo(output, ofs + i * 2);
                }
                keys = AdvanceGCKeys(keys);
            }
            return output;
        }
        internal static byte[] EncryptGC(byte[] input, int start, int end, ushort[] keys)
        {
            byte[] output = (byte[])input.Clone();
            for (int ofs = start; ofs < end; ofs += 8)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    ushort val = BigEndian.ToUInt16(input, ofs + i * 2);
                    val += keys[i];
                    BigEndian.GetBytes(val).CopyTo(output, ofs + i * 2);
                }
                keys = AdvanceGCKeys(keys);
            }
            return output;
        }

        public static ushort[] AdvanceGCKeys(ushort[] oldKeys)
        {
            oldKeys[0] += 0x43;
            oldKeys[1] += 0x29;
            oldKeys[2] += 0x17;
            oldKeys[3] += 0x13;

            ushort[] keys = new ushort[4];

            keys[0] = (ushort)(oldKeys[0] & 0xf         | oldKeys[1] << 4 & 0xf0    | oldKeys[2] << 8 & 0xf00   | oldKeys[3] << 12 & 0xf000);
            keys[1] = (ushort)(oldKeys[0] >> 4 & 0xf    | oldKeys[1] & 0xf0         | oldKeys[2] << 4 & 0xf00   | oldKeys[3] << 8 & 0xf000);
            keys[2] = (ushort)(oldKeys[2] & 0xf00       | (oldKeys[1] & 0xf00) >> 4 | (oldKeys[0] & 0xf00) >> 8 | oldKeys[3] << 4 & 0xf000);
            keys[3] = (ushort)(oldKeys[0] >> 12 & 0xf   | oldKeys[1] >> 8 & 0xf0    | oldKeys[2] >> 4 & 0xf00   | oldKeys[3] & 0xf000);

            return keys;
        }
    }
}
