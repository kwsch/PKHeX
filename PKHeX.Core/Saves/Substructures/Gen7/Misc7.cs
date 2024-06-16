using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Misc7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    public uint Money
    {
        get => ReadUInt32LittleEndian(Data[0x4..]);
        set
        {
            if (value > 9_999_999)
                value = 9_999_999;
            WriteUInt32LittleEndian(Data[0x4..], value);
        }
    }

    public uint Stamps
    {
        get => (ReadUInt32LittleEndian(Data[0x08..]) << 13) >> 17;  // 15 stamps; discard top13, lowest4
        set
        {
            uint flags = ReadUInt32LittleEndian(Data[0x08..]) & 0xFFF8000F;
            flags |= (value & 0x7FFF) << 4;
            WriteUInt32LittleEndian(Data[0x08..], flags);
        }
    }

    public uint BP
    {
        get => ReadUInt32LittleEndian(Data[0x11C..]);
        set
        {
            if (value > 9999)
                value = 9999;
            WriteUInt32LittleEndian(Data[0x11C..], value);
        }
    }

    public int DaysFromRefreshed
    {
        get => Data[0x123];
        set => Data[0x123] = (byte)value;
    }

    public int Vivillon
    {
        get => Data[0x130] & 0x1F;
        set => Data[0x130] = (byte)((Data[0x130] & ~0x1F) | (value & 0x1F));
    }

    public bool IsWormholeShiny
    {
        get => Data[0x136] == 1;
        set => Data[0x136] = (byte)(value ? 1 : 0);
    }

    public int GetSurfScore(int recordID)
    {
        if ((uint)recordID >= 4)
            recordID = 0;
        return ReadInt32LittleEndian(Data[(0x138 + (4 * recordID))..]);
    }

    public void SetSurfScore(int recordID, int score)
    {
        if ((uint)recordID >= 4)
            recordID = 0;
        WriteInt32LittleEndian(Data[(0x138 + (4 * recordID))..], score);
    }

    public uint StarterEncryptionConstant
    {
        get => ReadUInt32LittleEndian(Data[0x148..]);
        set => WriteUInt32LittleEndian(Data[0x148..], value);
    }
}
