using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// SUBE block that stores in-game event results.
/// </summary>
public abstract class SubEventLog6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw), IGymTeamInfo
{
    protected abstract int BadgeVictoryOffset { get; }

    /// <summary>
    /// Absolute offset of the <see cref="PK6"/> that the player has given an NPC.
    /// </summary>
    public abstract int Give { get; }

    public Memory<byte> GiveSlot => Raw.Slice(Give, PokeCrypto.SIZE_6STORED);

    /// <summary>
    /// Absolute offset of the <see cref="PK6"/> that is unreferenced?
    /// </summary>
    public abstract int UnusedPKM { get; }

    private int GetBadgeVictorySpeciesOffset(uint badge, uint slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(badge, 8);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(slot, 6);
        return BadgeVictoryOffset + (int)(((6 * badge) + slot) * sizeof(ushort));
    }

    public ushort GetBadgeVictorySpecies(uint badge, uint slot)
    {
        var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
        return ReadUInt16LittleEndian(Data[ofs..]);
    }

    public void SetBadgeVictorySpecies(uint badge, uint slot, ushort species)
    {
        var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
        WriteUInt16LittleEndian(Data[ofs..], species);
    }
}

public sealed class SubEventLog6XY(SAV6XY sav, Memory<byte> raw) : SubEventLog6(sav, raw)
{
    // Structure:

    // 0x00
    // u8[0x28] chateau data
    private ushort ChateauValue
    {
        get => ReadUInt16LittleEndian(Data);
        set => WriteUInt16LittleEndian(Data, value);
    }

    public ushort ChateauRank
    {
        get => (ushort)(ChateauValue & 0xF);
        set => ChateauValue = (ushort)((ChateauValue & ~0xFu) | (value & 0xFu));
    }

    public ushort ChateauPoints
    {
        get => (ushort)(ChateauValue >> 4);
        set => ChateauValue = (ushort)((ushort)(value << 4) | (ChateauValue & 0xFu));
    }
    // other château data?
    // u32 SUBE @ 0x28

    // 0x2C
    protected override int BadgeVictoryOffset => 0x2C; // thru 0x8B
    // u16[6 * 8] trainer teams for gyms
    // u32 SUBE @ 0x8C

    // 0x90
    // u8[0xE8] pk?
    public override int Give => 0x90;
    // u32 SUBE @ 0x178

    // 0x17C
    // u8[0xE8] pk?
    public override int UnusedPKM => 0x17C;
    // u32 SUBE @ 0x264

    // 0x268
    // u8[0xA0] unused?
}

public sealed class SubEventLog6AO(SAV6AO sav, Memory<byte> raw) : SubEventLog6(sav, raw)
{
    // Structure:

    // 0x00
    // u8[0x5A] trainer rematch flags
    // u8[2] unused (alignment)
    // u32 SUBE @ 0x5C

    // 0x60
    protected override int BadgeVictoryOffset => 0x60; // through 0xBF
    // u16[6 * 8] trainer teams for gyms
    // u32 SUBE @ 0xC0

    // 0xC4
    // u8[0xE8] pk?
    public override int Give => 0xC4;
    // u32 SUBE @ 0x1AC

    // 0x1B0
    // u8[0xE8] pk?
    public override int UnusedPKM => 0x1B0;
    // u32 SUBE @ 0x298

    // 0x29C
    // u16 SeasideCyclingRoadTimeMilliseconds 29C
    public ushort SeasideCyclingRoadTimeMilliseconds
    {
        get => ReadUInt16LittleEndian(Data[0x29C..]);
        set => WriteUInt16LittleEndian(Data[0x29C..], value);
    }
    // u16 SeasideCyclingRoadCollisions 29E
    public ushort SeasideCyclingRoadCollisions
    {
        get => ReadUInt16LittleEndian(Data[0x29E..]);
        set => WriteUInt16LittleEndian(Data[0x29E..], value);
    }
    // u16[7] 2A0
    // u16[7] 2AE
    // u16[17] 2BC
    // u16[7] 2EA
    // u16 2EC
    // u16 2EE
    // u16 2F0
    // u16 2F2
    // u32 SUBE @ 0x2F4

    // 0x2F8
    // u64[27] Ending Scroll ec-specform data -- player pk used & NPC used during storyline battles
    //  u32 ec
    //  u32 species:10
    //  u32 form:5
    //  u32 gender:2
    //  u32 isShiny:1
    //  u32 unused:14
    // u8[16]
    // u8[32] unused?
}
