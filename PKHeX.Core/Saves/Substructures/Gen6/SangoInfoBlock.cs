using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SangoInfoBlock(SAV6AO SAV, Memory<byte> raw) : SaveBlock<SAV6AO>(SAV, raw)
{
    private const uint EON_MAGIC = WC6.EonTicketConst;

    public uint EonTicketReceivedMagic // 0x319B8
    {
        get => ReadUInt32LittleEndian(Data[0x63B8..]);
        set => WriteUInt32LittleEndian(Data[0x63B8..], value);
    }

    public string SecretBaseQRText // 0x319BC -- 17*u16
    {
        get => SAV.GetString(Data.Slice(0x63BC, 36));
        set => SAV.SetString(Data.Slice(0x63BC, 36), value, 0x10, StringConverterOption.ClearZero);
    }

    public uint EonTicketSendMagic // 0x319DE
    {
        get => ReadUInt32LittleEndian(Data[0x63DE..]);
        set => WriteUInt32LittleEndian(Data[0x63DE..], value);
    }

    public void ReceiveEon() => EonTicketReceivedMagic = EON_MAGIC;
    public void EnableSendEon() => EonTicketSendMagic = EON_MAGIC;
}
