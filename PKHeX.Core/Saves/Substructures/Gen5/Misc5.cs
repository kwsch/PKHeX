using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public abstract class Misc5 : SaveBlock<SAV5>, IGymTeamInfo
{
    protected Misc5(SAV5 sav, int offset) : base(sav) => Offset = offset;

    public uint Money
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset), value);
    }

    public int Badges
    {
        get => Data[Offset + 0x4];
        set => Data[Offset + 0x4] = (byte)value;
    }

    public ushort PokeTransferMinigameScore
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + TransferMinigameScoreOffset));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + TransferMinigameScoreOffset), value);
    }

    protected abstract int BadgeVictoryOffset { get; }
    protected abstract int TransferMinigameScoreOffset { get; }

    private int GetBadgeVictorySpeciesOffset(uint badge, uint slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(badge, 8);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>(slot, 6);

        return Offset + BadgeVictoryOffset + (int)(((6 * badge) + slot) * sizeof(ushort));
    }

    public ushort GetBadgeVictorySpecies(uint badge, uint slot)
    {
        var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
        return ReadUInt16LittleEndian(Data.AsSpan(ofs));
    }

    public void SetBadgeVictorySpecies(uint badge, uint slot, ushort species)
    {
        var ofs = GetBadgeVictorySpeciesOffset(badge, slot);
        WriteUInt16LittleEndian(SAV.Data.AsSpan(ofs), species);
    }
}

public sealed class Misc5BW(SAV5BW sav, int offset) : Misc5(sav, offset)
{
    protected override int TransferMinigameScoreOffset => 0x14;
    protected override int BadgeVictoryOffset => 0x58; // thru 0xB7
}

public sealed class Misc5B2W2(SAV5B2W2 sav, int offset) : Misc5(sav, offset)
{
    protected override int TransferMinigameScoreOffset => 0x18;
    protected override int BadgeVictoryOffset => 0x5C; // thru 0xBB
}
