using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public enum GameVersion
    {
        Any = -1,
        Unknown = 0,
        S = 1, R = 2, E = 3, FR = 4, LG = 5, CXD = 15,
        D = 10, P = 11, Pt = 12, HG = 7, SS = 8, 
        W = 20, B = 21, W2 = 22, B2 = 23,
        X = 24, Y = 25, AS = 26, OR = 27,
        SN = 28, MN = 29,

        DP = 100,
        HGSS = 101,
    }
    internal static class SaveUtil
    {
        internal const int BEEF = 0x42454546;

        internal const int SIZE_G6XY = 0x65600;
        internal const int SIZE_G6ORAS = 0x76000;
        internal const int SIZE_G6ORASDEMO = 0x5A00;
        internal const int SIZE_G5RAW = 0x80000;
        internal const int SIZE_G5BW = 0x24000;
        internal const int SIZE_G5B2W2 = 0x26000;
        internal const int SIZE_G4RAW = 0x80000;

        internal static readonly byte[] FOOTER_DSV = Encoding.ASCII.GetBytes("|-DESMUME SAVE-|");

        internal static int getSAVGeneration(byte[] data)
        {
            if (getIsG4SAV(data) != -1)
                return 4;
            if (getIsG5SAV(data) != -1)
                return 5;
            if (getIsG6SAV(data) != -1)
                return 6;
            return -1;
        }
        internal static int getIsG4SAV(byte[] data)
        {
            if (data.Length != SIZE_G4RAW)
                return -1;

            int version = -1;
            if (BitConverter.ToUInt16(data, 0xC0FE) == ccitt16(data.Take(0xC0EC).ToArray()))
                version = 0; // DP
            else if (BitConverter.ToUInt16(data, 0xCF2A) == ccitt16(data.Take(0xCF18).ToArray()))
                version = 1; // PT
            else if (BitConverter.ToUInt16(data, 0xF626) == ccitt16(data.Take(0xF618).ToArray()))
                version = 2; // HGSS
            return version;
        }
        internal static int getIsG5SAV(byte[] data)
        {
            if (data.Length != SIZE_G5RAW)
                return -1;

            ushort chk1 = BitConverter.ToUInt16(data, SIZE_G5BW - 0x100 + 0x8C + 0xE);
            ushort actual1 = ccitt16(data.Skip(SIZE_G5BW - 0x100).Take(0x8C).ToArray());
            if (chk1 == actual1)
                return 0;
            ushort chk2 = BitConverter.ToUInt16(data, SIZE_G5B2W2 - 0x100 + 0x94 + 0xE);
            ushort actual2 = ccitt16(data.Skip(SIZE_G5B2W2 - 0x100).Take(0x94).ToArray());
            if (chk2 == actual2)
                return 1;
            return -1;
        }
        internal static int getIsG6SAV(byte[] data)
        {
            if (!SizeValidSAV6(data.Length))
                return -1;

            if (BitConverter.ToUInt32(data, data.Length - 0x1F0) != BEEF)
                return -1;

            switch (data.Length)
            {
                case SIZE_G6XY:
                    return 0;
                case SIZE_G6ORASDEMO:
                    return 1;
                case SIZE_G6ORAS:
                    return 2;
                default: // won't hit
                    return 3;
            }
        }
        internal static SaveFile getVariantSAV(byte[] data)
        {
            switch (getSAVGeneration(data))
            {
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
        internal static bool SizeValidSAV6(int size)
        {
            switch (size)
            {
                case SIZE_G6XY:
                case SIZE_G6ORASDEMO:
                case SIZE_G6ORAS:
                    return true;
            }
            return false;
        }

        // SAV Manipulation
        /// <summary>Calculates the CRC16-CCITT checksum over an input byte array.</summary>
        /// <param name="data">Input byte array</param>
        /// <returns>Checksum</returns>
        internal static ushort ccitt16(byte[] data)
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
        /// <summary>Simple check to see if the save is valid.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>True/False</returns>
        internal static bool verifyG6SAV(byte[] savefile)
        {
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i] % 0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i] % 0x200 + Lengths[i];

                if ((BlockIDs[i] != 0) || i == 0) continue;
                count = i;
                break;
            }
            // Verify checksums
            for (int i = 0; i < count; i++)
            {
                ushort chk = ccitt16(savefile.Skip(Start[i]).Take(Lengths[i]).ToArray());
                ushort old = BitConverter.ToUInt16(savefile, verificationOffset + 6 + i * 8);

                if (chk != old)
                    return false;
            }
            return true;
        }
        /// <summary>Verbose check to see if the save is valid.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>String containing invalid blocks.</returns>
        internal static string verifyG6CHK(byte[] savefile)
        {
            string rv = "";
            int invalid = 0;
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i] % 0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i] % 0x200 + Lengths[i];

                if (BlockIDs[i] != 0 || i == 0) continue;
                count = i;
                break;
            }
            // Apply checksums
            for (int i = 0; i < count; i++)
            {
                ushort chk = ccitt16(savefile.Skip(Start[i]).Take(Lengths[i]).ToArray());
                ushort old = BitConverter.ToUInt16(savefile, verificationOffset + 6 + i * 8);

                if (chk == old) continue;

                invalid++;
                rv += $"Invalid: {i.ToString("X2")} @ Region {Start[i].ToString("X5") + Environment.NewLine}";
            }
            // Return Outputs
            rv += $"SAV: {count - invalid}/{count + Environment.NewLine}";
            return rv;
        }
        /// <summary>Fix checksums in the input save file.</summary>
        /// <param name="savefile">Input binary file</param>
        /// <returns>Fixed save file.</returns>
        internal static void writeG6CHK(byte[] savefile)
        {
            // Dynamic handling of checksums regardless of save size.

            int verificationOffset = savefile.Length - 0x200 + 0x10;
            if (BitConverter.ToUInt32(savefile, verificationOffset) != BEEF)
                verificationOffset -= 0x200; // No savegames have more than 0x3D blocks, maybe in the future?

            int count = (savefile.Length - verificationOffset - 0x8) / 8;
            verificationOffset += 4;
            int[] Lengths = new int[count];
            ushort[] BlockIDs = new ushort[count];
            ushort[] Checksums = new ushort[count];
            int[] Start = new int[count];
            int CurrentPosition = 0;
            for (int i = 0; i < count; i++)
            {
                Start[i] = CurrentPosition;
                Lengths[i] = BitConverter.ToInt32(savefile, verificationOffset + 0 + 8 * i);
                BlockIDs[i] = BitConverter.ToUInt16(savefile, verificationOffset + 4 + 8 * i);
                Checksums[i] = BitConverter.ToUInt16(savefile, verificationOffset + 6 + 8 * i);

                CurrentPosition += Lengths[i] % 0x200 == 0 ? Lengths[i] : 0x200 - Lengths[i] % 0x200 + Lengths[i];

                if (BlockIDs[i] != 0 || i == 0) continue;
                count = i;
                break;
            }
            // Apply checksums
            for (int i = 0; i < count; i++)
            {
                byte[] array = savefile.Skip(Start[i]).Take(Lengths[i]).ToArray();
                BitConverter.GetBytes(ccitt16(array)).CopyTo(savefile, verificationOffset + 6 + i * 8);
            }
        }

        internal static int getDexFormIndexXY(int species, int formct)
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
        internal static int getDexFormIndexORAS(int species, int formct)
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
