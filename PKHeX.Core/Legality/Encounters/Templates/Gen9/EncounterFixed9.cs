using System;
using static PKHeX.Core.AbilityPermission;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Fixed Spawn Encounter
/// </summary>
public sealed record EncounterFixed9
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, IMoveset, IFlawlessIVCount, IGemType, IFixedGender
{
    public byte Generation => 9;
    ushort ILocation.Location => Location;
    public byte Location => Location0;
    public EntityContext Context => EntityContext.Gen9;
    public GameVersion Version => GameVersion.SV;
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public bool IsShiny => false;
    public ushort EggLocation => 0;
    public AbilityPermission Ability { get; init; }

    public required ushort Species { get; init; }
    public required byte Form { get; init; }
    public required byte Level { get; init; }
    public required byte FlawlessIVCount { get; init; }
    public required GemType TeraType { get; init; }
    public required byte Gender { get; init; }
    public required Moveset Moves { get; init; }
    private byte Location0 { get; init; }
    private byte Location1 { get; init; }
    private byte Location2 { get; init; }
    private byte Location3 { get; init; }

    private const int MinScaleStrongTera = 200; // [200,255]

    public static EncounterFixed9[] GetArray(ReadOnlySpan<byte> data)
    {
        const int size = 0x14;
        var count = data.Length / size;
        var result = new EncounterFixed9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * size, size));
        return result;
    }

    private static EncounterFixed9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Level = data[0x03],
        FlawlessIVCount = data[0x04],
        TeraType = (GemType)data[0x05],
        Gender = data[0x06],
        Ability = (AbilityPermission)data[0x07],
        Moves = new Moveset(
            ReadUInt16LittleEndian(data[0x08..]),
            ReadUInt16LittleEndian(data[0x0A..]),
            ReadUInt16LittleEndian(data[0x0C..]),
            ReadUInt16LittleEndian(data[0x0E..])),
        Location0 = data[0x10],
        Location1 = data[0x11],
        Location2 = data[0x12],
        Location3 = data[0x13],
    };

    public string Name => "Fixed Behavior Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

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
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
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

        criteria.SetRandomIVs(pk, FlawlessIVCount);

        var type = Tera9RNG.GetTeraType(rnd.Rand64(), TeraType, Species, Form);
        pk.TeraTypeOriginal = (MoveType)type;
        if (criteria.IsSpecifiedTeraType() && type != criteria.TeraType)
            pk.SetTeraType(type); // sets the override type

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.Scale = TeraType != 0
            ? (byte)(MinScaleStrongTera + rnd.Next(byte.MaxValue - MinScaleStrongTera + 1))
            : PokeSizeUtil.GetRandomScalar(rnd);
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
        if (!IsMatchForm(pk, evo))
            return false;
        if (TeraType != GemType.Random && pk is ITeraType t && !Tera9RNG.IsMatchTeraType(TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return false;
        if (TeraType != 0)
        {
            if (pk is IScaledSize3 size3)
            {
                if (size3.Scale < MinScaleStrongTera)
                    return false;
            }
            else if (pk is IScaledSize s2)
            {
                if (s2.HeightScalar < MinScaleStrongTera)
                    return false;
            }
        }
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        return true;
    }

    private bool IsMatchForm(PKM pk, EvoCriteria evo)
    {
        if (evo.Form == Form)
            return true;
        if (Species is (int)Core.Species.Deerling or (int)Core.Species.Sawsbuck)
            return pk.Form <= 3;
        return false;
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

    private bool IsMatchLocationExact(PKM pk)
    {
        var loc = pk.MetLocation;
        if (loc == Location0)
            return true;
        if (loc == 0)
            return false;
        return loc == Location1 || loc == Location2 || loc == Location3;
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
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    #endregion
}
