using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using static PKHeX.Core.MessageStrings;
using static PKHeX.Core.BatchModifications;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for editing many <see cref="PKM"/> with user provided <see cref="StringInstruction"/> list.
    /// </summary>
    public static class BatchEditing
    {
        public static readonly Type[] Types =
        {
            typeof (PK8),
            typeof (PB7),
            typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof(BK4),
            typeof (PK3), typeof (XK3), typeof (CK3),
            typeof (PK2), typeof (SK2), typeof (PK1),
        };

        public static readonly List<string> CustomProperties = new() { PROP_LEGAL, PROP_TYPENAME, PROP_RIBBONS };
        public static readonly string[][] Properties = GetPropArray();

        private static readonly Dictionary<string, PropertyInfo>[] Props = Types.Select(z => ReflectUtil.GetAllPropertyInfoPublic(z)
                .GroupBy(p => p.Name).Select(g => g.First()).ToDictionary(p => p.Name))
                .ToArray();

        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";
        private const string CONST_SUGGEST = "$suggest";
        private const string CONST_BYTES = "$[]";

        private const string PROP_LEGAL = "Legal";
        private const string PROP_TYPENAME = "ObjectType";
        private const string PROP_RIBBONS = "Ribbons";
        private const string IdentifierContains = nameof(PKM.Identifier) + "Contains";

        private static string[][] GetPropArray()
        {
            var p = new string[Types.Length][];
            for (int i = 0; i < p.Length; i++)
            {
                var pz = ReflectUtil.GetPropertiesPublic(Types[i]);
                p[i] = pz.Concat(CustomProperties).OrderBy(a => a).ToArray();
            }

            // Properties for any PKM
            var any = ReflectUtil.GetPropertiesPublic(typeof(PK1)).Union(p.SelectMany(a => a)).OrderBy(a => a).ToArray();
            // Properties shared by all PKM
            var all = p.Aggregate(new HashSet<string>(p[0]), (h, e) => { h.IntersectWith(e); return h; }).OrderBy(a => a).ToArray();

            var p1 = new string[Types.Length + 2][];
            Array.Copy(p, 0, p1, 1, p.Length);
            p1[0] = any;
            p1[^1] = all;

            return p1;
        }

        /// <summary>
        /// Tries to fetch the <see cref="PKM"/> property from the cache of available properties.
        /// </summary>
        /// <param name="pk">Pokémon to check</param>
        /// <param name="name">Property Name to check</param>
        /// <param name="pi">Property Info retrieved (if any).</param>
        /// <returns>True if has property, false if does not.</returns>
        public static bool TryGetHasProperty(PKM pk, string name, [NotNullWhen(true)] out PropertyInfo? pi)
        {
            var type = pk.GetType();
            return TryGetHasProperty(type, name, out pi);
        }

        /// <summary>
        /// Tries to fetch the <see cref="PKM"/> property from the cache of available properties.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <param name="name">Property Name to check</param>
        /// <param name="pi">Property Info retrieved (if any).</param>
        /// <returns>True if has property, false if does not.</returns>
        public static bool TryGetHasProperty(Type type, string name, [NotNullWhen(true)] out PropertyInfo? pi)
        {
            var index = Array.IndexOf(Types, type);
            if (index < 0)
            {
                pi = null;
                return false;
            }
            var props = Props[index];
            return props.TryGetValue(name, out pi);
        }

        /// <summary>
        /// Gets a list of <see cref="PKM"/> types that implement the requested <see cref="property"/>.
        /// </summary>
        public static IEnumerable<string> GetTypesImplementing(string property)
        {
            for (int i = 0; i < Types.Length; i++)
            {
                var type = Types[i];
                var props = Props[i];
                if (!props.TryGetValue(property, out var pi))
                    continue;
                yield return $"{type.Name}: {pi.PropertyType.Name}";
            }
        }

        /// <summary>
        /// Gets the type of the <see cref="PKM"/> property using the saved cache of properties.
        /// </summary>
        /// <param name="propertyName">Property Name to fetch the type for</param>
        /// <param name="typeIndex">Type index (within <see cref="Types"/>. Leave empty (0) for a nonspecific format.</param>
        /// <returns>Short name of the property's type.</returns>
        public static string? GetPropertyType(string propertyName, int typeIndex = 0)
        {
            if (CustomProperties.Contains(propertyName))
                return "Custom";

            if (typeIndex == 0) // Any
            {
                foreach (var p in Props)
                {
                    if (p.TryGetValue(propertyName, out var pi))
                        return pi.PropertyType.Name;
                }
                return null;
            }

            int index = typeIndex - 1 >= Props.Length ? 0 : typeIndex - 1; // All vs Specific
            var pr = Props[index];
            if (!pr.TryGetValue(propertyName, out var info))
                return null;
            return info.PropertyType.Name;
        }

        /// <summary>
        /// Initializes the <see cref="StringInstruction"/> list with a context-sensitive value. If the provided value is a string, it will attempt to convert that string to its corresponding index.
        /// </summary>
        /// <param name="il">Instructions to initialize.</param>
        public static void ScreenStrings(IEnumerable<StringInstruction> il)
        {
            foreach (var i in il.Where(i => !i.PropertyValue.All(char.IsDigit)))
            {
                string pv = i.PropertyValue;
                if (pv.StartsWith("$") && !pv.StartsWith(CONST_BYTES) && pv.Contains(','))
                    i.SetRandRange(pv);

                SetInstructionScreenedValue(i);
            }
        }

        /// <summary>
        /// Initializes the <see cref="StringInstruction"/> with a context-sensitive value. If the provided value is a string, it will attempt to convert that string to its corresponding index.
        /// </summary>
        /// <param name="i">Instruction to initialize.</param>
        private static void SetInstructionScreenedValue(StringInstruction i)
        {
            switch (i.PropertyName)
            {
                case nameof(PKM.Species): i.SetScreenedValue(GameInfo.Strings.specieslist); return;
                case nameof(PKM.HeldItem): i.SetScreenedValue(GameInfo.Strings.itemlist); return;
                case nameof(PKM.Ability): i.SetScreenedValue(GameInfo.Strings.abilitylist); return;
                case nameof(PKM.Nature): i.SetScreenedValue(GameInfo.Strings.natures); return;
                case nameof(PKM.Ball): i.SetScreenedValue(GameInfo.Strings.balllist); return;

                case nameof(PKM.Move1) or nameof(PKM.Move2) or nameof(PKM.Move3) or nameof(PKM.Move4):
                case nameof(PKM.RelearnMove1) or nameof(PKM.RelearnMove2) or nameof(PKM.RelearnMove3) or nameof(PKM.RelearnMove4):
                    i.SetScreenedValue(GameInfo.Strings.movelist); return;
            }
        }

        /// <summary>
        /// Checks if the object is filtered by the provided <see cref="filters"/>.
        /// </summary>
        /// <param name="filters">Filters which must be satisfied.</param>
        /// <param name="pk">Object to check.</param>
        /// <returns>True if <see cref="pk"/> matches all filters.</returns>
        public static bool IsFilterMatch(IEnumerable<StringInstruction> filters, PKM pk) => filters.All(z => IsFilterMatch(z, pk, Props[Array.IndexOf(Types, pk.GetType())]));

        /// <summary>
        /// Checks if the object is filtered by the provided <see cref="filters"/>.
        /// </summary>
        /// <param name="filters">Filters which must be satisfied.</param>
        /// <param name="obj">Object to check.</param>
        /// <returns>True if <see cref="obj"/> matches all filters.</returns>
        public static bool IsFilterMatch(IEnumerable<StringInstruction> filters, object obj)
        {
            foreach (var cmd in filters)
            {
                if (cmd.PropertyName is PROP_TYPENAME)
                {
                    if ((obj.GetType().Name == cmd.PropertyValue) != cmd.Evaluator)
                        return false;
                    continue;
                }

                if (!ReflectUtil.HasProperty(obj, cmd.PropertyName, out var pi))
                    return false;
                try
                {
                    if (pi.IsValueEqual(obj, cmd.PropertyValue) == cmd.Evaluator)
                        continue;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                // User provided inputs can mismatch the type's required value format, and fail to be compared.
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.WriteLine($"Unable to compare {cmd.PropertyName} to {cmd.PropertyValue}.");
                    Debug.WriteLine(e.Message);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to modify the <see cref="PKM"/>.
        /// </summary>
        /// <param name="pk">Object to modify.</param>
        /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
        /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
        /// <returns>Result of the attempted modification.</returns>
        public static bool TryModify(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
        {
            var result = TryModifyPKM(pk, filters, modifications);
            return result == ModifyResult.Modified;
        }

        /// <summary>
        /// Tries to modify the <see cref="BatchInfo"/>.
        /// </summary>
        /// <param name="pk">Command Filter</param>
        /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
        /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
        /// <returns>Result of the attempted modification.</returns>
        internal static ModifyResult TryModifyPKM(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
        {
            if (!pk.ChecksumValid || pk.Species == 0)
                return ModifyResult.Invalid;

            var info = new BatchInfo(pk);
            var pi = Props[Array.IndexOf(Types, pk.GetType())];
            foreach (var cmd in filters)
            {
                try
                {
                    if (!IsFilterMatch(cmd, info, pi))
                        return ModifyResult.Filtered;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                // Swallow any error because this can be malformed user input.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.WriteLine(MsgBEModifyFailCompare + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue);
                    return ModifyResult.Error;
                }
            }

            ModifyResult result = ModifyResult.Modified;
            foreach (var cmd in modifications)
            {
                try
                {
                    var tmp = SetPKMProperty(cmd, info, pi);
                    if (tmp != ModifyResult.Modified)
                        result = tmp;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                // Swallow any error because this can be malformed user input.
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.WriteLine(MsgBEModifyFail + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Sets the if the <see cref="BatchInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="info">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache (optional)</param>
        /// <returns>True if filtered, else false.</returns>
        private static ModifyResult SetPKMProperty(StringInstruction cmd, BatchInfo info, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            var pk = info.Entity;
            if (cmd.PropertyValue.StartsWith(CONST_BYTES))
                return SetByteArrayProperty(pk, cmd);

            if (cmd.PropertyValue.StartsWith(CONST_SUGGEST, true, CultureInfo.CurrentCulture))
                return SetSuggestedPKMProperty(cmd.PropertyName, info, cmd.PropertyValue);
            if (cmd.PropertyValue == CONST_RAND && cmd.PropertyName == nameof(PKM.Moves))
                return SetMoves(pk, info.Legality.GetMoveSet(true));

            if (SetComplexProperty(pk, cmd))
                return ModifyResult.Modified;

            if (!props.TryGetValue(cmd.PropertyName, out var pi))
                return ModifyResult.Error;

            if (!pi.CanWrite)
                return ModifyResult.Error;

            object val = cmd.Random ? cmd.RandomValue : cmd.PropertyValue;
            ReflectUtil.SetValue(pi, pk, val);
            return ModifyResult.Modified;
        }

        /// <summary>
        /// Checks if the <see cref="BatchInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="info">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache (optional)</param>
        /// <returns>True if filter matches, else false.</returns>
        private static bool IsFilterMatch(StringInstruction cmd, BatchInfo info, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            var match = FilterMods.Find(z => z.IsMatch(cmd.PropertyName));
            if (match != null)
                return match.IsFiltered(info, cmd);
            return IsPropertyFiltered(cmd, info.Entity, props);
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="pk">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache (optional)</param>
        /// <returns>True if filter matches, else false.</returns>
        private static bool IsFilterMatch(StringInstruction cmd, PKM pk, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            var match = FilterMods.Find(z => z.IsMatch(cmd.PropertyName));
            if (match != null)
                return match.IsFiltered(pk, cmd);
            return IsPropertyFiltered(cmd, pk, props);
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="pk">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache</param>
        /// <returns>True if filtered, else false.</returns>
        private static bool IsPropertyFiltered(StringInstruction cmd, PKM pk, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            if (!props.TryGetValue(cmd.PropertyName, out var pi))
                return false;
            if (!pi.CanRead)
                return false;
            return pi.IsValueEqual(pk, cmd.PropertyValue) == cmd.Evaluator;
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> data with a suggested value based on its <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="name">Property to modify.</param>
        /// <param name="info">Cached info storing Legal data.</param>
        /// <param name="propValue">Suggestion string which starts with <see cref="CONST_SUGGEST"/></param>
        private static ModifyResult SetSuggestedPKMProperty(string name, BatchInfo info, string propValue)
        {
            var first = SuggestionMods.Find(z => z.IsMatch(name, propValue, info));
            if (first != null)
                return first.Modify(name, propValue, info);
            return ModifyResult.Error;
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> byte array property to a specified value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="cmd">Modification</param>
        private static ModifyResult SetByteArrayProperty(PKM pk, StringInstruction cmd)
        {
            switch (cmd.PropertyName)
            {
                case nameof(PKM.Nickname_Trash):
                    pk.Nickname_Trash = ConvertToBytes(cmd.PropertyValue);
                    return ModifyResult.Modified;
                case nameof(PKM.OT_Trash):
                    pk.OT_Trash = ConvertToBytes(cmd.PropertyValue);
                    return ModifyResult.Modified;
                default:
                    return ModifyResult.Error;
            }
            static byte[] ConvertToBytes(string str) => str[CONST_BYTES.Length..].Split(',').Select(z => Convert.ToByte(z.Trim(), 16)).ToArray();
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> property to a non-specific smart value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="cmd">Modification</param>
        /// <returns>True if modified, false if no modifications done.</returns>
        private static bool SetComplexProperty(PKM pk, StringInstruction cmd)
        {
            if (cmd.PropertyName.StartsWith("IV") && cmd.PropertyValue == CONST_RAND)
            {
                SetRandomIVs(pk, cmd);
                return true;
            }

            var match = ComplexMods.Find(z => z.IsMatch(cmd.PropertyName, cmd.PropertyValue));
            if (match == null)
                return false;

            match.Modify(pk, cmd);
            return true;
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> IV(s) to a random value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="cmd">Modification</param>
        private static void SetRandomIVs(PKM pk, StringInstruction cmd)
        {
            if (cmd.PropertyName == nameof(PKM.IVs))
            {
                pk.SetRandomIVs();
                return;
            }

            if (TryGetHasProperty(pk, cmd.PropertyName, out var pi))
                ReflectUtil.SetValue(pi, pk, Util.Rand.Next(pk.MaxIV + 1));
        }

        public static readonly List<IComplexFilter> FilterMods = new()
        {
            new ComplexFilter(PROP_LEGAL,
                (pkm, cmd) => new LegalityAnalysis(pkm).Valid == cmd.Evaluator,
                (info, cmd) => info.Legality.Valid == cmd.Evaluator),

            new ComplexFilter(PROP_TYPENAME,
                (pkm, cmd) => (pkm.GetType().Name == cmd.PropertyValue) == cmd.Evaluator,
                (info, cmd) => (info.Entity.GetType().Name == cmd.PropertyValue) == cmd.Evaluator),

            new ComplexFilter(IdentifierContains,
                (pkm, cmd) => pkm.Identifier?.Contains(cmd.PropertyValue) == cmd.Evaluator,
                (info, cmd) => info.Entity.Identifier?.Contains(cmd.PropertyValue) == cmd.Evaluator),
        };

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

            new ComplexSuggestion(nameof(PKM.Moves), (_, _, info) => SetMoves(info.Entity, info.Legality.GetMoveSet())),
            new ComplexSuggestion(nameof(PKM.RelearnMoves), (_, value, info) => SetSuggestedRelearnData(info, value)),
            new ComplexSuggestion(PROP_RIBBONS, (_, value, info) => SetSuggestedRibbons(info, value)),
            new ComplexSuggestion(nameof(PKM.Met_Location), (_, _, info) => SetSuggestedMetData(info)),
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
                (pk, cmd) =>
                CommonEdits.SetShiny(pk, cmd.PropertyValue.EndsWith("0") ? Shiny.AlwaysSquare : cmd.PropertyValue.EndsWith("1") ? Shiny.AlwaysStar : Shiny.Random)),

            new ComplexSet(nameof(PKM.Species), value => value == "0", (pk, _) => Array.Clear(pk.Data, 0, pk.Data.Length)),
            new ComplexSet(nameof(PKM.IsNicknamed), value => string.Equals(value, "false", StringComparison.OrdinalIgnoreCase), (pk, _) => pk.SetDefaultNickname()),
        };
    }
}
