namespace PKHeX.Core
{
    public sealed class SaveHandlerSplitResult
    {
        public readonly byte[] Header;
        public readonly byte[] Footer;
        public readonly byte[] Data;

        public SaveHandlerSplitResult(byte[] data, byte[] header, byte[] footer)
        {
            Data = data;
            Header = header;
            Footer = footer;
        }
    }
}
