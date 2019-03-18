using System;
using System.Collections.Generic;
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
            bool Match(string item, string find) => item.Length == find.Length && item.IndexOf(find, StringComparison.OrdinalIgnoreCase) >= 0;
            return Array.FindIndex(arr, i => Match(i, val));
        }

        public static IEnumerable<string> Summarize(IEnumerable<IEncounterable> encounters)
        {
            var types = encounters.GroupBy(z => z.Name);
            foreach (var type in types)
            {
                var name = type.Key;
                var versions = type.OfType<IVersion>().Select(z => z.Version).Distinct();
                var locgroups = type.OfType<ILocation>().Select(z =>
                    z.GetEncounterLocation(
                        z is IGeneration g ? g.Generation : -1,
                        z is IVersion v ? (int) v.Version : -1));
                yield return $"{name}: {string.Join(", ", versions)}";

                var locations = locgroups.Distinct().OrderBy(z => z).Where(z => !string.IsNullOrWhiteSpace(z)).ToArray();
                if (locations.Length > 0)
                    yield return $"Locations: " + string.Join(", ", locations);
            }
        }
    }
}
