namespace PKHeX.Core;

/// <summary>
/// Generation 5 Dream Radar gift encounters
/// </summary>
public sealed record EncounterStatic5Radar(ushort Species, byte Form, AbilityPermission Ability = AbilityPermission.OnlyHidden)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>
{
    public int Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public GameVersion Version => GameVersion.B2W2;
    public int Location => 30015;
    public Ball FixedBall => Ball.Dream;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.Never;
    public bool EggEncounter => false;
    public int EggLocation => 0;
    public byte LevelMin => 5;
    public byte LevelMax => 40;
    public string Name => "Dream Radar Encounter";
    public string LongName => Name;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)FixedBall,

            ID32 = tr.ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = tr.Gender,
            OT_Name = tr.OT,

            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        int gender = criteria.GetGender(pi);
        int nature = (int)criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        PIDGenerator.SetRandomWildPID5(pk, nature, ability, gender);
        if (pk.IsShiny)
            pk.PID ^= 0x1000_0000;
        criteria.SetRandomIVs(pk);
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.Met_Location != Location)
            return false;
        if (!IsMatchLevel(pk.Met_Level))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }
    private static bool IsMatchLevel(int met)
    {
        // Level from 5->40 depends on the number of badges
        if (met % 5 != 0)
            return false; // must be a multiple of 5
        return (uint)(met - 5) <= 35; // 5 <= x <= 40
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.Egg_Location == expect;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
