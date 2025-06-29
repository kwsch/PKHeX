namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.GG"/>.
/// </summary>
public sealed record EncounterSlot7b(EncounterArea7b Parent, ushort Species, byte LevelMin, byte LevelMax, byte CrossoverFlags)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PB7>
{
    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.None;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.GG[Species];
        var date = EncounterDate.GetDateSwitch();
        var pk = new PB7
        {
            Species = Species,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            MetDate = date,
            Ball = (byte)Ball.Poke,

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),

            ReceivedDate = date,
            ReceivedTime = EncounterDate.GetTime(),
        };
        SetPINGA(pk, criteria, pi);
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB7 pk, in EncounterCriteria criteria, PersonalInfo7GG pi)
    {
        var rnd = Util.Rand;
        pk.PID = rnd.Rand32();
        if (criteria.Shiny.IsShiny())
            pk.PID = ShinyUtil.GetShinyPID(pk.TID16, pk.SID16, pk.PID, criteria.Shiny == Shiny.AlwaysSquare ? 0 : (uint)rnd.Next(1, 15));
        else if (criteria.Shiny == Shiny.Never && pk.IsShiny)
            pk.PID ^= 0x80000000; // flip top bit to ensure non-shiny

        pk.EncryptionConstant = rnd.Rand32();
        pk.Nature = criteria.GetNature();
        pk.Gender = criteria.GetGender(pi);
        pk.RefreshAbility(criteria.GetAbilityFromNumber(Ability));

        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);

        criteria.SetRandomIVs(pk);
    }
    #endregion

    private const int CatchComboBonus = 1;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel, 0, CatchComboBonus))
            return false;
        if (Form != evo.Form)
            return false;
        if (pk.MetLocation != Parent.Location && !IsCrossoverAllowed(pk.MetLocation))
            return false;
        return true;
    }

    private bool IsCrossoverAllowed(ushort metloc)
    {
        var bit = metloc == Parent.ToArea1 ? 0b01 : 0b10; // at most 2 locations, we already matched one
        return (CrossoverFlags & bit) != 0;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
}
