using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PKHeX.Core
{
    public static class EncounterLearn
    {
        static EncounterLearn()
        {
            if (!EncounterEvent.Initialized)
                EncounterEvent.RefreshMGDB();
        }

        private const string DefaultLang = "en";

        public static bool CanLearn(string species, IEnumerable<string> moves, string lang = DefaultLang)
        {
            var encs = GetLearn(species, moves, lang);
            return encs.Any();
        }

        public static IEnumerable<string> GetLearnSummary(string species, IEnumerable<string> moves, string lang = DefaultLang)
        {
            var encs = GetLearn(species, moves, lang);
            var msg = Summarize(encs).ToList();
            if (msg.Count == 0)
                msg.Add("None.");
            return msg;
        }

        public static IEnumerable<IEncounterable> GetLearn(string species, IEnumerable<string> moves, string lang = DefaultLang)
        {
            var str = GameInfo.GetStrings(lang);
            if (str == null)
                return Enumerable.Empty<IEncounterable>();

            var spec = FindIndexIgnoreCase(str.specieslist, species);
            if (spec <= 0)
                return Enumerable.Empty<IEncounterable>();

            var moveIDs = moves.Select(z => FindIndexIgnoreCase(str.movelist, z)).Where(z => z > 0).ToArray();

            return GetLearn(spec, moveIDs);
        }

        public static IEnumerable<IEncounterable> GetLearn(int spec, int[] moveIDs)
        {
            var blank = PKMConverter.GetBlank(PKX.Generation);
            blank.Species = spec;

            var vers = GameUtil.GameVersions;
            return EncounterMovesetGenerator.GenerateEncounters(blank, moveIDs, vers);
        }

        private static int FindIndexIgnoreCase(string[] arr, string val)
        {
            bool Match(string item, string find)
            {
                if (item.Length != find.Length)
                    return false;
                const CompareOptions options = CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase;
                var compare = CultureInfo.CurrentCulture.CompareInfo.Compare(item, find, options);
                return compare == 0;
            }

            return Array.FindIndex(arr, i => Match(i, val));
        }

        public static IEnumerable<string> Summarize(IEnumerable<IEncounterable> encounters)
        {
            var types = encounters.GroupBy(z => z.Name);
            return types.SelectMany(g => EnhancedSummary.SummarizeGroup(g.Key, g));
        }

        private struct EnhancedSummary
        {
            private readonly GameVersion Version;
            private readonly string LocationName;

            private EnhancedSummary(IEncounterable z)
            {
                Version = z is IVersion v ? v.Version : GameVersion.Any;
                LocationName = GetLocationName(z);
            }

            private static string GetLocationName(IEncounterable z)
            {
                var gen = z is IGeneration g ? g.Generation : -1;
                var version = z is IVersion v ? (int) v.Version : -1;
                if (gen < 0 && version > 0)
                    gen = ((GameVersion)version).GetGeneration();

                if (!(z is ILocation l))
                    return $"[Gen{gen}]\t";
                var loc = l.GetEncounterLocation(gen, version);

                if (string.IsNullOrWhiteSpace(loc))
                    return $"[Gen{gen}]\t";
                return $"[Gen{gen}]\t{loc}: ";
            }

            public static IEnumerable<string> SummarizeGroup(string header, IEnumerable<IEncounterable> items)
            {
                yield return $"=={header}==";
                var objs = items.Select(z => new EnhancedSummary(z)).GroupBy(z => z.LocationName);
                foreach (var g in objs)
                    yield return $"\t{g.Key}{string.Join(", ", g.Select(z => z.Version).Distinct())}";
            }
        }
    }
}
