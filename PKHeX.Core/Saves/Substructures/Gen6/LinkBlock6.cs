using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class LinkBlock6 : SaveBlock<SAV6>
{
    public LinkBlock6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public LinkBlock6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    public Memory<byte> PL6 => Raw.Slice(0x1FF, Core.PL6.Size);

    public PL6 Gifts => new(PL6);

    public void RefreshChecksum() => Checksum = GetCalculatedChecksum(); // [app,chk)

    private ushort GetCalculatedChecksum() => Checksums.CRC16_CCITT(Data[0x200..^4]); // [app,chk)

    public ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data[^4..]);
        set => WriteUInt16LittleEndian(Data[^4..], value);
    }
}
