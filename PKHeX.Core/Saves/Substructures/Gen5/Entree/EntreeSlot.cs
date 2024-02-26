using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="EntreeForest"/> slot
/// </summary>
public sealed class EntreeSlot(Memory<byte> Data) : ISpeciesForm
{
    /// <summary>
    /// <see cref="PKM.Species"/> index
    /// </summary>
    public ushort Species // bits 0-10
    {
        get => (ushort)((RawValue & 0x3FF) >> 0);
        set => RawValue = (RawValue & 0xFFFF_F800) | ((value & 0x3FFu) << 0);
    }

    /// <summary>
    /// Special Move
    /// </summary>
    public ushort Move // bits 11-20
    {
        get => (ushort)((RawValue & 0x001F_F800) >> 11);
        set => RawValue = (RawValue & 0xFFE0_07FF) | ((value & 0x3FFu) << 11);
    }

    /// <summary>
    /// <see cref="PKM.Gender"/> index
    /// </summary>
    public byte Gender // bits 21-22
    {
        get => (byte)((RawValue & 0x0060_0000) >> 21);
        set => RawValue = (RawValue & 0xFF9F_FFFF) | ((uint)(value & 0x3) << 21);
    }

    /// <summary>
    /// <see cref="PKM.Form"/> index
    /// </summary>
    public byte Form // bits 23-27
    {
        get => (byte)((RawValue & 0x0F80_0000) >> 23);
        set => RawValue = (RawValue & 0xF07F_FFFF) | ((value & 0x1Fu) << 23);
    }

    /// <summary>
    /// Visibility Flag
    /// </summary>
    public bool Invisible // bit 28
    {
        get => ((RawValue >> 28) & 1) == 1;
        set => RawValue = (RawValue & 0xEFFFFFFF) | (value ? 0 : 1u << 28);
    }

    /// <summary>
    /// Animation Leash (How many steps it can deviate from its spawn location).
    /// </summary>
    public int Animation // bits 29-31
    {
        get => (int)(RawValue >> 29);
        set => RawValue = ((RawValue << 3) >> 3) | (uint)((value & 0x7) << 29);
    }

    private Memory<byte> Data { get; } = Data;

    /// <summary>
    /// Raw Data Value
    /// </summary>
    public uint RawValue
    {
        get => ReadUInt32LittleEndian(Data.Span);
        set => WriteUInt32LittleEndian(Data.Span, value);
    }

    /// <summary>
    /// Resets the raw data value to 0.
    /// </summary>
    public void Delete() => RawValue = 0;

    public const int SIZE = 4;

    /// <summary>
    /// Indicates which area the slot data originated from.
    /// Extra metadata for the slot which is not stored in the raw data.
    /// </summary>
    public EntreeForestArea Area { get; init; }
}
