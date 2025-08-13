using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Static Encounter from N
/// </summary>
public sealed record EncounterStatic5N(uint PID)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>, IFixedTrainer, IFixedNature, ITrainerID32ReadOnly
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public GameVersion Version => GameVersion.B2W2;
    public const bool NSparkle = true;
    public bool IsFixedTrainer => true;
    private const ushort ID32 = 2;
    private const byte IV = 30;

    public byte Form => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => false;
    public Shiny Shiny => Shiny.FixedValue;
    public bool IsEgg => false;
    public ushort EggLocation => 0;
    public Ball FixedBall => Species == (int)Core.Species.Zorua ? Ball.Poke : Ball.None; // Zorua can't be captured; others can.

    public ushort TID16 => ID32;
    uint ITrainerID32ReadOnly.ID32 => ID32;
    public ushort SID16 => 0;

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required byte Location { get; init; }
    public required Nature Nature { get; init; }
    public required AbilityPermission Ability { get; init; }

    public string Name => "N's Pokémon";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),

            Version = version,
            Language = language,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            IV32 = 0b11110_11110_11110_11110_11110_11110, // all 30

            NSparkle = NSparkle,
            OriginalTrainerName = GetOT(language),
            OriginalTrainerGender = 0,
            ID32 = ID32,
            PID = PID,
            Nature = Nature,
            Gender = pi.Genderless ? (byte)2 : default,
            Ability = Ability switch
            {
                AbilityPermission.OnlyFirst => pi.Ability1,
                AbilityPermission.OnlySecond => pi.Ability2,
                _ => pi.AbilityH,
            },
            HiddenAbility = Ability == AbilityPermission.OnlyHidden,
        };

        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (PID != pk.PID)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (pk.MetLocation != Location)
            return false;
        if (pk.MetLevel != Level)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        var pi = PersonalTable.B2W2.GetFormEntry(Species, Form);
        if (pk.Gender != (pi.Genderless ? 2 : 0))
            return false;
        if (pk.OriginalTrainerGender != 0)
            return false;
        if (pk is not { IV_HP: IV, IV_ATK: IV, IV_DEF: IV, IV_SPA: IV, IV_SPD: IV, IV_SPE: IV })
            return false;
        if (pk.ID32 != ID32)
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.EggLocation == expect;
    }

    private static string GetOT(int lang) => lang == (int)LanguageID.Japanese ? "Ｎ" : "N";
    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int lang) => trainer.SequenceEqual(GetOT(lang));

    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    #endregion
}
