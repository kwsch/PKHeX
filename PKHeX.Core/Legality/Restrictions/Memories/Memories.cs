using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Memory parameters &amp; validation
/// </summary>
public static class Memories
{
    private static ReadOnlySpan<byte> ArgTypes =>
    [
        0, 1, 1, 1, 1, 5, 2, 3, 0, 3,
        0, 0, 4, 3, 3, 5, 4, 3, 3, 1,
        0, 3, 0, 0, 1, 3, 5, 0, 0, 3,
        0, 1, 1, 1, 5, 1, 1, 1, 1, 1,
        5, 0, 1, 0, 3, 3, 0, 0, 4, 4,
        3, 5, 1, 0, 0, 0, 0, 0, 0, 1,
        3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        1, 3, 3, 0, 0, 3, 0, 0, 0, 0,
        4, 4, 3, 3, 5, 0, 1, 3, 5, 4,
    ];

    public static MemoryArgType GetMemoryArgType(byte memory, int memoryGen)
    {
        var arr = ArgTypes;
        if (memory >= arr.Length)
            return MemoryArgType.None;

        var type = arr[memory];
        if (memoryGen > 6 && type == (int)MemoryArgType.SpecificLocation)
            return MemoryArgType.GeneralLocation;
        return (MemoryArgType)type;
    }

    public static MemoryContext GetContext(EntityContext context) => context.Generation() switch
    {
        <=7 => MemoryContext6.Instance,
        _ => MemoryContext8.Instance,
    };

    public static IEnumerable<ushort> GetMemoryItemParams(EntityContext context) => GetContext(context).GetMemoryItemParams();
}
