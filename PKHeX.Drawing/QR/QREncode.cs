using System.Drawing;
using PKHeX.Core;
using QRCoder;

namespace PKHeX.Drawing
{
    public static class QREncode
    {
        public static Image GenerateQRCode(DataMysteryGift mg) => GenerateQRCode(QRMessageUtil.GetMessage(mg));
        public static Image GenerateQRCode(PKM pkm) => GenerateQRCode(QRMessageUtil.GetMessage(pkm));

        public static Image GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int num_copies = 1)
        {
            byte[] data = QR7.GenerateQRData(pk7, box, slot, num_copies);
            var msg = QRMessageUtil.GetMessage(data);
            return GenerateQRCode(msg, ppm: 4);
        }

        private static Image GenerateQRCode(string msg, int ppm = 4)
        {
            using var generator = new QRCodeGenerator();
            using var qr_data = generator.CreateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
            using var qr_code = new QRCode(qr_data);
            return qr_code.GetGraphic(ppm);
        }
    }
}