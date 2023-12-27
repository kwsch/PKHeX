using System;
using System.Diagnostics;

namespace PKHeX.Core;

internal sealed class OPowerFlagSet(int Count, OPower6Type Identifier)
{
    public readonly OPower6Type Identifier = Identifier;
    public readonly int Count = Count;

    public int BaseCount => Math.Min(3, Count);
    public bool HasOPowerS => Count > 3;
    public bool HasOPowerMAX => Count == 5;
    public int Offset { get; init; }

    public int GetOPowerLevel(ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < BaseCount; i++)
        {
            if (GetFlag(data, i))
                continue;
            Debug.WriteLine($"Fetched {Identifier}: {i}");
            return i;
        }

        Debug.WriteLine($"Fetched {Identifier}: {BaseCount}");
        return BaseCount;
    }

    public void SetOPowerLevel(Span<byte> data, int value)
    {
        Debug.WriteLine($"Setting {Identifier}: {value}");
        for (int i = 0; i < BaseCount; i++)
            SetFlag(data, i, i + 1 <= value);
        Debug.Assert(value == GetOPowerLevel(data));
    }

    public bool GetOPowerS(ReadOnlySpan<byte> data) => HasOPowerS && GetFlag(data, 3);
    public bool GetOPowerMAX(ReadOnlySpan<byte> data) => HasOPowerMAX && GetFlag(data, 4);
    public void SetOPowerS(Span<byte> data, bool value) => SetFlag(data, 3, value);
    public void SetOPowerMAX(Span<byte> data, bool value) => SetFlag(data, 4, value);

    private bool GetFlag(ReadOnlySpan<byte> data, int index)
    {
        return data[Offset + index] == (byte)OPowerFlagState.Unlocked;
    }

    private void SetFlag(Span<byte> data, int index, bool value)
    {
        if (index < Count)
            data[Offset + index] = (byte)(value ? OPowerFlagState.Unlocked : OPowerFlagState.Locked);
    }
}
