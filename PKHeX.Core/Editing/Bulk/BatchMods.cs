using System;
using System.Collections.Generic;
using System.Globalization;
using static PKHeX.Core.BatchEditing;

namespace PKHeX.Core;

/// <summary>
/// Modifications for Batch Editing
/// </summary>
public static class BatchMods
{
    public static readonly List<ISuggestModification> SuggestionMods = new()
    {
        // Interface Specific
        new TypeSuggestion<ICombatPower>(nameof(ICombatPower.Stat_CP), p => p.ResetCP()),
        new TypeSuggestion<IScaledSizeValue>(nameof(IScaledSizeValue.HeightAbsolute), p => p.ResetHeight()),
        new TypeSuggestion<IScaledSizeValue>(nameof(IScaledSizeValue.WeightAbsolute), p => p.ResetWeight()),
        new TypeSuggestion<IHyperTrain>(nameof(Extensions.HyperTrainClear), p => p.HyperTrainClear()),
        new TypeSuggestion<IGeoTrack>(nameof(Extensions.ClearGeoLocationData), p => p.ClearGeoLocationData()),
        new TypeSuggestion<IAwakened>(nameof(AwakeningUtil.AwakeningClear), p => p.AwakeningClear()),
        new TypeSuggestion<IAwakened>(nameof(AwakeningUtil.AwakeningMax), p => p.AwakeningMax()),
        new TypeSuggestion<IGanbaru>(nameof(GanbaruExtensions.ClearGanbaruValues), p => p.ClearGanbaruValues()),
        new TypeSuggestion<IGanbaru>(nameof(GanbaruExtensions.SetSuggestedGanbaruValues), p => p.SetSuggestedGanbaruValues((PKM)p)),

        // Date Copy
        new TypeSuggestion<PKM>(nameof(PKM.EggMetDate), p => p.EggMetDate = p.MetDate),
        new TypeSuggestion<PKM>(nameof(PKM.MetDate), p => p.MetDate = p.EggMetDate),

        new TypeSuggestion<PKM>(nameof(PKM.Nature), p => p.Format >= 8, p => p.Nature = p.StatNature),
        new TypeSuggestion<PKM>(nameof(PKM.StatNature), p => p.Format >= 8, p => p.StatNature = p.Nature),
        new TypeSuggestion<PKM>(nameof(PKM.Stats), p => p.ResetPartyStats()),
        new TypeSuggestion<PKM>(nameof(PKM.Ball), p => BallApplicator.ApplyBallLegalByColor(p)),
        new TypeSuggestion<PKM>(nameof(PKM.Heal), p => p.Heal()),
        new TypeSuggestion<PKM>(nameof(PKM.HealPP), p => p.HealPP()),
        new TypeSuggestion<PKM>(nameof(IHyperTrain.HyperTrainFlags), p => p.SetSuggestedHyperTrainingData()),

        new TypeSuggestion<PKM>(nameof(PKM.Move1_PP), p => p.SetSuggestedMovePP(0)),
        new TypeSuggestion<PKM>(nameof(PKM.Move2_PP), p => p.SetSuggestedMovePP(1)),
        new TypeSuggestion<PKM>(nameof(PKM.Move3_PP), p => p.SetSuggestedMovePP(2)),
        new TypeSuggestion<PKM>(nameof(PKM.Move4_PP), p => p.SetSuggestedMovePP(3)),

        new ComplexSuggestion(nameof(PKM.Moves), (_, _, info) => BatchModifications.SetMoves(info.Entity, info.Legality.GetMoveSet())),
        new ComplexSuggestion(PROP_EVS, (_, _, info) => BatchModifications.SetEVs(info.Entity)),
        new ComplexSuggestion(nameof(PKM.RelearnMoves), (_, value, info) => BatchModifications.SetSuggestedRelearnData(info, value)),
        new ComplexSuggestion(PROP_RIBBONS, (_, value, info) => BatchModifications.SetSuggestedRibbons(info, value)),
        new ComplexSuggestion(nameof(PKM.Met_Location), (_, _, info) => BatchModifications.SetSuggestedMetData(info)),
        new ComplexSuggestion(nameof(PKM.CurrentLevel), (_, _, info) => BatchModifications.SetMinimumCurrentLevel(info)),
        new ComplexSuggestion(PROP_CONTESTSTATS, p => p is IContestStats, (_, value, info) => BatchModifications.SetContestStats(info.Entity, info.Legality, value)),
        new ComplexSuggestion(PROP_MOVEMASTERY, (_, value, info) => BatchModifications.SetSuggestedMasteryData(info, value)),
    };

    private static DateTime ParseDate(string val) => DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

    public static readonly List<IComplexSet> ComplexMods = new()
    {
        // Date
        new ComplexSet(nameof(PKM.MetDate), (pk, cmd) => pk.MetDate = ParseDate(cmd.PropertyValue)),
        new ComplexSet(nameof(PKM.EggMetDate), (pk, cmd) => pk.EggMetDate = ParseDate(cmd.PropertyValue)),

        // Value Swap
        new ComplexSet(nameof(PKM.EncryptionConstant), value => value == nameof(PKM.PID), (pk, _) => pk.EncryptionConstant = pk.PID),
        new ComplexSet(nameof(PKM.PID), value => value == nameof(PKM.EncryptionConstant), (pk, _) => pk.PID = pk.EncryptionConstant),

        // Realign to Derived Value
        new ComplexSet(nameof(PKM.Ability), value => value.Length == 2 && value.StartsWith(CONST_SPECIAL), (pk, cmd) => pk.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30)),
        new ComplexSet(nameof(PKM.AbilityNumber), value => value.Length == 2 && value.StartsWith(CONST_SPECIAL), (pk, cmd) => pk.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30)),

        // Random
        new ComplexSet(nameof(PKM.EncryptionConstant), value => value == CONST_RAND, (pk, _) => pk.EncryptionConstant = Util.Rand32()),
        new ComplexSet(nameof(PKM.PID), value => value == CONST_RAND, (pk, _) => pk.PID = Util.Rand32()),
        new ComplexSet(nameof(PKM.Gender), value => value == CONST_RAND, (pk, _) => pk.SetPIDGender(pk.Gender)),
        new ComplexSet(PROP_EVS, value => value == CONST_RAND, (pk, _) => SetRandomEVs(pk)),

        // Shiny
        new ComplexSet(nameof(PKM.PID),
            value => value.StartsWith(CONST_SHINY, true, CultureInfo.CurrentCulture),
            (pk, cmd) => CommonEdits.SetShiny(pk, GetRequestedShinyState(cmd.PropertyValue))),

        new ComplexSet(nameof(PKM.Species), value => value == "0", (pk, _) => Array.Clear(pk.Data, 0, pk.Data.Length)),
        new ComplexSet(nameof(PKM.IsNicknamed), value => string.Equals(value, "false", StringComparison.OrdinalIgnoreCase), (pk, _) => pk.SetDefaultNickname()),

        // Complicated
        new ComplexSet(nameof(PKM.EncryptionConstant), value => value.StartsWith(CONST_RAND), (pk, cmd) => SetComplicatedEC(pk, cmd.PropertyValue[^1])),
    };

    private static void SetComplicatedEC(PKM pk, char option)
    {
        var rng = Util.Rand;
        uint rand = rng.Rand32();
        uint mod = 1, noise = 0;
        if (pk.Species is >= (int)Species.Wurmple and <= (int)Species.Dustox)
        {
            mod = 10;
            bool lower = option is '0' or 'B' or 'S' || WurmpleUtil.GetWurmpleEvoGroup(pk.Species) == 0;
            noise = (lower ? 0u : 5u) + (uint)rng.Next(0, 5);
        }
        else if (pk.Species is (int)Species.Dunsparce or (int)Species.Dudunsparce or (int)Species.Tandemaus or (int)Species.Maushold)
        {
            mod = 100;
            noise = option is '0' or '3' ? 0u : (uint)rng.Next(1, 100);
        }
        else if (option is >= '0' and <= '5')
        {
            mod = 6;
            noise = (uint)(option - '0');
        }
        pk.EncryptionConstant = unchecked(rand - (rand % mod) + noise);
    }

    private static void SetRandomEVs(PKM pk)
    {
        Span<int> evs = stackalloc int[6];
        EffortValues.SetRandom(evs, pk.Format);
        pk.SetEVs(evs);
    }

    private static Shiny GetRequestedShinyState(string text) => text.Length == 0 ? Shiny.Random : GetRequestedShinyState(text[^1]);

    private static Shiny GetRequestedShinyState(char last) => last switch
    {
        '0' => Shiny.AlwaysSquare,
        '1' => Shiny.AlwaysStar,
        _ => Shiny.Random,
    };
}
