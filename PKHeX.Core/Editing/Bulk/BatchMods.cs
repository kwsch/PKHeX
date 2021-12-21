using System;
using System.Collections.Generic;
using System.Globalization;
using static PKHeX.Core.BatchEditing;

namespace PKHeX.Core
{
    public static class BatchMods
    {
        public static readonly List<ISuggestModification> SuggestionMods = new()
        {
            // PB7 Specific
            new TypeSuggestion<PB7>(nameof(PB7.Stat_CP), p => p.ResetCP()),
            new TypeSuggestion<PB7>(nameof(PB7.HeightAbsolute), p => p.HeightAbsolute = p.CalcHeightAbsolute),
            new TypeSuggestion<PB7>(nameof(PB7.WeightAbsolute), p => p.WeightAbsolute = p.CalcWeightAbsolute),

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
            new ComplexSuggestion(nameof(PKM.RelearnMoves), (_, value, info) => BatchModifications.SetSuggestedRelearnData(info, value)),
            new ComplexSuggestion(PROP_RIBBONS, (_, value, info) => BatchModifications.SetSuggestedRibbons(info, value)),
            new ComplexSuggestion(nameof(PKM.Met_Location), (_, _, info) => BatchModifications.SetSuggestedMetData(info)),
            new ComplexSuggestion("ContestStats", p => p is IContestStatsMutable, (_, value, info) => BatchModifications.SetContestStats(info.Entity, info.Legality.EncounterMatch, value)),
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
            new ComplexSet(nameof(PKM.Ability), value => value.StartsWith("$"), (pk, cmd) => pk.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30)),
            new ComplexSet(nameof(PKM.AbilityNumber), value => value.StartsWith("$"), (pk, cmd) => pk.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30)),

            // Random
            new ComplexSet(nameof(PKM.EncryptionConstant), value => value == CONST_RAND, (pk, _) => pk.EncryptionConstant = Util.Rand32()),
            new ComplexSet(nameof(PKM.PID), value => value == CONST_RAND, (pk, _) => pk.PID = Util.Rand32()),
            new ComplexSet(nameof(PKM.Gender), value => value == CONST_RAND, (pk, _) => pk.SetPIDGender(pk.Gender)),

            // Shiny
            new ComplexSet(nameof(PKM.PID),
                value => value.StartsWith(CONST_SHINY, true, CultureInfo.CurrentCulture),
                (pk, cmd) => CommonEdits.SetShiny(pk, GetRequestedShinyState(cmd.PropertyValue))),

            new ComplexSet(nameof(PKM.Species), value => value == "0", (pk, _) => Array.Clear(pk.Data, 0, pk.Data.Length)),
            new ComplexSet(nameof(PKM.IsNicknamed), value => string.Equals(value, "false", StringComparison.OrdinalIgnoreCase), (pk, _) => pk.SetDefaultNickname()),
        };

        private static Shiny GetRequestedShinyState(string text)
        {
            if (text.EndsWith("0"))
                return Shiny.AlwaysSquare;
            if (text.EndsWith("1"))
                return Shiny.AlwaysStar;
            return Shiny.Random;
        }
    }
}