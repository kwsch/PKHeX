using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen7"/> (GO Park, <seealso cref="GameVersion.GG"/>).
/// <inheritdoc cref="PogoSlotExtensions" />
/// </summary>
public sealed record EncounterSlot7GO(int StartDate, int EndDate, ushort Species, byte Form, byte LevelMin, byte LevelMax, Shiny Shiny, Gender Gender, PogoType Type)
    : IEncounterable, IEncounterMatch, IPogoSlot, IEncounterConvertible<PB7>
{
    public int Generation => 7;
    public EntityContext Context => EntityContext.Gen7b;
    public Ball FixedBall => Ball.None; // GO Park can override the ball; obey capture rules for LGP/E
    public bool EggEncounter => false;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public bool IsShiny => Shiny.IsShiny();
    public int EggLocation => 0;

    public GameVersion Version => GameVersion.GO;
    public int Location => Locations.GO7;
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

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB7 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB7 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var pk = new PB7
        {
            PID = Util.Rand32(),
            EncryptionConstant = Util.Rand32(),
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OT_Friendship = PersonalTable.GG[Species].BaseFriendship,
            Met_Location = Location,
            Met_Level = LevelMin,
            Version = (byte)Version,
            Ball = (byte)Ball.Poke,
            MetDate = this.GetRandomValidDate(),

            Language = lang,
            OT_Name = tr.OT,
            OT_Gender = tr.Gender,
            ID32 = tr.ID32,
        };
        SetPINGA(pk, criteria);
        EncounterUtil1.SetEncounterMoves(pk, Version, LevelMin);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation);
        SetEncounterMoves(pk, LevelMin);
        pk.AwakeningSetAllTo(2);
        pk.HeightScalar = PokeSizeUtil.GetRandomScalar();
        pk.WeightScalar = PokeSizeUtil.GetRandomScalar();
        pk.ResetHeight();
        pk.ResetWeight();
        pk.ResetCP();
        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PB7 pk, EncounterCriteria criteria)
    {
        var g = Gender == Gender.Random ? -1 : (int)Gender;
        int gender = criteria.GetGender(g, PersonalTable.GG[Species]);
        int nature = (int)criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);

        criteria.SetRandomIVsGO(pk, Type.GetMinIV());
        pk.Nature = pk.StatNature = nature;
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

    private void SetEncounterMoves(PB7 pk, int level)
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

        if (!this.IsLevelWithinRange(pk.Met_Level))
            return false;
        //if (!slot.IsBallValid(ball)) -- can have any of the in-game balls due to re-capture
        //    continue;
        if (!Shiny.IsValid(pk))
            return false;
        //if (slot.Gender != Gender.Random && (int) slot.Gender != pk.Gender)
        //    continue;

        return true;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        var stamp = PogoDateRangeExtensions.GetTimeStamp(pk.Met_Year + 2000, pk.Met_Month, pk.Met_Day);
        if (!this.IsWithinStartEnd(stamp))
            return EncounterMatchRating.DeferredErrors;
        if (!this.GetIVsValid(pk))
            return EncounterMatchRating.Deferred;
        return EncounterMatchRating.Match;
    }
    #endregion
}
