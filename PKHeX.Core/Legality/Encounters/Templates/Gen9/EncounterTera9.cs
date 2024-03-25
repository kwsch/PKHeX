using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Tera Raid Encounter
/// </summary>
public sealed record EncounterTera9
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, ITeraRaid9, IMoveset, IFlawlessIVCount, IFixedGender, IEncounterFormRandom
{
    public byte Generation => 9;
    public EntityContext Context => EntityContext.Gen9;
    public GameVersion Version => GameVersion.SV;
    ushort ILocation.Location => Location;
    public const ushort Location = Locations.TeraCavern9;
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
    public required byte Level { get; init; }
    public required Moveset Moves { get; init; }
    public required GemType TeraType { get; init; }
    public required byte Index { get; init; }
    public required byte Stars { get; init; }
    public required byte RandRate { get; init; } // weight chance of this encounter
    public required short RandRateMinScarlet { get; init; } // weight chance total of all lower index encounters, for Scarlet
    public required short RandRateMinViolet { get; init; } // weight chance total of all lower index encounters, for Violet
    public bool IsAvailableHostScarlet => RandRateMinScarlet != -1;
    public bool IsAvailableHostViolet => RandRateMinViolet != -1;
    public required TeraRaidMapParent Map { get; init; }

    public bool IsRandomUnspecificForm => Form >= EncounterUtil.FormDynamic;

    public string Name => $"Tera Raid Encounter [{(Index == 0 ? "Base" : Index)}] {Stars}★";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public bool CanBeEncountered(uint seed) => Tera9RNG.IsMatchStarChoice(seed, Stars, RandRate, RandRateMinScarlet, RandRateMinViolet, Map);

    /// <summary>
    /// Fetches the rate sum for the base ROM raid, depending on star count.
    /// </summary>
    /// <param name="star">Raid Difficulty</param>
    /// <param name="map">Map the encounter originates on.</param>
    /// <returns>Total rate value the game uses to call rand(x) with.</returns>
    public static short GetRateTotalSL(int star, TeraRaidMapParent map) => map switch
    {
        TeraRaidMapParent.Paldea => GetRateTotalBaseSL(star),
        TeraRaidMapParent.Kitakami => GetRateTotalKitakamiSL(star),
        TeraRaidMapParent.Blueberry => GetRateTotalBlueberry(star),
        _ => 0,
    };

    /// <inheritdoc cref="GetRateTotalSL(int, TeraRaidMapParent)"/>"/>
    public static short GetRateTotalVL(int star, TeraRaidMapParent map) => map switch
    {
        TeraRaidMapParent.Paldea => GetRateTotalBaseVL(star),
        TeraRaidMapParent.Kitakami => GetRateTotalKitakamiVL(star),
        TeraRaidMapParent.Blueberry => GetRateTotalBlueberry(star),
        _ => 0,
    };

    public static short GetRateTotalBaseSL(int star) => star switch
    {
        1 => 5800,
        2 => 5300,
        3 => 7400,
        4 => 8800, // Scarlet has one more encounter.
        5 => 9100,
        6 => 6500,
        _ => 0,
    };

    public static short GetRateTotalBaseVL(int star) => star switch
    {
        1 => 5800,
        2 => 5300,
        3 => 7400,
        4 => 8700, // Violet has one less encounter.
        5 => 9100,
        6 => 6500,
        _ => 0,
    };

    public static short GetRateTotalKitakamiSL(int star) => star switch
    {
        1 => 1500,
        2 => 1500,
        3 => 2500,
        4 => 2100,
        5 => 2250,
        6 => 2475, // -99
        _ => 0,
    };

    public static short GetRateTotalKitakamiVL(int star) => star switch
    {
        1 => 1500,
        2 => 1500,
        3 => 2500,
        4 => 2100,
        5 => 2250,
        6 => 2574, // +99
        _ => 0,
    };

    // finally the same for both games
    public static short GetRateTotalBlueberry(int star) => star switch
    {
        1 => 1100,
        2 => 1100,
        3 => 2000,
        4 => 1900,
        5 => 2100,
        6 => 2600,
        _ => 0,
    };

    public static EncounterTera9[] GetArray(ReadOnlySpan<byte> data, TeraRaidMapParent map)
    {
        const int size = 0x18;
        var count = data.Length / size;
        var result = new EncounterTera9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * size, size), map);
        return result;
    }

    private static EncounterTera9 ReadEncounter(ReadOnlySpan<byte> data, TeraRaidMapParent map) => new()
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
        RandRateMinScarlet = ReadInt16LittleEndian(data[0x14..]),
        RandRateMinViolet = ReadInt16LittleEndian(data[0x16..]),

        Map = map,
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
            undefinedSize, undefinedSize, undefinedSize, undefinedSize,
            Ability, Shiny);

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
        if (Form != evo.Form && !IsRandomUnspecificForm && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
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
            return EncounterMatchRating.PartialMatch;
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
            return EncounterMatchRating.DeferredErrors;

        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                var a = Ability;
                if (a is OnlyHidden)
                {
                    if (!AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                        return EncounterMatchRating.DeferredErrors;
                    a = num == 1 ? OnlyFirst : OnlySecond;
                }
                if (a is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return EncounterMatchRating.Match;
    }

    private bool IsMatchPartial(PKM pk)
    {
        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        var seed = Tera9RNG.GetOriginalSeed(pk);
        if (pk is ITeraType t && !Tera9RNG.IsMatchTeraType(seed, TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return true;
        if (!Tera9RNG.IsMatchStarChoice(seed, Stars, RandRate, RandRateMinScarlet, RandRateMinViolet, Map))
            return true;

        var pi = PersonalTable.SV.GetFormEntry(Species, Form);
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, 1, 0, 0, 0, 0, Ability, Shiny);
        if (!Encounter9RNG.IsMatch(pk, param, seed))
            return true;

        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    #endregion
}

public enum TeraRaidMapParent : byte
{
    Paldea,
    Kitakami,
    Blueberry,
}
