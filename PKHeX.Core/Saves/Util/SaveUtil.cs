using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for <see cref="SaveFile"/> data loading and manipulation.
    /// </summary>
    public static class SaveUtil
    {
        public const int BEEF = 0x42454546;

        public const int SIZE_G7GG = 0x100000;
        public const int SIZE_G7USUM = 0x6CC00;
        public const int SIZE_G7SM = 0x6BE00;
        public const int SIZE_G6XY = 0x65600;
        public const int SIZE_G6ORAS = 0x76000;
        public const int SIZE_G6ORASDEMO = 0x5A00;
        public const int SIZE_G5RAW = 0x80000;
        public const int SIZE_G5BW = 0x24000;
        public const int SIZE_G5B2W2 = 0x26000;
        public const int SIZE_G4BR = 0x380000;
        public const int SIZE_G4RAW = 0x80000;
        public const int SIZE_G3BOX = 0x76000;
        public const int SIZE_G3COLO = 0x60000;
        public const int SIZE_G3XD = 0x56000;
        public const int SIZE_G3BOXGCI = SIZE_G3BOX + 0x40; // GCI data
        public const int SIZE_G3COLOGCI = SIZE_G3COLO + 0x40; // GCI data
        public const int SIZE_G3XDGCI = SIZE_G3XD + 0x40; // GCI data
        public const int SIZE_G3RAW = 0x20000;
        public const int SIZE_G3RAWHALF = 0x10000;
        public const int SIZE_G2RAW_U = 0x8000;
        public const int SIZE_G2VC_U = 0x8010;
        public const int SIZE_G2BAT_U = 0x802C;
        public const int SIZE_G2EMU_U = 0x8030;
        public const int SIZE_G2RAW_J = 0x10000;
        public const int SIZE_G2VC_J = 0x10010;
        public const int SIZE_G2BAT_J = 0x1002C;
        public const int SIZE_G2EMU_J = 0x10030;
        public const int SIZE_G1RAW = 0x8000;
        public const int SIZE_G1BAT = 0x802C;

        // Bank Binaries
        public const int SIZE_G7BANK = 0xACA48;
        public const int SIZE_G4BANK = 0x405C4;
        public const int SIZE_G4RANCH = 0x54000;
        public const int SIZE_G4RANCH_PLAT = 0x7C000;

        private static readonly HashSet<int> SIZES_2 = new HashSet<int>
        {
            SIZE_G2RAW_U, SIZE_G2VC_U, SIZE_G2BAT_U, SIZE_G2EMU_U, SIZE_G2RAW_J, SIZE_G2BAT_J, SIZE_G2EMU_J, SIZE_G2VC_J,
        };

        private static readonly HashSet<int> SIZES = new HashSet<int>(SIZES_2)
        {
            SIZE_G7SM, SIZE_G7USUM, SIZE_G7GG,
            SIZE_G6XY, SIZE_G6ORAS, SIZE_G6ORASDEMO,
            SIZE_G5RAW, SIZE_G5BW, SIZE_G5B2W2,
            SIZE_G4BR, SIZE_G4RAW,
            SIZE_G3BOX, SIZE_G3BOXGCI, SIZE_G3COLO, SIZE_G3COLOGCI, SIZE_G3XD, SIZE_G3XDGCI, SIZE_G3RAW, SIZE_G3RAWHALF,
            // SIZES_2 covers gen2 sizes since there's so many
            SIZE_G1RAW, SIZE_G1BAT,

            SIZE_G7BANK, SIZE_G4BANK, SIZE_G4RANCH, SIZE_G4RANCH_PLAT,
        };

        private static readonly byte[] FOOTER_DSV = Encoding.ASCII.GetBytes("|-DESMUME SAVE-|");
        internal static readonly string[] HEADER_COLO =   { "GC6J","GC6E","GC6P" }; // NTSC-J, NTSC-U, PAL
        internal static readonly string[] HEADER_XD =     { "GXXJ","GXXE","GXXP" }; // NTSC-J, NTSC-U, PAL
        internal static readonly string[] HEADER_RSBOX =  { "GPXJ","GPXE","GPXP" }; // NTSC-J, NTSC-U, PAL

        /// <summary>Determines the type of the provided save data.</summary>
        /// <param name="data">Save data of which to determine the origins of</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetSAVGeneration(byte[] data)
        {
            if (GetIsG1SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen1;
            if (GetIsG2SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen2;
            if (GetIsG3SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen3;
            if (GetIsG4SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen4;
            if (GetIsG5SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen5;
            if (GetIsG6SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen6;
            if (GetIsG7SAV(data) != GameVersion.Invalid)
                return GameVersion.Gen7;

            if (GetIsBelugaSAV(data) != GameVersion.Invalid)
                return GameVersion.GG;
            if (GetIsG3COLOSAV(data) != GameVersion.Invalid)
                return GameVersion.COLO;
            if (GetIsG3XDSAV(data) != GameVersion.Invalid)
                return GameVersion.XD;
            if (GetIsG3BOXSAV(data) != GameVersion.Invalid)
                return GameVersion.RSBOX;
            if (GetIsG4BRSAV(data) != GameVersion.Invalid)
                return GameVersion.BATREV;

            if (GetIsBank7(data)) // pokebank
                return GameVersion.USUM;
            if (GetIsBank4(data)) // pokestock
                return GameVersion.HGSS;
            if (GetIsBank3(data)) // pokestock
                return GameVersion.RS;
            if (GetIsRanch4(data)) // ranch
                return GameVersion.DP;

            return GameVersion.Invalid;
        }

        /// <summary>
        /// Determines if a Gen1/2 Pokémon List is Invalid
        /// </summary>
        /// <param name="data">Save data</param>
        /// <param name="offset">Offset the list starts at</param>
        /// <param name="listCount">Max count of Pokémon in the list</param>
        /// <returns>True if a valid list, False otherwise</returns>
        private static bool IsG12ListValid(byte[] data, int offset, int listCount)
        {
            byte num_entries = data[offset];
            return num_entries <= listCount && data[offset + 1 + num_entries] == 0xFF;
        }

        /// <summary>Checks to see if the data belongs to a Gen1 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG1SAV(byte[] data)
        {
            if (data.Length != SIZE_G1RAW && data.Length != SIZE_G1BAT)
                return GameVersion.Invalid;

            // Check if it's not an american save or a japanese save
            if (!(GetIsG1SAVU(data) || GetIsG1SAVJ(data)))
                return GameVersion.Invalid;
            // I can't actually detect which game version, because it's not stored anywhere.
            // If you can think of anything to do here, please implement :)
            return GameVersion.RBY;
        }

        /// <summary>Checks to see if the data belongs to an International Gen1 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid International save, False otherwise.</returns>
        private static bool GetIsG1SAVU(byte[] data)
        {
            return IsG12ListValid(data, 0x2F2C, 20) && IsG12ListValid(data, 0x30C0, 20);
        }

        /// <summary>Checks to see if the data belongs to a Japanese Gen1 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid Japanese save, False otherwise.</returns>
        internal static bool GetIsG1SAVJ(byte[] data)
        {
            return IsG12ListValid(data, 0x2ED5, 30) && IsG12ListValid(data, 0x302D, 30);
        }

        /// <summary>Checks to see if the data belongs to a Gen2 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG2SAV(byte[] data)
        {
            if (!SIZES_2.Contains(data.Length))
                return GameVersion.Invalid;

            // Check if it's not an International, Japanese, or Korean save file
            GameVersion result;
            if ((result = GetIsG2SAVU(data)) != GameVersion.Invalid)
                return result;
            if ((result = GetIsG2SAVJ(data)) != GameVersion.Invalid)
                return result;
            if ((result = GetIsG2SAVK(data)) != GameVersion.Invalid)
                return result;
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to an International (not Japanese or Korean) Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid International save, False otherwise.</returns>
        private static GameVersion GetIsG2SAVU(byte[] data)
        {
            if (IsG12ListValid(data, 0x288A, 20) && IsG12ListValid(data, 0x2D6C, 20))
                return GameVersion.GS;
            if (IsG12ListValid(data, 0x2865, 20) && IsG12ListValid(data, 0x2D10, 20))
                return GameVersion.C;
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Japanese Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid Japanese save, False otherwise.</returns>
        internal static GameVersion GetIsG2SAVJ(byte[] data)
        {
            if (!IsG12ListValid(data, 0x2D10, 30))
                return GameVersion.Invalid;
            if (IsG12ListValid(data, 0x283E, 30))
                return GameVersion.GS;
            if (IsG12ListValid(data, 0x281A, 30))
                return GameVersion.C;
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Korean Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid Korean save, False otherwise.</returns>
        internal static GameVersion GetIsG2SAVK(byte[] data)
        {
            if (IsG12ListValid(data, 0x2DAE, 20) && IsG12ListValid(data, 0x28CC, 20))
                return GameVersion.GS;
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG3SAV(byte[] data)
        {
            if (data.Length != SIZE_G3RAW && data.Length != SIZE_G3RAWHALF)
                return GameVersion.Invalid;

            // check the save file(s)
            int count = data.Length/SIZE_G3RAWHALF;
            for (int s = 0; s < count; s++)
            {
                const int blockcount = 14;
                const int blocksize = 0x1000;
                int ofs = blockcount * blocksize * s;
                int[] BlockOrder = new int[blockcount];
                for (int i = 0; i < BlockOrder.Length; i++)
                    BlockOrder[i] = BitConverter.ToUInt16(data, (i * blocksize) + 0xFF4 + ofs);

                if (Array.FindIndex(BlockOrder, i => i > 0xD) >= 0) // invalid block ID
                    continue;

                int Block0 = Array.IndexOf(BlockOrder, 0);

                // Sometimes not all blocks are present (start of game), yielding multiple block0's.
                // Real 0th block comes before block1.
                if (BlockOrder[0] == 1 && Block0 != BlockOrder.Length - 1)
                    continue;
                if (Array.FindIndex(BlockOrder, v => v != 0) < 0) // all blocks are 0
                    continue;
                // Detect RS/E/FRLG
                return SAV3.GetVersion(data, (blocksize * Block0) + ofs);
            }
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 Box RS save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG3BOXSAV(byte[] data)
        {
            if (data.Length != SIZE_G3BOX && data.Length != SIZE_G3BOXGCI)
                return GameVersion.Invalid;

            byte[] sav = data;

            // Verify first checksum
            ushort chk = 0; // initial value
            var ofs = data.Length - SIZE_G3BOX + 0x2000;
            for (int i = 0x4; i < 0x1FFC; i += 2)
                chk += BigEndian.ToUInt16(sav, ofs + i);

            ushort chkA = chk;
            ushort chkB = (ushort)(0xF004 - chkA);

            ushort CHK_A = BigEndian.ToUInt16(sav, ofs + 0);
            ushort CHK_B = BigEndian.ToUInt16(sav, ofs + 2);

            return CHK_A == chkA && CHK_B == chkB ? GameVersion.RSBOX : GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Colosseum save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG3COLOSAV(byte[] data)
        {
            if (data.Length != SIZE_G3COLO && data.Length != SIZE_G3COLOGCI)
                return GameVersion.Invalid;

            // Check the intro bytes for each save slot
            int offset = data.Length - SIZE_G3COLO;
            for (int i = 0; i < 3; i++)
            {
                var ofs = 0x6000 + offset + (0x1E000 * i);
                if (BitConverter.ToUInt32(data, ofs) != 0x00000101)
                    return GameVersion.Invalid;
            }
            return GameVersion.COLO;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 XD save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG3XDSAV(byte[] data)
        {
            if (data.Length != SIZE_G3XD && data.Length != SIZE_G3XDGCI)
                return GameVersion.Invalid;

            // Check the intro bytes for each save slot
            int offset = data.Length - SIZE_G3XD;
            for (int i = 0; i < 2; i++)
            {
                var ofs = 0x6000 + offset + (0x28000 * i);
                if ((BitConverter.ToUInt32(data, ofs) & 0xFFFE_FFFF) != 0x00000101)
                    return GameVersion.Invalid;
            }
            return GameVersion.XD;
        }

        /// <summary>Checks to see if the data belongs to a Gen4 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG4SAV(byte[] data)
        {
            if (data.Length != SIZE_G4RAW)
                return GameVersion.Invalid;

            // The block footers contain a u32 'size' followed by a u32 binary-coded-decimal timestamp(?)
            // Korean savegames have a different timestamp from other localizations.
            bool validSequence(int offset)
            {
                var size = BitConverter.ToUInt32(data, offset - 0xC);
                if (size != (offset & 0xFFFF))
                    return false;
                var sdk = BitConverter.ToUInt32(data, offset - 0x8);

                const int DATE_INT = 0x20060623;
                const int DATE_KO  = 0x20070903;
                return sdk == DATE_INT || sdk == DATE_KO;
            }

            // Check the other save -- first save is done to the latter half of the binary.
            // The second save should be all that is needed to check.
            if (validSequence(0x4C100))
                return GameVersion.DP;
            if (validSequence(0x4CF2C))
                return GameVersion.Pt;
            if (validSequence(0x4F628))
                return GameVersion.HGSS;

            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen4 Battle Revolution save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG4BRSAV(byte[] data)
        {
            if (data.Length != SIZE_G4BR)
                return GameVersion.Invalid;

            byte[] sav = SAV4BR.DecryptPBRSaveData(data);
            return SAV4BR.IsChecksumsValid(sav) ? GameVersion.BATREV : GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen5 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        internal static GameVersion GetIsG5SAV(byte[] data)
        {
            if (data.Length != SIZE_G5RAW)
                return GameVersion.Invalid;

            // check the checksum block validity; nobody would normally modify this region
            ushort chk1 = BitConverter.ToUInt16(data, SIZE_G5BW - 0x100 + 0x8C + 0xE);
            ushort actual1 = Checksums.CRC16_CCITT(data, SIZE_G5BW - 0x100, 0x8C);
            if (chk1 == actual1)
                return GameVersion.BW;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE_G5B2W2 - 0x100 + 0x94 + 0xE);
            ushort actual2 = Checksums.CRC16_CCITT(data, SIZE_G5B2W2 - 0x100, 0x94);
            if (chk2 == actual2)
                return GameVersion.B2W2;
            return GameVersion.Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen6 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG6SAV(byte[] data)
        {
            if (data.Length != SIZE_G6XY && data.Length != SIZE_G6ORAS && data.Length != SIZE_G6ORASDEMO)
                return GameVersion.Invalid;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return GameVersion.Invalid;

            if (data.Length == SIZE_G6XY)
                return GameVersion.XY;
            if (data.Length == SIZE_G6ORAS)
                return GameVersion.ORAS;
            return GameVersion.ORASDEMO; // least likely
        }

        /// <summary>Checks to see if the data belongs to a Gen7 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG7SAV(byte[] data)
        {
            if (data.Length != SIZE_G7SM && data.Length != SIZE_G7USUM)
                return GameVersion.Invalid;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return GameVersion.Invalid;

            return data.Length == SIZE_G7SM ? GameVersion.SM : GameVersion.USUM;
        }

        /// <summary>Determines if the input data belongs to a <see cref="SAV7b"/> save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsBelugaSAV(byte[] data)
        {
            if (data.Length != SIZE_G7GG)
                return GameVersion.Invalid;

            const int actualLength = 0xB8800;
            if (BitConverter.ToUInt32(data, actualLength - 0x1F0) != BEEF) // beef table start
                return GameVersion.Invalid;
            if (BitConverter.ToUInt16(data, actualLength - 0x200 + 0xB0) != 0x13) // check a block number to double check
                return GameVersion.Invalid;

            return GameVersion.GG;
        }

        private static bool GetIsBank7(byte[] data) => data.Length == SIZE_G7BANK && data[0] != 0;
        private static bool GetIsBank4(byte[] data) => data.Length == SIZE_G4BANK && BitConverter.ToUInt32(data, 0x3FC00) != 0; // box name present
        private static bool GetIsBank3(byte[] data) => data.Length == SIZE_G4BANK && BitConverter.ToUInt32(data, 0x3FC00) == 0; // size collision with ^
        private static bool GetIsRanch4(byte[] data) => (data.Length == SIZE_G4RANCH && BigEndian.ToUInt32(data, 0x22AC) != 0) || (data.Length == SIZE_G4RANCH_PLAT && BigEndian.ToUInt32(data, 0x268C) != 0);

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="path">File location from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile GetVariantSAV(string path)
        {
            var data = File.ReadAllBytes(path);
            var sav = GetVariantSAV(data);
            sav?.SetFileInfo(path);
            return sav;
        }

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="data">Save data from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile GetVariantSAV(byte[] data)
        {
            // Pre-check for header/footer signatures
            CheckHeaderFooter(ref data, out var header, out var footer);
            var sav = GetVariantSAVInternal(data);
            if (sav == null)
                return null;
            sav.Header = header;
            sav.Footer = footer;
            return sav;
        }

        private static SaveFile GetVariantSAVInternal(byte[] data)
        {
            switch (GetSAVGeneration(data))
            {
                // Main Games
                case GameVersion.Gen1: return new SAV1(data);
                case GameVersion.Gen2: return new SAV2(data);
                case GameVersion.Gen3: return new SAV3(data);
                case GameVersion.Gen4: return new SAV4(data);
                case GameVersion.Gen5: return new SAV5(data);
                case GameVersion.Gen6: return new SAV6(data);
                case GameVersion.Gen7: return new SAV7(data);

                // Side Games
                case GameVersion.COLO:   return new SAV3Colosseum(data);
                case GameVersion.XD:     return new SAV3XD(data);
                case GameVersion.RSBOX:  return new SAV3RSBox(data);
                case GameVersion.BATREV: return new SAV4BR(data);
                case GameVersion.GG:     return new SAV7b(data);

                // Bulk Storage
                case GameVersion.RS:     return new Bank3(data);
                case GameVersion.DP:     return new SAV4Ranch(data);
                case GameVersion.HGSS:   return new Bank4(data);
                case GameVersion.USUM:   return Bank7.GetBank7(data);

                // No pattern matched
                default: return null;
            }
        }

        public static SaveFile GetVariantSAV(SAV3GCMemoryCard MC)
        {
            // Pre-check for header/footer signatures
            SaveFile sav;
            byte[] data = MC.SelectedSaveData;
            CheckHeaderFooter(ref data, out var header, out var footer);

            switch (MC.SelectedGameVersion)
            {
                // Side Games
                case GameVersion.COLO: sav = new SAV3Colosseum(data, MC); break;
                case GameVersion.XD: sav = new SAV3XD(data, MC); break;
                case GameVersion.RSBOX: sav = new SAV3RSBox(data, MC); break;

                // No pattern matched
                default: return null;
            }
            sav.Header = header;
            sav.Footer = footer;
            return sav;
        }

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="Game">Version to create the save file for.</param>
        /// <param name="OT">Trainer Name</param>
        /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
        public static SaveFile GetBlankSAV(GameVersion Game, string OT)
        {
            var SAV = GetBlankSAV(Game);
            if (SAV == null)
                return null;

            SAV.Game = (int)Game;
            SAV.OT = OT;

            // Secondary Properties may not be used but can be filled in as template.
            SAV.TID = 12345;
            SAV.SID = 54321;
            SAV.Language = (int)LanguageID.English; // English
            SAV.Country = 49; // USA
            SAV.SubRegion = 7; // CA
            SAV.ConsoleRegion = 1; // Americas

            return SAV;
        }

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="Game">Version to create the save file for.</param>
        /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
        private static SaveFile GetBlankSAV(GameVersion Game)
        {
            switch (Game)
            {
                case GameVersion.RD: case GameVersion.BU: case GameVersion.GN: case GameVersion.YW:
                case GameVersion.RBY:
                    return new SAV1(versionOverride: Game);

                case GameVersion.GD: case GameVersion.SV: case GameVersion.C:
                case GameVersion.GS: case GameVersion.GSC:
                    return new SAV2();

                case GameVersion.R: case GameVersion.S: case GameVersion.E: case GameVersion.FR: case GameVersion.LG:
                    return new SAV3(versionOverride: Game);
                case GameVersion.FRLG:
                    return new SAV3(versionOverride: GameVersion.FR);
                case GameVersion.RS:
                    return new SAV3(versionOverride: GameVersion.R);
                case GameVersion.RSE:
                    return new SAV3(versionOverride: GameVersion.E);

                case GameVersion.CXD:
                case GameVersion.COLO:
                    return new SAV3Colosseum();
                case GameVersion.XD:
                    return new SAV3XD();
                case GameVersion.RSBOX:
                    return new SAV3RSBox();

                case GameVersion.D: case GameVersion.P: case GameVersion.DP:
                case GameVersion.DPPt:
                    return new SAV4(new byte[SIZE_G4RAW], GameVersion.DP);
                case GameVersion.Pt:
                    return new SAV4(new byte[SIZE_G4RAW], GameVersion.Pt);
                case GameVersion.HG: case GameVersion.SS: case GameVersion.HGSS:
                    return new SAV4(new byte[SIZE_G4RAW], GameVersion.HGSS);

                case GameVersion.B: case GameVersion.W: case GameVersion.BW:
                    return new SAV5(new byte[SIZE_G5RAW], GameVersion.BW);
                case GameVersion.B2: case GameVersion.W2: case GameVersion.B2W2:
                    return new SAV5(new byte[SIZE_G5RAW], GameVersion.B2W2);

                case GameVersion.X: case GameVersion.Y: case GameVersion.XY:
                    return new SAV6(new byte[SIZE_G6XY]);
                case GameVersion.ORASDEMO:
                    return new SAV6(new byte[SIZE_G6ORASDEMO]);
                case GameVersion.OR: case GameVersion.AS: case GameVersion.ORAS:
                    return new SAV6(new byte[SIZE_G6ORAS]);

                case GameVersion.SN: case GameVersion.MN: case GameVersion.SM:
                    return new SAV7(new byte[SIZE_G7SM]);
                case GameVersion.US: case GameVersion.UM: case GameVersion.USUM:
                    return new SAV7(new byte[SIZE_G7USUM]);
                case GameVersion.GO:
                case GameVersion.GP: case GameVersion.GE: case GameVersion.GG:
                    return new SAV7b(new byte[SIZE_G7GG]);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="generation">Generation of the Save File.</param>
        /// <param name="OT">Trainer Name</param>
        /// <returns>Save File for that generation.</returns>
        public static SaveFile GetBlankSAV(int generation, string OT)
        {
            var ver = GameUtil.GetVersion(generation);
            return GetBlankSAV(ver, OT);
        }

        /// <summary>
        /// Retrieves possible save file paths from the provided <see cref="folderPath"/>.
        /// </summary>
        /// <param name="folderPath">Folder to look within</param>
        /// <param name="deep">Search all subfolders</param>
        /// <param name="result">If this function returns true, full path of all <see cref="SaveFile"/> that match criteria. If this function returns false, the error message, or null if the directory could not be found</param>
        /// <returns>Boolean indicating whether or not operation was successful.</returns>
        public static bool GetSavesFromFolder(string folderPath, bool deep, out IEnumerable<string> result)
        {
            if (!Directory.Exists(folderPath))
            {
                result = null;
                return false;
            }
            try
            {
                var searchOption = deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var files = Directory.EnumerateFiles(folderPath, "*", searchOption);
                int safelen(string file)
                {
                    try { return (int) new FileInfo(file).Length; }
                    catch { return -1; } // Bad File / Locked
                }
                result = files.Where(f => IsSizeValid(safelen(f)));
                return true;
            }
            catch (ArgumentException)
            {
                result = new[] { MsgFileLoadFailAuto + Environment.NewLine + folderPath, MsgFileLoadFailAutoAdvise + Environment.NewLine + MsgFileLoadFailAutoCause };
                return false;
            }
        }

        /// <summary>
        /// Determines whether the save data size is valid for autodetecting saves.
        /// </summary>
        /// <param name="size">Size in bytes of the save data</param>
        /// <returns>A boolean indicating whether or not the save data size is valid.</returns>
        public static bool IsSizeValid(int size) => SIZES.Contains(size);

        /// <summary>
        /// Checks the provided <see cref="input"/> and pulls out any <see cref="header"/> and/or <see cref="footer"/> arrays.
        /// </summary>
        /// <param name="input">Input byte array to strip</param>
        /// <param name="header">Header data</param>
        /// <param name="footer">Footer data</param>
        private static void CheckHeaderFooter(ref byte[] input, out byte[] header, out byte[] footer)
        {
            header = Array.Empty<byte>(); footer = Array.Empty<byte>();
            if ((input.Length & 0xFF) == 0) // catch most non-header/footers
                return;
            if (input.Length > SIZE_G4RAW) // DeSmuME Gen4/5 DSV
            {
                if (input.Length == 0x800A4) // Action Replay
                {
                    header = GetSubsection(input, 0, 0xA4);
                    input = GetSubsection(input, 0xA4);
                    return;
                }
                int start = input.Length - FOOTER_DSV.Length;
                for (int i = 0; i < FOOTER_DSV.Length; i++)
                {
                    if (FOOTER_DSV[i] != input[start + i])
                        return;
                }

                footer = GetSubsection(input, SIZE_G4RAW);
                input = GetSubsection(input, 0, SIZE_G4RAW);
            }
            else if (input.Length == SIZE_G3BOXGCI)
            {
                if (!IsGameMatchHeader(HEADER_RSBOX, input))
                    return; // not gci
                header = GetSubsection(input, 0, SIZE_G3BOXGCI - SIZE_G3BOX);
                input = GetSubsection(input, header.Length);
            }
            else if (input.Length == SIZE_G3COLOGCI)
            {
                if (!IsGameMatchHeader(HEADER_COLO, input))
                    return; // not gci
                header = GetSubsection(input, 0, SIZE_G3COLOGCI - SIZE_G3COLO);
                input = GetSubsection(input, header.Length);
            }
            else if (input.Length == SIZE_G3XDGCI)
            {
                if (!IsGameMatchHeader(HEADER_XD, input))
                    return; // not gci
                header = GetSubsection(input, 0, SIZE_G3XDGCI - SIZE_G3XD);
                input = GetSubsection(input, header.Length);
            }

            byte[] GetSubsection(byte[] data, int start, int length = -1)
            {
                if (length < 0)
                    length = data.Length - start;
                byte[] result = new byte[length];
                Buffer.BlockCopy(data, start, result, 0, length);
                return result;
            }
            bool IsGameMatchHeader(IEnumerable<string> headers, byte[] data) => headers.Contains(Encoding.ASCII.GetString(data, 0, 4));
        }

        /// <summary>
        /// Force loads the provided <see cref="sav"/> to the requested <see cref="ver"/>.
        /// </summary>
        /// <param name="sav">SaveFile data to force</param>
        /// <param name="ver">Version to retrieve for</param>
        /// <returns>New <see cref="SaveFile"/> object.</returns>
        public static SaveFile GetG3SaveOverride(SaveFile sav, GameVersion ver)
        {
            switch (ver) // Reset save file info
            {
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.RS:
                    return new SAV3(sav.BAK, GameVersion.RS);
                case GameVersion.E:
                    return new SAV3(sav.BAK, GameVersion.E);
                case GameVersion.FRLG:
                case GameVersion.FR:
                case GameVersion.LG:
                    return new SAV3(sav.BAK, GameVersion.FRLG);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="PersonalTable"/> for a Gen3 save file.
        /// </summary>
        /// <param name="ver">Version to retrieve for</param>
        /// <returns>Reference to the <see cref="PersonalTable"/>.</returns>
        public static PersonalTable GetG3Personal(GameVersion ver)
        {
            switch (ver)
            {
                case GameVersion.FRLG:
                case GameVersion.FR:
                    return PersonalTable.FR;
                case GameVersion.LG:
                    return PersonalTable.LG;
                case GameVersion.E:
                    return PersonalTable.E;
                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.RS:
                    return PersonalTable.RS;
                default:
                    return null;
            }
        }
    }
}
