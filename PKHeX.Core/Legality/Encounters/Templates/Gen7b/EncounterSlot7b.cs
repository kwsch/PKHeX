namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.GG"/>.
/// </summary>
public sealed record EncounterSlot7b(EncounterArea7b Parent, ushort Species, byte LevelMin, byte LevelMax)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB7>
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public bool EggEncounter => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool IsShiny => false;
    public int EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name}";
    public GameVersion Version => Parent.Version;
    public int Location => Parent.Location;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.GG[Species];
        var pk = new PB7
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OT_Friendship = pi.BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            MetDate = EncounterDate.GetDateSwitch(),
            Ball = (byte)Ball.Poke,

            HeightScalar = PokeSizeUtil.GetRandomScalar(),
            WeightScalar = PokeSizeUtil.GetRandomScalar(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        SetPINGA(pk, criteria, pi);
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB7 pk, EncounterCriteria criteria, PersonalInfo7GG pi)
    {
        pk.PID = Util.Rand32();
        pk.EncryptionConstant = Util.Rand32();
        pk.Nature = (int)criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        criteria.SetRandomIVs(pk);
    }
    #endregion

    private const int CatchComboBonus = 1;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.Met_Level, 0, CatchComboBonus))
            return false;
        if (Form != evo.Form)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
}
