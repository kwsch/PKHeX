using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ResortSave7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    private const int SIZE_7STORED = PokeCrypto.SIZE_6STORED;
    private const int SIZE_7STORED_R = SIZE_7STORED + 4; // 3 bytes of extra metadata

    // tinymt64
    public UInt128 State { get => ReadUInt128LittleEndian(Data); set => WriteUInt128LittleEndian(Data, value); }

    // 6 bytes ???

    public const int ResortCount1 = 18;
    public const int ResortCount2 = 3;
    public const int ResortCount3 = ResortCount1; // 18
    public const int ResortCount4 = ResortCount1; // 18
    public const int ResortCount5 = ResortCount1; // 18
    public const int ResortCount6 = ResortCount1; // 18
    public const int ResortCount = ResortCount1 + ResortCount2 + ResortCount3 + ResortCount4 + ResortCount5 + ResortCount6; // 93
    // End of Resort Slots: 0x55D2

    public static int GetResortSlotOffset(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)ResortCount);
        return 0x16 + (index * SIZE_7STORED_R);
    }

    public Memory<byte> this[int index] => Raw.Slice(GetResortSlotOffset(index), SIZE_7STORED);

    public const int BEANS_MAX = 15;
    public Span<byte> GetBeans() => Data.Slice(0x564C, BEANS_MAX);
    public void ClearBeans() => GetBeans().Clear();
    public void FillBeans(byte value = 255) => GetBeans().Fill(value);

    public int GetPokebeanCount(int bean_id) => GetBeans()[bean_id];

    public void SetPokebeanCount(int bean_id, int count)
    {
        if (count < 0)
            count = 0;
        if (count > 255)
            count = 255;
        GetBeans()[bean_id] = (byte)count;
    }

    /// <summary>
    /// Utility to indicate the bean pouch indexes.
    /// </summary>
    public static string[] GetBeanIndexNames()
    {
        var colors = Enum.GetNames<BeanColor7>();
        return GetBeanIndexNames(colors);
    }

    private static string[] GetBeanIndexNames(ReadOnlySpan<string> colors)
    {
        // 7 regular, 7 patterned, one rainbow
        var beans = new string[(colors.Length * 2) + 1];
        for (int i = 0; i < colors.Length; i++)
        {
            var z = colors[i];
            beans[i] = $"{z} Bean";
            beans[i + colors.Length] = $"{z} Patterned Bean";
        }
        beans[^1] = "Rainbow Bean";
        return beans;
    }
}

public enum BeanColor7 : byte
{
    Red,
    Blue,
    LightBlue,
    Green,
    Yellow,
    Purple,
    Orange,
}
