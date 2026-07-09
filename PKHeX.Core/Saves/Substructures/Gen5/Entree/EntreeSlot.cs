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
    public byte Form // bits 23-28 (6 bits)
    {
        get => (byte)((RawValue & 0x1F80_0000) >> 23);
        set => RawValue = (RawValue & 0xE07F_FFFF) | ((value & 0x3Fu) << 23);
    }

    /// <summary>
    /// Animation restriction.
    /// </summary>
    public EntreeForestAnimation Animation // bits 29-31
    {
        get => (EntreeForestAnimation)(RawValue >> 29);
        set => RawValue = ((RawValue << 3) >> 3) | (uint)(((byte)value & 0x7) << 29);
    }

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

/// <summary>
/// Movement patterns, as observed in-game.
/// </summary>
public enum EntreeForestAnimation : byte
{
    LookRandom             = 0, // random looking around 
    Leash3                 = 1, // walking in a 3x3 radius block
    Leash5                 = 2, // walking in a 5x5 radius block
    MoveUpDown             = 3, // moving up and down only
    MoveLeftRight          = 4, // moving left and right only
    MoveLeftRightLook      = 5, // moving left and right and looking around
    RotateClockwise        = 6, // clockwise rotating
    RotateCounterClockwise = 7, // counterclockwise rotating
}
