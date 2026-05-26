using System;
using static System.Buffers.Binary.BinaryPrimitives;


namespace PKHeX.Core;

/// <summary>
/// Manages the Pokeathlon Data for <see cref="SAV4HGSS"/>
/// </summary>
public class Pokeathlon4(Memory<byte> Raw) // 0xD9D4 within SAV4HGSS
{
    public const int SIZE = 0xB80;

    public Span<byte> Data => Raw.Span;

    // 5 courses to store record data
    public PokeathlonCourseRecord4 GetCourseRecord(PokeathlonStat4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonStat4.Count);
        return new(Raw.Slice((int)index * PokeathlonCourseRecord4.SIZE, PokeathlonCourseRecord4.SIZE));
    }

    // 0xDC, 0xDAB0 within SAV
    public PokeathlonMedalManager4 Medals => new(Raw.Slice(0xDC, PokeathlonMedalManager4.SIZE));
    // 3 bytes alignment

    // 0x2CC, 0xDCA0 within SAV
    public PokeathlonEventData4 GetEventSelf(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return new(Raw.Slice(0x2CC + ((int)index * PokeathlonEventData4.SIZE), PokeathlonEventData4.SIZE));
    }

    // 0x484, 0xDE58 within SAV
    public PokeathlonConnection4 GetEventConnection(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return new(Raw.Slice(0x484 + ((int)index * PokeathlonConnection4.SIZE), PokeathlonConnection4.SIZE));
    }

    // 0xAEC, 0xE4C0 within SAV

    /// <summary>
    /// Player's highest score for each of the ten individual events (after conversion to Athlete Points)
    /// </summary>
    public ushort GetBestScore(PokeathlonEvent4 index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        return ReadUInt16LittleEndian(Data[(0xAEC + ((int)index * 2))..]);
    }

    public void SetBestScore(PokeathlonEvent4 index, ushort score)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)PokeathlonEvent4.Count);
        WriteUInt16LittleEndian(Data[(0xAEC + ((int)index * 2))..], score);
    }

    // 0xB00, 0xE4D4 within SAV global counters
    public PokeathlonGlobalCounters4 GlobalCounters => new(Raw.Slice(0xB00, PokeathlonGlobalCounters4.SIZE));

    // remainder @ 0xE548
    public const uint MaxPoints = 99_999;

    /// <summary>
    /// Current points count accumulated, used in buying items from shops.
    /// </summary>
    public uint Points { get => ReadUInt32LittleEndian(Data[0xB74..]); set => WriteUInt32LittleEndian(Data[0xB74..], Math.Min(MaxPoints, value)); }

    /// <summary>
    /// Obtained Data Card indexes [0,26], where each bit represents whether a Data Card has been obtained or not.
    /// They can be purchased in exchange for points in the Pokéathlon Dome (sold at the central reception desk).
    /// </summary>
    public uint FlagsDataCard { get => ReadUInt32LittleEndian(Data[0xB78..]); set => WriteUInt32LittleEndian(Data[0xB78..], Math.Min(DataCardAllObtained, value)); }

    // items exist as Key Items, not really advisable to have a one-shot method unlock, end-user implementation beware.
    public const uint DataCardAllObtained = 0x07FFFFFFu; // 27 bits, all obtained

    /// <summary>
    /// Once-daily shop purchase flags for the Athlete Shop.
    /// Since shops only have 12 items daily, only 12 bits are used.
    /// </summary>
    public ushort FlagsDailyShop { get => ReadUInt16LittleEndian(Data[0xB7C..]); set => WriteUInt16LittleEndian(Data[0xB7C..], Math.Min(FlagsShopAllObtained, value)); }
    public const ushort FlagsShopAllObtained = 0x0FFF; // 12 bits, all obtained

    // last 2 bytes unused, total size 0xB80

    /// <summary>
    /// The global Pokéathlon score is calculated as the sum of:
    /// - the player's best final score in each of the five courses,
    /// - the player's highest score for each of the ten individual events (after conversion to Athlete Points),
    /// - the total number of medals displayed in the box in the Trust room (so each Medalist species will add five to this total).
    /// </summary>
    public uint CalculateGlobalScore()
    {
        uint result = 0;
        for (PokeathlonStat4 i = 0; i < PokeathlonStat4.Count; i++)
            result += GetCourseRecord(i).ScoreMax;
        result += Medals.GetTotalCount();
        for (PokeathlonEvent4 i = 0; i < PokeathlonEvent4.Count; i++)
            result += GetBestScore(i);
        return result;
    }

    public static ReadOnlySpan<ushort> FriendshipTrophyThresholds => [3000, 3100, 3200, 3300, 3400, 3600, 3800, 4000, 4200, 4500];

    public static int CalculateFriendshipTrophyCount(uint globalScore)
    {
        int result = 0;
        foreach (var threshold in FriendshipTrophyThresholds)
        {
            if (globalScore >= threshold)
                result++;
            else
                break;
        }
        return result; // 10 max
    }
}

public struct PokeathlonCourseRecord4(Memory<byte> Raw)
{
    public const int SIZE = 0x2C;

    private Span<byte> Data => Raw.Span;

    public ushort Score0 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Score1 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public ushort Score2 { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }
    public ushort ScoreMax { get => ReadUInt16LittleEndian(Data[6..]); set => WriteUInt16LittleEndian(Data[6..], value); }

    public const int CountParticipant = 3;

    public PokeathlonParticipant4 GetParticipant(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, CountParticipant);
        return new(Raw.Slice(8 + (index * PokeathlonParticipant4.SIZE), PokeathlonParticipant4.SIZE));
    }
}

public struct PokeathlonParticipant4(Memory<byte> Raw) : ISpeciesForm, ITrainerID32, IFixedGender, IShiny
{
    public const int SIZE = 0xC;

    private Span<byte> Data => Raw.Span;

    private uint Packed { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort Species { get => (ushort)(Packed & 0x1FF); set => Packed = (Packed & ~0x1FFu) | ((uint)value & 0x1FF); }
    public byte Form { get => (byte)((Packed >> 9) & 0x1F); set => Packed = (Packed & ~(0x1Fu << 9)) | (((uint)value & 0x1F) << 9); }
    public byte Gender { get => (byte)((Packed >> 14) & 0x3); set => Packed = (Packed & ~(0x3u << 14)) | (((uint)value & 0x3) << 14); }
    public bool IsShiny { get => ((Packed >> 16) & 0x1) != 0; set => Packed = (Packed & ~(0x1u << 16)) | ((value ? 1u : 0u) << 16); }
    // remainder of bits unused

    /// <summary> <see cref="PKM.EncryptionConstant"/> </summary>
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }

    /// <summary> <see cref="PKM.ID32"/> </summary>
    public uint ID32 { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data[8..]); set => WriteUInt16LittleEndian(Data[8..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[10..]); set => WriteUInt16LittleEndian(Data[10..], value); }
    public TrainerIDFormat TrainerIDDisplayFormat => TrainerIDFormat.SixteenBit;
}

/// <summary>
/// Stores a bitflag medal completion state for all species.
/// </summary>
/// <remarks>
/// <see cref="PokeathlonStat4"/> for bitflag indexes.
/// </remarks>
public struct PokeathlonMedalManager4(Memory<byte> Raw)
{
    public const int SIZE = 493; // 1-indexed species [Bulbasaur..Arceus]
    public const byte MaxMedalBits = 0b11111; // 5 courses, 5 bits per species

    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Retrieves the medal bits for the given species, where each bit represents whether a medal for a particular course has been obtained or not.
    /// </summary>
    public byte GetMedal(ushort species)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        return Data[species];
    }

    /// <summary>
    /// Updates the medal bits for the given species.
    /// </summary>
    public void SetMedal(ushort species, byte medalBits)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        Data[species] = medalBits;
    }

    /// <summary>
    /// Awards the provided bit(s) to the species.
    /// </summary>
    public void AwardMedal(ushort species, byte medalBit)
    {
        species--;
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, (ushort)Legal.MaxSpeciesID_4);
        Data[species] |= medalBit;
    }

    /// <summary>
    /// Awards all medals to all species (complete).
    /// </summary>
    /// <param name="medalBits">Medal value to set to every species entry.</param>
    public void SetAllMedals(byte medalBits = MaxMedalBits) => Data[..SIZE].Fill(medalBits);

    /// <summary>
    /// Removes all medals from all species (resets progress).
    /// </summary>
    public void Clear() => SetAllMedals(0);

    public uint GetTotalCount()
    {
        uint result = 0;
        foreach (var bits in Data[..SIZE])
            result += (uint)System.Numerics.BitOperations.PopCount((uint)bits & MaxMedalBits);
        return result;
    }
}

public struct PokeathlonEventData4(Memory<byte> Raw)
{
    public const int SIZE = 0x2C;
    public const uint MaxAttempts = 9_999_999;
    public const uint MaxRecord = 5;

    public Span<byte> Data => Raw.Span;

    public PokeathlonEventRecord4 GetRecord(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, MaxRecord);
        return new(Raw.Slice(index * PokeathlonEventRecord4.SIZE, PokeathlonEventRecord4.SIZE));
    }

    public uint Attempts { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], Math.Min(MaxAttempts, value)); }
}

public struct PokeathlonConnection4(Memory<byte> Raw)
{
    public const int SIZE = 0xA4;

    public const uint MaxTrainer = PokeathlonEventData4.MaxRecord;

    public Span<byte> Data => Raw.Span;

    public PokeathlonEventData4 Inner => new(Raw.Slice(0 * PokeathlonEventData4.SIZE, PokeathlonEventData4.SIZE));

    /// <summary>
    /// Correlated to the <see cref="Inner"/> indexed records.
    /// </summary>
    public PokeathlonEventTrainer4 GetTrainer(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, MaxTrainer);
        return new(Raw.Slice(0x2C + (index * PokeathlonEventTrainer4.SIZE), PokeathlonEventTrainer4.SIZE));
    }
}

public struct PokeathlonEventTrainer4(Memory<byte> Raw) : ITrainerID32
{
    public const int SIZE = 0x18;
    public Span<byte> Data => Raw.Span;
    public uint ID32 { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public TrainerIDFormat TrainerIDDisplayFormat => TrainerIDFormat.SixteenBit;

    public Span<byte> OriginalTrainerTrash => Data.Slice(8, 8 * sizeof(ushort));

    public byte Language { get => Data[0x14]; set => Data[0x14] = value; }
    // remaining 3 bytes unused

    public string OT
    {
        get => StringConverter4.GetString(OriginalTrainerTrash);
        set => StringConverter4.SetString(OriginalTrainerTrash, value, 7, Language);
    }
}

/// <summary>
/// Event record storage for a given event type.
/// </summary>
/// <remarks>
/// <see cref="PokeathlonEvent4"/>
/// </remarks>
/// <param name="Raw"></param>
public struct PokeathlonEventRecord4(Memory<byte> Raw)
{
    public const int SIZE = 8;

    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Stores the record for the particular event. The meaning of this value depends on the event type.
    /// </summary>
    /// <remarks>
    /// Hurdle Dash: Time in frames (lower is better).
    /// </remarks>
    public ushort Record { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }

    // team of three Pokémon responsible for this record, simply (species,form)*3.
    public SpeciesForm10 Entry0 { get => ReadUInt16LittleEndian(Data[2..]); set => WriteUInt16LittleEndian(Data[2..], value); }
    public SpeciesForm10 Entry1 { get => ReadUInt16LittleEndian(Data[4..]); set => WriteUInt16LittleEndian(Data[4..], value); }
    public SpeciesForm10 Entry2 { get => ReadUInt16LittleEndian(Data[6..]); set => WriteUInt16LittleEndian(Data[6..], value); }
}

public record struct SpeciesForm10(ushort Value) : ISpeciesForm
{
    // 10 bits species
    public ushort Species { get => (ushort)(Value & 0x3FF); set => Value = (ushort)((Value & ~0x3FF) | (value & 0x3FF)); }
    public byte Form { get => (byte)((Value >> 10) & 0x3F); set => Value = (ushort)((Value & ~(0x3Fu << 10)) | ((((uint)value & 0x3F) << 10))); }

    /// <summary>
    /// Useful sanity check.
    /// </summary>
    public bool IsValid => Value is 0 || (Species != 0 && PersonalTable.HGSS.IsPresentInGame(Species, Form));

    public static implicit operator SpeciesForm10(ushort value) => new(value);
    public static implicit operator ushort(SpeciesForm10 value) => value.Value;
}

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
    /// <see cref="Aprijuice5.Fame"/>
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
public struct Aprijuice5(Memory<byte> Raw)
{
    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// The price of drinks are determined by how famous the Trainers selling them are.
    /// </summary>
    /// <remarks>
    /// <see cref="PokeathlonGlobalCounters4.Fame"/>
    /// </remarks>
    public ushort Fame { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }

    // Every 100 steps taken increases the mildness of an Aprijuice by 1, up to a maximum of 255.
    // Any mildness increases are made before the mixing of new Apricorns into the Aprijuice is performed.
    public byte Mildness { get => Data[2]; set => Data[2] = value; }

    // Each flavor is capped at a maximum of 63 points and a minimum of 0.
    public byte Spicy { get => Data[3]; set => Data[3] = value; }
    public byte Sour { get => Data[4]; set => Data[4] = value; }
    public byte Dry { get => Data[5]; set => Data[5] = value; }
    public byte Bitter { get => Data[6]; set => Data[6] = value; }
    public byte Sweet { get => Data[7]; set => Data[7] = value; }

    public const ushort LevelMax = 100;

    public byte CalculateLevel()
    {
        var level = Spicy + Sour + Dry + Bitter + Sweet;
        return (byte)Math.Clamp(level, 0, LevelMax); // never will see 0 unless it's hacked :)
    }

    public const ushort PriceMin = 100;
    public const ushort PriceMax = 5000;

    public ushort CalculatePrice()
    {
        var level = CalculateLevel();
        var price = (Fame / 10) + (level / 2);
        return (ushort)Math.Clamp(price, PriceMin, PriceMax);
    }

    // When a Pokémon is put into a PC box, all effects of an Aprijuice disappear.
}

/// <summary>
/// Performance stats
/// </summary>
public enum PokeathlonStat4 : byte
{
    Speed = 0,
    Power = 1,
    Skill = 2,
    Stamina = 3,
    Jump = 4,

    Count = 5,
}

public enum PokeathlonEvent4 : byte
{
    HurdleDash = 0,
    PennantCapture = 1,
    CirclePush = 2,
    BlockSmash = 3,
    DiscCatch = 4,
    LampJump = 5,
    RelayRun = 6,
    RingDrop = 7,
    SnowThrow = 8,
    GoalRoll = 9,

    Count = 10,
}

/// <summary>
/// Data Card indexes and the stats they retrieve in <see cref="PokeathlonGlobalCounters4"/>.
/// </summary>
public enum DataCard4 : byte
{
    PlacedFirst     = 0, // Pokéathlon 1st Place
    PlacedLast      = 1, // Pokéathlon Last Place
    Dashed          = 2, // Times Pokémon Dashed
    Jumped          = 3, // Times Pokémon Jumped

    FirstHurdle     = 4, // Hurdle Dash 1st Places
    FirstRelay      = 5, // Relay Run 1st Places
    FirstPennant    = 6, // Pennant Capture 1st Places
    FirstBlockSmash = 7, // Block Smash 1st Places
    FirstDiscCatch  = 8, // Disc Catch 1st Places
    FirstSnowThrow  = 9, // Snow Throw 1st Places

    PointsAcquired  = 10, // Pokémon Acquired Points
    Failed          = 11, // Pokémon Failed
    SelfImpeded     = 12, // Times Pokémon Self-Impeded
    Tackled         = 13, // Times Pokémon Tackled
    FellDown        = 14, // Pokémon Fell Down

    FirstRingDrop   = 15, // Ring Drop 1st Places
    FirstLampJump   = 16, // Lamp Jump 1st Places
    FirstCirclePush = 17, // Circle Push 1st Places

    ConnectionFirst = 18, // Connection 1st Places
    ConnectionLast  = 19, // Connection Last Places

    EventFirst      = 20, // Event 1st Places
    EventLast       = 21, // Event Last Places

    Switched        = 22, // Times Pokémon Switched

    FirstGoalRoll   = 23, // Goal Roll 1st Places

    BonusesEarned   = 24, // Bonuses Earned
    Instructions    = 25, // Pokémon Instructions
    TimeSpent       = 26, // Time Spent in Pokéathlon

    Count = 27,
};
