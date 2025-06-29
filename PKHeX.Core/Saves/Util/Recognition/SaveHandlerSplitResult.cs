namespace PKHeX.Core;

/// <summary>
/// Represents the result of a save operation split into distinct components.
/// </summary>
/// <remarks>This type encapsulates the data, header, and footer segments of a save operation,  along with the
/// associated save handler responsible for processing the operation.</remarks>
/// <param name="Data">The main data segment of the save operation, typically containing the core save data.</param>
/// <param name="Header">The header segment of the save operation, which may contain metadata or other relevant information.</param>
/// <param name="Footer">The footer segment of the save operation, which may contain additional metadata or checksums.</param>
/// <param name="Handler">The save handler responsible for processing the save operation, providing methods for recognition and finalization.</param>
public sealed class SaveHandlerSplitResult(byte[] Data, byte[] Header, byte[] Footer, ISaveHandler Handler)
{
    public readonly byte[] Header = Header;
    public readonly byte[] Footer = Footer;
    public readonly byte[] Data = Data;
    public readonly ISaveHandler Handler = Handler;
}
