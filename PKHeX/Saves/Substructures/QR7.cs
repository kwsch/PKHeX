using System;
using System.Drawing;
using System.Linq;

using QRCoder;

namespace PKHeX.Saves.Substructures
{
    public class QR7
    {
        private static bool hasGenderDifferences(int species)
        {
            var gendered = new[]
            {
                3, 12, 19, 20, 25, 26, 41, 42, 44, 45, 64, 65, 84, 85, 97, 111, 112, 118, 119, 123, 129, 130, 154, 165, 166,
                178, 185, 186, 190, 194, 195, 198, 202, 203, 207, 208, 212, 214, 215, 217, 221, 224, 229, 232, 255, 256,
                257, 267, 269, 272, 274, 275, 307, 308, 315, 316, 317, 322, 323, 332, 350, 369, 396, 397, 398, 399, 400,
                401, 402, 403, 404, 405, 407, 415, 417, 418, 419, 424, 443, 444, 445, 449, 450, 453, 454, 456, 457, 459,
                460, 461, 464, 465, 473, 521, 592, 593, 668, 678
            };
            return gendered.Contains(species);
        }
        private static byte[] GetRawQR(int species, int formnum, bool shiny, int gender)
        {
            var basedata = "FFFFFFFFFFFF00000000000000000000000000000000000000000000000000000000000000000000D20200000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000".ToByteArray();
            BitConverter.GetBytes((ushort)species).CopyTo(basedata, 0x28);
            basedata[0x2A] = (byte)formnum;
            basedata[0x2C] = (byte)(shiny ? 1 : 0);
            var forme_index = PersonalTable.SM[species].FormeIndex(species, formnum);
            var raw_gender = PersonalTable.SM[forme_index].Gender;
            switch (raw_gender)
            {
                case 0:
                    basedata[0x2D] = 0;
                    basedata[0x2B] = 0;
                    break;
                case 0xFE:
                    basedata[0x2D] = 0;
                    basedata[0x2B] = 1;
                    break;
                case 0xFF:
                    basedata[0x2D] = 0;
                    basedata[0x2B] = 2;
                    break;
                default:
                    basedata[0x2D] = (byte)(hasGenderDifferences(species) ? 0 : 1);
                    basedata[0x2B] = (byte)gender;
                    break;
            }
            return basedata;
        }
        private static byte[] GenerateQRData(PK7 pk7, int box = 0, int slot = 0)
        {
            if (box > 31)
                box = 31;
            if (slot > 29)
                slot = 29;
            if (box < 0)
                box = 0;
            if (slot < 0)
                slot = 0;

            byte[] data = new byte[0x1A2];
            BitConverter.GetBytes(0x454B4F50).CopyTo(data, 0); // POKE magic
            data[0x5] = 0xFF; // QR Type
            BitConverter.GetBytes(box).CopyTo(data, 0x8);
            BitConverter.GetBytes(slot).CopyTo(data, 0xC);

            pk7.EncryptedPartyData.CopyTo(data, 0x30); // Copy in pokemon data
            GetRawQR(pk7.Species, pk7.AltForm, pk7.IsShiny, pk7.Gender).CopyTo(data, 0x140);
            BitConverter.GetBytes((ushort) SaveUtil.check16(data.Take(0x1A0).ToArray(), 0)).CopyTo(data, 0x1A0);
            return data;
        }

        public static Bitmap GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0)
        {
            byte[] data = GenerateQRData(pk7, box, slot);
            using (var generator = new QRCodeGenerator())
            using (var qr_data = generator.CreateQRCode(data))
            using (var qr_code = new QRCode(qr_data))
                return qr_code.GetGraphic(4);
        }

        public static Bitmap GenerateQRCode(byte[] data, int ppm = 4)
        {
            using (var generator = new QRCodeGenerator())
            using (var qr_data = generator.CreateQRCode(data))
            using (var qr_code = new QRCode(qr_data))
                return qr_code.GetGraphic(ppm);
        }
    }
}
