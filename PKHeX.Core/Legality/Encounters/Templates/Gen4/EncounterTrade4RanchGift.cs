using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Trade Encounter with a fixed PID value, met location, and version.
/// </summary>
public sealed record EncounterTrade4RanchGift : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>,
    IFatefulEncounterReadOnly, IFixedTrainer, IMoveset, IFixedGender, IFixedNature, IMetLevel, ITrainerID32ReadOnly
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;

    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    public Nature Nature => FatefulEncounter ? Nature.Random : (Nature)(PID % 25);

    public required ushort Location { get; init; }
    public Shiny Shiny => FatefulEncounter ? Shiny.Never : Shiny.FixedValue;
    public GameVersion Version { get; }
    public bool IsEgg => false;
    public ushort EggLocation { get; init; }

    public Ball FixedBall { get; init; } = Ball.Poke;
    public bool IsShiny => false;
    public bool IsFixedTrainer => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public ushort Species { get; }
    public byte MetLevel { get; }
    public byte Level { get; }

    public bool FatefulEncounter { get; }
    public required Moveset Moves { get; init; }
    public ushort TID16 => 1000;
    public required ushort SID16 { get; init; }
    public uint ID32 => (uint)(TID16 | (SID16 << 16));
    public required byte OTGender { get; init; }
    public required byte Gender { get; init; }
    public required AbilityPermission Ability { get; init; }
    public byte Form { get; init; }

    private static string GetTrainerName(int language) => language switch
    {
        1 => "ユカリ",
        2 => "Hayley",
        3 => "EULALIE",
        4 => "GIULIA",
        5 => "EUKALIA",
        7 => "Eulalia",
        _ => "",
    };

    private const string _name = "My Pokémon Ranch - Trade";
    public string Name => _name;
    public string LongName => _name;

    public EncounterTrade4RanchGift(uint pid, ushort species, byte met, byte level)
    {
        Version = GameVersion.D;
        PID = pid;
        Species = species;
        MetLevel = met;
        Level = level;
    }

    public EncounterTrade4RanchGift(ushort species, byte met, byte level)
    {
        Version = GameVersion.D;
        Species = species;
        MetLevel = met;
        Level = level;
        FatefulEncounter = true;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.DP[Species];
        var pk = new PK4
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            MetLocation = Location,
            MetLevel = MetLevel,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,
            FatefulEncounter = FatefulEncounter,

            ID32 = ID32,
            Version = version,
            Language = language,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = GetTrainerName(language),

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),

            HandlingTrainerName = tr.OT,
            HandlingTrainerGender = tr.Gender,
        };

        if (EggLocation != 0)
        {
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, Level);
        SetPINGA(pk, criteria);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK4 pk, in EncounterCriteria criteria)
    {
        var pid = FatefulEncounter ? Util.Rand32() : PID;
        pk.PID = pid;
        pk.Nature = (Nature)(pid % 25);
        pk.Gender = Gender;
        pk.RefreshAbility((int)(pid % 2));

        if (criteria.IsSpecifiedIVsAll())
            pk.IV32 = criteria.GetCombinedIVs();
        else
            criteria.SetRandomIVs(pk);
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => trainer.SequenceEqual(GetTrainerName(language));

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchLevel(pk, evo))
            return false;
        if (pk.ID32 != ID32)
            return false;
        if (!IsMatchNatureGenderShiny(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (evo.Form != Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (pk.OriginalTrainerGender != OTGender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.IsEgg)
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        // Met location is lost on transfer
        if (pk is not G4PKM pk4)
            return true;

        var met = pk4.MetLocation;
        return met == Location;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return evo.LevelMax >= Level;
        return pk.MetLevel == MetLevel;
    }

    private bool IsMatchNatureGenderShiny(PKM pk)
    {
        if (pk.Gender != Gender)
            return false;
        if (FatefulEncounter)
            return !pk.IsShiny;
        return PID == pk.EncryptionConstant;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = EggLocation;
        if (pk is PB8)
            expect = Locations.Default8bNone;
        return pk.EggLocation == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
