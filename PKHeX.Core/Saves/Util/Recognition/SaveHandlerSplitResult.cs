namespace PKHeX.Core;

public sealed class SaveHandlerSplitResult(byte[] Data, byte[] Header, byte[] Footer, ISaveHandler Handler)
{
    public readonly byte[] Header = Header;
    public readonly byte[] Footer = Footer;
    public readonly byte[] Data = Data;
    public readonly ISaveHandler Handler = Handler;
}
