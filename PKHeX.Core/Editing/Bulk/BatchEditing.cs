using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using static PKHeX.Core.MessageStrings;

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
            typeof (PK2), typeof (PK1),
        };

        public static readonly string[] CustomProperties = { PROP_LEGAL, PROP_RIBBONS };
        public static readonly string[][] Properties = GetPropArray();

        private static readonly Dictionary<string, PropertyInfo>[] Props = Types.Select(z => ReflectUtil.GetAllPropertyInfoPublic(z)
                .GroupBy(p => p.Name).Select(g => g.First()).ToDictionary(p => p.Name))
                .ToArray();

        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";
        private const string CONST_SUGGEST = "$suggest";
        private const string CONST_BYTES = "$[]";

        private const string PROP_LEGAL = "Legal";
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
            p1[p1.Length - 1] = all;

            return p1;
        }

        /// <summary>
        /// Tries to fetch the <see cref="PKM"/> property from the cache of available properties.
        /// </summary>
        /// <param name="pk">Pokémon to check</param>
        /// <param name="name">Property Name to check</param>
        /// <param name="pi">Property Info retrieved (if any).</param>
        /// <returns>True if has property, false if does not.</returns>
        public static bool TryGetHasProperty(PKM pk, string name, out PropertyInfo pi)
        {
            var props = Props[Array.IndexOf(Types, pk.GetType())];
            return props.TryGetValue(name, out pi);
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
                case nameof(PKM.Move1):
                case nameof(PKM.Move2):
                case nameof(PKM.Move3):
                case nameof(PKM.Move4):
                case nameof(PKM.RelearnMove1):
                case nameof(PKM.RelearnMove2):
                case nameof(PKM.RelearnMove3):
                case nameof(PKM.RelearnMove4):
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
                if (!ReflectUtil.HasProperty(obj, cmd.PropertyName, out var pi))
                    return false;
                try
                {
                    if (pi == null)
                        continue;
                    if (pi.IsValueEqual(obj, cmd.PropertyValue) == cmd.Evaluator)
                        continue;
                }
                catch (Exception e)
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
        /// Tries to modify the <see cref="PKMInfo"/>.
        /// </summary>
        /// <param name="pk">Command Filter</param>
        /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
        /// <param name="modifications">Modifications to perform on the <see cref="pk"/>.</param>
        /// <returns>Result of the attempted modification.</returns>
        internal static ModifyResult TryModifyPKM(PKM pk, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
        {
            if (!pk.ChecksumValid || pk.Species == 0)
                return ModifyResult.Invalid;

            PKMInfo info = new PKMInfo(pk);
            var pi = Props[Array.IndexOf(Types, pk.GetType())];
            foreach (var cmd in filters)
            {
                try
                {
                    if (!IsFilterMatch(cmd, info, pi))
                        return ModifyResult.Filtered;
                }
                catch (Exception ex)
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
                catch (Exception ex) { Debug.WriteLine(MsgBEModifyFail + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue); }
            }
            return result;
        }

        /// <summary>
        /// Sets the if the <see cref="PKMInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="info">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache (optional)</param>
        /// <returns>True if filtered, else false.</returns>
        private static ModifyResult SetPKMProperty(StringInstruction cmd, PKMInfo info, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            var pk = info.Entity;
            if (cmd.PropertyValue.StartsWith(CONST_BYTES))
                return SetByteArrayProperty(pk, cmd);

            if (cmd.PropertyValue.StartsWith(CONST_SUGGEST))
                return SetSuggestedPKMProperty(cmd.PropertyName, info, cmd.PropertyValue);
            if (cmd.PropertyValue == CONST_RAND && cmd.PropertyName == nameof(PKM.Moves))
                return SetMoves(pk, pk.GetMoveSet(info.Legality, true));

            if (SetComplexProperty(pk, cmd))
                return ModifyResult.Modified;

            if (!props.TryGetValue(cmd.PropertyName, out var pi))
                return ModifyResult.Error;

            if (!pi.CanWrite)
                return ModifyResult.Error;

            object val = cmd.Random ? (object)cmd.RandomValue : cmd.PropertyValue;
            ReflectUtil.SetValue(pi, pk, val);
            return ModifyResult.Modified;
        }

        /// <summary>
        /// Checks if the <see cref="PKMInfo"/> should be filtered due to the <see cref="StringInstruction"/> provided.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="info">Pokémon to check.</param>
        /// <param name="props">PropertyInfo cache (optional)</param>
        /// <returns>True if filter matches, else false.</returns>
        private static bool IsFilterMatch(StringInstruction cmd, PKMInfo info, IReadOnlyDictionary<string, PropertyInfo> props)
        {
            if (IsLegalFiltered(cmd, () => info.Legal))
                return true;
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
            if (IsLegalFiltered(cmd, () => new LegalityAnalysis(pk).Valid))
                return true;
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
            if (IsIdentifierFiltered(cmd, pk))
                return true;
            if (!props.TryGetValue(cmd.PropertyName, out var pi))
                return false;
            if (!pi.CanRead)
                return false;
            return pi.IsValueEqual(pk, cmd.PropertyValue) == cmd.Evaluator;
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> should be filtered due to its <see cref="PKM.Identifier"/> containing a value.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="pk">Pokémon to check.</param>
        /// <returns>True if filtered, else false.</returns>
        private static bool IsIdentifierFiltered(StringInstruction cmd, PKM pk)
        {
            if (cmd.PropertyName != IdentifierContains)
                return false;

            bool result = pk.Identifier?.Contains(cmd.PropertyValue) ?? false;
            return result == cmd.Evaluator;
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> should be filtered due to its legality.
        /// </summary>
        /// <param name="cmd">Command Filter</param>
        /// <param name="isLegal">Function to check if the <see cref="PKM"/> is legal.</param>
        /// <returns>True if filtered, else false.</returns>
        private static bool IsLegalFiltered(StringInstruction cmd, Func<bool> isLegal)
        {
            if (cmd.PropertyName != PROP_LEGAL)
                return false;

            if (!bool.TryParse(cmd.PropertyValue, out bool legal))
                return true;
            return legal == isLegal() == cmd.Evaluator;
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> data with a suggested value based on its <see cref="LegalityAnalysis"/>.
        /// </summary>
        /// <param name="name">Property to modify.</param>
        /// <param name="info">Cached info storing Legal data.</param>
        /// <param name="propValue">Suggestion string which starts with <see cref="CONST_SUGGEST"/></param>
        private static ModifyResult SetSuggestedPKMProperty(string name, PKMInfo info, string propValue)
        {
            bool isAll() => propValue.EndsWith("All", true, CultureInfo.CurrentCulture);
            bool isNone() => propValue.EndsWith("None", true, CultureInfo.CurrentCulture);
            var pk = info.Entity;
            switch (name)
            {
                // pb7 only
                case nameof(PB7.Stat_CP) when pk is PB7 pb7:
                    pb7.ResetCP();
                    return ModifyResult.Modified;
                case nameof(PB7.HeightAbsolute) when pk is PB7 pb7:
                    pb7.HeightAbsolute = pb7.CalcHeightAbsolute;
                    return ModifyResult.Modified;
                case nameof(PB7.WeightAbsolute) when pk is PB7 pb7:
                    pb7.WeightAbsolute = pb7.CalcWeightAbsolute;
                    return ModifyResult.Modified;

                case nameof(PKM.Nature) when pk.Format >= 8:
                    pk.Nature = pk.StatNature;
                    return ModifyResult.Modified;
                case nameof(PKM.StatNature) when pk.Format >= 8:
                    pk.StatNature = pk.Nature;
                    return ModifyResult.Modified;
                case nameof(PKM.Stats):
                    pk.ResetPartyStats();
                    return ModifyResult.Modified;
                case nameof(IHyperTrain.HyperTrainFlags):
                    pk.SetSuggestedHyperTrainingData();
                    return ModifyResult.Modified;
                case nameof(PKM.RelearnMoves):
                    if (pk.Format >= 8)
                    {
                        pk.ClearRecordFlags();
                        if (isAll())
                            pk.SetRecordFlags(); // all
                        else if (!isNone())
                            pk.SetRecordFlags(pk.Moves); // whatever fit the current moves
                    }
                    pk.SetRelearnMoves(info.SuggestedRelearn);
                    return ModifyResult.Modified;
                case PROP_RIBBONS:
                    if (isNone())
                        RibbonApplicator.RemoveAllValidRibbons(pk);
                    else // All
                        RibbonApplicator.SetAllValidRibbons(pk);
                    return ModifyResult.Modified;
                case nameof(PKM.Met_Location):
                    var encounter = EncounterSuggestion.GetSuggestedMetInfo(pk);
                    if (encounter == null)
                        return ModifyResult.Error;

                    int level = encounter.LevelMin;
                    int location = encounter.Location;
                    int minlvl = EncounterSuggestion.GetLowestLevel(pk, encounter.LevelMin);

                    pk.Met_Level = level;
                    pk.Met_Location = location;
                    pk.CurrentLevel = Math.Max(minlvl, level);

                    return ModifyResult.Modified;

                case nameof(PKM.Heal):
                    pk.Heal();
                    return ModifyResult.Modified;
                case nameof(PKM.HealPP):
                    pk.HealPP();
                    return ModifyResult.Modified;

                case nameof(PKM.Move1_PP):
                case nameof(PKM.Move2_PP):
                case nameof(PKM.Move3_PP):
                case nameof(PKM.Move4_PP):
                    pk.SetSuggestedMovePP(name[4] - '1'); // 0-3 int32
                    return ModifyResult.Modified;

                case nameof(PKM.Moves):
                    return SetMoves(pk, pk.GetMoveSet(la: info.Legality));

                case nameof(PKM.Ball):
                    BallApplicator.ApplyBallLegalByColor(pk);
                    return ModifyResult.Modified;

                default:
                    return ModifyResult.Error;
            }
        }

        /// <summary>
        /// Sets the provided moves in a random order.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves">Moves to apply.</param>
        private static ModifyResult SetMoves(PKM pk, int[] moves)
        {
            pk.SetMoves(moves);
            return ModifyResult.Modified;
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> byte array propery to a specified value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="cmd">Modification</param>
        private static ModifyResult SetByteArrayProperty(PKM pk, StringInstruction cmd)
        {
            switch (cmd.PropertyName)
            {
                case nameof(PKM.Nickname_Trash):
                    pk.Nickname_Trash = string2arr(cmd.PropertyValue);
                    return ModifyResult.Modified;
                case nameof(PKM.OT_Trash):
                    pk.OT_Trash = string2arr(cmd.PropertyValue);
                    return ModifyResult.Modified;
                default:
                    return ModifyResult.Error;
            }
            static byte[] string2arr(string str) => str.Substring(CONST_BYTES.Length).Split(',').Select(z => Convert.ToByte(z.Trim(), 16)).ToArray();
        }

        /// <summary>
        /// Sets the <see cref="PKM"/> property to a non-specific smart value.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="cmd">Modification</param>
        /// <returns>True if modified, false if no modifications done.</returns>
        private static bool SetComplexProperty(PKM pk, StringInstruction cmd)
        {
            static DateTime parseDate(string val) => DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (cmd.PropertyName == nameof(PKM.MetDate))
                pk.MetDate = parseDate(cmd.PropertyValue);
            else if (cmd.PropertyName == nameof(PKM.EggMetDate))
                pk.EggMetDate = parseDate(cmd.PropertyValue);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == CONST_RAND)
               pk.EncryptionConstant = Util.Rand32();
            else if ((cmd.PropertyName == nameof(PKM.Ability) || cmd.PropertyName == nameof(PKM.AbilityNumber)) && cmd.PropertyValue.StartsWith("$"))
                pk.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30);
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_RAND)
                pk.SetPIDGender(pk.Gender);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == nameof(PKM.PID))
                pk.EncryptionConstant = pk.PID;
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue.StartsWith(CONST_SHINY))
                CommonEdits.SetShiny(pk, cmd.PropertyValue.EndsWith("0"));
            else if (cmd.PropertyName == nameof(PKM.Species) && cmd.PropertyValue == "0")
                Array.Clear(pk.Data, 0, pk.Data.Length);
            else if (cmd.PropertyName.StartsWith("IV") && cmd.PropertyValue == CONST_RAND)
                SetRandomIVs(pk, cmd);
            else if (cmd.PropertyName == nameof(PKM.IsNicknamed) && string.Equals(cmd.PropertyValue, "false", StringComparison.OrdinalIgnoreCase))
                pk.SetDefaultNickname();
            else
                return false;

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
    }
}
