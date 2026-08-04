using System.IO;
using PKHeX.Application.Abstractions;
using QRCoder;
using SkiaSharp;
using ZXing;
using ZXing.Common;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// QR image encode/decode. Payloads from <c>QRMessageUtil</c> are raw bytes stored char-per-byte,
/// so both directions pin the character set to ISO-8859-1 (matching upstream WinForms PKHeX).
/// </summary>
public sealed class QrCodeService : IQrCodeService
{
    public byte[] GeneratePng(string message, int pixelsPerModule = 8)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(message, QRCodeGenerator.ECCLevel.Q,
            forceUtf8: false, utf8BOM: false, eciMode: QRCodeGenerator.EciMode.Iso8859_1);
        using var png = new PngByteQRCode(data);
        return png.GetGraphic(pixelsPerModule);
    }

    public string? DecodePng(byte[] imageBytes)
    {
        using var codec = SKCodec.Create(new MemoryStream(imageBytes));
        if (codec is null)
            return null; // not a recognizable image

        using var decoded = SKBitmap.Decode(codec);
        if (decoded is null)
            return null;

        using var bitmap = decoded.ColorType == SKColorType.Bgra8888
            ? decoded.Copy() // Copy so both branches own their bitmap and the using stays uniform.
            : decoded.Copy(SKColorType.Bgra8888);
        if (bitmap is null)
            return null;

        var source = new RGBLuminanceSource(bitmap.Bytes, bitmap.Width, bitmap.Height,
            RGBLuminanceSource.BitmapFormat.BGRA32);
        var reader = new BarcodeReaderGeneric
        {
            Options = new DecodingOptions
            {
                TryHarder = true,
                PossibleFormats = [BarcodeFormat.QR_CODE],
                CharacterSet = "ISO-8859-1",
            },
        };
        return reader.Decode(source)?.Text;
    }
}
