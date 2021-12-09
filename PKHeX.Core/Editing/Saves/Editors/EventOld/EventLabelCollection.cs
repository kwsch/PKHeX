using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class EventLabelCollection
    {
        public readonly IReadOnlyList<NamedEventWork> Work;
        public readonly IReadOnlyList<NamedEventValue> Flag;

        public EventLabelCollection(string game, int maxFlag = int.MaxValue, int maxValue = int.MaxValue)
        {
            var f = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "flags");
            var c = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "const");
            Flag = GetFlags(f, maxFlag);
            Work = GetValues(c, maxValue);
        }

        private static List<NamedEventValue> GetFlags(IReadOnlyCollection<string> strings, int maxValue)
        {
            var result = new List<NamedEventValue>(strings.Count);
            var processed = new HashSet<int>();
            foreach (var s in strings)
            {
                var split = s.Split('\t');
                if (split.Length != 3)
                    continue;

                var index = TryParseHexDec(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                var type = GetEventType(split[1]);
                var desc = split[2];

                var item = new NamedEventValue(desc, index, type);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static readonly NamedEventConst Custom = new("Custom", NamedEventConst.CustomMagicValue);
        private static readonly NamedEventConst[] Empty = {Custom};

        private static IReadOnlyList<NamedEventWork> GetValues(IReadOnlyCollection<string> strings, int maxValue)
        {
            var result = new List<NamedEventWork>(strings.Count);
            var processed = new HashSet<int>();
            foreach (var s in strings)
            {
                var split = s.Split('\t');
                if (split.Length is not (3 or 4))
                    continue;

                var index = TryParseHexDecConst(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                var type = GetEventType(split[1]);
                var desc = split[2];
                var predefined = split.Length is 3 ? Empty : GetPredefinedArray(split[3]);
                var item = new NamedEventWork(desc, index, type, predefined);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static IReadOnlyList<NamedEventConst> GetPredefinedArray(string combined)
        {
            var result = new List<NamedEventConst> {Custom};
            var split = combined.Split(',');
            foreach (var entry in split)
            {
                var subsplit = entry.Split(':');
                var name = subsplit[1];
                var value = Convert.ToUInt16(subsplit[0]);
                result.Add(new NamedEventConst(name, value));
            }
            return result;
        }

        private static int TryParseHexDec(string flag)
        {
            if (!flag.StartsWith("0x"))
                return Convert.ToInt16(flag);
            flag = flag[2..];
            return Convert.ToInt16(flag, 16);
        }

        private static int TryParseHexDecConst(string c)
        {
            if (!c.StartsWith("0x40"))
                return Convert.ToInt16(c);
            c = c[4..];
            return Convert.ToInt16(c, 16);
        }

        private static NamedEventType GetEventType(string s) => s.Length == 0 ? 0 : GetEventType(s[0]);

        private static NamedEventType GetEventType(char c) => c switch
        {
            'h' => NamedEventType.HiddenItem,
            'm' => NamedEventType.Misc,
            'f' => NamedEventType.FlyToggle,
            't' => NamedEventType.TrainerToggle,
            's' => NamedEventType.StoryProgress,

            'a' => NamedEventType.Achievement,
            '+' => NamedEventType.Statistic,
            '*' => NamedEventType.UsefulFeature,
            'e' => NamedEventType.EncounterEvent,
            'g' => NamedEventType.GiftAvailable,
            'r' => NamedEventType.Rebattle,
            _ => NamedEventType.None,
        };
    }

    public sealed class EventLabelCollectionSystem
    {
        public readonly IReadOnlyList<NamedEventWork> Work;
        public readonly IReadOnlyList<NamedEventValue> Flag;
        public readonly IReadOnlyList<NamedEventValue> System;

        public EventLabelCollectionSystem(string game, int maxFlag = int.MaxValue, int maxSystem = int.MaxValue, int maxValue = int.MaxValue)
        {
            var f = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "flag");
            var s = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "system");
            var c = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "work");
            Flag = GetFlags(f, maxFlag);
            System = GetFlags(s, maxSystem);
            Work = GetValues(c, maxValue);
        }

        private static List<NamedEventValue> GetFlags(IReadOnlyCollection<string> strings, int maxValue)
        {
            var result = new List<NamedEventValue>(strings.Count);
            var processed = new HashSet<int>();
            foreach (var s in strings)
            {
                var split = s.Split('\t');
                if (split.Length != 3)
                    continue;

                var index = TryParseHexDec(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                var type = GetEventType(split[1]);
                var desc = split[2];

                var item = new NamedEventValue(desc, index, type);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static readonly NamedEventConst Custom = new("Custom", NamedEventConst.CustomMagicValue);
        private static readonly NamedEventConst[] Empty = { Custom };

        private static IReadOnlyList<NamedEventWork> GetValues(IReadOnlyCollection<string> strings, int maxValue)
        {
            var result = new List<NamedEventWork>(strings.Count);
            var processed = new HashSet<int>();
            foreach (var s in strings)
            {
                var split = s.Split('\t');
                if (split.Length is not (3 or 4))
                    continue;

                var index = TryParseHexDecConst(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                var type = GetEventType(split[1]);
                var desc = split[2];
                var predefined = split.Length is 3 ? Empty : GetPredefinedArray(split[3]);
                var item = new NamedEventWork(desc, index, type, predefined);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static IReadOnlyList<NamedEventConst> GetPredefinedArray(string combined)
        {
            var result = new List<NamedEventConst> { Custom };
            var split = combined.Split(',');
            foreach (var entry in split)
            {
                var subsplit = entry.Split(':');
                var name = subsplit[1];
                var value = Convert.ToUInt16(subsplit[0]);
                result.Add(new NamedEventConst(name, value));
            }
            return result;
        }

        private static int TryParseHexDec(string flag)
        {
            if (!flag.StartsWith("0x"))
                return Convert.ToInt16(flag);
            flag = flag[2..];
            return Convert.ToInt16(flag, 16);
        }

        private static int TryParseHexDecConst(string c)
        {
            if (!c.StartsWith("0x40"))
                return Convert.ToInt16(c);
            c = c[4..];
            return Convert.ToInt16(c, 16);
        }

        private static NamedEventType GetEventType(string s) => s.Length == 0 ? 0 : GetEventType(s[0]);

        private static NamedEventType GetEventType(char c) => c switch
        {
            'h' => NamedEventType.HiddenItem,
            'm' => NamedEventType.Misc,
            'f' => NamedEventType.FlyToggle,
            't' => NamedEventType.TrainerToggle,
            's' => NamedEventType.StoryProgress,

            'a' => NamedEventType.Achievement,
            '+' => NamedEventType.Statistic,
            '*' => NamedEventType.UsefulFeature,
            'e' => NamedEventType.EncounterEvent,
            'g' => NamedEventType.GiftAvailable,
            'r' => NamedEventType.Rebattle,
            _ => NamedEventType.None,
        };
    }

    public enum NamedEventType
    {
        None,
        HiddenItem,
        TrainerToggle,
        StoryProgress,
        FlyToggle,
        Misc,
        Statistic,

        Achievement,
        UsefulFeature,
        EncounterEvent,
        GiftAvailable,
        Rebattle = 100,
    }

    public record NamedEventValue(string Name, int Index, NamedEventType Type);

    public sealed record NamedEventWork(string Name, int Index, NamedEventType Type, IReadOnlyList<NamedEventConst> PredefinedValues) : NamedEventValue(Name, Index, Type);

    public sealed record NamedEventConst(string Name, ushort Value)
    {
        public bool IsCustom => Value == CustomMagicValue;
        public const ushort CustomMagicValue = ushort.MaxValue;
    }
}
