using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct Aprijuice4(Memory<byte> Raw)
{
    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// The price of drinks are determined by how famous the Trainers selling them are.
    /// </summary>
    /// <remarks>
    /// <see cref="PokeathlonGlobalCounters4.Fame"/>
    /// </remarks>
    public ushort Fame { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }

    // Every 100 steps taken increases the mildness of an Aprijuice by 1, up to a maximum of 255.
    // Any mildness increases are made before the mixing of new Apricorns into the Aprijuice is performed.
    public byte Mildness { get => Data[2]; set => Data[2] = value; }

    // Each flavor is capped at a maximum of 63 points and a minimum of 0.
    public byte Spicy { get => Data[3]; set => Data[3] = value; }
    public byte Sour { get => Data[4]; set => Data[4] = value; }
    public byte Dry { get => Data[5]; set => Data[5] = value; }
    public byte Bitter { get => Data[6]; set => Data[6] = value; }
    public byte Sweet { get => Data[7]; set => Data[7] = value; }

    public const ushort LevelMax = 100;

    public byte CalculateLevel()
    {
        var level = Spicy + Sour + Dry + Bitter + Sweet;
        return (byte)Math.Clamp(level, 0, LevelMax); // never will see 0 unless it's hacked :)
    }

    public const ushort PriceMin = 100;
    public const ushort PriceMax = 5000;

    public ushort CalculatePrice()
    {
        var level = CalculateLevel();
        var price = (Fame / 10) + (level / 2);
        return (ushort)Math.Clamp(price, PriceMin, PriceMax);
    }

    // When a Pokémon is put into a PC box, all effects of an Aprijuice disappear.
}
