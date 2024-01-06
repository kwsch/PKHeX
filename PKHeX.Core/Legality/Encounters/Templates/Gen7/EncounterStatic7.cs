namespace PKHeX.Core;

/// <summary>
/// Generation 7 Static Encounter
/// </summary>
public sealed record EncounterStatic7(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK7>, IRelearn, IEncounterFormRandom, IFlawlessIVCount, IFatefulEncounterReadOnly, IFixedGender, IFixedNature, IFixedIVSet
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    int ILocation.Location => Location;
    int ILocation.EggLocation => EggLocation;
    public bool RibbonWishing => Species == (int)Core.Species.Magearna;

    public bool EggEncounter => EggLocation != 0;
    public bool IsShiny => false;

    public Moveset Relearn { get; init; }
    public IndividualValueSet IVs { get; init; }
    public ushort Location { get; init; }
    public ushort EggLocation { get; init; }
    public required ushort Species { get; init; }
    public byte Form { get; init; }
    public required byte Level { get; init; }
    public bool FatefulEncounter { get; init; }
    public byte FlawlessIVCount { get; init; }
    public Shiny Shiny { get; init; }
    public AbilityPermission Ability { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public Nature Nature { get; init; } = Nature.Random;
    public Ball FixedBall { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public bool IsTotem => FormInfo.IsTotemForm(Species, Form);
    public bool IsTotemNoTransfer => Species is (int)Core.Species.Marowak or (int)Core.Species.Araquanid or (int)Core.Species.Togedemaru or (int)Core.Species.Ribombee;
    public int GetTotemBaseForm() => FormInfo.GetTotemBaseForm(Species, Form);

    public bool IsRandomUnspecificForm => Form >= FormDynamic;

    private const int FormDynamic = FormVivillon;
    internal const int FormVivillon = 30;
    //protected const int FormRandom = 31;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion((GameVersion)tr.Game);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.USUM[Species, Form];
        var pk = new PK7
        {
            EncryptionConstant = Util.Rand32(),
            Species = Species,
            Form = GetWildForm(Form, tr),
            CurrentLevel = LevelMin,
            Met_Location = Location,
            Met_Level = LevelMin,
            MetDate = EncounterDate.GetDate3DS(),
            Ball = (byte)(FixedBall is Ball.None ? Ball.Poke : FixedBall),
            FatefulEncounter = FatefulEncounter,

            ID32 = tr.ID32,
            Version = (byte)version,
            Language = lang,
            OT_Gender = tr.Gender,
            OT_Name = tr.OT,

            OT_Friendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        if (RibbonWishing)
            pk.RibbonWishing = true;

        if (tr is IRegionOrigin r)
            r.CopyRegionOrigin(pk);
        else
            pk.SetDefaultRegionOrigins(lang);

        if (EggEncounter)
        {
            // Fake as hatched.
            pk.Met_Location = Locations.HatchLocation7;
            pk.Met_Level = EggStateLegality.EggMetLevel;
            pk.Egg_Location = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        if (Relearn.HasMoves)
            pk.SetRelearnMoves(Relearn);
        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);
        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private static byte GetWildForm(byte form, ITrainerInfo tr)
    {
        if (form == FormVivillon)
        {
            if (tr is IRegionOrigin r)
                return Vivillon3DS.GetPattern(r.Country, r.Region);
            if (tr.Language == 1)
                return Vivillon3DS.GetPattern(1, 0);
            return Vivillon3DS.GetPattern(49, 7); // USA, California
        }
        return form;
    }

    private void SetPINGA(PK7 pk, EncounterCriteria criteria, PersonalInfo7 pi)
    {
        pk.PID = Util.Rand32();
        if (pk.IsShiny)
        {
            if (Shiny == Shiny.Never || (Shiny != Shiny.Always && !criteria.Shiny.IsShiny()))
                pk.PID ^= 0x1000_0000;
        }
        else if (Shiny == Shiny.Always || (Shiny != Shiny.Never && criteria.Shiny.IsShiny()))
        {
            var low = pk.PID & 0xFFFF;
            pk.PID = ((low ^ pk.TID16 ^ pk.SID16) << 16) | low;
        }

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk, FlawlessIVCount);

        var ability = criteria.GetAbilityFromNumber(Ability);
        pk.Nature = (int)criteria.GetNature(Nature);
        pk.Gender = criteria.GetGender(Gender, pi);
        pk.AbilityNumber = 1 << ability;
        pk.Ability = pi.GetAbilityAtIndex(ability);
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (pk.Met_Level != Level)
            return false;
        if (!IsMatchForm(pk, evo))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (Nature != Nature.Random && pk.Nature != (int)Nature)
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        return true;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    private bool IsMatchLocation(PKM pk)
    {
        if (EggEncounter)
            return true;

        return pk.Met_Location == Location;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.Egg_Location == expect;
        }

        // Gift Eevee edge case
        if (EggLocation == Locations.Daycare5 && !Relearn.HasMoves && pk.RelearnMove1 != 0)
            return false;

        var eggloc = pk.Egg_Location;
        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }

    private bool IsMatchForm(PKM pk, EvoCriteria evo)
    {
        if (IsRandomUnspecificForm)
            return true;

        if (IsTotem)
        {
            var expectForm = pk.Format == 7 ? Form : FormInfo.GetTotemBaseForm(Species, Form);
            return expectForm == evo.Form;
        }

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }
    #endregion
}
