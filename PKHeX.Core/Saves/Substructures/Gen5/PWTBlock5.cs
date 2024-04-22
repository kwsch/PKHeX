using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PWTBlock5(SAV5B2W2 sav, Memory<byte> raw) : SaveBlock<SAV5B2W2>(sav, raw)
{
    public ushort GetPWTRecord(int id) => GetPWTRecord((PWTRecordID)id);
    public void SetPWTRecord(int id, ushort value) => SetPWTRecord((PWTRecordID)id, value);

    public ushort GetPWTRecord(PWTRecordID id) => ReadUInt16LittleEndian(Data[GetRecordOffset(id)..]);
    public void SetPWTRecord(PWTRecordID id, ushort value) => WriteUInt16LittleEndian(Data[GetRecordOffset(id)..], value);

    private static int GetRecordOffset(PWTRecordID id)
    {
        if (id is < PWTRecordID.Normal or > PWTRecordID.MixMaster)
            throw new ArgumentOutOfRangeException(nameof(id));
        return 0x5C + ((int)id * 2);
    }
}
