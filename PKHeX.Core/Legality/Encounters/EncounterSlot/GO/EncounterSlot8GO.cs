using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot representing data transferred to <see cref="GameVersion.Gen8"/> (HOME).
/// <inheritdoc cref="EncounterSlotGO" />
/// </summary>
public sealed record EncounterSlot8GO : EncounterSlotGO, IFixedOTFriendship
{
    public override int Generation => 8;

    /// <summary>
    /// Encounters need a Parent Game to determine the original moves when transferred to HOME.
    /// </summary>
    /// <remarks>
    /// Future game releases might change this value.
    /// With respect to date legality, new dates might be incompatible with initial <seealso cref="OriginFormat"/> values.
    /// </remarks>
    public PogoImportFormat OriginFormat { get; }

    public EncounterSlot8GO(EncounterArea8g area, ushort species, byte form, int start, int end, Shiny shiny, Gender gender, PogoType type, PogoImportFormat originFormat)
        : base(area, species, form, start, end, shiny, gender, type)
    {
        OriginFormat = originFormat;
    }

    /// <summary>
    /// Checks if the <seealso cref="Ball"/> is compatible with the <seealso cref="PogoType"/>.
    /// </summary>
    public bool IsBallValid(Ball ball, ushort currentSpecies)
    {
        // GO does not natively produce Shedinja when evolving Nincada, and thus must be evolved in future games.
        if (currentSpecies == (int)Shedinja && currentSpecies != Species)
            return ball == Ball.Poke;
        return Type.IsBallValid(ball);
    }

    protected override PKM GetBlank() => OriginFormat switch
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

    internal GameVersion OriginGroup => OriginFormat switch
    {
        PogoImportFormat.PK7 => GameVersion.USUM,
        PogoImportFormat.PB7 => GameVersion.GG,
        PogoImportFormat.PK8 => GameVersion.SWSH,
        PogoImportFormat.PA8 => GameVersion.PLA,
        PogoImportFormat.PK9 => GameVersion.SV,
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    public override EntityContext Context => OriginFormat switch
    {
        PogoImportFormat.PK7 or PogoImportFormat.PB7 =>
              PersonalTable.BDSP.IsPresentInGame(Species, Form) ? EntityContext.Gen8b
            : PersonalTable.LA.IsPresentInGame(Species, Form) ? EntityContext.Gen8a
            : EntityContext.Gen8, // don't throw an exception, just give them a context.
        PogoImportFormat.PK8 => EntityContext.Gen8,
        PogoImportFormat.PA8 => EntityContext.Gen8a,
        PogoImportFormat.PK9 => EntityContext.Gen9,
        _ => throw new ArgumentOutOfRangeException(nameof(OriginFormat)),
    };

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        pk.HT_Name = "PKHeX";
        pk.CurrentHandler = 1;
        if (pk is IHandlerLanguage l)
            l.HT_Language = 2;

        base.ApplyDetails(sav, criteria, pk);
        var ball = Type.GetValidBall();
        if (ball != Ball.None)
            pk.Ball = (int)ball;

        if (pk is IScaledSize s)
        {
            s.HeightScalar = PokeSizeUtil.GetRandomScalar();
            s.WeightScalar = PokeSizeUtil.GetRandomScalar();
        }

        if (OriginFormat is PogoImportFormat.PA8)
        {
            var pa8 = (PA8)pk;
            pa8.ResetHeight();
            pa8.ResetWeight();
            pa8.Scale = pa8.HeightScalar;
        }

        pk.OT_Friendship = OT_Friendship;

        pk.SetRandomEC();
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = GetPersonal();
        if (OriginFormat is PogoImportFormat.PK7)
            pk.EXP = Experience.GetEXP(LevelMin, pi.EXPGrowth);
        int gender = criteria.GetGender(-1, pi);
        int nature = (int)criteria.GetNature(Nature.Random);
        var ability = criteria.GetAbilityFromNumber(Ability);

        pk.PID = Util.Rand32();
        pk.Nature = pk.StatNature = nature;
        pk.Gender = gender;

        pk.AbilityNumber = 1 << ability;
        if ((uint)ability < pi.AbilityCount)
            pk.Ability = pi.GetAbilityAtIndex(ability);

        pk.SetRandomIVsGO();
        base.SetPINGA(pk, criteria);
    }

    protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        Span<ushort> moves = stackalloc ushort[4];
        GetInitialMoves(level, moves);
        pk.SetMoves(moves);
        pk.SetMaximumPPCurrent(moves);
    }

    public void GetInitialMoves(int level, Span<ushort> moves) => MoveLevelUp.GetEncounterMoves(moves, Species, Form, level, OriginGroup);

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return base.GetMatchRating(pk) == EncounterMatchRating.PartialMatch ? EncounterMatchRating.PartialMatch : EncounterMatchRating.Match;
    }

    public byte OT_Friendship => Species switch
    {
        (int)Timburr  when Form == 0 => 70,
        (int)Stunfisk when Form == 0 => 70,
        (int)Hoopa    when Form == 1 => 50,
        _ => GetHOMEFriendship(),
    };

    private byte GetHOMEFriendship()
    {
        var fs = (byte)GetPersonal().BaseFriendship;
        if (fs == 70)
            return 50;
        return fs;
    }

    private bool IsMatchPartial(PKM pk)
    {
        var stamp = GetTimeStamp(pk.Met_Year + 2000, pk.Met_Month, pk.Met_Day);
        if (!IsWithinStartEnd(stamp))
            return true;
        if (!GetIVsAboveMinimum(pk))
            return true;

        // Eevee & Glaceon have different base friendships. Make sure if it is invalid that we yield the other encounter before.
        if (pk.OT_Friendship != OT_Friendship)
            return true;

        return IsFormArgIncorrect(pk);
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
        (int)Gholdengo   => pk is IFormArgument { FormArgument: not 0 },
        (int)Kingambit   => pk is IFormArgument { FormArgument: not 0 },
        (int)Annihilape  => pk is IFormArgument { FormArgument: not 0 },

        // No Form Argument relevant to check
        _ => false,
    };
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
