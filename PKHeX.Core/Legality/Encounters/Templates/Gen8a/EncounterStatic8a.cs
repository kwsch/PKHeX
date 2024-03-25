using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Static Encounter
/// </summary>
public sealed record EncounterStatic8a
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PA8>, ISeedCorrelation64<PKM>,
        IAlphaReadOnly, IMasteryInitialMoveShop8, IScaledSizeReadOnly,
        IMoveset, IFlawlessIVCount, IFatefulEncounterReadOnly, IFixedGender
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8a;
    public GameVersion Version => GameVersion.PLA;
    public ushort EggLocation => 0;
    ushort ILocation.Location => Location;
    public bool IsShiny => Shiny == Shiny.Always;
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;

    public ushort Species { get; }
    public byte Form { get; }
    public byte HeightScalar { get; }
    public byte WeightScalar { get; }
    public byte LevelMin { get; }
    public byte LevelMax { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public required byte Location { get; init; }
    public byte FlawlessIVCount { get; init; }
    public Shiny Shiny { get; init; } = Shiny.Never;
    public bool IsAlpha { get; init; }
    public bool FatefulEncounter { get; init; }
    public Ball FixedBall { get; init; }
    public Moveset Moves { get; init; }
    public EncounterStatic8aCorrelation Method { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;

    public bool HasFixedHeight => HeightScalar != NoScalar;
    public bool HasFixedWeight => WeightScalar != NoScalar;

    private const byte NoScalar = 0;
    private const byte ScaleMax = 255;

    public EncounterStatic8a(ushort species, byte form, byte level, byte heightScalar = NoScalar, byte weightScalar = NoScalar)
    {
        Species = species;
        Form = form;
        HeightScalar = heightScalar;
        WeightScalar = weightScalar;
        LevelMin = LevelMax = level;
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PA8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PA8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pi = PersonalTable.LA[Species, Form];
        var pk = new PA8
        {
            Language = lang,
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,
            FatefulEncounter = FatefulEncounter,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateSwitch(),
            Version = Version,

            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,

            IsAlpha = IsAlpha,
            Ball = (byte)(FixedBall == Ball.None ? Ball.LAPoke : FixedBall),
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };
        SetPINGA(pk, criteria);
        pk.ResetHeight();
        pk.ResetWeight();
        SetEncounterMoves(pk, pk.MetLevel);

        if (IsAlpha)
            pk.IsAlpha = true;

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PA8 pk, EncounterCriteria criteria)
    {
        var para = GetParams();
        var (_, slotSeed) = Overworld8aRNG.ApplyDetails(pk, criteria, para, IsAlpha);
        // Phione and Zorua have random levels; follow the correlation instead of giving the lowest level.
        if (LevelMin != LevelMax)
            pk.MetLevel = pk.CurrentLevel = Overworld8aRNG.GetRandomLevel(slotSeed, LevelMin, LevelMax);

        // Disassociate the correlation if it is supposed to use the global 128-bit RNG state instead.
        if (Method == EncounterStatic8aCorrelation.Fixed)
            pk.EncryptionConstant = Util.Rand32();

        if (HasFixedHeight)
            pk.HeightScalar = HeightScalar;
        if (HasFixedWeight)
            pk.WeightScalar = WeightScalar;
        pk.Scale = pk.HeightScalar;
    }

    private void SetEncounterMoves(PA8 pk, int level)
    {
        Span<ushort> moves = stackalloc ushort[4];
        var (learn, mastery) = GetLevelUpInfo();
        LoadInitialMoveset(pk, moves, learn, level);
        pk.SetMoves(moves);
        pk.SetEncounterMasteryFlags(moves, mastery, level);
        if (pk.AlphaMove != 0)
            pk.SetMasteryFlagMove(pk.AlphaMove);
    }

    public (Learnset Learn, Learnset Mastery) GetLevelUpInfo() => LearnSource8LA.GetLearnsetAndMastery(Species, Form);

    public void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, int level)
    {
        if (Moves.HasMoves)
            Moves.CopyTo(moves);
        else
            learn.SetEncounterMoves(level, moves);
        if (IsAlpha)
            pa8.AlphaMove = moves[0];
    }

    private OverworldParam8a GetParams()
    {
        var gender = GetGenderRatio();
        return new OverworldParam8a
        {
            IsAlpha = IsAlpha,
            FlawlessIVs = FlawlessIVCount,
            Shiny = Shiny,
            RollCount = 1, // Everything is shiny locked anyway
            GenderRatio = gender,
        };
    }

    private byte GetGenderRatio() => Gender switch
    {
        0 => PersonalInfo.RatioMagicMale,
        1 => PersonalInfo.RatioMagicFemale,
        _ => GetGenderRatioPersonal(),
    };

    private byte GetGenderRatioPersonal()
    {
        var pt = PersonalTable.LA;
        var entry = pt.GetFormEntry(Species, Form);
        return entry.Gender;
    }
    #endregion

    #region Matching
    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (pk is IAlpha a && a.IsAlpha != IsAlpha)
            return false;
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        if (FlawlessIVCount != 0 && pk.FlawlessIVCount < FlawlessIVCount)
            return false;

        if (pk is not IScaledSize s)
            return true;

        // 3 of the Alpha statics were mistakenly set as 127 scale. If they enter HOME on 3.0.1, they'll get bumped to 255.
        if (IsAlpha && this is { HeightScalar: 127, WeightScalar: 127 }) // Average Size Alphas
        {
            // HOME >=3.0.1 ensures 255 scales for the 127's
            // PLA and S/V could have safe-harbored them via <=3.0.0
            if (pk.Context is EntityContext.Gen8a or EntityContext.Gen9)
            {
                if (s is not { HeightScalar: 127, WeightScalar: 127 }) // Original? 
                {
                    // Must match the HOME updated values AND must have the Alpha ribbon (visited HOME).
                    if (s is not { HeightScalar: ScaleMax, WeightScalar: ScaleMax })
                        return false;
                    if (pk is IRibbonSetMark9 { RibbonMarkAlpha: false })
                        return false;
                    if (pk.IsUntraded)
                        return false;
                }
            }
            else
            {
                // Must match the HOME updated values
                if (s is not { HeightScalar: ScaleMax, WeightScalar: ScaleMax })
                    return false;
            }
        }
        else
        {
            if (HasFixedHeight && s.HeightScalar != HeightScalar)
                return false;
            if (HasFixedWeight && s.WeightScalar != WeightScalar)
                return false;
        }

        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
        return pk.EggLocation == expect;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var metState = LocationsHOME.GetRemapState(Context, pk.Context);
        if (metState == LocationRemapState.Original)
            return pk.MetLocation == Location;
        if (metState == LocationRemapState.Remapped)
            return IsMetRemappedSWSH(pk);
        return pk.MetLocation == Location || IsMetRemappedSWSH(pk);
    }

    private static bool IsMetRemappedSWSH(PKM pk) => pk.MetLocation == LocationsHOME.SWLA;

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (Shiny != Shiny.Random && !Shiny.IsValid(pk))
            return EncounterMatchRating.DeferredErrors;
        if (FixedBall != Ball.None && pk.Ball != (byte)FixedBall)
            return EncounterMatchRating.DeferredErrors;

        if (!IsForcedMasteryCorrect(pk))
            return EncounterMatchRating.DeferredErrors;

        if (!MarkRules.IsMarkValidAlpha(pk, IsAlpha))
            return EncounterMatchRating.DeferredErrors;

        if (IsAlpha && pk is PA8 { AlphaMove: 0 })
            return EncounterMatchRating.Deferred;

        return EncounterMatchRating.Match;
    }

    public bool IsForcedMasteryCorrect(PKM pk)
    {
        ushort alpha = 0;
        if (IsAlpha && Moves.HasMoves)
        {
            if (pk is PA8 pa && (alpha = pa.AlphaMove) != Moves.Move1)
                return false;
        }

        if (pk is not IMoveShop8Mastery p)
            return true;

        const bool allowAlphaPurchaseBug = true; // Everything else Alpha is pre-1.1
        var level = pk.MetLevel;
        var (learn, mastery) = GetLevelUpInfo();
        if (!p.IsValidPurchasedEncounter(learn, level, alpha, allowAlphaPurchaseBug))
            return false;

        Span<ushort> moves = stackalloc ushort[4];
        if (Moves.HasMoves)
            Moves.CopyTo(moves);
        else
            learn.SetEncounterMoves(level, moves);

        return p.IsValidMasteredEncounter(moves, learn, mastery, level, alpha, allowAlphaPurchaseBug);
    }
    #endregion

    public bool TryGetSeed(PKM pk, out ulong seed)
    {
        // Check if it matches any single-roll seed.
        var param = GetParams();
        var solver = new XoroMachineSkip(pk.EncryptionConstant, pk.PID);
        foreach (var s in solver)
        {
            if (!Overworld8aRNG.Verify(pk, s, param, HasFixedHeight, HasFixedWeight))
                continue;
            seed = s;
            return true;
        }
        seed = default;
        return false;
    }
}
