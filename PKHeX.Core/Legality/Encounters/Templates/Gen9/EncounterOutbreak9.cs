using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Fixed Spawn Encounter
/// </summary>
public sealed record EncounterOutbreak9
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, IFixedGender
{
    public byte Generation => 9;
    ushort ILocation.Location => Location;
    public ushort Location => GetFirstMetLocation(MetBase, MetFlags);
    public EntityContext Context => EntityContext.Gen9;
    public GameVersion Version => GameVersion.SV;
    public Shiny Shiny => IsShiny ? Shiny.Always : Shiny.Random;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public ushort EggLocation => 0;
    public AbilityPermission Ability => AbilityPermission.Any12;

    public required ushort Species { get; init; }
    public required byte Form { get; init; }
    public required byte LevelMin { get; init; }
    public required byte LevelMax { get; init; }
    public required byte Gender { get; init; }
    public required RibbonIndex Ribbon { get; init; }
    public required byte MetBase { get; init; }
    public required bool IsForcedScaleRange { get; init; }
    public required byte ScaleMin { get; init; }
    public required byte ScaleMax { get; init; }
    public required bool IsShiny { get; init; }
    public required UInt128 MetFlags { get; init; }

    private const int SIZE = 0x14 + 8;

    public static EncounterOutbreak9[] GetArray(ReadOnlySpan<byte> data)
    {
        var count = data.Length / SIZE;
        var result = new EncounterOutbreak9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * SIZE, SIZE));
        return result;
    }

    private static EncounterOutbreak9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Gender = data[0x03],

        LevelMin = data[0x04],
        LevelMax = data[0x05],
        Ribbon = (RibbonIndex)data[0x06],
        MetBase = data[0x07],

        IsForcedScaleRange = data[0x08] != 0,
        ScaleMin = data[0x09],
        ScaleMax = data[0x0A],
        IsShiny = data[0x0B] != 0,
        MetFlags = ReadUInt128LittleEndian(data[0x0C..]),
    };

    public string Name => "Distribution Outbreak Encounter";
    public string LongName => Name;
    public byte Level => LevelMin;

    private const RibbonIndex Unset = unchecked((RibbonIndex)(-1));

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

        // Be nice and set the ribbon if it's not unset
        if (Ribbon != Unset)
            pk.SetRibbonIndex(Ribbon);

        SetPINGA(pk, criteria, pi);
        EncounterUtil.SetEncounterMoves(pk, Version, Level);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK9 pk, EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        pk.EncryptionConstant = rnd.Rand32();
        pk.Nature = pk.StatNature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        criteria.SetRandomIVs(pk);

        var type = Tera9RNG.GetTeraType(rnd.Rand64(), GemType.Default, Species, Form);
        pk.TeraTypeOriginal = (MoveType)type;
        if (criteria.IsSpecifiedTeraType() && type != criteria.TeraType)
            pk.SetTeraType(type); // sets the override type

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.Scale = !IsForcedScaleRange
            ? PokeSizeUtil.GetRandomScalar(rnd)
            : (byte)rnd.Next(ScaleMin, ScaleMax + 1);
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (evo.Form != Form)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
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

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = pk.MetLocation;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetSV(met, version);
        return LocationsHOME.GetMetSWSH(Location, version) == met;
    }

    private bool IsMatchLocationExact(PKM pk) => IsMetLocationMatch(MetBase, MetFlags, pk.MetLocation);

    private static ushort GetFirstMetLocation(byte met, UInt128 flags)
    {
        // Get first bitflag index
        var index = System.Numerics.BitOperations.TrailingZeroCount((ulong)flags);
        if (index == 0)
            index = 64 + System.Numerics.BitOperations.TrailingZeroCount((ulong)(flags >> 64));
        return (ushort)(met + index);
    }

    private static bool IsMetLocationMatch(byte met, UInt128 flags, int actual)
    {
        var index = actual - met;
        if ((uint)index >= 128)
            return false;
        var flag = flags >> index;
        return (flag & 1) == 1;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return IsMatchDeferred(pk);
    }

    private EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return EncounterMatchRating.DeferredErrors;

        if (Ability != AbilityPermission.Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not AbilityPermission.OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                var a = Ability;
                if (a is AbilityPermission.OnlyHidden)
                {
                    if (!AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                        return EncounterMatchRating.DeferredErrors;
                    a = num == 1 ? AbilityPermission.OnlyFirst : AbilityPermission.OnlySecond;
                }
                if (a is AbilityPermission.OnlyFirst or AbilityPermission.OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return EncounterMatchRating.Match;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    #endregion
}
