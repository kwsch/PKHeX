using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot representing data transferred to <see cref="GameVersion.Gen8"/> (HOME).
/// <inheritdoc cref="PogoSlotExtensions" />
/// </summary>
public sealed record EncounterSlot8GO(int StartDate, int EndDate, ushort Species, byte Form, byte LevelMin, byte LevelMax, Shiny Shiny, Gender Gender, PogoType Type, PogoImportFormat OriginFormat)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PKM>, IPogoSlot, IFixedOTFriendship, IEncounterServerDate
{
    public byte Generation => 8;
    public bool IsDateRestricted => true;
    public bool IsShiny => Shiny.IsShiny();
    public Ball FixedBall => Type.GetValidBall();
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public ushort EggLocation => 0;
    public GameVersion Version => GameVersion.GO;
    public ushort Location => Locations.GO8;

    public string Name => $"Wild Encounter ({Version})";
    public string LongName
    {
        get
        {
            var init = $"{Name} ({Type})";
            if (StartDate == 0 && EndDate == 0)
                return init;
            var start = PogoDateRangeExtensions.GetDateString(StartDate);
            var end = PogoDateRangeExtensions.GetDateString(EndDate);
            return $"{init}: {start}-{end}";
        }
    }

    private Ball GetRequiredBall(Ball fallback)
    {
        var fix = FixedBall;
        return fix == Ball.None ? fallback : fix;
    }

    /// <summary>
    /// Encounters need a Parent Game to determine the original moves when transferred to HOME.
    /// </summary>
    /// <remarks>
    /// Future game releases might change this value.
    /// With respect to date legality, new dates might be incompatible with initial <seealso cref="OriginFormat"/> values.
    /// </remarks>
    public PogoImportFormat OriginFormat { get; } = OriginFormat;

    /// <summary>
    /// Checks if the <seealso cref="Ball"/> is compatible with the <seealso cref="PogoType"/>.
    /// </summary>
    public bool IsBallValid(Ball ball, ushort currentSpecies, PKM pk)
    {
        // GO does not natively produce Shedinja when evolving Nincada, and thus must be evolved in future games.
        if (currentSpecies == (int)Shedinja && currentSpecies != Species)
            return ball == Ball.Poke;
        if (ball == Ball.Master)
            return Type.IsMasterBallUsable() && pk.MetDate >= new DateOnly(2023, 5, 21);
        return Type.IsBallValid(ball);
    }

    private PKM GetBlank() => OriginFormat switch
    {
        PogoImportFormat.PK7 => new PK8(),
        PogoImportFormat.PB7 => new PB7(),
        PogoImportFormat.PK8 => new PK8(),
        PogoImportFormat.PA8 => new PA8(),
        PogoImportFormat.PK9 => new PK9(),
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    private IPersonalInfo GetPersonal() => OriginFormat switch
    {
        PogoImportFormat.PK7 => PersonalTable.USUM.GetFormEntry(Species, Form),
        PogoImportFormat.PB7 => PersonalTable.GG.GetFormEntry(Species, Form),
        PogoImportFormat.PK8 => PersonalTable.SWSH.GetFormEntry(Species, Form),
        PogoImportFormat.PA8 => PersonalTable.LA.GetFormEntry(Species, Form),
        PogoImportFormat.PK9 => PersonalTable.SV.GetFormEntry(Species, Form),
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    private GameVersion OriginGroup => OriginFormat switch
    {
        PogoImportFormat.PK7 => GameVersion.USUM,
        PogoImportFormat.PB7 => GameVersion.GG,
        PogoImportFormat.PK8 => GameVersion.SWSH,
        PogoImportFormat.PA8 => GameVersion.PLA,
        PogoImportFormat.PK9 => GameVersion.SV,
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    public EntityContext Context => OriginFormat switch
    {
        PogoImportFormat.PK7 or PogoImportFormat.PB7 =>
              PersonalTable.BDSP.IsPresentInGame(Species, Form) ? EntityContext.Gen8b
            : PersonalTable.LA.IsPresentInGame(Species, Form) ? EntityContext.Gen8a
            : PersonalTable.SV.IsPresentInGame(Species, Form) ? EntityContext.Gen9
            : EntityContext.Gen8, // don't throw an exception, just give them a context.
        PogoImportFormat.PK8 => EntityContext.Gen8,
        PogoImportFormat.PA8 => EntityContext.Gen8a,
        PogoImportFormat.PK9 => EntityContext.Gen9,
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pk = GetBlank();
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var rnd = Util.Rand;
        {
            pk.Language = lang;
            pk.PID = rnd.Rand32();
            pk.EncryptionConstant = rnd.Rand32();
            pk.Species = Species;
            pk.Form = Form;
            pk.CurrentLevel = LevelMin;
            pk.OriginalTrainerFriendship = OriginalTrainerFriendship;
            pk.MetLocation = Location;
            pk.MetLevel = LevelMin;
            pk.Version = GameVersion.GO;
            pk.Ball = (byte)GetRequiredBall(Ball.Poke);
            pk.MetDate = this.GetRandomValidDate();

            pk.OriginalTrainerName = tr.OT;
            pk.ID32 = tr.ID32;
            pk.OriginalTrainerGender = tr.Gender;
            pk.HandlingTrainerName = "PKHeX";
            pk.CurrentHandler = 1;
            if (pk is IHandlerLanguage l)
                l.HandlingTrainerLanguage = 2;
        }
        SetPINGA(pk, criteria);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameImportHOME(Species, lang, Generation);
        SetEncounterMoves(pk, LevelMin);

        if (pk is IScaledSize s2)
        {
            s2.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            s2.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            if (pk is IScaledSize3 s3)
                s3.Scale = s2.HeightScalar;
        }
        if (pk is PA8 pa8)
        {
            pa8.ResetHeight();
            pa8.ResetWeight();
        }
        else if (pk is PK9 pk9)
        {
            var pi = pk9.PersonalInfo;
            pk9.TeraTypeOriginal = pk9.TeraTypeOverride = TeraTypeUtil.GetTeraTypeImport(pi.Type1, pi.Type2);
            pk9.ObedienceLevel = pk9.MetLevel;
        }
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = GetPersonal();
        if (OriginFormat is PogoImportFormat.PK7)
            pk.EXP = Experience.GetEXP(LevelMin, pi.EXPGrowth);
        var gender = criteria.GetGender(Gender, pi);
        var nature = criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);

        pk.Nature = pk.StatNature = nature;
        pk.Gender = gender;

        pk.AbilityNumber = 1 << ability;
        if ((uint)ability < pi.AbilityCount)
            pk.Ability = pi.GetAbilityAtIndex(ability);

        criteria.SetRandomIVsGO(pk, Type.GetMinIV());

        switch (Shiny)
        {
            case Shiny.Random when !pk.IsShiny && criteria.Shiny.IsShiny():
            case Shiny.Always when !pk.IsShiny: // Force Square
                var low = pk.PID & 0xFFFF;
                pk.PID = ((low ^ pk.TID16 ^ pk.SID16 ^ 0) << 16) | low;
                break;
            case Shiny.Random when pk.IsShiny && !criteria.Shiny.IsShiny():
            case Shiny.Never when pk.IsShiny: // Force Not Shiny
                pk.PID ^= 0x1000_0000;
                break;
        }
    }

    private void SetEncounterMoves(PKM pk, int level)
    {
        Span<ushort> moves = stackalloc ushort[4];
        GetInitialMoves(level, moves);
        pk.SetMoves(moves);
    }

    public void GetInitialMoves(int level, Span<ushort> moves)
    {
        var source = GameData.GetLearnSource(OriginGroup);
        source.SetEncounterMoves(Species, Form, level, moves);
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        if (!IsBallValid((Ball)pk.Ball, pk.Species, pk))
            return false;
        if (!Shiny.IsValid(pk))
            return false;
        if (Gender != Gender.Random && (int)Gender != pk.Gender)
            return false;
        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        if (Species is (int)Farfetchd && IsReallySirfetchd(pk))
            return EncounterMatchRating.DeferredErrors;
        if (!this.GetIVsValid(pk))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }

    /// <summary>
    /// Checks if a Farfetch'd was originally a Sirfetch'd.
    /// </summary>
    /// <remarks>Only basis we can check with is if it has the bad HOME apostrophe.</remarks>
    private static bool IsReallySirfetchd(PKM pk)
    {
        if (pk.Species != (int)Sirfetchd)
            return false;

        // Check for the "wrong" apostrophe. If it matches the species name, then it was originally Farfetch'd.
        if (pk.IsNicknamed || !SpeciesName.IsApostropheFarfetchdLanguage(pk.Language))
            return false; // can't tell if it was originally Farfetch'd

        Span<char> name = stackalloc char[pk.TrashCharCountNickname];
        pk.LoadString(pk.NicknameTrash, name);
        return name is "Sirfetch'd"; // only way to get the bad apostrophe is to originate in HOME with it.
    }

    public byte OriginalTrainerFriendship => Species switch
    {
        (int)Timburr  when Form == 0 => 70,
        (int)Stunfisk when Form == 0 => 70,
        (int)Hoopa    when Form == 1 => 50,
        _ => GetHOMEFriendship(),
    };

    private byte GetHOMEFriendship()
    {
        var fs = GetPersonal().BaseFriendship;
        if (fs == 70)
            return 50;
        return fs;
    }

    private bool IsMatchPartial(PKM pk)
    {
        if (!IsWithinDistributionWindow(pk))
            return true;
        if (!this.GetIVsAboveMinimum(pk))
            return true;

        // Eevee & Glaceon have different base friendships. Make sure if it is invalid that we yield the other encounter before.
        if (pk.OriginalTrainerFriendship != OriginalTrainerFriendship)
            return true;

        return IsFormArgIncorrect(pk);
    }

    public bool IsWithinDistributionWindow(PKM pk)
    {
        var date = new DateOnly(pk.MetYear + 2000, pk.MetMonth, pk.MetDay);
        return IsWithinDistributionWindow(date);
    }

    public bool IsWithinDistributionWindow(DateOnly date)
    {
        var stamp = PogoDateRangeExtensions.GetTimeStamp(date.Year, date.Month, date.Day);
        return this.IsWithinStartEnd(stamp);
    }

    private bool IsFormArgIncorrect(ISpeciesForm pk) => Species switch
    {
        // Evolved without Form Argument changing from default
        (int)Yamask     when pk.Species != Species && Form == 1 => pk is IFormArgument { FormArgument: 0 },
        (int)Milcery    when pk.Species != Species              => pk is IFormArgument { FormArgument: 0 },
        (int)Stantler   when pk.Species != Species              => pk is IFormArgument { FormArgument: 0 },
        (int)Qwilfish   when pk.Species != Species && Form == 1 => pk is IFormArgument { FormArgument: 0 },
        (int)Basculin   when pk.Species != Species && Form == 2 => pk is IFormArgument { FormArgument: 0 },
        (int)Primeape   when pk.Species != Species              => pk is IFormArgument { FormArgument: 0 },
        (int)Bisharp    when pk.Species != Species              => pk is IFormArgument { FormArgument: 0 },

        // Not evolved, but Form Argument changed from default
        (int)Runerigus   => pk is IFormArgument { FormArgument: not 0 },
        (int)Alcremie    => pk is IFormArgument { FormArgument: not 0 },
        (int)Wyrdeer     => pk is IFormArgument { FormArgument: not 0 },
        (int)Overqwil    => pk is IFormArgument { FormArgument: not 0 },
        (int)Basculegion => pk is IFormArgument { FormArgument: not 0 },
        (int)Annihilape  => pk is IFormArgument { FormArgument: not 0 },
        (int)Kingambit   => pk is IFormArgument { FormArgument: not 0 },
        (int)Gholdengo   => pk is IFormArgument { FormArgument: not 0 },

        // No Form Argument relevant to check
        _ => false,
    };
    #endregion
}

/// <summary>
/// Enumerates the possible ways Pokémon GO data can be initialized with when imported to HOME.
/// </summary>
public enum PogoImportFormat : byte
{
    PK7 = 0,
    PB7 = 1,
    PK8 = 2,
    PA8 = 3,
    PK9 = 4,
}
