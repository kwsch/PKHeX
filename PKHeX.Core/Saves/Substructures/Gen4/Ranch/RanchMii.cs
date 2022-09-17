using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchMii
{
    public const int SIZE = 40;
    public readonly byte[] Data;

    public RanchMii(byte[] miiData)
    {
        this.Data = miiData;
        MiiIdOffset = 0x00;
        MiiIdSize = 0x04;
        SystemIdOffset = 0x04;
        SystemIdSize = 0x04;
        MiiNameOffset = 0x10;
        MiiNameSize = 0x0A;
    }

    public uint GetMiiId()
    {
        return ReadUInt32BigEndian(Data.AsSpan(MiiIdOffset, MiiIdSize));
    }

    public void SetMiiId(uint miiId)
    {
        WriteUInt32BigEndian(Data.AsSpan(MiiIdOffset, MiiIdSize), miiId);
    }

    public uint GetSystemId()
    {
        return ReadUInt32BigEndian(Data.AsSpan(SystemIdOffset, SystemIdSize));
    }

    public void SetSystemId(uint systemId)
    {
        WriteUInt32BigEndian(Data.AsSpan(SystemIdOffset, SystemIdSize), systemId);
    }

    public string GetMiiName() => StringConverter4GC.GetStringUnicode(Data.AsSpan(MiiNameOffset, MiiNameSize));

    public int SetMiiName(string name)
    {
        Span<byte> destBuffer = new Span<byte>(new byte[MiiNameSize]);
        return StringConverter4GC.SetStringUnicode(name.AsSpan(), destBuffer, name.Length, StringConverterOption.None);
    }

    private int MiiIdOffset;
    private int MiiIdSize;
    private int SystemIdOffset;
    private int SystemIdSize;
    private int MiiNameOffset;
    private int MiiNameSize;
}
