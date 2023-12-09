using System;

namespace PKHeX.Core;

public sealed class ResortSave7 : SaveBlock<SAV7>
{
    public ResortSave7(SAV7 sav, int offset) : base(sav) => Offset = offset;

    private const int SIZE_7STORED_R = PokeCrypto.SIZE_6STORED + 4;

    public const int ResortCount = 93;
    public int GetResortSlotOffset(int slot) => Offset + 0x16 + (slot * SIZE_7STORED_R);

    public PK7[] ResortPKM
    {
        get
        {
            PK7[] result = new PK7[ResortCount];
            for (int i = 0; i < result.Length; i++)
            {
                var ofs = GetResortSlotOffset(i);
                var data = SAV.Data.AsSpan(ofs, PokeCrypto.SIZE_6STORED).ToArray();
                result[i] = new PK7(data);
            }
            return result;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, ResortCount);

            for (int i = 0; i < value.Length; i++)
            {
                var ofs = GetResortSlotOffset(i);
                var dest = Data.AsSpan(ofs, PokeCrypto.SIZE_6STORED);
                SAV.SetSlotFormatStored(value[i], dest);
            }
        }
    }

    public const int BEANS_MAX = 15;
    public Span<byte> GetBeans() => Data.AsSpan(Offset + 0x564C, BEANS_MAX);
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
        var colors = Enum.GetNames(typeof(BeanColor7));
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
