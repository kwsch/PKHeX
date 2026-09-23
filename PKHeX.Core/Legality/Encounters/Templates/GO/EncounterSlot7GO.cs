using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="EntityContext.Gen7b"/> (GO Park).
/// <inheritdoc cref="PogoSlotExtensions" />
/// </summary>
public sealed record EncounterSlot7GO(ushort DayStart, ushort DayEnd, ushort Species, byte Form, byte LevelMin, byte MinimumIV, Shiny Shiny, Gender Gender, PogoType Type, PogoFlags Flags)
    : IEncounterable, IEncounterMatch, IPogoSlot, IEncounterConvertible<PB7>, IEncounterServerDate
{
    public bool IsDateRestricted => true;
    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public Ball FixedBall => Ball.None; // GO Park can override the ball; obey capture rules for LGP/E
    public bool IsEgg => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool IsShiny => Shiny.IsShiny();
    ushort ILocation.EggLocation => 0;

    public GameVersion Version => GameVersion.GO;
    public ushort Location => Locations.GO7;
    public byte LevelMax => EncountersGO.MAX_LEVEL;
    public bool IsLocalDayStart => Flags.HasFlag(PogoFlags.LocalDateStart);
    public bool IsLocalDayEnd => Flags.HasFlag(PogoFlags.LocalDateEnd);

    public string Name => $"GO Encounter ({Version})";
    public string LongName
    {
        get
        {
            var init = $"{Name} ({Type})";
            var start = PogoDateRangeExtensions.GetDateString(DayStart, IsLocalDayStart ? 1 : 0, true);
            var end = PogoDateRangeExtensions.GetDateString(DayEnd, IsLocalDayEnd ? -1 : 0);
            return $"{init}: {start}-{end}";
        }
    }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage789((LanguageID)tr.Language);
        var rnd = Util.Rand;
        var date = this.GetRandomValidDate();
        var pk = new PB7
        {
            PID = EncounterUtil.GetRandomPID(tr, rnd, Shiny, criteria.Shiny),
            EncryptionConstant = rnd.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = PersonalTable.GG[Species].BaseFriendship,
            MetLocation = Location,
            MetLevel = LevelMin,
            Version = Version,
            Ball = (byte)Ball.Poke,
            MetDate = date,

            Language = language,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,

            ReceivedDate = date,
            ReceivedTime = EncounterDate.GetTime(),
        };
        SetPINGA(pk, criteria);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation);
        SetEncounterMoves(pk, LevelMin);
        pk.AwakeningSetAllTo(2);
        pk.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB7 pk, in EncounterCriteria criteria)
    {
        var pi = PersonalTable.GG[Species];
        var gender = criteria.GetGender(Gender, pi);
        var nature = criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);

        criteria.SetRandomIVsGO(pk, MinimumIV);
        pk.Nature = pk.StatAlignment = nature;
        pk.Gender = gender;
        pk.RefreshAbility(ability);

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

    private void SetEncounterMoves(PB7 pk, byte level)
    {
        Span<ushort> moves = stackalloc ushort[4];
        var source = LearnSource7GG.Instance;
        ((ILearnSource)source).SetEncounterMoves(Species, Form, level, moves);
        pk.SetMoves(moves);
    }
    #endregion

    #region Matching

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Find the first chain that has slots defined.
        // Since it is possible to evolve before transferring, we only need the highest evolution species possible.
        // PoGoEncTool has already extrapolated the evolutions to separate encounters!

        if (!this.IsLevelWithinRange(pk.MetLevel))
            return false;
        //if (!slot.IsBallValid(ball)) -- can have any of the in-game balls due to re-capture
        //    continue;
        if (!Shiny.IsValid(pk))
            return false;

        // At least one encounter is a single gender (Pikachu 24-11-04 yay...) so check.
        if (Gender != Gender.Random && (int)Gender != pk.Gender)
            return false;

        return true;
    }

    public bool IsWithinDistributionWindow(PKM pk)
    {
        var date = new DateOnly(pk.MetYear + 2000, pk.MetMonth, pk.MetDay);
        return IsWithinDistributionWindow(date);
    }

    public bool IsWithinDistributionWindow(DateOnly date) => this.IsWithinStartEnd(date);

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (!IsWithinDistributionWindow(pk))
            return EncounterMatchRating.DeferredErrors;
        if (!this.GetIVsValid(pk))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }
    #endregion
}

public enum PogoFlags : byte
{
    LocalDateStart = 1 << 0,
    LocalDateEnd = 1 << 1,
    FeaturedWildArea = 1 << 2,
}
