using System;
using static PKHeX.Core.BattleFrontierFacility3;
using static PKHeX.Core.BattleFrontierStatType3;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Live view over the Generation 3 (Emerald) Battle Frontier save data block.
/// Provides access to statistics, streaks, and symbols for all seven Battle Frontier facilities.
/// </summary>
public ref struct BattleFrontier3(Span<byte> data)
{
    public const int SIZE = 0x1DC;
    private readonly Span<byte> Data = data;

    private const int ContinueOffset = 0x000;

    /// <summary>Event flag index for the Frontier Pass activation.</summary>
    public static int FrontierPassFlagIndex => 0x860 + 0x72;

    /// <summary>Gets the event flag index for a facility's silver symbol.</summary>
    public static int GetSymbolSilverFlagIndex(BattleFrontierFacility3 facility) => 0x860 + 0x64 + ((int)facility * 2);

    /// <summary>Gets the event flag index for a facility's gold symbol.</summary>
    public static int GetSymbolGoldFlagIndex(BattleFrontierFacility3 facility) => 0x860 + 0x64 + ((int)facility * 2) + 1;

    #region Streaks and Statistics
    /// <summary>
    /// Gets which stat types are valid for a given facility.
    /// </summary>
    public static BattleFrontierStatType3[] GetValidStats(BattleFrontierFacility3 facility) => facility switch
    {
        Tower or Palace or Arena or Pyramid => [CurrentStreak, RecordStreak],
        Dome => [CurrentStreak, RecordStreak, Championships],
        Pike => [CurrentStreak, RecordStreak, RecordCleared],
        Factory => [CurrentStreak, CurrentSwapped, RecordStreak, RecordSwapped],
        _ => [],
    };

    /// <summary>
    /// Gets a specific statistic value.
    /// </summary>
    public readonly ushort GetStat(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record, BattleFrontierStatType3 stat)
    {
        var offset = GetStatOffset(facility, mode, record, stat);
        return ReadUInt16LittleEndian(Data[offset..]);
    }

    /// <summary>
    /// Sets a specific statistic value.
    /// </summary>
    public void SetStat(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record, BattleFrontierStatType3 stat, ushort value)
    {
        var offset = GetStatOffset(facility, mode, record, stat);
        WriteUInt16LittleEndian(Data[offset..], value);
    }

    /// <summary>
    /// Gets the continue flag for a facility/mode/record combination.
    /// </summary>
    public readonly bool GetContinueFlag(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record)
    {
        int bitPosition = GetContinueBitPosition(facility, mode, record);
        var flags = ReadUInt32LittleEndian(Data[ContinueOffset..]);
        return ((flags >> bitPosition) & 1) != 0;
    }

    /// <summary>
    /// Sets the continue flag for a facility/mode/record combination.
    /// </summary>
    public void SetContinueFlag(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record, bool value)
    {
        int bitPosition = GetContinueBitPosition(facility, mode, record);
        var flags = ReadUInt32LittleEndian(Data[ContinueOffset..]);
        if (value)
            flags |= 1u << bitPosition;
        else
            flags &= ~(1u << bitPosition);
        WriteUInt32LittleEndian(Data[ContinueOffset..], flags);
    }

    /// <summary>
    /// Gets the available battle modes for a facility.
    /// </summary>
    public static int GetModeCount(BattleFrontierFacility3 facility) => facility switch
    {
        Tower => 4, // Singles, Doubles, Multi, Linked
        Dome or Palace or Factory => 2, // Singles, Doubles
        Arena or Pike or Pyramid => 1, // Singles only
        _ => 1,
    };

    #endregion

    #region Offset Calculations
    private static int GetStatOffset(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record, BattleFrontierStatType3 stat)
    {
        int modeIndex = Math.Min((int)mode, GetModeCount(facility) - 1);
        int recordIndex = (int)record;

        return facility switch
        {
            Tower => GetTowerOffset(modeIndex, recordIndex, stat),
            Dome => GetDomeOffset(modeIndex, recordIndex, stat),
            Palace => GetPalaceOffset(modeIndex, recordIndex, stat),
            Arena => GetArenaOffset(recordIndex, stat),
            Factory => GetFactoryOffset(modeIndex, recordIndex, stat),
            Pike => GetPikeOffset(recordIndex, stat),
            Pyramid => GetPyramidOffset(recordIndex, stat),
            _ => throw new ArgumentOutOfRangeException(nameof(facility)),
        };
    }

    private static int GetTowerOffset(int mode, int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x004,
            RecordStreak => 0x014,
            _ => throw new ArgumentException("Tower only supports Current and Record streaks"),
        };
        return baseOffset + (4 * mode) + (2 * record);
    }

    private static int GetDomeOffset(int mode, int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x030,
            RecordStreak => 0x038,
            Championships => 0x040,
            _ => throw new ArgumentException("Dome only supports Current, Record, and Total streaks"),
        };
        return baseOffset + (4 * mode) + (2 * record);
    }

    private static int GetPalaceOffset(int mode, int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x0EC,
            RecordStreak => 0x0F4,
            _ => throw new ArgumentException("Palace only supports Current and Record streaks"),
        };
        return baseOffset + (4 * mode) + (2 * record);
    }

    private static int GetArenaOffset(int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x0FE,
            RecordStreak => 0x102,
            _ => throw new ArgumentException("Arena only supports Current and Record streaks"),
        };
        return baseOffset + (2 * record);
    }

    private static int GetFactoryOffset(int mode, int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x106,
            RecordStreak => 0x10E,
            CurrentSwapped => 0x116,
            RecordSwapped => 0x11E,
            _ => throw new ArgumentException("Invalid stat type for Factory"),
        };
        return baseOffset + (4 * mode) + (2 * record);
    }

    private static int GetPikeOffset(int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x128,
            RecordStreak => 0x12C,
            RecordCleared => 0x130,
            _ => throw new ArgumentException("Pike only supports Current, Record, and Total streaks"),
        };
        return baseOffset + (2 * record);
    }

    private static int GetPyramidOffset(int record, BattleFrontierStatType3 stat)
    {
        int baseOffset = stat switch
        {
            CurrentStreak => 0x13E,
            RecordStreak => 0x142,
            _ => throw new ArgumentException("Pyramid only supports Current and Record streaks"),
        };
        return baseOffset + (2 * record);
    }

    private static int GetContinueBitPosition(BattleFrontierFacility3 facility, BattleFrontierBattleMode3 mode, BattleFrontierRecordType3 record)
    {
        int modeIndex = Math.Min((int)mode, GetModeCount(facility) - 1);
        int recordIndex = (int)record;

        // Each facility has specific bit positions for each mode (from original BFF array)
        return facility switch
        {
            Tower => modeIndex switch
            {
                0 => 0x00 + recordIndex, // Singles: bits 0-1
                1 => 0x0E + recordIndex, // Doubles: bits 14-15
                2 => 0x10 + recordIndex, // Multi: bits 16-17
                3 => 0x12 + recordIndex, // Linked: bits 18-19
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            },
            Dome => modeIndex switch
            {
                0 => 0x02 + recordIndex, // Singles: bits 2-3
                1 => 0x14 + recordIndex, // Doubles: bits 20-21
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            },
            Palace => modeIndex switch
            {
                0 => 0x04 + recordIndex, // Singles: bits 4-5
                1 => 0x16 + recordIndex, // Doubles: bits 22-23
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            },
            Arena => 0x06 + recordIndex, // Singles only: bits 6-7
            Factory => modeIndex switch
            {
                0 => 0x08 + recordIndex, // Singles: bits 8-9
                1 => 0x18 + recordIndex, // Doubles: bits 24-25
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            },
            Pike => 0x0A + recordIndex, // Singles only: bits 10-11
            Pyramid => 0x0C + recordIndex, // Singles only: bits 12-13
            _ => throw new ArgumentOutOfRangeException(nameof(facility)),
        };
    }
    #endregion
}
