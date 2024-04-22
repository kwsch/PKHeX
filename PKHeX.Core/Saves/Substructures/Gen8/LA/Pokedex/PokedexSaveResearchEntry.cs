using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.PokedexResearchTaskType8a;

namespace PKHeX.Core;

/// <summary>
/// Per-species research logs used for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public sealed class PokedexSaveResearchEntry(Memory<byte> raw)
{
    private Span<byte> Data => raw.Span;
    public const int SIZE = 0x58;

    public uint Flags              { get => ReadUInt32LittleEndian(Data);  set => WriteUInt32LittleEndian(Data, value); }
    public bool HasEverBeenUpdated { get => (Flags & (1u << 0)) != 0; set => Flags = (Flags & ~(1u << 0)) | ((value ? 1u : 0u) << 0); }
    public bool HasAnyReport       { get => (Flags & (1u << 1)) != 0; set => Flags = (Flags & ~(1u << 1)) | ((value ? 1u : 0u) << 1); }
    public bool IsPerfect          { get => (Flags & (1u << 2)) != 0; set => Flags = (Flags & ~(1u << 2)) | ((value ? 1u : 0u) << 2); }
    public bool SelectedGender1    { get => (Flags & (1u << 3)) != 0; set => Flags = (Flags & ~(1u << 3)) | ((value ? 1u : 0u) << 3); }
    public bool SelectedShiny      { get => (Flags & (1u << 4)) != 0; set => Flags = (Flags & ~(1u << 4)) | ((value ? 1u : 0u) << 4); }
    public bool SelectedAlpha      { get => (Flags & (1u << 5)) != 0; set => Flags = (Flags & ~(1u << 5)) | ((value ? 1u : 0u) << 5); }
    public bool IsSolitudeComplete { get => (Flags & (1u << 6)) != 0; set => Flags = (Flags & ~(1u << 6)) | ((value ? 1u : 0u) << 6); }

    private uint ReportedResearchProgress { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }

    public int GetReportedResearchProgress(int idx) => (int)((ReportedResearchProgress >> (3 * idx)) & 7);
    public void SetReportedResearchProgress(int idx, int level)
    {
        var prog = ReportedResearchProgress;
        prog &= ~(7u << (3 * idx));
        prog |= (uint)((level & 7u) << (3 * idx));
        ReportedResearchProgress = prog;
    }

    public ushort ResearchRate { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    public ushort NumObtained { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }
    public ushort UpdateCounter { get => ReadUInt16LittleEndian(Data[0x0C..]); set => WriteUInt16LittleEndian(Data[0x0C..], value); }
    public ushort LastUpdatedReportCounter { get => ReadUInt16LittleEndian(Data[0x0E..]); set => WriteUInt16LittleEndian(Data[0x0E..], value); }

    public ushort GetMoveUseCount(int index)
    {
        if (index >= 4)
            return 0;

        return ReadUInt16LittleEndian(Data[(0x10 + (0x2 * index))..]);
    }

    public void SetMoveUseCount(int index, ushort newCount)
    {
        if (index >= 4)
            return;

        WriteUInt16LittleEndian(Data[(0x10 + (0x2 * index))..], newCount);
    }

    public ushort GetDefeatWithMoveTypeCount(int index)
    {
        if (index >= 3)
            return 0;

        return ReadUInt16LittleEndian(Data[(0x18 + (0x2 * index))..]);
    }

    public void SetDefeatWithMoveTypeCount(int index, ushort newCount)
    {
        if (index >= 3)
            return;

        WriteUInt16LittleEndian(Data[(0x18 + (0x2 * index))..], newCount);
    }

    public ushort NumDefeated { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }
    public ushort NumEvolved { get => ReadUInt16LittleEndian(Data[0x20..]); set => WriteUInt16LittleEndian(Data[0x20..], value); }
    public ushort NumAlphaCaught { get => ReadUInt16LittleEndian(Data[0x22..]); set => WriteUInt16LittleEndian(Data[0x22..], value); }
    public ushort NumLargeCaught { get => ReadUInt16LittleEndian(Data[0x24..]); set => WriteUInt16LittleEndian(Data[0x24..], value); }
    public ushort NumSmallCaught { get => ReadUInt16LittleEndian(Data[0x26..]); set => WriteUInt16LittleEndian(Data[0x26..], value); }
    public ushort NumHeavyCaught { get => ReadUInt16LittleEndian(Data[0x28..]); set => WriteUInt16LittleEndian(Data[0x28..], value); }
    public ushort NumLightCaught { get => ReadUInt16LittleEndian(Data[0x2A..]); set => WriteUInt16LittleEndian(Data[0x2A..], value); }
    public ushort NumCaughtAtTime { get => ReadUInt16LittleEndian(Data[0x2C..]); set => WriteUInt16LittleEndian(Data[0x2C..], value); }
    public ushort NumCaughtSleeping { get => ReadUInt16LittleEndian(Data[0x2E..]); set => WriteUInt16LittleEndian(Data[0x2E..], value); }
    public ushort NumCaughtInAir { get => ReadUInt16LittleEndian(Data[0x30..]); set => WriteUInt16LittleEndian(Data[0x30..], value); }
    public ushort NumCaughtNotSpotted { get => ReadUInt16LittleEndian(Data[0x32..]); set => WriteUInt16LittleEndian(Data[0x32..], value); }
    public ushort NumGivenFood { get => ReadUInt16LittleEndian(Data[0x34..]); set => WriteUInt16LittleEndian(Data[0x34..], value); }
    public ushort NumStunnedWithItems { get => ReadUInt16LittleEndian(Data[0x36..]); set => WriteUInt16LittleEndian(Data[0x36..], value); }
    public ushort NumScared { get => ReadUInt16LittleEndian(Data[0x38..]); set => WriteUInt16LittleEndian(Data[0x38..], value); }
    public ushort NumLured { get => ReadUInt16LittleEndian(Data[0x3A..]); set => WriteUInt16LittleEndian(Data[0x3A..], value); }
    public ushort NumStrongStyleMovesUsed { get => ReadUInt16LittleEndian(Data[0x3C..]); set => WriteUInt16LittleEndian(Data[0x3C..], value); }
    public ushort NumAgileStyleMovesUsed { get => ReadUInt16LittleEndian(Data[0x3E..]); set => WriteUInt16LittleEndian(Data[0x3E..], value); }
    public ushort NumLeapFromTrees { get => ReadUInt16LittleEndian(Data[0x40..]); set => WriteUInt16LittleEndian(Data[0x40..], value); }
    public ushort NumLeapFromLeaves { get => ReadUInt16LittleEndian(Data[0x42..]); set => WriteUInt16LittleEndian(Data[0x42..], value); }
    public ushort NumLeapFromSnow { get => ReadUInt16LittleEndian(Data[0x44..]); set => WriteUInt16LittleEndian(Data[0x44..], value); }
    public ushort NumLeapFromOre { get => ReadUInt16LittleEndian(Data[0x46..]); set => WriteUInt16LittleEndian(Data[0x46..], value); }
    public ushort NumLeapFromTussocks { get => ReadUInt16LittleEndian(Data[0x48..]); set => WriteUInt16LittleEndian(Data[0x48..], value); }
    public ushort Field_4A { get => ReadUInt16LittleEndian(Data[0x4A..]); set => WriteUInt16LittleEndian(Data[0x4A..], value); }
    public uint Field_4C { get => ReadUInt32LittleEndian(Data[0x4C..]); set => WriteUInt32LittleEndian(Data[0x4C..], value); }
    public byte SelectedForm { get => Data[0x50]; set => Data[0x50] = value; }
    // 51-53 padding
    public uint Field_54 { get => ReadUInt32LittleEndian(Data[0x54..]); set => WriteUInt32LittleEndian(Data[0x54..], value); }

    public void IncreaseCurrentResearchLevel(PokedexResearchTaskType8a task, int idx, ushort delta)
    {
        SetCurrentResearchLevel(task, idx, GetCurrentResearchLevel(task, idx) + delta);
    }

    public int GetCurrentResearchLevel(PokedexResearchTaskType8a task, int idx) => task switch
    {
        Catch => NumObtained,
        UseMove => GetMoveUseCount(idx),
        DefeatWithMoveType => GetDefeatWithMoveTypeCount(idx),
        Defeat => NumDefeated,
        Evolve => NumEvolved,
        CatchAlpha => NumAlphaCaught,
        CatchLarge => NumLargeCaught,
        CatchSmall => NumSmallCaught,
        CatchHeavy => NumHeavyCaught,
        CatchLight => NumLightCaught,
        CatchAtTime => NumCaughtAtTime,
        CatchSleeping => NumCaughtSleeping,
        CatchInAir => NumCaughtInAir,
        CatchNotSpotted => NumCaughtNotSpotted,
        GiveFood => NumGivenFood,
        StunWithItems => NumStunnedWithItems,
        ScareWithScatterBang => NumScared,
        LureWithPokeshiDoll => NumLured,
        UseStrongStyleMove => NumStrongStyleMovesUsed,
        UseAgileStyleMove => NumAgileStyleMovesUsed,
        LeapFromTrees => NumLeapFromTrees,
        LeapFromLeaves => NumLeapFromLeaves,
        LeapFromSnow => NumLeapFromSnow,
        LeapFromOre => NumLeapFromOre,
        LeapFromTussocks => NumLeapFromTussocks,
        _ => throw new ArgumentOutOfRangeException(nameof(task)),
    };

    public void SetCurrentResearchLevel(PokedexResearchTaskType8a task, int idx, int value)
    {
        // Bound values in [0, 60000]
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        var cappedValue = (ushort)Math.Min(value, PokedexConstants8a.MaxPokedexResearchPoints);
        switch (task)
        {
            case Catch:           NumObtained             = cappedValue; break;
            case UseMove:            SetMoveUseCount(idx, cappedValue);  break;
            case DefeatWithMoveType: SetDefeatWithMoveTypeCount(idx, cappedValue); break;
            case Defeat:          NumDefeated             = cappedValue; break;
            case Evolve:          NumEvolved              = cappedValue; break;
            case CatchAlpha:      NumAlphaCaught          = cappedValue; break;
            case CatchLarge:      NumLargeCaught          = cappedValue; break;
            case CatchSmall:      NumSmallCaught          = cappedValue; break;
            case CatchHeavy:      NumHeavyCaught          = cappedValue; break;
            case CatchLight:      NumLightCaught          = cappedValue; break;
            case CatchAtTime:     NumCaughtAtTime         = cappedValue; break;
            case CatchSleeping:   NumCaughtSleeping       = cappedValue; break;
            case CatchInAir:      NumCaughtInAir          = cappedValue; break;
            case CatchNotSpotted: NumCaughtNotSpotted     = cappedValue; break;
            case GiveFood:        NumGivenFood            = cappedValue; break;
            case StunWithItems:   NumStunnedWithItems     = cappedValue; break;
            case ScareWithScatterBang: NumScared          = cappedValue; break;
            case LureWithPokeshiDoll:  NumLured           = cappedValue; break;
            case UseStrongStyleMove:   NumStrongStyleMovesUsed = cappedValue; break;
            case UseAgileStyleMove:    NumAgileStyleMovesUsed  = cappedValue; break;
            case LeapFromTrees:        NumLeapFromTrees        = cappedValue; break;
            case LeapFromLeaves:       NumLeapFromLeaves       = cappedValue; break;
            case LeapFromSnow:         NumLeapFromSnow         = cappedValue; break;
            case LeapFromOre:          NumLeapFromOre          = cappedValue; break;
            case LeapFromTussocks:     NumLeapFromTussocks     = cappedValue; break;
            default: throw new ArgumentOutOfRangeException(nameof(task));
        }
    }
}
