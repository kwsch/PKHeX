using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Misc5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw), IGymTeamInfo
{
    public uint Money
    {
        get => ReadUInt32LittleEndian(Data);
        set => WriteUInt32LittleEndian(Data, value);
    }

    public int Badges
    {
        get => Data[0x4];
        set => Data[0x4] = (byte)value;
    }

    public ushort PokeTransferMinigameScore
    {
        get => ReadUInt16LittleEndian(Data[TransferMinigameScoreOffset..]);
        set => WriteUInt16LittleEndian(Data[TransferMinigameScoreOffset..], value);
    }

    protected abstract int BadgeVictoryOffset { get; }
    protected abstract int TransferMinigameScoreOffset { get; }

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

public sealed class Misc5BW(SAV5BW sav, Memory<byte> raw) : Misc5(sav, raw)
{
    protected override int TransferMinigameScoreOffset => 0x14;
    protected override int BadgeVictoryOffset => 0x58; // thru 0xB7

    public const uint LibertyTicketMagic = 2010_04_06; // 0x132B536

    public uint LibertyTicketState
    {
        get => ReadUInt32LittleEndian(Data[0xBC..]);
        set => WriteUInt32LittleEndian(Data[0xBC..], value);
    }

    public uint LibertyTicketExpectValue => LibertyTicketMagic ^ sav.ID32;

    public bool IsLibertyTicketActivated
    {
        get => LibertyTicketState == LibertyTicketExpectValue;
        set => LibertyTicketState = value ? LibertyTicketExpectValue : 0;
    }
}

public sealed class Misc5B2W2(SAV5B2W2 sav, Memory<byte> raw) : Misc5(sav, raw)
{
    protected override int TransferMinigameScoreOffset => 0x18;
    protected override int BadgeVictoryOffset => 0x5C; // thru 0xBB
}
