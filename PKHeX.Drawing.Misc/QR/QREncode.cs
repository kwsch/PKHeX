using System.Drawing;
using PKHeX.Core;
using QRCoder;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides methods for generating QR codes from various PKHeX data types.
/// </summary>
public static class QREncode
{
    /// <summary>
    /// Generates a QR code bitmap from a <see cref="DataMysteryGift"/> object.
    /// </summary>
    /// <param name="mg">The mystery gift data to encode.</param>
    /// <returns>A bitmap containing the QR code.</returns>
    public static Bitmap GenerateQRCode(DataMysteryGift mg) => GenerateQRCode(QRMessageUtil.GetMessage(mg));

    /// <summary>
    /// Generates a QR code bitmap from a <see cref="PKM"/> object.
    /// </summary>
    /// <param name="pk">The PKM data to encode.</param>
    /// <returns>A bitmap containing the QR code.</returns>
    public static Bitmap GenerateQRCode(PKM pk) => GenerateQRCode(QRMessageUtil.GetMessage(pk));

    /// <summary>
    /// Generates a QR code bitmap for a Generation 7 PKM with additional options.
    /// </summary>
    /// <param name="pk7">The Generation 7 PKM data to encode.</param>
    /// <param name="box">The box number for the PKM.</param>
    /// <param name="slot">The slot number in the box.</param>
    /// <param name="copies">The number of copies to encode.</param>
    /// <returns>A bitmap containing the QR code.</returns>
    public static Bitmap GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int copies = 1)
    {
        byte[] data = QR7.GenerateQRData(pk7, box, slot, copies);
        var msg = QRMessageUtil.GetMessage(data);
        return GenerateQRCode(msg, ppm: 4);
    }

    /// <summary>
    /// Generates a QR code bitmap from a message string.
    /// </summary>
    /// <param name="msg">The message to encode in the QR code.</param>
    /// <param name="ppm">Pixels per module for the QR code graphic.</param>
    /// <returns>A bitmap containing the QR code.</returns>
    private static Bitmap GenerateQRCode(string msg, int ppm = 4)
    {
        using var data = QRCodeGenerator.GenerateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
        using var code = new QRCode(data);
        return code.GetGraphic(ppm, darkColor: SystemColors.ControlText, lightColor: SystemColors.ControlLightLight, drawQuietZones: true);
    }
}
