using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;
// ReSharper disable UnusedMember.Global

namespace PKHeX.Core;

public sealed class RaidSevenStar9(SAV9SV sav, SCBlock captureBlock, SCBlock defeatedBlock)
{
    public readonly RaidSevenStarCaptured9 Captured = new(sav, captureBlock);
    public readonly RaidSevenStarDefeated9 Defeated = new(sav, defeatedBlock);

    public int CountAll => Captured.CountAll;

    public SevenStarRaidDetail GetRaid(int entry) => new(Captured.GetRaid(entry), Defeated.GetRaid(entry));

    public SevenStarRaidDetail[] GetAllRaids()
    {
        var result = new SevenStarRaidDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }
}

public sealed class SevenStarRaidDetail(SevenStarRaidCapturedDetail captured, SevenStarRaidDefeatedDetail? defeated)
{
    private const string General = nameof(General);

    [Category(General), Description("Identifier used for this 7 Star Raid. Matches the date this raid was first distributed.")]
    public uint Identifier
    {
        get => captured.Identifier;
        set {
            captured.Identifier = value;
            if (defeated != null) defeated.Identifier = value;
        }
    }

    [Category(General), Description("Indicates if this Tera Raid Boss has been captured by the player.")]
    public bool Captured
    {
        get => captured.Captured;
        set => captured.Captured = value;
    }

    [Category(General), Description("Indicates if this Tera Raid Boss has been defeated at least once by the player.")]
    public bool Defeated
    {
        get => defeated != null ? defeated.Defeated : captured.Defeated;
        set {
            if (defeated != null)
                defeated.Defeated = value;
            else
                captured.Defeated = value;
        } 
    }
}

public sealed class RaidSevenStarCaptured9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public readonly int CountAll = block.Data.Length / SevenStarRaidCapturedDetail.SIZE;

    public SevenStarRaidCapturedDetail GetRaid(int entry) => new(Data, 0x00 + (entry * SevenStarRaidCapturedDetail.SIZE));

    public SevenStarRaidCapturedDetail[] GetAllRaids()
    {
        var result = new SevenStarRaidCapturedDetail[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }
}

public sealed class SevenStarRaidCapturedDetail(byte[] Data, int Offset)
{
    public const int SIZE = 0x08;

    public uint Identifier
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }

    public bool Captured
    {
        get => Data[Offset + 0x04] == 1;
        set => Data[Offset + 0x04] = (byte)(value ? 1 : 0);
    }

    public bool Defeated
    {
        get => Data[Offset + 0x05] == 1;
        set => Data[Offset + 0x05] = (byte)(value ? 1 : 0);
    }

    // 0x06 - 0x07 padding
}

public sealed class RaidSevenStarDefeated9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    // Structure matches the RaidSevenStarCapture9 but there are 4 bytes at the front to indicate if the copy of previous defeated flags happened when updating save
    public readonly int CountAll = (block.Data.Length - 0x04) / SevenStarRaidDefeatedDetail.SIZE;

    public SevenStarRaidDefeatedDetail? GetRaid(int entry) => block.Type != SCTypeCode.None ? new(Data, 0x04 + (entry * SevenStarRaidDefeatedDetail.SIZE)) : null;

    public SevenStarRaidDefeatedDetail?[] GetAllRaids()
    {
        var result = new SevenStarRaidDefeatedDetail?[CountAll];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetRaid(i);
        return result;
    }
}

public sealed class SevenStarRaidDefeatedDetail(byte[] Data, int Offset)
{
    public const int SIZE = 0x08;

    public uint Identifier
    {
        get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }

    public bool Defeated
    {
        get => Data[Offset + 0x04] == 1;
        set => Data[Offset + 0x04] = (byte)(value ? 1 : 0);
    }

    // 0x05 - 0x07 padding
}
