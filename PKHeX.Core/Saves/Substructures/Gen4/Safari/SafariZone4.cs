using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class SafariZone4(Memory<byte> Raw)
{
    public const int SIZE = (2 * SafariZoneAreaSet4.SIZE) + SafariZoneLinkLeader4.SIZE + 4; // 0x5FC

    private Span<byte> Data => Raw.Span;

    public SafariZoneAreaSet4 Self => new(Raw[..SafariZoneAreaSet4.SIZE]);
    public SafariZoneAreaSet4 Link => new(Raw.Slice(SafariZoneAreaSet4.SIZE, SafariZoneAreaSet4.SIZE));
    public SafariZoneLinkLeader4 LinkLeader => new(Raw.Slice(SafariZoneAreaSet4.SIZE * 2, SafariZoneLinkLeader4.SIZE));

    private const int Tail = (2 * SafariZoneAreaSet4.SIZE) + SafariZoneLinkLeader4.SIZE; // 0x5F8

    public byte DayDelay { get => Data[Tail]; set => Data[Tail] = value; }
    private byte Flags { get => Data[Tail + 1]; set => Data[Tail + 1] = value; } // raw offset in save file: 0xC735

    // 0x5FA
    /// <summary>
    /// Amount of minutes needed for thresholds to unlock.
    /// </summary>
    public ushort ElapsedPlayMinutes { get => ReadUInt16LittleEndian(Data[(Tail + 2)..]); set => WriteUInt16LittleEndian(Data[(Tail + 2)..], value); }

    // Offset 0xC735: Safari Zone Progress
    // 0x00 Initial State
    // 0x40 Unlocked Area Switcher
    // 0x41 Unlocked Waterside Objects
    // 0x42 Unlocked Plains Objects
    // 0x43 Unlocked Forest Objects
    // 0x44 Unlocked Peak Objects
    // ^Looks like top bits are story progression, low bits are progression on which Objects have been unlocked

    // 6 bits for level
    public byte ObjectUnlockLevel
    {
        get => (byte)(Flags & 0x3F);
        set => Flags = (byte)((Flags & 0xC0) | (value & 0x3F));
    }

    public byte AreaSwitcherState // 0 or 1, even though 2 bits are available
    {
        get => (byte)((Flags >> 6) & 0x03);
        set => Flags = (byte)((Flags & 0x3F) | (((value == 1 ? 1 : 0) & 0x03) << 6)); // ignore 2 & 3, only 0 or 1 is valid
    }
}
