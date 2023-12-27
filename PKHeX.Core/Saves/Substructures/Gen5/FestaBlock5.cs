using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class FestaBlock5 : SaveBlock<SAV5B2W2>
{
    public FestaBlock5(SAV5B2W2 SAV, int offset) : base(SAV) => Offset = offset;

    public const ushort MaxScore = 9999;
    public const int FunfestFlag = 2438;

    public const int MaxMissionIndex = (int)Funfest5Mission.TheBerryHuntingAdventure;

    public ushort Hosted
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xF0));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xF0), Math.Min(MaxScore, value));
    }

    public ushort Participated
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xF2));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xF2), Math.Min(MaxScore, value));
    }

    public ushort Completed
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xF4));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xF4), Math.Min(MaxScore, value));
    }

    public ushort TopScores
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0xF6));
        set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0xF6), Math.Min(MaxScore, value));
    }

    public byte WhiteEXP
    {
        get => Data[Offset + 0xF8];
        set => Data[Offset + 0xF8] = value;
    }

    public byte BlackEXP
    {
        get => Data[Offset + 0xF9];
        set => Data[Offset + 0xF9] = value;
    }

    public byte Participants
    {
        get => Data[Offset + 0xFA];
        set => Data[Offset + 0xFA] = value;
    }

    private static int GetMissionRecordOffset(int mission)
    {
        if ((uint)mission > MaxMissionIndex)
            throw new ArgumentOutOfRangeException(nameof(mission));
        return mission * sizeof(uint);
    }

    public Funfest5Score GetMissionRecord(int mission)
    {
        var raw = ReadUInt32LittleEndian(Data.AsSpan(Offset + GetMissionRecordOffset(mission)));
        return new Funfest5Score(raw);
    }

    public void SetMissionRecord(int mission, Funfest5Score score)
    {
        var value = score.RawValue;
        WriteUInt32LittleEndian(Data.AsSpan(Offset + GetMissionRecordOffset(mission)), value);
    }

    public bool IsFunfestMissionsUnlocked
    {
        get => SAV.GetEventFlag(FunfestFlag);
        set => SAV.SetEventFlag(FunfestFlag, value);
    }

    public bool IsFunfestMissionUnlocked(int mission)
    {
        if ((uint) mission > MaxMissionIndex)
            throw new ArgumentOutOfRangeException(nameof(mission));

        if (mission == 0)
            return !IsFunfestMissionsUnlocked;

        var req = FunfestMissionUnlockFlagRequired[mission];
        foreach (var f in req)
        {
            if (!SAV.GetEventFlag(f))
                return false;
        }
        return true;
    }

    public void UnlockFunfestMission(int mission)
    {
        if ((uint)mission > MaxMissionIndex)
            throw new ArgumentOutOfRangeException(nameof(mission));

        IsFunfestMissionsUnlocked = true;
        var req = FunfestMissionUnlockFlagRequired[mission];
        foreach (var f in req)
            SAV.SetEventFlag(f, true);
    }

    public void UnlockAllFunfestMissions()
    {
        for (int i = 0; i < MaxMissionIndex; i++)
            UnlockFunfestMission(i);
    }

    private readonly int[][] FunfestMissionUnlockFlagRequired =
    [
        [],           // 00
        [],           // 01
        [2444],       // 02
        [],           // 03
        [2445],       // 04
        [],           // 05
        [2462],       // 06
        [2452, 2476], // 07
        [2476, 2548], // 08
        [2447], [2447], // 09
        [2453], [2453], // 10
        [2504],       // 11
        [2457, 2507], // 12
        [2458, 2478], // 13
        [2456, 2508], // 14
        [2448], [2448], // 15
        [2549],       // 16
        [2449],       // 17
        [2479, 2513], // 18
        [2479, 2550], // 19
        [2481],       // 20
        [2459],       // 21
        [2454],       // 22
        [2551],       // 23
        [2400],       // 24
        [2400],       // 25
        [2400], [2400], // 26
        [2400], [2400], // 27
        [2400],       // 28
        [2400, 2460], // 29
        [2400],       // 30
        [2400, 2461], [2400, 2461], // 31
        [2437],       // 32
        [2450],       // 33
        [2451],       // 34
        [2455],       // 35
        [0105],       // 36
        [2400],       // 37
        [2557],       // 38
    ];

    public static int GetExpNeededForLevelUp(int lv)
    {
        return lv > 8 ? 50 : (lv * 5) + 5;
    }

    public static int GetTotalEntreeExp(int lv)
    {
        if (lv < 9)
            return lv * (lv + 1) * 5 / 2;
        return ((lv - 9) * 50) + 225;
    }
}

public enum Funfest5Mission
{
    TheFirstBerrySearch = 0,
    CollectBerries = 1,
    FindLostItems = 2,
    FindLostBoys = 3,
    EnjoyShopping = 4,
    FindAudino = 5,
    SearchFor3Pokemon = 6,
    TrainwithMartialArtists = 7,
    Sparringwith10Trainers = 8,
    B_GetRichQuick = 9,
    W_TreasureHunting = 10,
    B_ExcitingTrading = 11,
    W_ExhilaratingTrading = 12,
    FindEmolga = 13,
    WingsFallingontheDrawbridge = 14,
    FindTreasures = 15,
    MushroomsHideAndSeek = 16,
    B_FindMysteriousOres = 17,
    W_FindShiningOres = 18,
    The2LostTreasures = 19,
    BigHarvestofBerries = 20,
    RingtheBell = 21,
    TheBellthatRings3Times = 22,
    PathtoanAce = 23,
    ShockingShopping = 24,
    MemoryTraining = 25,
    PushtheLimitofYourMemory = 26,
    FindRustlingGrass = 27,
    FindShards = 28,
    B_ForgottenLostItems = 29,
    W_NotFoundLostItems = 30,
    B_WhatistheBestPrice = 31,
    W_WhatistheRealPrice = 32,
    GivemetheItem = 33,
    DoaGreatTradeUp = 34,
    SearchHiddenGrottes = 35,
    B_NoisyHiddenGrottes = 36,
    W_QuietHiddenGrottes = 37,
    FishingCompetition = 38,
    MulchCollector = 39,
    WhereareFlutteringHearts = 40,
    RockPaperScissorsCompetition = 41,
    TakeaWalkwithEggs = 42,
    FindSteelix = 43,
    TheBerryHuntingAdventure = 44,
}

public record struct Funfest5Score(uint RawValue)
{
    public Funfest5Score(int total, int score, int level, bool isNew) : this(0)
    {
        Total = total;
        Score = score;
        Level = level;
        IsNew = isNew;
    }

    // Structure - 32bits
    // u32 bestTotal:14
    // u32 bestScore:14
    // u32 level:3
    // u32 isNew:1

    public int Total
    {
        readonly get => (int)(RawValue & 0x3FFFu);
        set => RawValue = (RawValue & ~0x3FFFu) | ((uint)value & 0x3FFFu);
    }

    public int Score
    {
        readonly get => (int)((RawValue >> 14) & 0x3FFFu);
        set => RawValue = (RawValue & 0xF0003FFFu) | (((uint)value & 0x3FFFu) << 14);
    }

    public int Level
    {
        readonly get => (int)((RawValue >> 28) & 0x7u);
        set => RawValue = (RawValue & 0x8FFFFFFFu) | (((uint)value & 0x7u) << 28);
    }

    public bool IsNew
    {
        readonly get => RawValue >> 31 == 1;
        set => RawValue = (RawValue & 0x7FFFFFFFu) | ((value ? 1u : 0) << 31);
    }
}
