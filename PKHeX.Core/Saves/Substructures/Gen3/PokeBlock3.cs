using System;
using System.ComponentModel;

namespace PKHeX.Core;

public sealed class PokeBlock3(byte[] Data)
{
    public const int SIZE = 8;

    private const string Stats = nameof(Stats);

    public PokeBlock3Color Color { get => (PokeBlock3Color)Data[0]; set => Data[0] = (byte)value; }

    [Category(Stats), Description("Cool Stat Boost")]
    public byte Spicy  { get => Data[1]; set => Data[1] = value; }

    [Category(Stats), Description("Beauty Stat Boost")]
    public byte Dry    { get => Data[2]; set => Data[2] = value; }

    [Category(Stats), Description("Cute Stat Boost")]
    public byte Sweet  { get => Data[3]; set => Data[3] = value; }

    [Category(Stats), Description("Smart/Clever Stat Boost")]
    public byte Bitter { get => Data[4]; set => Data[4] = value; }

    [Category(Stats), Description("Tough Stat Boost")]
    public byte Sour   { get => Data[5]; set => Data[5] = value; }

    [Category(Stats), Description("Sheen Stat Boost")]
    public byte Feel   { get => Data[6]; set => Data[6] = value; }
    // 7 unused alignment byte

    public byte Level => Math.Max(Math.Max(Math.Max(Math.Max(Spicy, Dry), Sweet), Bitter), Sour);

    public void Delete() => Data.AsSpan().Clear();

    public void Maximize(bool create = false)
    {
        if (Color == 0 && !create)
            return;
        Spicy = Dry = Sweet = Bitter = Sour = Feel = 255;
        Color = PokeBlock3Color.Gold;
    }

    public void SetBlock(Span<byte> data1) => Data.CopyTo(data1);

    public static PokeBlock3 GetBlock(ReadOnlySpan<byte> data, int offset)
    {
        byte[] result = data.Slice(offset, SIZE).ToArray();
        return new PokeBlock3(result);
    }
}
