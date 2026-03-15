using PKHeX.Core;
using QRCoder;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class QREncode
{
    public static SKBitmap GenerateQRCode(DataMysteryGift mg) => GenerateQRCode(QRMessageUtil.GetMessage(mg));

    public static SKBitmap GenerateQRCode(PKM pk) => GenerateQRCode(QRMessageUtil.GetMessage(pk));

    public static SKBitmap GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int copies = 1)
        => GenerateQRCode(QRMessageUtil.GetMessage(pk7, box, slot, copies), ppm: 4);

    private static SKBitmap GenerateQRCode(string msg, int ppm = 4)
    {
        using var data = QRCodeGenerator.GenerateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
        var code = new PngByteQRCode(data);
        var pngBytes = code.GetGraphic(ppm, [0, 0, 0], [255, 255, 255], drawQuietZones: true);
        return SKBitmap.Decode(pngBytes);
    }
}
