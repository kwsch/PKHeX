using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for <see cref="SaveFile"/> data loading and manipulation.
    /// </summary>
    public static class SaveUtil
    {
        public const int BEEF = 0x42454546;

        public const int SIZE_G8SWSH = 0x1716B3; // 1.0
        public const int SIZE_G8SWSH_1 = 0x17195E; // 1.0 -> 1.1
        public const int SIZE_G8SWSH_2 = 0x180B19; // 1.0 -> 1.1 -> 1.2
        public const int SIZE_G8SWSH_2B = 0x180AD0; // 1.0 -> 1.2
        public const int SIZE_G8SWSH_3 = 0x1876B1; // 1.0 -> 1.1 -> 1.2 -> 1.3
        public const int SIZE_G8SWSH_3A = 0x187693; // 1.0 -> 1.1 -> 1.3
        public const int SIZE_G8SWSH_3B = 0x187668; // 1.0 -> 1.2 -> 1.3
        public const int SIZE_G8SWSH_3C = 0x18764A; // 1.0 -> 1.3

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
        public const int SIZE_G3RAW = 0x20000;
        public const int SIZE_G3RAWHALF = 0x10000;
        public const int SIZE_G2STAD = 0x20000; // same as G3RAW_U
        public const int SIZE_G2RAW_U = 0x8000;
        public const int SIZE_G2VC_U = 0x8010;
        public const int SIZE_G2BAT_U = 0x802C;
        public const int SIZE_G2EMU_U = 0x8030;
        public const int SIZE_G2RAW_J = 0x10000;
        public const int SIZE_G2VC_J = 0x10010;
        public const int SIZE_G2BAT_J = 0x1002C;
        public const int SIZE_G2EMU_J = 0x10030;
        public const int SIZE_G1STAD = 0x20000; // same as G3RAW_U
        public const int SIZE_G1STADJ = 0x8000; // same as G1RAW
        public const int SIZE_G1RAW = 0x8000;
        public const int SIZE_G1BAT = 0x802C;

        // Bank Binaries
        public const int SIZE_G7BANK = 0xACA48;
        public const int SIZE_G4BANK = 0x405C4;
        public const int SIZE_G4RANCH = 0x54000;
        public const int SIZE_G4RANCH_PLAT = 0x7C000;

        private static readonly SaveHandlerGCI DolphinHandler = new();

        /// <summary>
        /// Pre-formatters for loading save files from non-standard formats (e.g. emulators).
        /// </summary>
        public static readonly ICollection<ISaveHandler> Handlers = new List<ISaveHandler>
        {
            DolphinHandler,
            new SaveHandlerDeSmuME(),
            new SaveHandlerARDS(),
        };

        internal static readonly HashSet<int> SizesSWSH = new()
        {
            SIZE_G8SWSH, SIZE_G8SWSH_1, SIZE_G8SWSH_2, SIZE_G8SWSH_2B, SIZE_G8SWSH_3, SIZE_G8SWSH_3A, SIZE_G8SWSH_3B, SIZE_G8SWSH_3C,
        };

        private static readonly HashSet<int> SizesGen2 = new()
        {
            SIZE_G2RAW_U, SIZE_G2VC_U, SIZE_G2BAT_U, SIZE_G2EMU_U, SIZE_G2RAW_J, SIZE_G2BAT_J, SIZE_G2EMU_J, SIZE_G2VC_J,
        };

        private static readonly HashSet<int> Sizes = new(SizesGen2.Concat(SizesSWSH))
        {
            // SizesSWSH covers gen8 sizes since there's so many
            SIZE_G7SM, SIZE_G7USUM, SIZE_G7GG,
            SIZE_G6XY, SIZE_G6ORAS, SIZE_G6ORASDEMO,
            SIZE_G5RAW, SIZE_G5BW, SIZE_G5B2W2,
            SIZE_G4BR, SIZE_G4RAW,
            SIZE_G3BOX, SIZE_G3COLO, SIZE_G3XD, SIZE_G3RAW, SIZE_G3RAWHALF,
            // SizesGen2 covers gen2 sizes since there's so many
            SIZE_G1RAW, SIZE_G1BAT,

            SIZE_G7BANK, SIZE_G4BANK, SIZE_G4RANCH, SIZE_G4RANCH_PLAT,
        };

        /// <summary>Determines the type of the provided save data.</summary>
        /// <param name="data">Save data of which to determine the origins of</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetSAVType(byte[] data)
        {
            GameVersion ver;
            if ((ver = GetIsG1SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG2SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG3SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG4SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG5SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG6SAV(data)) != Invalid)
                return ver;
            if ((ver = GetIsG7SAV(data)) != Invalid)
                return ver;

            if (GetIsBelugaSAV(data) != Invalid)
                return GG;
            if (GetIsG3COLOSAV(data) != Invalid)
                return COLO;
            if (GetIsG3XDSAV(data) != Invalid)
                return XD;
            if (GetIsG3BOXSAV(data) != Invalid)
                return RSBOX;
            if (GetIsG4BRSAV(data) != Invalid)
                return BATREV;

            if (GetIsBank7(data)) // pokebank
                return Gen7;
            if (GetIsBank4(data)) // pokestock
                return Gen4;
            if (GetIsBank3(data)) // pokestock
                return Gen3;
            if (GetIsRanch4(data)) // ranch
                return DPPt;
            if (SAV2Stadium.IsStadium(data))
                return Stadium2;
            if (SAV1Stadium.IsStadium(data))
                return Stadium;
            if (SAV1StadiumJ.IsStadium(data))
                return StadiumJ;

            if ((ver = GetIsG8SAV(data)) != Invalid)
                return ver;

            return Invalid;
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
            if (data.Length is not (SIZE_G1RAW or SIZE_G1BAT))
                return Invalid;

            // Check if it's not an american save or a japanese save
            if (!(GetIsG1SAVU(data) || GetIsG1SAVJ(data)))
                return Invalid;
            // I can't actually detect which game version, because it's not stored anywhere.
            // If you can think of anything to do here, please implement :)
            return RBY;
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
            if (!SizesGen2.Contains(data.Length))
                return Invalid;

            // Check if it's not an International, Japanese, or Korean save file
            GameVersion result;
            if ((result = GetIsG2SAVU(data)) != Invalid)
                return result;
            if ((result = GetIsG2SAVJ(data)) != Invalid)
                return result;
            if ((result = GetIsG2SAVK(data)) != Invalid)
                return result;
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to an International (not Japanese or Korean) Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid International save, False otherwise.</returns>
        private static GameVersion GetIsG2SAVU(byte[] data)
        {
            if (IsG12ListValid(data, 0x288A, 20) && IsG12ListValid(data, 0x2D6C, 20))
                return GS;
            if (IsG12ListValid(data, 0x2865, 20) && IsG12ListValid(data, 0x2D10, 20))
                return C;
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Japanese Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid Japanese save, False otherwise.</returns>
        internal static GameVersion GetIsG2SAVJ(byte[] data)
        {
            if (!IsG12ListValid(data, 0x2D10, 30))
                return Invalid;
            if (IsG12ListValid(data, 0x283E, 30))
                return GS;
            if (IsG12ListValid(data, 0x281A, 30))
                return C;
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Korean Gen2 save</summary>
        /// <param name="data">Save data of which to determine the region</param>
        /// <returns>True if a valid Korean save, False otherwise.</returns>
        internal static GameVersion GetIsG2SAVK(byte[] data)
        {
            if (IsG12ListValid(data, 0x2DAE, 20) && IsG12ListValid(data, 0x28CC, 20))
                return GS;
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG3SAV(byte[] data)
        {
            if (data.Length is not (SIZE_G3RAW or SIZE_G3RAWHALF))
                return Invalid;

            // check the save file(s)
            int count = data.Length/SIZE_G3RAWHALF;
            for (int s = 0; s < count; s++)
            {
                const int blockCount = 14;
                const int blockSize = 0x1000;
                int ofs = blockCount * blockSize * s;
                int[] order = new int[blockCount];
                for (int i = 0; i < order.Length; i++)
                    order[i] = BitConverter.ToUInt16(data, (i * blockSize) + 0xFF4 + ofs);

                if (Array.FindIndex(order, i => i > 0xD) >= 0) // invalid block ID
                    continue;

                int block0 = Array.IndexOf(order, 0);

                // Sometimes not all blocks are present (start of game), yielding multiple block0's.
                // Real 0th block comes before block1.
                if (order[0] == 1 && block0 != order.Length - 1)
                    continue;
                if (Array.FindIndex(order, v => v != 0) < 0) // all blocks are 0
                    continue;
                // Detect RS/E/FRLG
                return SAV3.GetVersion(data, (blockSize  * block0) + ofs);
            }
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 Box RS save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG3BOXSAV(byte[] data)
        {
            if (data.Length is not SIZE_G3BOX)
                return Invalid;

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

            return CHK_A == chkA && CHK_B == chkB ? RSBOX : Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Colosseum save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG3COLOSAV(byte[] data)
        {
            if (data.Length is not SIZE_G3COLO)
                return Invalid;

            // Check the intro bytes for each save slot
            int offset = data.Length - SIZE_G3COLO;
            for (int i = 0; i < 3; i++)
            {
                var ofs = 0x6000 + offset + (0x1E000 * i);
                if (BitConverter.ToUInt32(data, ofs) != 0x00000101)
                    return Invalid;
            }
            return COLO;
        }

        /// <summary>Checks to see if the data belongs to a Gen3 XD save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG3XDSAV(byte[] data)
        {
            if (data.Length is not SIZE_G3XD)
                return Invalid;

            // Check the intro bytes for each save slot
            int offset = data.Length - SIZE_G3XD;
            for (int i = 0; i < 2; i++)
            {
                var ofs = 0x6000 + offset + (0x28000 * i);
                if ((BitConverter.ToUInt32(data, ofs) & 0xFFFE_FFFF) != 0x00000101)
                    return Invalid;
            }
            return XD;
        }

        /// <summary>Checks to see if the data belongs to a Gen4 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG4SAV(byte[] data)
        {
            if (data.Length != SIZE_G4RAW)
                return Invalid;

            // The block footers contain a u32 'size' followed by a u32 binary-coded-decimal timestamp(?)
            // Korean saves have a different timestamp from other localizations.
            bool validSequence(int offset)
            {
                var size = BitConverter.ToUInt32(data, offset - 0xC);
                if (size != (offset & 0xFFFF))
                    return false;
                var sdk = BitConverter.ToUInt32(data, offset - 0x8);

                const int DATE_INT = 0x20060623;
                const int DATE_KO  = 0x20070903;
                return sdk is DATE_INT or DATE_KO;
            }

            // Check the other save -- first save is done to the latter half of the binary.
            // The second save should be all that is needed to check.
            if (validSequence(0x4C100))
                return DP;
            if (validSequence(0x4CF2C))
                return Pt;
            if (validSequence(0x4F628))
                return HGSS;

            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen4 Battle Revolution save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG4BRSAV(byte[] data)
        {
            if (data.Length != SIZE_G4BR)
                return Invalid;

            byte[] sav = SAV4BR.DecryptPBRSaveData(data);
            return SAV4BR.IsChecksumsValid(sav) ? BATREV : Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen5 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG5SAV(byte[] data)
        {
            if (data.Length != SIZE_G5RAW)
                return Invalid;

            // check the checksum block validity; nobody would normally modify this region
            ushort chk1 = BitConverter.ToUInt16(data, SIZE_G5BW - 0x100 + 0x8C + 0xE);
            ushort actual1 = Checksums.CRC16_CCITT(data, SIZE_G5BW - 0x100, 0x8C);
            if (chk1 == actual1)
                return BW;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE_G5B2W2 - 0x100 + 0x94 + 0xE);
            ushort actual2 = Checksums.CRC16_CCITT(data, SIZE_G5B2W2 - 0x100, 0x94);
            if (chk2 == actual2)
                return B2W2;
            return Invalid;
        }

        /// <summary>Checks to see if the data belongs to a Gen6 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG6SAV(byte[] data)
        {
            if (data.Length is not (SIZE_G6XY or SIZE_G6ORAS or SIZE_G6ORASDEMO))
                return Invalid;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return Invalid;

            return data.Length switch
            {
                SIZE_G6XY => XY,
                SIZE_G6ORAS => ORAS,
                _ => ORASDEMO  // least likely
            };
        }

        /// <summary>Checks to see if the data belongs to a Gen7 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG7SAV(byte[] data)
        {
            if (data.Length is not (SIZE_G7SM or SIZE_G7USUM))
                return Invalid;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return Invalid;

            return data.Length == SIZE_G7SM ? SM : USUM;
        }

        /// <summary>Determines if the input data belongs to a <see cref="SAV7b"/> save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsBelugaSAV(byte[] data)
        {
            if (data.Length != SIZE_G7GG)
                return Invalid;

            const int actualLength = 0xB8800;
            if (BitConverter.ToUInt32(data, actualLength - 0x1F0) != BEEF) // beef table start
                return Invalid;
            if (BitConverter.ToUInt16(data, actualLength - 0x200 + 0xB0) != 0x13) // check a block number to double check
                return Invalid;

            return GG;
        }

        /// <summary>Checks to see if the data belongs to a Gen7 save</summary>
        /// <param name="data">Save data of which to determine the type</param>
        /// <returns>Version Identifier or Invalid if type cannot be determined.</returns>
        private static GameVersion GetIsG8SAV(byte[] data)
        {
            if (!SizesSWSH.Contains(data.Length))
                return Invalid;

            return SwishCrypto.GetIsHashValid(data) ? SWSH : Invalid;
        }

        private static bool GetIsBank7(byte[] data) => data.Length == SIZE_G7BANK && data[0] != 0;
        private static bool GetIsBank4(byte[] data) => data.Length == SIZE_G4BANK && BitConverter.ToUInt32(data, 0x3FC00) != 0; // box name present
        private static bool GetIsBank3(byte[] data) => data.Length == SIZE_G4BANK && BitConverter.ToUInt32(data, 0x3FC00) == 0; // size collision with ^
        private static bool GetIsRanchDP(byte[] data) => data.Length == SIZE_G4RANCH && BigEndian.ToUInt32(data, 0x22AC) != 0;
        private static bool GetIsRanchPlat(byte[] data) => data.Length == SIZE_G4RANCH_PLAT && BigEndian.ToUInt32(data, 0x268C) != 0;
        private static bool GetIsRanch4(byte[] data) => GetIsRanchDP(data) || GetIsRanchPlat(data);

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="path">File location from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile? GetVariantSAV(string path)
        {
            var data = File.ReadAllBytes(path);
            var sav = GetVariantSAV(data);
            if (sav == null)
                return null;
            sav.Metadata.SetExtraInfo(path);
            return sav;
        }

        /// <summary>Creates an instance of a SaveFile using the given save data.</summary>
        /// <param name="data">Save data from which to create a SaveFile.</param>
        /// <returns>An appropriate type of save file for the given data, or null if the save data is invalid.</returns>
        public static SaveFile? GetVariantSAV(byte[] data)
        {
            var sav = GetVariantSAVInternal(data);
            if (sav != null)
                return sav;

            foreach (var h in Handlers)
            {
                if (!h.IsRecognized(data.Length))
                    continue;

                var split = h.TrySplit(data);
                if (split == null)
                    continue;

                sav = GetVariantSAVInternal(split.Data);
                if (sav == null)
                    continue;

                sav.Metadata.SetExtraInfo(split.Header, split.Footer);
                return sav;
            }

            // unrecognized.
            return null;
        }

        private static SaveFile? GetVariantSAVInternal(byte[] data)
        {
            var type = GetSAVType(data);
            return type switch
            {
                // Main Games
                RBY => new SAV1(data, type),
                GS or C => new SAV2(data, type),
                RS or E or FRLG => new SAV3(data, type),

                DP => new SAV4DP(data),
                Pt => new SAV4Pt(data),
                HGSS => new SAV4HGSS(data),

                BW => new SAV5BW(data),
                B2W2 => new SAV5B2W2(data),

                XY => new SAV6XY(data),
                ORAS => new SAV6AO(data),
                ORASDEMO => new SAV6AODemo(data),

                SM => new SAV7SM(data),
                USUM => new SAV7USUM(data),
                GG => new SAV7b(data),

                SWSH => new SAV8SWSH(data),

                // Side Games
                COLO => new SAV3Colosseum(data),
                XD => new SAV3XD(data),
                RSBOX => new SAV3RSBox(data),
                BATREV => new SAV4BR(data),
                Stadium2 => new SAV2Stadium(data),
                Stadium => new SAV1Stadium(data),
                StadiumJ => new SAV1StadiumJ(data),

                // Bulk Storage
                Gen3 => new Bank3(data),
                DPPt => new SAV4Ranch(data),
                Gen4 => new Bank4(data),
                Gen7 => Bank7.GetBank7(data),

                // No pattern matched
                _ => null,
            };
        }

        public static SaveFile? GetVariantSAV(SAV3GCMemoryCard memCard)
        {
            // Pre-check for header/footer signatures
            SaveFile sav;
            byte[] data = memCard.SelectedSaveData;
            var split = DolphinHandler.TrySplit(data);
            if (split == null)
                return null;

            data = split.Data;

            switch (memCard.SelectedGameVersion)
            {
                // Side Games
                case COLO: sav = new SAV3Colosseum(data, memCard); break;
                case XD: sav = new SAV3XD(data, memCard); break;
                case RSBOX: sav = new SAV3RSBox(data, memCard); break;

                // No pattern matched
                default: return null;
            }

            sav.Metadata.SetExtraInfo(split.Header, split.Footer);
            return sav;
        }

        /// <summary>
        /// Returns a <see cref="LanguageID"/> that feels best for the save file's language.
        /// </summary>
        public static LanguageID GetSafeLanguage(SaveFile? sav) => sav switch
        {
            null => LanguageID.English,
            ILangDeviantSave s => s.Japanese ? LanguageID.Japanese : s.Korean ? LanguageID.Korean : LanguageID.English,
            _ => (uint)sav.Language <= Legal.GetMaxLanguageID(sav.Generation) ? (LanguageID)sav.Language : LanguageID.English,
        };

        /// <summary>
        /// Returns a Trainer Name that feels best for the save file's language.
        /// </summary>
        public static string GetSafeTrainerName(SaveFile? sav, LanguageID lang) => lang switch
        {
            LanguageID.Japanese => sav?.Generation >= 3 ? "ＰＫＨｅＸ" : "1337",
            _ => "PKHeX",
        };

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="game">Version to create the save file for.</param>
        /// <param name="trainerName">Trainer Name</param>
        /// <param name="language">Language to initialize with</param>
        /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
        public static SaveFile GetBlankSAV(GameVersion game, string trainerName, LanguageID language = LanguageID.English)
        {
            var sav = GetBlankSAV(game, language);
            sav.Game = (int)game;
            sav.OT = trainerName;
            if (sav.Generation >= 4)
                sav.Language = (int)language;

            // Secondary Properties may not be used but can be filled in as template.
            if (sav.Generation >= 7)
            {
                sav.TrainerID7 = 123456;
                sav.TrainerSID7 = 1234;
            }
            else
            {
                sav.TID = 12345;
                sav.SID = 54321;
            }
            sav.Language = (int)language;

            // Only set geolocation data for 3DS titles
            if (sav is IRegionOrigin o)
                o.SetDefaultRegionOrigins();

            return sav;
        }

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="game">Version to create the save file for.</param>
        /// <param name="language">Save file language to initialize for</param>
        /// <returns>Blank save file from the requested game, null if no game exists for that <see cref="GameVersion"/>.</returns>
        private static SaveFile GetBlankSAV(GameVersion game, LanguageID language) => game switch
        {
            RD or BU or GN or YW or RBY => new SAV1(version: game, japanese: language == LanguageID.Japanese || game == BU),
            StadiumJ => new SAV1StadiumJ(),
            Stadium => new SAV1Stadium(language == LanguageID.Japanese),

            GD or SV or GS => new SAV2(version: GS, lang: language),
            C or GSC => new SAV2(version: C, lang: language),
            Stadium2 => new SAV2Stadium(language == LanguageID.Japanese),

            R or S or E or FR or LG => new SAV3(version: game, language == LanguageID.Japanese),
            RS => new SAV3(version: R, language == LanguageID.Japanese),
            RSE => new SAV3(version: E, language == LanguageID.Japanese),
            FRLG => new SAV3(version: FR, language == LanguageID.Japanese),

            CXD or COLO => new SAV3Colosseum(),
            XD => new SAV3XD(),
            RSBOX => new SAV3RSBox(),

            D or P or DP => new SAV4DP(),
            Pt or DPPt => new SAV4Pt(),
            HG or SS or HGSS => new SAV4HGSS(),

            B or W or BW => new SAV5BW(),
            B2 or W2 or B2W2 => new SAV5B2W2(),

            X or Y or XY => new SAV6XY(),
            ORASDEMO => new SAV6AODemo(),
            OR or AS or ORAS => new SAV6AO(),

            SN or MN or SM => new SAV7SM(),
            US or UM or USUM => new SAV7USUM(),
            GP or GE or GG or GO => new SAV7b(),

            SW or SH or SWSH => new SAV8SWSH(),

            _ => throw new ArgumentException(nameof(game)),
        };

        /// <summary>
        /// Creates an instance of a SaveFile with a blank base.
        /// </summary>
        /// <param name="generation">Generation of the Save File.</param>
        /// <param name="trainerName">Trainer Name</param>
        /// <param name="language">Save file language to initialize for</param>
        /// <returns>Save File for that generation.</returns>
        public static SaveFile GetBlankSAV(int generation, string trainerName, LanguageID language = LanguageID.English)
        {
            var ver = GameUtil.GetVersion(generation);
            return GetBlankSAV(ver, trainerName, language);
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
                result = Array.Empty<string>();
                return false;
            }
            try
            {
                var searchOption = deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                // force evaluation so that an invalid path will throw before we return true/false.
                // EnumerateFiles throws an exception while iterating, which won't be caught by the try-catch here.
                var files = Directory.GetFiles(folderPath, "*", searchOption);
                result = files.Where(f => IsSizeValid(FileUtil.GetFileSize(f)));
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                result = new[]
                {
                    MsgFileLoadFailAuto + Environment.NewLine + folderPath,
                    MsgFileLoadFailAutoAdvise + Environment.NewLine + MsgFileLoadFailAutoCause,
                    ex.Message,
                };
                return false;
            }
        }

        /// <summary>
        /// Determines whether the save data size is valid for automatically detecting saves.
        /// </summary>
        /// <param name="size">Size in bytes of the save data</param>
        /// <returns>A boolean indicating whether or not the save data size is valid.</returns>
        public static bool IsSizeValid(int size) => Sizes.Contains(size) || Handlers.Any(z => z.IsRecognized(size));

        /// <summary>
        /// Force loads the provided <see cref="sav"/> to the requested <see cref="ver"/>.
        /// </summary>
        /// <param name="sav">SaveFile data to force</param>
        /// <param name="ver">Version to retrieve for</param>
        /// <returns>New <see cref="SaveFile"/> object.</returns>
        public static SAV3 GetG3SaveOverride(SaveFile sav, GameVersion ver) => ver switch // Reset save file info
        {
            R => new SAV3(sav.State.BAK, RS),
            S => new SAV3(sav.State.BAK, RS),
            RS => new SAV3(sav.State.BAK, RS),
            E => new SAV3(sav.State.BAK, E),
            FRLG => new SAV3(sav.State.BAK, FRLG),
            FR => new SAV3(sav.State.BAK, FRLG),
            LG => new SAV3(sav.State.BAK, FRLG),
            _ => throw new ArgumentException(nameof(ver))
        };

        /// <summary>
        /// Gets the <see cref="PersonalTable"/> for a Gen3 save file.
        /// </summary>
        /// <param name="ver">Version to retrieve for</param>
        /// <returns>Reference to the <see cref="PersonalTable"/>.</returns>
        public static PersonalTable GetG3Personal(GameVersion ver) => ver switch
        {
            RS => PersonalTable.RS,
            E => PersonalTable.E,
            FRLG => PersonalTable.FR,
            FR => PersonalTable.FR,
            LG => PersonalTable.LG,
            R => PersonalTable.RS,
            S => PersonalTable.RS,
            _ => throw new ArgumentException(nameof(ver))
        };
    }
}
