using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Pokéwalker  Encounter
/// </summary>
public sealed record EncounterStatic4Pokewalker(PokewalkerCourse4 Course)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IMoveset
{
    public int Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    public GameVersion Version => GameVersion.HGSS;

    public int Location => Locations.PokeWalker4;
    public bool IsShiny => false;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Shiny Shiny => Shiny.Never;
    public byte Form => 0;
    public int EggLocation => 0;

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
        const int size = 0xC;
        var count = data.Length / size;
        System.Diagnostics.Debug.Assert(count == 6 * (int)PokewalkerCourse4.MAX_COUNT);
        var result = new EncounterStatic4Pokewalker[count];
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * size;
            var slice = data.Slice(offset, size);
            var course = (PokewalkerCourse4)(i / 6);
            result[i] = new(slice, course);
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
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        var pk = new PK4
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.HGSS[Species].BaseFriendship,

            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)version,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature.Random);

        // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
        // int ability = criteria.GetAbilityFromNumber(Ability, pi);

        PIDGenerator.SetRandomPIDPokewalker(pk, nature, gender);
        criteria.SetRandomIVs(pk);
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
        return pk.Egg_Location == expect;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (pk.Format == 4)
            return pk.Met_Location == Location;
        return true; // transfer location verified later
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return evo.LevelMax >= Level;
        return pk.Met_Level == Level;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    private static bool IsMatchPartial(PKM pk) => pk.Ball != (byte)Ball.Poke;
    #endregion
}
