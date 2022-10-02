using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchMii
{
    public const int SIZE = 0x28;
    public readonly byte[] Data;

    public RanchMii(byte[] data) => Data = data;

    public uint MiiId { get => ReadUInt32BigEndian(Data); set => WriteUInt32BigEndian(Data, value); }
    public uint SystemId { get => ReadUInt32BigEndian(Data.AsSpan(0x04)); set => WriteUInt32BigEndian(Data.AsSpan(0x04), value); }
    public Span<byte> Name_Trash => Data.AsSpan(0x10, 0x18);

    public string MiiName
    {
        get => StringConverter4GC.GetStringUnicode(Name_Trash);
        set => StringConverter4GC.SetStringUnicode(value.AsSpan(), Name_Trash, value.Length, StringConverterOption.None);
    }
}
