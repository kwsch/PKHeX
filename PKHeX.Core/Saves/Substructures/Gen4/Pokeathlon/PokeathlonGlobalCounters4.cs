using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public struct PokeathlonGlobalCounters4(Memory<byte> Raw)
{
    public const int SIZE = 0x74;
    public Span<byte> Data => Raw.Span;

    public const uint MaxPlay = 59_999;
    public const uint MaxStat = 9_999_999;
    public const uint MaxFame = ushort.MaxValue;

    /// <summary> Time Spent in Pokéathlon, in minutes </summary>
    public uint TimeSpent      { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, Math.Min(MaxPlay, value)); }

    public uint SessionsJoined { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], Math.Min(MaxStat, value)); }
    public uint PlacedFirst    { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], Math.Min(MaxStat, value)); }
    public uint PlacedLast     { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], Math.Min(MaxStat, value)); }

    /// <summary>
    /// Bonuses Earned
    /// </summary>
    public uint BonusesEarned  { get => ReadUInt32LittleEndian(Data[0x10..]); set => WriteUInt32LittleEndian(Data[0x10..], Math.Min(MaxStat, value)); }

    /// <summary>
    /// Pokémon Instructions
    /// </summary>
    public uint Instructions { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], Math.Min(MaxStat, value)); }
    public uint Failed       { get => ReadUInt32LittleEndian(Data[0x18..]); set => WriteUInt32LittleEndian(Data[0x18..], Math.Min(MaxStat, value)); }
    public uint Jumped       { get => ReadUInt32LittleEndian(Data[0x1C..]); set => WriteUInt32LittleEndian(Data[0x1C..], Math.Min(MaxStat, value)); }
    public uint Acquired     { get => ReadUInt32LittleEndian(Data[0x20..]); set => WriteUInt32LittleEndian(Data[0x20..], Math.Min(MaxStat, value)); }
    public uint Tackled      { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], Math.Min(MaxStat, value)); }
    public uint FellDown     { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], Math.Min(MaxStat, value)); }
    public uint Dashed       { get => ReadUInt32LittleEndian(Data[0x2C..]); set => WriteUInt32LittleEndian(Data[0x2C..], Math.Min(MaxStat, value)); }
    public uint Switched     { get => ReadUInt32LittleEndian(Data[0x30..]); set => WriteUInt32LittleEndian(Data[0x30..], Math.Min(MaxStat, value)); }
    public uint SelfImpeded  { get => ReadUInt32LittleEndian(Data[0x34..]); set => WriteUInt32LittleEndian(Data[0x34..], Math.Min(MaxStat, value)); }

    public uint ConnectionJoined { get => ReadUInt32LittleEndian(Data[0x38..]); set => WriteUInt32LittleEndian(Data[0x38..], Math.Min(MaxStat, value)); }
    public uint ConnectionFirst  { get => ReadUInt32LittleEndian(Data[0x3C..]); set => WriteUInt32LittleEndian(Data[0x3C..], Math.Min(MaxStat, value)); }
    public uint ConnectionLast   { get => ReadUInt32LittleEndian(Data[0x40..]); set => WriteUInt32LittleEndian(Data[0x40..], Math.Min(MaxStat, value)); }

    // Per-event 1st-place counters
    public uint this[PokeathlonEvent4 index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
            return ReadUInt32LittleEndian(Data[(0x44 + ((int)index * 4))..]);
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
            WriteUInt32LittleEndian(Data[(0x44 + ((int)index * 4))..], Math.Min(MaxStat, value));
        }
    }

    // helper to calculate
    public uint TotalEventFirst
    {
        get
        {
            uint total = 0;
            for (PokeathlonEvent4 i = 0; i < PokeathlonEvent4.Count; i++)
                total += this[i];
            return total;
        }
    }

    // Aggregate event last-place total
    public uint TotalEventLast { get => ReadUInt32LittleEndian(Data[0x6C..]); set => WriteUInt32LittleEndian(Data[0x6C..], Math.Min(MaxStat, value)); }

    /// <summary>
    /// When the player plays the Pokéathlon over wireless play the default drinks will be replaced with the drinks of the opposing player.
    /// The price of the drinks are determined on how famous the Trainers selling them are.
    /// </summary>
    /// <remarks>
    /// <see cref="Aprijuice4.Fame"/>
    /// </remarks>
    public uint Fame { get => ReadUInt32LittleEndian(Data[0x70..]); set => WriteUInt32LittleEndian(Data[0x70..], Math.Min(MaxFame, value)); }

    /// <summary>
    /// Retrieves the value of a particular statistic for Data Card purposes, based on the provided stat identifier.
    /// </summary>
    public uint GetDataCardStat(DataCard4 stat) => stat switch
    {
        DataCard4.PlacedFirst => this.PlacedFirst,
        DataCard4.PlacedLast => this.PlacedLast,
        DataCard4.Dashed => this.Dashed,
        DataCard4.Jumped => this.Jumped,
        DataCard4.FirstHurdle => this[PokeathlonEvent4.HurdleDash],
        DataCard4.FirstRelay => this[PokeathlonEvent4.RelayRun],
        DataCard4.FirstPennant => this[PokeathlonEvent4.PennantCapture],
        DataCard4.FirstBlockSmash => this[PokeathlonEvent4.BlockSmash],
        DataCard4.FirstDiscCatch => this[PokeathlonEvent4.DiscCatch],
        DataCard4.FirstSnowThrow => this[PokeathlonEvent4.SnowThrow],
        DataCard4.PointsAcquired => this.Acquired,
        DataCard4.Failed => this.Failed,
        DataCard4.SelfImpeded => this.SelfImpeded,
        DataCard4.Tackled => this.Tackled,
        DataCard4.FellDown => this.FellDown,
        DataCard4.FirstRingDrop => this[PokeathlonEvent4.RingDrop],
        DataCard4.FirstLampJump => this[PokeathlonEvent4.LampJump],
        DataCard4.FirstCirclePush => this[PokeathlonEvent4.CirclePush],
        DataCard4.ConnectionFirst => this.ConnectionFirst,
        DataCard4.ConnectionLast => this.ConnectionLast,
        DataCard4.EventFirst => this.TotalEventFirst, // aggregate event first-place total is not stored, but can be calculated by summing per-event counters
        DataCard4.EventLast => this.TotalEventLast,
        DataCard4.Switched => this.Switched,
        DataCard4.FirstGoalRoll => this[PokeathlonEvent4.GoalRoll],
        DataCard4.BonusesEarned => this.BonusesEarned,
        DataCard4.Instructions => this.Instructions,
        DataCard4.TimeSpent => this.TimeSpent,
        _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null),
    };

    /// <summary>
    /// Updates the value of a particular statistic for Data Card purposes, based on the provided stat identifier.
    /// </summary>
    public void SetDataCardStat(DataCard4 stat, uint value)
    {
        value = Math.Min(MaxStat, value);
        switch (stat)
        {
            case DataCard4.PlacedFirst: this.PlacedFirst = value; break;
            case DataCard4.PlacedLast: this.PlacedLast = value; break;
            case DataCard4.Dashed: this.Dashed = value; break;
            case DataCard4.Jumped: this.Jumped = value; break;
            case DataCard4.FirstHurdle: this[PokeathlonEvent4.HurdleDash] = value; break;
            case DataCard4.FirstRelay: this[PokeathlonEvent4.RelayRun] = value; break;
            case DataCard4.FirstPennant: this[PokeathlonEvent4.PennantCapture] = value; break;
            case DataCard4.FirstBlockSmash: this[PokeathlonEvent4.BlockSmash] = value; break;
            case DataCard4.FirstDiscCatch: this[PokeathlonEvent4.DiscCatch] = value; break;
            case DataCard4.FirstSnowThrow: this[PokeathlonEvent4.SnowThrow] = value; break;
            case DataCard4.PointsAcquired: this.Acquired = value; break;
            case DataCard4.Failed: this.Failed = value; break;
            case DataCard4.SelfImpeded: this.SelfImpeded = value; break;
            case DataCard4.Tackled: this.Tackled = value; break;
            case DataCard4.FellDown: this.FellDown = value; break;
            case DataCard4.FirstRingDrop: this[PokeathlonEvent4.RingDrop] = value; break;
            case DataCard4.FirstLampJump: this[PokeathlonEvent4.LampJump] = value; break;
            case DataCard4.FirstCirclePush: this[PokeathlonEvent4.CirclePush] = value; break;
            case DataCard4.ConnectionFirst: this.ConnectionFirst = value; break;
            case DataCard4.ConnectionLast: this.ConnectionLast = value; break;
            case DataCard4.EventFirst:
                return; // skip, not going to spread the count across all 10 events
            case DataCard4.EventLast: this.TotalEventLast = value; break;
            case DataCard4.Switched: this.Switched = value; break;
            case DataCard4.FirstGoalRoll: this[PokeathlonEvent4.GoalRoll] = value; break;
            case DataCard4.BonusesEarned: this.BonusesEarned = value; break;
            case DataCard4.Instructions: this.Instructions = value; break;
            case DataCard4.TimeSpent: this.TimeSpent = value; break;
            default: throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }
    }

    public static ReadOnlySpan<ushort> FameInclusiveThreshold => [10, 25, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000];

    /// <summary>
    /// Used to evaluate the fame level of a given trainer, ratcheting up/down based on how much fame has been aggregated.
    /// </summary>
    /// <param name="value">Player fame value</param>
    /// <returns>Value [0,12]</returns>
    public static int GetFameLevel(uint value)
    {
        int level = 0;
        foreach (var threshold in FameInclusiveThreshold)
        {
            if (value <= threshold)
                break;
            level++;
        }
        return level; // 12 max
    }

    // disassembly also calculates fame for NPCs by summing all performance stats of their team, then (sum/3)-8, clamped [0,12].
}
