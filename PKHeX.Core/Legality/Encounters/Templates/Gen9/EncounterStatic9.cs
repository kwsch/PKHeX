using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Static Encounter
/// </summary>
public sealed record EncounterStatic9(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK9>, IMoveset, IFlawlessIVCount, IFixedIVSet, IGemType, IFatefulEncounterReadOnly, IFixedGender, IFixedNature, IEncounterMarkExtra
{
    public byte Generation => 9;
    public EntityContext Context => EntityContext.Gen9;
    public bool IsShiny => Shiny == Shiny.Always;
    public bool IsEgg => EggLocation != 0;
    ushort ILocation.Location => Location;
    ushort ILocation.EggLocation => EggLocation;

    public Ball FixedBall { get; init; }
    public required ushort Location { get; init; }
    public ushort EggLocation { get; init; }
    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public byte Form { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public AbilityPermission Ability { get; init; }
    public byte FlawlessIVCount { get; init; }
    public Shiny Shiny { get; init; }
    public Moveset Moves { get; init; }
    public IndividualValueSet IVs { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public GemType TeraType { get; init; }
    public byte Size { get; init; }
    public bool IsTitan { get; init; }
    public bool RibbonMarkCrafty => Species == (int)Core.Species.Munchlax; // Shiny etc
    public bool FatefulEncounter { get; init; }
    public bool IsMissingExtraMark(PKM pk, out RibbonIndex index)
    {
        if (RibbonMarkCrafty)
        {
            if (pk is IRibbonSetMark8 m8 && !m8.HasMark8(RibbonIndex.MarkCrafty))
            {
                index = RibbonIndex.MarkCrafty;
                return true;
            }
        }
        index = default;
        return false;
    }

    private bool Gift => FixedBall != Ball.None;

    private bool NoScalarsDefined => Size == 0;
    public bool GiftWithLanguage => Gift && !ScriptedYungoos; // Nice error by GameFreak -- all gifts (including eggs) set the HT_Language memory value in addition to OT_Language.
    public bool StarterBoxLegend => Gift && Species is (int)Core.Species.Koraidon or (int)Core.Species.Miraidon;
    public bool ScriptedYungoos => Species == (int)Core.Species.Yungoos && Level == 2;

    public SizeType9 ScaleType => NoScalarsDefined ? SizeType9.RANDOM : SizeType9.VALUE;
    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    public const byte RideLegendFormArg = 1;

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK9 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PK9 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.SV[Species, Form];
        var pk = new PK9
        {
            Language = lang,
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = version,
            Ball = (byte)Ball.Poke,
            FatefulEncounter = FatefulEncounter,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
            ObedienceLevel = LevelMin,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLocation = Locations.HatchLocation9;
            pk.MetLevel = EggStateLegality.EggMetLevel;
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        if (GiftWithLanguage)
            pk.HandlingTrainerLanguage = (byte)pk.Language;
        if (StarterBoxLegend)
            pk.FormArgument = RideLegendFormArg; // Not Ride Form.
        if (IsTitan)
        {
            pk.RibbonMarkTitan = true;
            pk.AffixedRibbon = (sbyte)RibbonIndex.MarkTitan;
        }
        else if (RibbonMarkCrafty)
        {
            pk.RibbonMarkCrafty = true;
            pk.AffixedRibbon = (sbyte)RibbonIndex.MarkCrafty;
        }

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK9 pk, EncounterCriteria criteria, PersonalInfo9SV pi)
    {
        const byte undefinedSize = 0;
        byte height, weight, scale;
        if (NoScalarsDefined)
        {
            height = weight = scale = undefinedSize;
        }
        else
        {
            // Gifts have a defined H/W/S, while capture-able only have scale.
            height = weight = Gift ? Size : undefinedSize;
            scale = Size;
        }

        const byte rollCount = 1;
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, rollCount, height, weight, ScaleType, scale,
            Ability, Shiny);

        ulong init = Util.Rand.Rand64();
        var success = this.TryApply64(pk, init, param, criteria, IVs.IsSpecified);
        if (!success)
            this.TryApply64(pk, init, param, EncounterCriteria.Unrestricted, IVs.IsSpecified);
        if (IVs.IsSpecified)
        {
            pk.IV_HP = IVs.HP;
            pk.IV_ATK = IVs.ATK;
            pk.IV_DEF = IVs.DEF;
            pk.IV_SPA = IVs.SPA;
            pk.IV_SPD = IVs.SPD;
            pk.IV_SPE = IVs.SPE;
        }

        if (Gender != FixedGenderUtil.GenderRandom)
            pk.Gender = Gender;
        if (Nature != Nature.Random)
            pk.Nature = pk.StatNature = Nature;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (IVs.IsSpecified && !Legal.GetIsFixedIVSequenceValidSkipRand(IVs, pk))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;
        if (TeraType != GemType.Random && pk is ITeraType t && !Tera9RNG.IsMatchTeraType(TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return false;
        if (Nature != Nature.Random && pk.Nature != Nature)
            return false;

        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var eggLoc = pk.EggLocation;
        if (!IsEgg)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return eggLoc == expect;
        }

        if (!pk.IsEgg) // hatched
            return eggLoc == EggLocation || eggLoc == Locations.LinkTrade6;

        // Unhatched:
        if (eggLoc != EggLocation)
            return false;
        if (pk.MetLocation is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return IsMatchLocationExact(pk);
        if (metState == LocationRemapState.Remapped)
            return IsMatchLocationRemapped(pk);
        return IsMatchLocationExact(pk) || IsMatchLocationRemapped(pk);
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return IsMatchDeferred(pk);
    }

    private bool IsMatchLocationExact(PKM pk)
    {
        if (IsEgg)
            return true;
        return pk.MetLocation == Location;
    }

    private bool IsMatchLocationRemapped(PKM pk)
    {
        var met = pk.MetLocation;
        var version = pk.Version;
        if (pk.Context == EntityContext.Gen8)
            return LocationsHOME.IsValidMetSV(met, version);
        return LocationsHOME.GetMetSWSH(Location, version) == met;
    }

    private EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return EncounterMatchRating.DeferredErrors;

        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                var a = Ability;
                if (a is OnlyHidden)
                {
                    if (!AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                        return EncounterMatchRating.DeferredErrors;
                    a = num == 1 ? OnlyFirst : OnlySecond;
                }
                if (a is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return EncounterMatchRating.Match;
    }

    private bool IsMatchPartial(PKM pk)
    {
        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        if (pk is IScaledSize v && !NoScalarsDefined)
        {
            if (Gift)
            {
                if (v.HeightScalar != Size)
                    return true;
                if (v.WeightScalar != Size)
                    return true;
            }
            var current = pk is IScaledSize3 size3 ? size3.Scale : v.HeightScalar;
            if (current != Size)
                return true;
        }

        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    #endregion
}
