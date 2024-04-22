using System;
using static PKHeX.Core.AbilityPermission;
using static PKHeX.Core.EncounterMatchRating;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record EncounterDist9
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, ITeraRaid9, IMoveset, IFlawlessIVCount, IFixedGender, IFixedNature
{
    public byte Generation => 9;
    ushort ILocation.Location => Location;
    public const ushort Location = Locations.TeraCavern9;
    public EntityContext Context => EntityContext.Gen9;
    public GameVersion Version => GameVersion.SV;
    public bool IsDistribution => Index != 0;
    public Ball FixedBall => Ball.None;
    public bool IsEgg => false;
    public bool IsShiny => Shiny == Shiny.Always;
    public ushort EggLocation => 0;

    public required ushort Species { get; init; }
    public required byte Form { get; init; }
    public required byte Gender { get; init; }
    public required AbilityPermission Ability { get; init; }
    public required byte FlawlessIVCount { get; init; }
    public required Shiny Shiny { get; init; }
    public required Nature Nature { get; init; }
    public required byte Level { get; init; }
    public required Moveset Moves { get; init; }
    public required IndividualValueSet IVs { get; init; }
    public required GemType TeraType { get; init; }
    public byte Index { get; private init; }
    public byte Stars { get; private init; }
    public byte RandRate { get; private init; } // weight chance of this encounter

    /// <summary> Indicates how the <see cref="Scale"/> value is used, if at all. </summary>
    public SizeType9 ScaleType { get; private init; }
    /// <summary>  Used only for <see cref="ScaleType"/> == <see cref="SizeType9.VALUE"/> </summary>
    public byte Scale { get; private init; }

    public ushort RandRate0MinScarlet { get; private init; }
    public ushort RandRate0MinViolet { get; private init; }
    public ushort RandRate0TotalScarlet { get; private init; }
    public ushort RandRate0TotalViolet { get; private init; }

    public ushort RandRate1MinScarlet { get; private init; }
    public ushort RandRate1MinViolet { get; private init; }
    public ushort RandRate1TotalScarlet { get; private init; }
    public ushort RandRate1TotalViolet { get; private init; }

    public ushort RandRate2MinScarlet { get; private init; }
    public ushort RandRate2MinViolet { get; private init; }
    public ushort RandRate2TotalScarlet { get; private init; }
    public ushort RandRate2TotalViolet { get; private init; }

    public ushort RandRate3MinScarlet { get; private init; }
    public ushort RandRate3MinViolet { get; private init; }
    public ushort RandRate3TotalScarlet { get; private init; }
    public ushort RandRate3TotalViolet { get; private init; }

    public string Name => "Distribution Tera Raid Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public ushort GetRandRateTotalScarlet(int stage) => stage switch
    {
        0 => RandRate0TotalScarlet,
        1 => RandRate1TotalScarlet,
        2 => RandRate2TotalScarlet,
        3 => RandRate3TotalScarlet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateTotalViolet(int stage) => stage switch
    {
        0 => RandRate0TotalViolet,
        1 => RandRate1TotalViolet,
        2 => RandRate2TotalViolet,
        3 => RandRate3TotalViolet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateMinScarlet(int stage) => stage switch
    {
        0 => RandRate0MinScarlet,
        1 => RandRate1MinScarlet,
        2 => RandRate2MinScarlet,
        3 => RandRate3MinScarlet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateMinViolet(int stage) => stage switch
    {
        0 => RandRate0MinViolet,
        1 => RandRate1MinViolet,
        2 => RandRate2MinViolet,
        3 => RandRate3MinViolet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    private const int StageCount = 4;
    private const int StageNone = -1;

    public bool CanBeEncountered(uint seed) => GetProgressMaximum(seed) != StageNone;

    public int ProgressStageMin
    {
        get
        {
            for (int stage = 0; stage < StageCount; stage++)
            {
                if (GetRandRateTotalScarlet(stage) != 0 || GetRandRateTotalViolet(stage) != 0)
                    return stage;
            }
            return StageNone;
        }
    }

    public int ProgressStageMax
    {
        get
        {
            for (int stage = StageCount - 1; stage >= 0; stage--)
            {
                if (GetRandRateTotalScarlet(stage) != 0 || GetRandRateTotalViolet(stage) != 0)
                    return stage;
            }
            return StageNone;
        }
    }

    public int GetProgressMaximum(uint seed)
    {
        // We loop from the highest progress, since that is where the majority of samples will be from.
        for (int i = StageCount - 1; i >= 0; i--)
        {
            if (GetIsPossibleSlot(seed, i))
                return i;
        }
        return StageNone;
    }

    private bool GetIsPossibleSlot(uint seed, int stage)
    {
        var totalScarlet = GetRandRateTotalScarlet(stage);
        if (totalScarlet != 0)
        {
            var rand = new Xoroshiro128Plus(seed);
            _ = rand.NextInt(100);
            var val = rand.NextInt(totalScarlet);
            var min = GetRandRateMinScarlet(stage);
            if ((uint)((int)val - min) < RandRate)
                return true;
        }

        var totalViolet = GetRandRateTotalViolet(stage);
        if (totalViolet != 0)
        {
            var rand = new Xoroshiro128Plus(seed);
            _ = rand.NextInt(100);
            var val = rand.NextInt(totalViolet);
            var min = GetRandRateMinViolet(stage);
            if ((uint)((int)val - min) < RandRate)
                return true;
        }
        return false;
    }

    public static EncounterDist9[] GetArray(ReadOnlySpan<byte> data)
    {
        var count = data.Length / SerializedSize;
        var result = new EncounterDist9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * SerializedSize, SerializedSize));
        return result;
    }

    private const int SerializedSize = WeightStart + (sizeof(ushort) * 2 * 2 * 4) + 10;
    private const int WeightStart = 0x14;
    private static EncounterDist9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Gender = (byte)(data[0x03] - 1),
        Ability = GetAbility(data[0x04]),
        FlawlessIVCount = data[5],
        Shiny = data[0x06] switch { 0 => Shiny.Random, 1 => Shiny.Never, 2 => Shiny.Always, _ => throw new ArgumentOutOfRangeException(nameof(data)) },
        Level = data[0x07],
        Moves = new Moveset(
            ReadUInt16LittleEndian(data[0x08..]),
            ReadUInt16LittleEndian(data[0x0A..]),
            ReadUInt16LittleEndian(data[0x0C..]),
            ReadUInt16LittleEndian(data[0x0E..])),
        TeraType = (GemType)data[0x10],
        Index = data[0x11],
        Stars = data[0x12],
        RandRate = data[0x13],

        RandRate0MinScarlet   = ReadUInt16LittleEndian(data[WeightStart..]),
        RandRate0MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + sizeof(ushort))..]),
        RandRate0TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 2))..]),
        RandRate0TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 3))..]),

        RandRate1MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 4))..]),
        RandRate1MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 5))..]),
        RandRate1TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 6))..]),
        RandRate1TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 7))..]),

        RandRate2MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 8))..]),
        RandRate2MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 9))..]),
        RandRate2TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 10))..]),
        RandRate2TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 11))..]),

        RandRate3MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 12))..]),
        RandRate3MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 13))..]),
        RandRate3TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 14))..]),
        RandRate3TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 15))..]),

        Nature = (Nature)data[0x34],
        IVs = new IndividualValueSet((sbyte)data[0x35], (sbyte)data[0x36], (sbyte)data[0x37], (sbyte)data[0x38], (sbyte)data[0x39], (sbyte)data[0x3A], (IndividualValueSetType)data[0x3B]),
        ScaleType = (SizeType9)data[0x3C],
        Scale = data[0x3D],
    };

    private static AbilityPermission GetAbility(byte b) => b switch
    {
        0 => Any12,
        1 => Any12H,
        2 => OnlyFirst,
        3 => OnlySecond,
        4 => OnlyHidden,
        _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.SV[Species, Form];
        var pk = new PK9
        {
            Language = lang,
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = version,
            Ball = (byte)Ball.Poke,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            ObedienceLevel = LevelMin,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
        };
        SetPINGA(pk, criteria, pi);
        pk.SetMoves(Moves);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK9 pk, EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        const byte rollCount = 1;
        const byte undefinedSize = 0;
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, rollCount,
            undefinedSize, undefinedSize, ScaleType, Scale,
            Ability, Shiny, Nature, IVs: IVs);

        var init = Util.Rand.Rand64();
        var success = this.TryApply32(pk, init, param, criteria);
        if (!success)
            this.TryApply32(pk, init, param, EncounterCriteria.Unrestricted);
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;

        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchLocationRemapped(pk);
        return IsMatchLocationExact(pk) || IsMatchLocationRemapped(pk);
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return PartialMatch;
        return IsMatchDeferred(pk);
    }

    private static bool IsMatchLocationExact(PKM pk) => pk.MetLocation == Location;

    private static bool IsMatchLocationRemapped(PKM pk)
    {
        var met = pk.MetLocation;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetSV(met, version);
        return LocationsHOME.GetMetSWSH(Location, version) == met;
    }

    private EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return DeferredErrors;

        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                var a = Ability;
                if (a is OnlyHidden)
                {
                    if (!AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                        return DeferredErrors;
                    a = num == 1 ? OnlyFirst : OnlySecond;
                }
                if (a is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
                    return DeferredErrors;
            }
        }

        return Match;
    }

    private bool IsMatchPartial(PKM pk)
    {
        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return true;

        var seed = Tera9RNG.GetOriginalSeed(pk);
        if (pk is ITeraType t && !Tera9RNG.IsMatchTeraType(seed, TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return true;
        if (!CanBeEncountered(seed))
            return true;

        var pi = PersonalTable.SV.GetFormEntry(Species, Form);
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, 1, 0, 0, ScaleType, Scale, Ability, Shiny, Nature, IVs: IVs);
        if (!Encounter9RNG.IsMatch(pk, param, seed))
            return true;

        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }
    #endregion
}
