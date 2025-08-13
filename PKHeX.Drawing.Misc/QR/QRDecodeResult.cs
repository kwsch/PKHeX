namespace PKHeX.Drawing.Misc;

/// <summary>
/// Specifies the possible results of a QR code decoding operation.
/// </summary>
public enum QRDecodeResult
{
    /// <summary>Decoding was successful.</summary>
    Success,
    /// <summary>The provided path or URL was invalid.</summary>
    BadPath,
    /// <summary>The image could not be found or was invalid.</summary>
    BadImage,
    /// <summary>The file type is not supported for decoding.</summary>
    BadType,
    /// <summary>There was a connection error when accessing the QR code.</summary>
    BadConnection,
    /// <summary>The QR code data could not be converted to the expected format.</summary>
    BadConversion,
}
