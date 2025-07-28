using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class RanchMii(Memory<byte> Raw)
{
    public const int SIZE = 0x28;
    public Span<byte> Data => Raw.Span;

    public uint MiiId { get => ReadUInt32BigEndian(Data); set => WriteUInt32BigEndian(Data, value); }
    public uint SystemId { get => ReadUInt32BigEndian(Data[0x04..]); set => WriteUInt32BigEndian(Data[0x04..], value); }
    public Span<byte> MiiNameTrash => Data.Slice(0x10, 0x18);

    public string MiiName
    {
        get => StringConverter4GC.GetStringUnicode(MiiNameTrash);
        set => StringConverter4GC.SetStringUnicode(value, MiiNameTrash, value.Length, StringConverterOption.None);
    }
}
