namespace PKHeX.Core;

public interface ISanityChecksum
{
    ushort Sanity { get; set; }
    ushort Checksum { get; set; }
}
