using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PWTBlock5(SAV5B2W2 sav, int offset) : SaveBlock<SAV5B2W2>(sav, offset)
{
    public ushort GetPWTRecord(int id) => GetPWTRecord((PWTRecordID)id);

    public ushort GetPWTRecord(PWTRecordID id)
    {
        if (id is < PWTRecordID.Normal or > PWTRecordID.MixMaster)
            throw new ArgumentOutOfRangeException(nameof(id));
        int ofs = Offset + 0x5C + ((int)id * 2);
        return ReadUInt16LittleEndian(Data.AsSpan(ofs));
    }

    public void SetPWTRecord(int id, ushort value) => SetPWTRecord((PWTRecordID)id, value);

    public void SetPWTRecord(PWTRecordID id, ushort value)
    {
        if (id is < PWTRecordID.Normal or > PWTRecordID.MixMaster)
            throw new ArgumentOutOfRangeException(nameof(id));
        int ofs = Offset + 0x5C + ((int)id * 2);
        WriteUInt16LittleEndian(Data.AsSpan(ofs), value);
    }
}
