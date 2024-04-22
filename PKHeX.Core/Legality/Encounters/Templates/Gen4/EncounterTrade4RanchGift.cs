using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Trade Encounter with a fixed PID value, met location, and version.
/// </summary>
public sealed record EncounterTrade4RanchGift
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IFatefulEncounterReadOnly, IFixedTrainer, IMoveset, IFixedGender, IFixedNature
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;

    /// <summary>
    /// Fixed <see cref="PKM.PID"/> value the encounter must have.
    /// </summary>
    public readonly uint PID;

    public Nature Nature => (Nature)(PID % 25);

    public ushort Location { get; init; }
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
    public byte Level { get; }

    public bool FatefulEncounter { get; }
    public required Moveset Moves { get; init; }
    public required ushort TID16 { get; init; }
    public required ushort SID16 { get; init; }
    private uint ID32 => (uint)(TID16 | (SID16 << 16));
    public required byte OTGender { get; init; }
    public required byte Gender { get; init; }
    public required AbilityPermission Ability { get; init; }
    public byte CurrentLevel { get; init; }
    public byte Form { get; init; }

    private static readonly string[] TrainerNames = [string.Empty, "ユカリ", "Hayley", "EULALIE", "GIULIA", "EUKALIA", string.Empty, "Eulalia"];

    private const string _name = "In-game Trade";
    public string Name => _name;
    public string LongName => _name;

    public EncounterTrade4RanchGift(uint pid, ushort species, byte level)
    {
        Version = GameVersion.D;
        PID = pid;
        Species = species;
        Level = level;
    }

    public EncounterTrade4RanchGift(ushort species, byte level)
    {
        Version = GameVersion.D;
        Species = species;
        Level = level;
        FatefulEncounter = true;
        Location = 3000;
    }

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var actualLevel = CurrentLevel != default ? CurrentLevel : Level;
        var pi = PersonalTable.DP[Species];
        var pk = new PK4
        {
            Species = Species,
            Form = Form,
            CurrentLevel = actualLevel,
            MetLocation = Location,
            MetLevel = Level,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,
            FatefulEncounter = FatefulEncounter,

            ID32 = ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = OTGender,
            OriginalTrainerName = TrainerNames[lang],

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),

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
            EncounterUtil.SetEncounterMoves(pk, version, actualLevel);
        SetPINGA(pk, criteria);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK4 pk, EncounterCriteria criteria)
    {
        var pid = FatefulEncounter ? Util.Rand32() : PID;
        pk.PID = pid;
        pk.Nature = (Nature)(pid % 25);
        pk.Gender = Gender;
        pk.RefreshAbility((int)(pid % 2));
        criteria.SetRandomIVs(pk);
    }

    #endregion

    #region Matching

    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => (uint)language < TrainerNames.Length && trainer.SequenceEqual(TrainerNames[language]);

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

        if (Location != default)
            return pk.MetLevel == Level;
        return pk.MetLevel >= LevelMin;
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
