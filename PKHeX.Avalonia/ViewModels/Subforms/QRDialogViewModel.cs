using System;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Avalonia;
using PKHeX.Drawing.PokeSprite.Avalonia;
using SkiaSharp;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the QR Code dialog. Generates and displays a QR code for a PKM entity.
/// </summary>
public partial class QRDialogViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private Bitmap? _qrImage;

    [ObservableProperty]
    private string _summaryText = string.Empty;

    /// <summary>
    /// The composed QR code as an SKBitmap, retained for copy/save operations.
    /// </summary>
    private SKBitmap? _qrBitmap;

    public QRDialogViewModel(PKM pk)
    {
        GenerateQR(pk);
    }

    private void GenerateQR(PKM pk)
    {
        var qr = QREncode.GenerateQRCode(pk);
        var sprite = pk.Sprite();
        var composed = QRImageUtil.GetQRImage(qr, sprite);
        qr.Dispose();
        sprite.Dispose();

        _qrBitmap?.Dispose();
        _qrBitmap = composed;
        var old = QrImage;
        QrImage = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(composed);
        old?.Dispose();

        var lines = pk.GetQRLines();
        SummaryText = string.Join("\n", lines);
    }

    public void Dispose()
    {
        _qrBitmap?.Dispose();
        QrImage?.Dispose();
    }

    /// <summary>
    /// Gets the raw PNG bytes of the QR image for clipboard/save operations.
    /// </summary>
    public byte[]? GetQRImageBytes()
    {
        if (_qrBitmap is null)
            return null;

        using var image = SKImage.FromBitmap(_qrBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
