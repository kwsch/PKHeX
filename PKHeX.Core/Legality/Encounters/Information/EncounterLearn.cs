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

        /// <summary>
        /// Default response if there are no matches.
        /// </summary>
        public const string NoMatches = "None";

        /// <summary>
        /// Checks if a <see cref="species"/> can learn all input <see cref="moves"/>.
        /// </summary>
        public static bool CanLearn(string species, IEnumerable<string> moves, string lang = GameLanguage.DefaultLanguage)
        {
            var encs = GetLearn(species, moves, lang);
            return encs.Any();
        }

        /// <summary>
        /// Gets a summary of all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
        /// </summary>
        public static IEnumerable<string> GetLearnSummary(string species, IEnumerable<string> moves, string lang = GameLanguage.DefaultLanguage)
        {
            var encs = GetLearn(species, moves, lang);
            var msg = Summarize(encs).ToList();
            if (msg.Count == 0)
                msg.Add(NoMatches);
            return msg;
        }

        /// <summary>
        /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
        /// </summary>
        public static IEnumerable<IEncounterable> GetLearn(string species, IEnumerable<string> moves, string lang = GameLanguage.DefaultLanguage)
        {
            var str = GameInfo.GetStrings(lang);

            var spec = StringUtil.FindIndexIgnoreCase(str.specieslist, species);
            var moveIDs = StringUtil.GetIndexes(str.movelist, moves.ToList());

            return GetLearn(spec, moveIDs);
        }

        /// <summary>
        /// Gets all encounters where a <see cref="species"/> can learn all input <see cref="moves"/>.
        /// </summary>
        public static IEnumerable<IEncounterable> GetLearn(int species, int[] moves)
        {
            if (species <= 0)
                return Array.Empty<IEncounterable>();
            if (moves.Any(z => z < 0))
                return Array.Empty<IEncounterable>();

            var blank = PKMConverter.GetBlank(PKX.Generation);
            blank.Species = species;

            var vers = GameUtil.GameVersions;
            return EncounterMovesetGenerator.GenerateEncounters(blank, moves, vers);
        }

        /// <summary>
        /// Summarizes all encounters by grouping by <see cref="IEncounterable.Name"/>.
        /// </summary>
        public static IEnumerable<string> Summarize(IEnumerable<IEncounterable> encounters, bool advanced = false)
        {
            var types = encounters.GroupBy(z => z.Name);
            return Summarize(types, advanced);
        }

        /// <summary>
        /// Summarizes groups of encounters.
        /// </summary>
        public static IEnumerable<string> Summarize(IEnumerable<IGrouping<string, IEncounterable>> types, bool advanced = false)
        {
            return types.SelectMany(g => EncounterSummary.SummarizeGroup(g, g.Key, advanced));
        }
    }

    public readonly struct EncounterSummary
    {
        private readonly GameVersion Version;
        private readonly string LocationName;

        private EncounterSummary(IEncounterable z, string type)
        {
            Version = z is IVersion v ? v.Version : GameVersion.Any;
            LocationName = GetLocationName(z) + $"({type}) ";
        }

        private EncounterSummary(IEncounterable z)
        {
            Version = z is IVersion v ? v.Version : GameVersion.Any;
            LocationName = GetLocationName(z);
        }

        private static string GetLocationName(IEncounterable z)
        {
            var gen = z is IGeneration g ? g.Generation : -1;
            var version = z is IVersion v ? (int)v.Version : -1;
            if (gen < 0 && version > 0)
                gen = ((GameVersion)version).GetGeneration();

            if (!(z is ILocation l))
                return $"[Gen{gen}]\t";
            var loc = l.GetEncounterLocation(gen, version);

            if (string.IsNullOrWhiteSpace(loc))
                return $"[Gen{gen}]\t";
            return $"[Gen{gen}]\t{loc}: ";
        }

        public static IEnumerable<string> SummarizeGroup(IEnumerable<IEncounterable> items, string header = "", bool advanced = false)
        {
            if (!string.IsNullOrWhiteSpace(header))
                yield return $"=={header}==";
            var summaries = advanced ? GetSummaries(items) : items.Select(z => new EncounterSummary(z));
            var objs = summaries.GroupBy(z => z.LocationName);
            foreach (var g in objs)
                yield return $"\t{g.Key}{string.Join(", ", g.Select(z => z.Version).Distinct())}";
        }

        public static IEnumerable<EncounterSummary> GetSummaries(IEnumerable<IEncounterable> items)
        {
            return items.SelectMany(GetSummaries);
        }

        private static IEnumerable<EncounterSummary> GetSummaries(IEncounterable item)
        {
            switch (item)
            {
                case EncounterSlot s:
                    var type = s.Type;
                    if (type == 0)
                    {
                        yield return new EncounterSummary(item);
                        break;
                    }
                    for (int i = 0; i < sizeof(SlotType) * 8; i++)
                    {
                        var flag = (SlotType)(1 << i);
                        if ((type & flag) != 0)
                            yield return new EncounterSummary(item, flag.ToString());
                    }

                    break;

                default:
                    yield return new EncounterSummary(item);
                    break;
            }
        }

        public bool Equals(EncounterSummary obj) => obj.Version == Version && obj.LocationName == LocationName;
        public override bool Equals(object obj) => obj is EncounterSummary t && Equals(t);
        public override int GetHashCode() => LocationName.GetHashCode();
        public static bool operator ==(EncounterSummary left, EncounterSummary right) => left.Equals(right);
        public static bool operator !=(EncounterSummary left, EncounterSummary right) => !(left == right);
    }
}
