using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class PlayTime9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    private readonly SAV9ZA _sav = sav; // for remapped value access

    // Actually unused!

    public int PlayedHoursUnused
    {
        get => ReadInt32LittleEndian(Data);
        set => WriteInt32LittleEndian(Data, (ushort)value);
    }

    public int PlayedMinutesUnused
    {
        get => ReadInt32LittleEndian(Data[4..]);
        set => WriteInt32LittleEndian(Data[4..], (ushort)value);
    }

    public int PlayedSecondsUnused
    {
        get => ReadInt32LittleEndian(Data[8..]);
        set => WriteInt32LittleEndian(Data[8..], (ushort)value);
    }

    /// <summary>
    /// Elapsed play time in seconds (floating point, double precision).
    /// </summary>
    public double RawSeconds
    {
        get => _sav.Blocks.GetBlockValue<double>(SaveBlockAccessor9ZA.KPlayedSeconds);
        set => _sav.Blocks.SetBlockValue(SaveBlockAccessor9ZA.KPlayedSeconds, value);
    }

    public int PlayedHours
    {
        get => (int)(RawSeconds / 3600);
        set => RawSeconds = GetPlayedSeconds(value, PlayedMinutes, PlayedSeconds, PlayedFractionalSeconds);
    }

    public int PlayedMinutes
    {
        get => (int)((RawSeconds % 3600) / 60);
        set => RawSeconds = GetPlayedSeconds(PlayedHours, value, PlayedSeconds, PlayedFractionalSeconds);
    }

    public int PlayedSeconds
    {
        get => (int)(RawSeconds % 60);
        set => RawSeconds = GetPlayedSeconds(PlayedHours, PlayedMinutes, value, PlayedFractionalSeconds);
    }

    public double PlayedFractionalSeconds
    {
        get => RawSeconds - Math.Floor(RawSeconds);
        set
        {
            var whole = Math.Floor(RawSeconds);
            RawSeconds = whole + value;
        }
    }

    public static double GetPlayedSeconds(double hours, double minutes, double seconds, double fraction) => (hours * 3600) + (minutes * 60) + seconds + fraction;

    /// <summary>
    /// In-game time in seconds since midnight (floating point).
    /// </summary>
    public double RawDaySeconds
    {
        get => _sav.Blocks.GetBlockValue<float>(SaveBlockAccessor9ZA.KTimeSecondsPastMidnight);
        set => _sav.Blocks.SetBlockValue(SaveBlockAccessor9ZA.KTimeSecondsPastMidnight, value);
    }

    public int CurrentDayHours
    {
        get => (int)(RawDaySeconds / 3600);
        set => RawDaySeconds = GetCurrentDaySeconds(value, CurrentDayMinutes, CurrentDaySeconds, CurrentDayFractionalSeconds);
    }

    public int CurrentDayMinutes
    {
        get => (int)((RawDaySeconds % 3600) / 60);
        set => RawDaySeconds = GetCurrentDaySeconds(CurrentDayHours, value, CurrentDaySeconds, CurrentDayFractionalSeconds);
    }

    public int CurrentDaySeconds
    {
        get => (int)(RawDaySeconds % 60);
        set => RawDaySeconds = GetCurrentDaySeconds(CurrentDayHours, CurrentDayMinutes, value, CurrentDayFractionalSeconds);
    }

    public float CurrentDayFractionalSeconds
    {
        get => (float)(RawDaySeconds - Math.Floor(RawDaySeconds));
        set
        {
            var whole = Math.Floor(RawDaySeconds);
            RawDaySeconds = whole + value;
        }
    }

    public static float GetCurrentDaySeconds(float hours, float minutes, float seconds, float fraction) => (hours * 3600) + (minutes * 60) + seconds + fraction;

    /// <summary>
    /// Elapsed play time in seconds (floating point, double precision).
    /// </summary>
    public double OverworldSeconds
    {
        get => _sav.Blocks.GetBlockValue<double>(SaveBlockAccessor9ZA.KPlayTimeOverworld);
        set => _sav.Blocks.SetBlockValue(SaveBlockAccessor9ZA.KPlayTimeOverworld, value);
    }

    public int PlayedOverworldHours
    {
        get => (int)(OverworldSeconds / 3600);
        set => OverworldSeconds = GetPlayedOverworldSeconds(value, PlayedOverworldMinutes, PlayedOverworldSeconds, PlayedOverworldFractionalSeconds);
    }

    public int PlayedOverworldMinutes
    {
        get => (int)((OverworldSeconds % 3600) / 60);
        set => OverworldSeconds = GetPlayedOverworldSeconds(PlayedOverworldHours, value, PlayedOverworldSeconds, PlayedOverworldFractionalSeconds);
    }

    public int PlayedOverworldSeconds
    {
        get => (int)(OverworldSeconds % 60);
        set => OverworldSeconds = GetPlayedOverworldSeconds(PlayedOverworldHours, PlayedOverworldMinutes, value, PlayedOverworldFractionalSeconds);
    }

    public double PlayedOverworldFractionalSeconds
    {
        get => OverworldSeconds - Math.Floor(OverworldSeconds);
        set
        {
            var whole = Math.Floor(OverworldSeconds);
            OverworldSeconds = whole + value;
        }
    }

    public static double GetPlayedOverworldSeconds(double hours, double minutes, double seconds, double fraction) => (hours * 3600) + (minutes * 60) + seconds + fraction;
}
