namespace PKHeX.Core;

public sealed class SaveHandlerSplitResult(byte[] Data, byte[] Header, byte[] Footer)
{
    public readonly byte[] Header = Header;
    public readonly byte[] Footer = Footer;
    public readonly byte[] Data = Data;
}
