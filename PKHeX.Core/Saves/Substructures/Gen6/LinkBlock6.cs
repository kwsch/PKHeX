using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class LinkBlock6 : SaveBlock<SAV6>
{
    public LinkBlock6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public LinkBlock6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    public Span<byte> InfoSpan() => Data.Slice(0x1FF, PL6.Size);

    public PL6 GetLinkInfo() => new(InfoSpan().ToArray());

    public void SetLinkInfoData(ReadOnlySpan<byte> data)
    {
        data.CopyTo(Data);
        Checksum = GetCalculatedChecksum(); // [app,chk)
    }

    public void SetLinkInfo(PL6 pl6)
    {
        pl6.Data.CopyTo(InfoSpan());
        Checksum = GetCalculatedChecksum(); // [app,chk)
    }

    private ushort GetCalculatedChecksum() => Checksums.CRC16_CCITT(Data[0x200..^4]); // [app,chk)

    public ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data[^4..]);
        set => WriteUInt16LittleEndian(Data[^4..], value);
    }
}
