namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="EntityContext.Gen1"/>.
/// </summary>
public sealed record EncounterSlot1(EncounterArea1 Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK1>, INumberedSlot
{
    public byte Generation => 1;
    public EntityContext Context => EntityContext.Gen1;
    public bool IsEgg => false;
    public Ball FixedBall => Ball.Poke;
    public AbilityPermission Ability => TransporterLogic.IsHiddenDisallowedVC1(Species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden;
    public Shiny Shiny => Shiny.Random;
    public bool IsShiny => false;
    public ushort EggLocation => 0;

    public byte Form => 0;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName => $"{Name} {Type.ToString().Replace('_', ' ')}";
    public GameVersion Version => Parent.Version;
    public ushort Location => Parent.Location;
    public SlotType1 Type => Parent.Type;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);

    public PK1 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK1 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var isJapanese = language == (int)LanguageID.Japanese;
        var pi = EncounterUtil.GetPersonal1(version, Species);
        var pk = new PK1(isJapanese)
        {
            Species = Species,
            CurrentLevel = LevelMin,
            CatchRate = pi.CatchRate,
            DV16 = EncounterUtil.GetRandomDVs(Util.Rand, criteria.Shiny.IsShiny(), criteria.HiddenPowerType),

            OriginalTrainerName = EncounterUtil.GetTrainerName(tr, language),
            TID16 = tr.TID16,
            Type1 = pi.Type1,
            Type2 = pi.Type2,
        };
        pk.SetNotNicknamed(language);
        if (criteria.Shiny.IsShiny())
            pk.SetShiny();

        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (LevelMin > evo.LevelMax)
            return false;

        if (pk is not PK1 pk1)
            return true;

        var rate = pk1.CatchRate;
        var expect = EncounterUtil.GetPersonal1(Version, Species).CatchRate;
        if (expect != rate && !(ParseSettings.AllowGen1Tradeback && ItemConverter.IsCatchRateHeldItem(rate)))
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;
    #endregion
}
