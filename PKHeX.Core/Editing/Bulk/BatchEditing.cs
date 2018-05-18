using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public static class BatchEditing
    {
        public static readonly Type[] Types =
        {
            typeof (PK7), typeof (PK6), typeof (PK5), typeof (PK4), typeof(BK4),
            typeof (PK3), typeof (XK3), typeof (CK3),
            typeof (PK2), typeof (PK1),
        };
        public static readonly string[] CustomProperties = { PROP_LEGAL };
        public static readonly string[][] Properties = GetPropArray();

        private static readonly Dictionary<string, PropertyInfo>[] Props = Types.Select(z => ReflectUtil.GetAllPropertyInfoCanWritePublic(z)
                .GroupBy(p => p.Name).Select(g => g.First()).ToDictionary(p => p.Name))
                .ToArray();

        private const string CONST_RAND = "$rand";
        private const string CONST_SHINY = "$shiny";
        private const string CONST_SUGGEST = "$suggest";
        private const string CONST_BYTES = "$[]";

        private const string PROP_LEGAL = "Legal";

        private static string[][] GetPropArray()
        {
            var p = new string[Types.Length][];
            for (int i = 0; i < p.Length; i++)
            {
                var pz = ReflectUtil.GetPropertiesCanWritePublic(Types[i]).Distinct();
                p[i] = pz.Concat(CustomProperties).OrderBy(a => a).ToArray();
            }

            // Properties for any PKM
            var any = ReflectUtil.GetPropertiesCanWritePublic(typeof(PK1)).Distinct().Union(p.SelectMany(a => a)).OrderBy(a => a).ToArray();
            // Properties shared by all PKM
            var all = p.Aggregate(new HashSet<string>(p[0]), (h, e) => { h.IntersectWith(e); return h; }).OrderBy(a => a).ToArray();

            var p1 = new string[Types.Length + 2][];
            Array.Copy(p, 0, p1, 1, p.Length);
            p1[0] = any;
            p1[p1.Length - 1] = all;

            return p1;
        }

        public static bool TryGetHasProperty(PKM pk, string name, out PropertyInfo pi)
        {
            var props = Props[Array.IndexOf(Types, pk.GetType())];
            return props.TryGetValue(name, out pi);
        }
        public static bool TryGetPropertyValue(PKM pk, string name, out object value)
        {
            if (TryGetHasProperty(pk, name, out var pi))
            {
                value = pi.GetValue(pk);
                return true;
            }
            value = null;
            return false;
        }

        public static string GetPropertyType(string propertyName, int typeIndex = 0)
        {
            if (CustomProperties.Contains(propertyName))
                return "Custom";

            if (typeIndex == 0) // Any
            {
                foreach (var p in Props)
                    if (p.TryGetValue(propertyName, out var pi))
                        return pi.PropertyType.Name;
                return null;
            }

            int index = typeIndex == Props.Length - 1 ? 0 : typeIndex - 1; // All vs Specific
            var pr = Props[typeIndex - 1];
            if (!pr.TryGetValue(propertyName, out var info))
                return null;
            return info.PropertyType.Name;
        }

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

        internal static ModifyResult TryModifyPKM(PKM PKM, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (!PKM.ChecksumValid || PKM.Species == 0)
                return ModifyResult.Invalid;

            PKMInfo info = new PKMInfo(PKM);
            var pi = Props[Array.IndexOf(Types, PKM.GetType())];
            foreach (var cmd in Filters)
            {
                try
                {
                    var filter = IsPKMFiltered(cmd, info, pi);
                    if (filter != ModifyResult.None)
                        return filter; // why it was filtered out
                }
                catch (Exception ex) { Debug.WriteLine(MsgBEModifyFailCompare + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue); }
            }

            ModifyResult result = ModifyResult.Modified;
            foreach (var cmd in Instructions)
            {
                try
                {
                    var tmp = SetPKMProperty(PKM, info, cmd, pi);
                    if (result != ModifyResult.Modified)
                        result = tmp;
                }
                catch (Exception ex) { Debug.WriteLine(MsgBEModifyFail + " " + ex.Message, cmd.PropertyName, cmd.PropertyValue); }
            }
            return result;
        }
        private static ModifyResult SetPKMProperty(PKM PKM, PKMInfo info, StringInstruction cmd, Dictionary<string, PropertyInfo> props)
        {
            if (cmd.PropertyValue.StartsWith(CONST_BYTES))
                return SetByteArrayProperty(PKM, cmd)
                    ? ModifyResult.Modified
                    : ModifyResult.Error;

            if (cmd.PropertyValue == CONST_SUGGEST)
                return SetSuggestedPKMProperty(cmd, info)
                    ? ModifyResult.Modified
                    : ModifyResult.Error;

            if (SetComplexProperty(PKM, cmd))
                return ModifyResult.Modified;

            object val = cmd.Random ? (object)cmd.RandomValue : cmd.PropertyValue;
            if (TryGetHasProperty(PKM, cmd.PropertyName, out var pi))
            {
                ReflectUtil.SetValue(pi, PKM, val);
                return ModifyResult.Modified;
            }
            return ModifyResult.Error;
        }
        private static ModifyResult IsPKMFiltered(StringInstruction cmd, PKMInfo info, Dictionary<string, PropertyInfo> props)
        {
            if (cmd.PropertyName == PROP_LEGAL)
            {
                if (!bool.TryParse(cmd.PropertyValue, out bool legal))
                    return ModifyResult.Error;
                if (legal == info.Legal == cmd.Evaluator)
                    return ModifyResult.None;
                return ModifyResult.Filtered;
            }

            if (!props.TryGetValue(cmd.PropertyName, out var pi))
                return ModifyResult.Filtered;
            if (pi.IsValueEqual(info.pkm, cmd.PropertyValue) != cmd.Evaluator)
                return ModifyResult.Filtered;
            return ModifyResult.None;
        }
        private static bool SetSuggestedPKMProperty(StringInstruction cmd, PKMInfo info)
        {
            var PKM = info.pkm;
            switch (cmd.PropertyName)
            {
                case nameof(PKM.HyperTrainFlags):
                    PKM.HyperTrainFlags = GetSuggestedHyperTrainingStatus(PKM);
                    return true;
                case nameof(PKM.RelearnMoves):
                    PKM.RelearnMoves = info.SuggestedRelearn;
                    return true;
                case nameof(PKM.Met_Location):
                    var encounter = info.SuggestedEncounter;
                    if (encounter == null)
                        return false;

                    int level = encounter.Level;
                    int location = encounter.Location;
                    int minlvl = Legal.GetLowestLevel(PKM, encounter.LevelMin);

                    PKM.Met_Level = level;
                    PKM.Met_Location = location;
                    PKM.CurrentLevel = Math.Max(minlvl, level);

                    return true;

                case nameof(PKM.Moves):
                    var moves = info.SuggestedMoves;
                    Util.Shuffle(moves);
                    PKM.SetMoves(moves);
                    return true;

                default:
                    return false;
            }
        }

        private static int GetSuggestedHyperTrainingStatus(PKM pkm)
        {
            if (pkm.Format < 7 || pkm.CurrentLevel != 100)
                return 0;

            int val = 0;
            if (pkm.IV_HP != 31)
                val |= 1 << 0;
            if (pkm.IV_ATK < 31 && pkm.IV_ATK > 1)
                val |= 1 << 1;
            if (pkm.IV_DEF != 31)
                val |= 1 << 2;
            if (pkm.IV_SPE < 31 && pkm.IV_SPE > 1)
                val |= 1 << 3;
            if (pkm.IV_SPA != 31)
                val |= 1 << 4;
            if (pkm.IV_SPD != 31)
                val |= 1 << 5;
            return val;
        }

        private static bool SetByteArrayProperty(PKM PKM, StringInstruction cmd)
        {
            switch (cmd.PropertyName)
            {
                case nameof(PKM.Nickname_Trash):
                    PKM.Nickname_Trash = string2arr(cmd.PropertyValue);
                    return true;
                case nameof(PKM.OT_Trash):
                    PKM.OT_Trash = string2arr(cmd.PropertyValue);
                    return true;
                default:
                    return false;
            }
            byte[] string2arr(string str) => str.Substring(CONST_BYTES.Length).Split(',').Select(z => Convert.ToByte(z.Trim(), 16)).ToArray();
        }
        private static bool SetComplexProperty(PKM PKM, StringInstruction cmd)
        {
            if (cmd.PropertyName == nameof(PKM.MetDate))
                PKM.MetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            else if (cmd.PropertyName == nameof(PKM.EggMetDate))
                PKM.EggMetDate = DateTime.ParseExact(cmd.PropertyValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == CONST_RAND)
               PKM.EncryptionConstant = Util.Rand32();
            else if ((cmd.PropertyName == nameof(PKM.Ability) || cmd.PropertyName == nameof(PKM.AbilityNumber)) && cmd.PropertyValue.StartsWith("$"))
                PKM.RefreshAbility(Convert.ToInt16(cmd.PropertyValue[1]) - 0x30);
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_RAND)
                PKM.SetPIDGender(PKM.Gender);
            else if (cmd.PropertyName == nameof(PKM.EncryptionConstant) && cmd.PropertyValue == nameof(PKM.PID))
                PKM.EncryptionConstant = PKM.PID;
            else if (cmd.PropertyName == nameof(PKM.PID) && cmd.PropertyValue == CONST_SHINY)
                PKM.SetShinyPID();
            else if (cmd.PropertyName == nameof(PKM.Species) && cmd.PropertyValue == "0")
                PKM.Data = new byte[PKM.Data.Length];
            else if (cmd.PropertyName.StartsWith("IV") && cmd.PropertyValue == CONST_RAND)
                SetRandomIVs(PKM, cmd);
            else if (cmd.PropertyName == nameof(PKM.IsNicknamed) && string.Equals(cmd.PropertyValue, "false", StringComparison.OrdinalIgnoreCase))
            { PKM.IsNicknamed = false; PKM.Nickname = PKX.GetSpeciesNameGeneration(PKM.Species, PKM.Language, PKM.Format); }
            else
                return false;

            return true;
        }
        private static void SetRandomIVs(PKM PKM, StringInstruction cmd)
        {
            if (cmd.PropertyName == nameof(PKM.IVs))
            {
                PKM.SetRandomIVs();
                return;
            }

            if (TryGetHasProperty(PKM, cmd.PropertyName, out var pi))
                ReflectUtil.SetValue(pi, PKM, Util.Rand32() & PKM.MaxIV);
        }
    }
}
