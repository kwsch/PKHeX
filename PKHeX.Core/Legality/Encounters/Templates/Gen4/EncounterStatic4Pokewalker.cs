using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Pokéwalker  Encounter
/// </summary>
public sealed record EncounterStatic4Pokewalker(PokewalkerCourse4 Course)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IMoveset, IRandomCorrelation, IFixedGender
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public GameVersion Version => GameVersion.HGSS;

    public ushort Location => Locations.PokeWalker4;
    public bool IsShiny => false;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Never;
    public byte Form => 0;
    public ushort EggLocation => 0;

    public ushort Species { get; }
    public byte Level { get; }
    public byte Gender { get; }
    public Moveset Moves { get; }

    public string Name => $"Pokéwalker Encounter ({Course})";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    private EncounterStatic4Pokewalker(ReadOnlySpan<byte> data, PokewalkerCourse4 course) : this(course)
    {
        Species = ReadUInt16LittleEndian(data);
        Level = data[2];
        Gender = data[3];
        var move1 = ReadUInt16LittleEndian(data[0x4..]);
        var move2 = ReadUInt16LittleEndian(data[0x6..]);
        var move3 = ReadUInt16LittleEndian(data[0x8..]);
        var move4 = ReadUInt16LittleEndian(data[0xA..]);
        Moves = new(move1, move2, move3, move4);
    }

    public static EncounterStatic4Pokewalker[] GetAll(ReadOnlySpan<byte> data)
    {
        const int SlotSize = 0xC;
        const int SlotsPerCourse = PokewalkerRNG.SlotsPerCourse;
        const int SlotCount = SlotsPerCourse * (int)PokewalkerCourse4.MAX_COUNT;
        System.Diagnostics.Debug.Assert(data.Length == SlotCount * SlotSize);

        PokewalkerCourse4 course = 0;
        var result = new EncounterStatic4Pokewalker[SlotCount];
        for (int i = 0, offset = 0; i < result.Length; course++)
        {
            for (int s = 0; s < SlotsPerCourse; s++, offset += SlotSize)
                result[i++] = new(data.Slice(offset, SlotSize), course);
        }
        return result;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.HGSS[Species];
        var pk = new PK4
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK4 pk, EncounterCriteria criteria, PersonalInfo4 pi)
    {
        var gender = criteria.GetGender(Gender, pi);
        var nature = (uint)criteria.GetNature();

        var pid = pk.PID = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, nature, pk.Gender = gender, pi.Gender);
        // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
        pk.RefreshAbility((int)(pid & 1));
        Span<int> ivs = stackalloc int[6];
        PokewalkerRNG.SetRandomIVs(ivs, criteria);
        pk.SetIVs(ivs);
    }

    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (!IsMatchGender(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private bool IsMatchSeed(PKM pk)
    {
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);
        var seed = PokewalkerRNG.GetFirstSeed(Species, Course, ivs);
        return seed.Type != PokewalkerSeedType.None;
    }

    private bool IsMatchGender(PKM pk)
    {
        if (pk.Gender == Gender)
            return true;

        // Azurill-F can change to M when evolving in Gen4 (but not in Gen5+) due to Gender Ratio differences.
        if (pk.Species != Species && Species == (ushort)Core.Species.Azurill && Gender == 1)
            return EntityGender.GetFromPIDAndRatio(pk.PID, 0xBF) == Gender;

        return true;
    }

    private static bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format == 4)
            return pk.MetLocation == Location;
        return true; // transfer location verified later
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return evo.LevelMax >= Level;
        return pk.MetLevel == Level;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (pk.IsShiny)
            return EncounterMatchRating.DeferredErrors;
        return EncounterMatchRating.Match;
    }

    private bool IsMatchPartial(PKM pk) => pk.Ball != (byte)Ball.Poke || !IsMatchSeed(pk);
    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (val is PIDType.Pokewalker)
            return true;

        // Pokewalker can sometimes be confused with CuteCharm due to the PID creation routine. Double check if it is okay.
        if (val is PIDType.CuteCharm)
            return MethodFinder.IsCuteCharm(pk, pk.EncryptionConstant) && MethodFinder.IsCuteCharm4Valid(this, pk);
        return false;
    }

    public PIDType GetSuggestedCorrelation() => PIDType.Pokewalker;
}
