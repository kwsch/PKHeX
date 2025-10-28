using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SealSticker8b
{
    private const int SIZE = 0xC;
    public readonly int Index; // not serialized
    public const int MaxValue = SealList8b.SealMaxCount;

    public bool IsGet { get; set; }
    public int Count { get; set; }
    public int TotalCount { get; set; }

    public SealSticker8b(Span<byte> raw, int index)
    {
        Index = index;
        var offset = (SIZE * index);
        var span = raw.Slice(offset, SIZE);
        ReadAbsolute(span);
    }

    private void ReadAbsolute(ReadOnlySpan<byte> span)
    {
        IsGet = ReadUInt32LittleEndian(span) == 1;
        Count = ReadInt32LittleEndian(span[4..]);
        TotalCount = ReadInt32LittleEndian(span[8..]);
    }

    public void Write(Span<byte> data)
    {
        var offset = (SIZE * Index);
        var span = data.Slice(offset, SIZE);
        WriteAbsolute(span);
    }

    private void WriteAbsolute(Span<byte> span)
    {
        WriteUInt32LittleEndian(span, IsGet ? 1u : 0u);
        WriteInt32LittleEndian(span[4..], Count);
        WriteInt32LittleEndian(span[8..], TotalCount);
    }
}

public enum Seal8b
{
    UNKNOWN = 0,
    HEART_A = 1,
    HEART_B = 2,
    HEART_C = 3,
    HEART_D = 4,
    HEART_E = 5,
    HEART_F = 6,
    STAR_A = 7,
    STAR_B = 8,
    STAR_C = 9,
    STAR_D = 10,
    STAR_E = 11,
    STAR_F = 12,
    LINE_A = 13,
    LINE_B = 14,
    LINE_C = 15,
    LINE_D = 16,
    SMOKE_A = 17,
    SMOKE_B = 18,
    SMOKE_C = 19,
    SMOKE_D = 20,
    ELECTRIC_A = 21,
    ELECTRIC_B = 22,
    ELECTRIC_C = 23,
    ELECTRIC_D = 24,
    BUBBLE_A = 25,
    BUBBLE_B = 26,
    BUBBLE_C = 27,
    BUBBLE_D = 28,
    FIRE_A = 29,
    FIRE_B = 30,
    FIRE_C = 31,
    FIRE_D = 32,
    PARTY_A = 33,
    PARTY_B = 34,
    PARTY_C = 35,
    PARTY_D = 36,
    FLOWER_A = 37,
    FLOWER_B = 38,
    FLOWER_C = 39,
    FLOWER_D = 40,
    FLOWER_E = 41,
    FLOWER_F = 42,
    SONG_A = 43,
    SONG_B = 44,
    SONG_C = 45,
    SONG_D = 46,
    SONG_E = 47,
    SONG_F = 48,
    SONG_G = 49,
    DARK_A = 50,
    DARK_B = 51,
    DARK_C = 52,
    PRETTY_A = 53,
    PRETTY_B = 54,
    PRETTY_C = 55,
    COOL_A = 56,
    COOL_B = 57,
    COOL_C = 58,
    BURNING_A = 59,
    BURNING_B = 60,
    BURNING_C = 61,
    SKY_A = 62,
    SKY_B = 63,
    SKY_C = 64,
    ROCK_A = 65,
    ROCK_B = 66,
    ROCK_C = 67,
    LEAF_A = 68,
    LEAF_B = 69,
    LEAF_C = 70,
    SPARK_A = 71,
    SPARK_B = 72,
    SPARK_C = 73,
    DRESSER_A = 74,
    KAKKOYOSA_A = 75,
    KAKKOYOSA_B = 76,
    KAKKOYOSA_C = 77,
    KAKKOYOSA_D = 78,
    BEAUTY_A = 79,
    BEAUTY_B = 80,
    BEAUTY_C = 81,
    BEAUTY_D = 82,
    SMART_A = 83,
    SMART_B = 84,
    SMART_C = 85,
    SMART_D = 86,
    STRENGTH_A = 87,
    STRENGTH_B = 88,
    STRENGTH_C = 89,
    STRENGTH_D = 90,
    CUTE_A = 91,
    CUTE_B = 92,
    CUTE_C = 93,
    CUTE_D = 94,
    SHOWMASTER_A = 95,
    CHAMPION_A = 96,
}
