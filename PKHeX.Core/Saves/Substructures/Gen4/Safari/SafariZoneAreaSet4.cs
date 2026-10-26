using System;

namespace PKHeX.Core;

public sealed class SafariZoneAreaSet4(Memory<byte> Raw)
{
    private Span<byte> Data => Raw.Span;

    public const int MapCountConfiguration = 6;
    private const int MapCountTotal = 12;

    public const int SIZE = MapCountConfiguration * SafariZoneArea4.SIZE + MapCountTotal; // 0x2E8

    public SafariZoneArea4[] Configurations { get; } = ReadConfigurations(Raw.Span); // raw offset 0xC13C within the save file.
    private byte[] AreaAges { get; } = ReadMapAges(Raw.Span[(MapCountConfiguration * SafariZoneArea4.SIZE)..]);


    public byte GetMapAge(SafariZoneType4 map) => AreaAges[(int)map];
    public void SetMapAge(SafariZoneType4 map, byte level) => AreaAges[(int)map] = level;

    public void FlushMapAges() => WriteMapAges(Data[(MapCountConfiguration * SafariZoneArea4.SIZE)..], AreaAges);

    private static SafariZoneArea4[] ReadConfigurations(ReadOnlySpan<byte> data)
    {
        var result = new SafariZoneArea4[MapCountConfiguration];
        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * SafariZoneArea4.SIZE, SafariZoneArea4.SIZE);
            result[i] = new SafariZoneArea4(slice);
        }
        return result;
    }

    public void FlushConfiguration(int configurationIndex, bool autoCount = false)
    {
        var configuration = Configurations[configurationIndex];
        var slice = Data.Slice(configurationIndex * SafariZoneArea4.SIZE, SafariZoneArea4.SIZE);
        configuration.Write(slice, autoCount);
    }

    private static byte[] ReadMapAges(ReadOnlySpan<byte> data) => [.. data[..MapCountTotal]];
    private static void WriteMapAges(Span<byte> result, ReadOnlySpan<byte> ages) => ages.CopyTo(result);
}
