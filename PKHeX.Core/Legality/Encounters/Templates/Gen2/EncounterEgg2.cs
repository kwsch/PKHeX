namespace PKHeX.Core;

/// <summary>
/// Egg Encounter Data
/// </summary>
public sealed record EncounterEgg2(ushort Species, GameVersion Version) : IEncounterEgg
{
    public byte Form => 0;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 5;
    public bool CanHaveVoltTackle => false;

    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public bool IsEgg => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsShiny => false;
    public ushort Location => 0;
    public ushort EggLocation => 0;
    public Ball FixedBall => Generation <= 5 ? Ball.Poke : Ball.None;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12H;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = Version;
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var rnd = Util.Rand;

        var pk = new PK2(language == (int)LanguageID.Japanese)
        {
            Species = Species,
            CurrentLevel = Level,
            TID16 = tr.TID16,
            SID16 = tr.SID16,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            OriginalTrainerName = tr.OT,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = 120,

            DV16 = criteria.IsSpecifiedIVsAll()
                ? criteria.GetCombinedDVs()
                : EncounterUtil.GetRandomDVs(rnd, criteria.Shiny.IsShiny(), criteria.HiddenPowerType)
        };

        SetEncounterMoves(pk, version);
        pk.HealPP();

        if (version == GameVersion.C)
        {
            // Set met data for Crystal hatch.
            pk.MetLocation = Locations.HatchLocationC;
            pk.MetLevel = 1;
            pk.MetTimeOfDay = rnd.Next(1, 4); // Morning | Day | Night
            pk.OriginalTrainerGender = (byte)(tr.Gender & 1);
        }

        if (criteria.IsSpecifiedIVsAny(out _))
            criteria.SetRandomIVs(pk);
        else
            criteria.SetRandomIVs(pk, 3);

        return pk;
    }

    private void SetEncounterMoves(PK2 pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
