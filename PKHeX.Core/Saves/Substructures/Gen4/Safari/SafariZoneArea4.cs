using System;

namespace PKHeX.Core;

/// <summary>
/// Configuration for a specific Safari Zone area.
/// </summary>
public sealed class SafariZoneArea4
{
    /// <summary>
    /// Configuration for a specific Safari Zone area.
    /// </summary>
    /// <param name="data"></param>
    public SafariZoneArea4(ReadOnlySpan<byte> data)
    {
        Map = (SafariZoneType4)data[0];
        Count = data[1];
        Objects = ReadObjects(data[BlockStart..]);
    }

    public const int BlockCount = 30;
    private const int BlockStart = 2;
    public const int SIZE = BlockStart + (BlockCount * SafariZoneObject4.SIZE); // 0x7A

    /// <summary> Area the configuration is for. </summary>
    public SafariZoneType4 Map { get; set; }

    /// <summary> Count of <see cref="Objects"/> that are placed by the owner. </summary>
    public byte Count { get; set; }

    /// <summary> Array of <see cref="SafariZoneObject4"/> that are placed by the owner. </summary>
    public SafariZoneObject4[] Objects { get; private set; }

    private static SafariZoneObject4[] ReadObjects(ReadOnlySpan<byte> data)
    {
        var result = new SafariZoneObject4[BlockCount];
        for (int i = 0; i < result.Length; i++)
        {
            var slice = data.Slice(i * 4, 4);
            result[i] = new SafariZoneObject4(slice);
        }
        return result;
    }

    private static void WriteObjects(Span<byte> data, ReadOnlySpan<SafariZoneObject4> objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            var slice = data.Slice(i * 4, 4);
            objects[i].Write(slice);
        }
    }

    public void SortObjects()
    {
        // Sort the objects by type, then by X, then by Y, then by Z.
        // Empty type slots will be sorted to the end.
        Array.Sort(Objects, (a, b) =>
        {
            if (a is { IsEmpty: true }) return 1;
            if (b is { IsEmpty: true }) return -1;

            int typeComparison = a.Type.CompareTo(b.Type);
            if (typeComparison != 0) return typeComparison;
            int xComparison = a.X.CompareTo(b.X);
            if (xComparison != 0) return xComparison;
            int yComparison = a.Y.CompareTo(b.Y);
            if (yComparison != 0) return yComparison;
            return a.Z.CompareTo(b.Z);
        });
    }

    public int CleanEmpty()
    {
        SortObjects();
        int count = 0;
        for (var i = 0; i < Objects.Length; i++)
        {
            ref var obj = ref Objects[i];
            if (obj.IsEmpty)
                obj = default;
            else
                count++;
        }
        return count;
    }

    public void Read(ReadOnlySpan<byte> data)
    {
        Map = (SafariZoneType4)data[0];
        Count = data[1];
        Objects = ReadObjects(data[BlockStart..]);
    }

    public void Write(Span<byte> data, bool autoCount)
    {
        data[0] = (byte)Map;
        data[1] = autoCount ? (byte)CleanEmpty() : Count;
        WriteObjects(data[BlockStart..], Objects);
    }

    public void ClearObjects()
    {
        for (var i = 0; i < Objects.Length; i++)
            Objects[i] = default;
        Count = 0;
    }
}
