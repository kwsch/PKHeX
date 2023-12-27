using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class WonderCard3 : Gen3MysteryData
{
    /// <summary>
    /// 0x150: Total Size of this object on international saves
    /// </summary>
    public const int SIZE = sizeof(uint) + 332;

    /// <summary>
    /// 0xA8: Total Size of this object on Japanese saves
    /// </summary>
    public const int SIZE_JAP = sizeof(uint) + 164;

    public WonderCard3(byte[] data) : base(data) => AssertLength(data.Length);

    private static void AssertLength(int length)
    {
        if (length is not (SIZE or SIZE_JAP))
            throw new ArgumentOutOfRangeException(nameof(length), length, "Invalid size.");
    }

    public bool Japanese => Data.Length is SIZE_JAP;

    public ushort CardID { get => ReadUInt16LittleEndian(Data.AsSpan(4)); set => WriteUInt16LittleEndian(Data.AsSpan(4), value); }
    public ushort Icon { get => ReadUInt16LittleEndian(Data.AsSpan(6)); set => WriteUInt16LittleEndian(Data.AsSpan(6), value); }
    public uint Count { get => ReadUInt16LittleEndian(Data.AsSpan(8)); set => WriteUInt32LittleEndian(Data.AsSpan(8), value); }

    public byte Type { get => (byte)(Data[0xC] & 0b11); set => Data[0xC] = (byte)((Data[0xC] & ~0b11) | (value & 0b11)); }
    public byte Color { get => (byte)(Data[0xC] & 0b00111100); set => Data[0xC] = (byte)((Data[0xC] & ~0b00111100) | (value & 0b00111100)); }
    public byte ShareState { get => (byte)(Data[0xC] & 0b11000000); set => Data[0xC] = (byte)((Data[0xC] & ~0b11000000) | (value & 0b11000000)); }

    public byte Count2 { get => Data[0xD]; set => Data[0xD] = value; }

    public string Title
    {
        get => StringConverter3.GetString(Data.AsSpan(0xE, Japanese ? 18 : 40), Japanese);
        set
        {
            var dest = Data.AsSpan(0xE, Japanese ? 18 : 40);
            StringConverter3.SetString(dest, value, Japanese ? 18 : 40, Japanese, StringConverterOption.ClearFF);
        }
    }
    public string Subtitle
    {
        get => StringConverter3.GetString(Data.AsSpan(0x20, Japanese ? 13 : 40), Japanese);
        set
        {
            var dest = Data.AsSpan(0x20, Japanese ? 13 : 40);
            StringConverter3.SetString(dest, value, Japanese ? 13 : 40, Japanese, StringConverterOption.ClearFF);
        }
    }
}
