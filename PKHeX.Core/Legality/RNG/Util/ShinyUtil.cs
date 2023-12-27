using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

public static class ShinyUtil
{
    public static uint GetShinyPID(in ushort tid, in ushort sid, in uint pid, in uint type)
    {
        var low = pid & 0xFFFF;
        return ((type ^ tid ^ sid ^ low) << 16) | low;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetIsShiny(in uint id32, in uint pid, [ConstantExpected(Max = 16, Min = 8)] uint cmp = 16) => GetShinyXor(id32, pid) < cmp;

    public static uint GetShinyXor(in uint pid, in uint id32)
    {
        var xor = pid ^ id32;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForceShinyState(bool isShiny, ref uint pid, uint id32, in uint xorType)
    {
        if (isShiny)
        {
            if (!GetIsShiny(id32, pid))
                pid = GetShinyPID((ushort)(id32 & 0xFFFFu), (ushort)(id32 >> 16), pid, xorType);
        }
        else
        {
            if (GetIsShiny(id32, pid))
                pid ^= 0x1000_0000;
        }
    }
}
