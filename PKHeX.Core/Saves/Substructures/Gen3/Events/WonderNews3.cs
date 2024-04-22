using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class WonderNews3 : Gen3MysteryData
{
    /// <summary>
    /// 0x1C0: Total Size of this object on international saves
    /// </summary>
    public const int SIZE = sizeof(uint) + 444;

    /// <summary>
    /// 0xE4: Total Size of this object on Japanese saves
    /// </summary>
    public const int SIZE_JAP = sizeof(uint) + 224;

    public WonderNews3(byte[] data) : base(data) => AssertLength(data.Length);

    private static void AssertLength(int length)
    {
        if (length is not (SIZE or SIZE_JAP))
            throw new ArgumentOutOfRangeException(nameof(length), length, "Invalid size.");
    }

    public bool Japanese => Data.Length is SIZE_JAP;

    public ushort NewsID   { get => ReadUInt16LittleEndian(Data.AsSpan(4)); set => WriteUInt16LittleEndian(Data.AsSpan(4), value); }
    public byte Flag { get => Data[6]; set => Data[6] = value; }
    public byte Color { get => Data[7]; set => Data[7] = value; }

    public string Title
    {
        get => StringConverter3.GetString(Data.AsSpan(8, Japanese ? 20 : 40), Japanese);
        set
        {
            var dest = Data.AsSpan(8, Japanese ? 20 : 40);
            StringConverter3.SetString(dest, value, Japanese ? 20 : 40, Japanese, StringConverterOption.ClearFF);
        }
    }
}
