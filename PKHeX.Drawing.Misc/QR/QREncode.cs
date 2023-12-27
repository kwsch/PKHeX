using System.Drawing;
using PKHeX.Core;
using QRCoder;

namespace PKHeX.Drawing.Misc;

public static class QREncode
{
    public static Bitmap GenerateQRCode(DataMysteryGift mg) => GenerateQRCode(QRMessageUtil.GetMessage(mg));
    public static Bitmap GenerateQRCode(PKM pk) => GenerateQRCode(QRMessageUtil.GetMessage(pk));

    public static Bitmap GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int copies = 1)
    {
        byte[] data = QR7.GenerateQRData(pk7, box, slot, copies);
        var msg = QRMessageUtil.GetMessage(data);
        return GenerateQRCode(msg, ppm: 4);
    }

    private static Bitmap GenerateQRCode(string msg, int ppm = 4)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
        using var code = new QRCode(data);
        return code.GetGraphic(ppm);
    }
}
