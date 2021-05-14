using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    // anatomy of a QR7:
    // u32 magic; // POKE
    // u32 _0xFF;
    // u32 box;
    // u32 slot;
    // u32 num_copies;
    // u8  reserved[0x1C];
    // u8  ek7[0x104];
    // u8  dex_data[0x60];
    // u16 crc16
    // sizeof(QR7) == 0x1A2

    /// <summary>
    /// Generation 7 QR format (readable by the in-game QR Scanner feature)
    /// </summary>
    public static class QR7
    {
        private static readonly HashSet<int> GenderDifferences = new()
        {
            003, 012, 019, 020, 025, 026, 041, 042, 044, 045,
            064, 065, 084, 085, 097, 111, 112, 118, 119, 123,
            129, 130, 154, 165, 166, 178, 185, 186, 190, 194,
            195, 198, 202, 203, 207, 208, 212, 214, 215, 217,
            221, 224, 229, 232, 255, 256, 257, 267, 269, 272,
            274, 275, 307, 308, 315, 316, 317, 322, 323, 332,
            350, 369, 396, 397, 398, 399, 400, 401, 402, 403,
            404, 405, 407, 415, 417, 418, 419, 424, 443, 444,
            445, 449, 450, 453, 454, 456, 457, 459, 460, 461,
            464, 465, 473, 521, 592, 593, 668, 678
        };

        private static readonly byte[] BaseQR =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD2, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        private static byte[] GetRawQR(int species, int form, bool shiny, int gender)
        {
            var basedata = (byte[])BaseQR.Clone();
            BitConverter.GetBytes((ushort)species).CopyTo(basedata, 0x28);

            var pi = PersonalTable.USUM.GetFormEntry(species, form);
            bool biGender = false;
            if (pi.OnlyMale)
                gender = 0;
            else if (pi.OnlyFemale)
                gender = 1;
            else if (pi.Genderless)
                gender = 2;
            else
                biGender = !GenderDifferences.Contains(species);

            basedata[0x2A] = (byte)form;
            basedata[0x2B] = (byte)gender;
            basedata[0x2C] = shiny ? (byte)1 : (byte)0;
            basedata[0x2D] = biGender ? (byte)1 : (byte)0;
            return basedata;
        }

        public static byte[] GenerateQRData(PK7 pk7, int box = 0, int slot = 0, int num_copies = 1)
        {
            if (box > 31)
                box = 31;
            if (slot > 29)
                slot = 29;
            if (box < 0)
                box = 0;
            if (slot < 0)
                slot = 0;
            if (num_copies < 0)
                num_copies = 1;

            byte[] data = new byte[0x1A2];
            BitConverter.GetBytes(0x454B4F50).CopyTo(data, 0); // POKE magic
            data[0x4] = 0xFF; // QR Type
            BitConverter.GetBytes(box).CopyTo(data, 0x8);
            BitConverter.GetBytes(slot).CopyTo(data, 0xC);
            BitConverter.GetBytes(num_copies).CopyTo(data, 0x10); // No need to check max num_copies, payload parser handles it on-console.

            pk7.EncryptedPartyData.CopyTo(data, 0x30); // Copy in pokemon data
            GetRawQR(pk7.Species, pk7.Form, pk7.IsShiny, pk7.Gender).CopyTo(data, 0x140);

            var span = new ReadOnlySpan<byte>(data, 0, 0x1A0);
            var chk = Checksums.CRC16Invert(span);
            BitConverter.GetBytes(chk).CopyTo(data, 0x1A0);
            return data;
        }
    }
}
