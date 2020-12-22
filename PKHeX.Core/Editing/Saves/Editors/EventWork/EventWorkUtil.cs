using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides utility for various <see cref="EventWork{T}"/> logic.
    /// </summary>
    public static class EventWorkUtil
    {
        private static readonly Dictionary<char, EventVarType> TypeDict = new()
        {
            ['z'] = EventVarType.Zone,
            ['s'] = EventVarType.System,
            ['v'] = EventVarType.Vanish,
            ['c'] = EventVarType.Scene,
            ['e'] = EventVarType.Event,
        };

        private static bool GetIndex(string l, out int index, out EventVarType type)
        {
            if (!TypeDict.TryGetValue(l[0], out type))
            {
                Debug.WriteLine($"Rejected line due to bad type. {l[0]}");
                index = -1;
                return false;
            }
            if (!int.TryParse(l.Substring(1), out index))
            {
                Debug.WriteLine($"Rejected line due to bad index. {l[0]}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parses and converts <see cref="lines"/> into <see cref="EventVarGroup"/> values with the provided <see cref="constructor"/>.
        /// </summary>
        /// <param name="lines">Lines to parse</param>
        /// <param name="constructor">Object constructor</param>
        /// <returns>Converted lines grouped together by <see cref="EventVarType"/></returns>
        public static List<EventVarGroup> GetVars(IEnumerable<string> lines, Func<int, EventVarType, string[], EventVar> constructor)
        {
            var list = new List<EventVarGroup>();
            foreach (var l in lines)
            {
                var split = l.Split('\t');
                if (split.Length < 2 || split[0].Length < 2)
                    continue;

                if (!GetIndex(split[0], out var index, out var type))
                    continue;

                var group = list.Find(z => z.Type == type);
                if (group == null)
                {
                    group = new EventVarGroup(type);
                    list.Add(group);
                }

                var entry = constructor(index, type, split);
                group.Vars.Add(entry);
            }
            return list;
        }

        /// <summary>
        /// Compares a <see cref="before"/> and <see cref="after"/> <see cref="IEventWork{T}"/> object of the same type to find <see cref="EventFlag"/> changes.
        /// </summary>
        /// <typeparam name="T">Type of value used by <see cref="EventWork{T}"/></typeparam>
        /// <param name="before">Data before the event was triggered</param>
        /// <param name="after">Data after the event was triggered</param>
        /// <param name="on">List of flags that were turned on</param>
        /// <param name="off">List of flags that were turned off</param>
        public static void DiffSavesFlag<T>(IEventWork<T> before, IEventWork<T> after, List<int> on, List<int> off)
        {
            int max = before.MaxFlag;
            for (int i = 0; i < max; i++)
            {
                var b = before.GetFlag(i);
                var a = after.GetFlag(i);
                if (b == a)
                    continue;

                var arr = a ? on : off;
                arr.Add(i);
            }
        }

        /// <summary>
        /// Compares a <see cref="before"/> and <see cref="after"/> <see cref="IEventWork{T}"/> object of the same type to find <see cref="EventWork{T}"/> changes.
        /// </summary>
        /// <typeparam name="T">Type of value used by <see cref="EventWork{T}"/></typeparam>
        /// <param name="before">Data before the event was triggered</param>
        /// <param name="after">Data after the event was triggered</param>
        /// <param name="changed"><see cref="EventVar.RawIndex"/> values that changed</param>
        /// <param name="changes">Summary of the <see cref="EventWork{T}"/> value change</param>
        public static void DiffSavesWork<T>(IEventWork<T> before, IEventWork<T> after, List<int> changed, List<string> changes)
        {
            int max = before.MaxWork;
            for (int i = 0; i < max; i++)
            {
                var b = before.GetWork(i);
                var a = after.GetWork(i);
                if (b is null || b.Equals(a))
                    continue;

                changed.Add(i);
                changes.Add($"{b} => {a}");
            }
        }
    }
}