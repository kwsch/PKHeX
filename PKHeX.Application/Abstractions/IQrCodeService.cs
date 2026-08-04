namespace PKHeX.Application.Abstractions;

/// <summary>
/// Encodes text payloads (see <c>PKHeX.Core.QRMessageUtil</c>) as QR code images and decodes them back.
/// Payloads are raw byte data mapped char-per-byte, so both directions use ISO-8859-1 semantics.
/// </summary>
public interface IQrCodeService
{
    /// <summary>Renders the message as a QR code PNG.</summary>
    byte[] GeneratePng(string message, int pixelsPerModule = 8);

    /// <summary>Decodes a QR code from PNG/JPEG image bytes. Returns null when no QR code is found.</summary>
    string? DecodePng(byte[] imageBytes);
}
