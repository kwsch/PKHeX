using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core.Searching
{
    /// <summary>
    /// <see cref="PKM"/> searching utility
    /// </summary>
    public static class SearchUtil
    {
        public static IEnumerable<PKM> FilterByFormat(IEnumerable<PKM> res, int format, SearchComparison formatOperand)
        {
            switch (formatOperand)
            {
                case SearchComparison.GreaterThanEquals:
                    res = res.Where(pk => pk.Format >= format); break;
                case SearchComparison.Equals:
                    res = res.Where(pk => pk.Format == format); break;
                case SearchComparison.LessThanEquals:
                    res = res.Where(pk => pk.Format <= format); break;

                default:
                    return res; /* Do nothing */
            }

            if (format <= 2) // 1-2
                return res.Where(pk => pk.Format <= 2);
            if (format >= 3 && format <= 6) // 3-6
                return res.Where(pk => pk.Format >= 3);

            return res;
        }

        public static IEnumerable<PKM> FilterByGeneration(IEnumerable<PKM> res, int generation)
        {
            return generation switch
            {
                1 => res.Where(pk => pk.VC || pk.Format < 3),
                2 => res.Where(pk => pk.VC || pk.Format < 3),
                _ => res.Where(pk => pk.GenNumber == generation)
            };
        }

        public static IEnumerable<PKM> FilterByLVL(IEnumerable<PKM> res, SearchComparison option, int level)
        {
            if (level > 100)
                return res;

            return option switch
            {
                SearchComparison.LessThanEquals =>    res.Where(pk => pk.Stat_Level <= level),
                SearchComparison.Equals =>            res.Where(pk => pk.Stat_Level == level),
                SearchComparison.GreaterThanEquals => res.Where(pk => pk.Stat_Level >= level),
                _ => res
            };
        }

        public static IEnumerable<PKM> FilterByEVs(IEnumerable<PKM> res, int option)
        {
            return option switch
            {
                1 => res.Where(pk => pk.EVTotal == 0), // None (0)
                2 => res.Where(pk => pk.EVTotal < 128), // Some (127-0)
                3 => res.Where(pk => pk.EVTotal >= 128 && pk.EVTotal < 508), // Half (128-507)
                4 => res.Where(pk => pk.EVTotal >= 508), // Full (508+)
                _ => res
            };
        }

        public static IEnumerable<PKM> FilterByIVs(IEnumerable<PKM> res, int option)
        {
            return option switch
            {
                1 => res.Where(pk => pk.IVTotal <= 90), // <= 90
                2 => res.Where(pk => pk.IVTotal > 90 && pk.IVTotal <= 120), // 91-120
                3 => res.Where(pk => pk.IVTotal > 120 && pk.IVTotal <= 150), // 121-150
                4 => res.Where(pk => pk.IVTotal > 150 && pk.IVTotal < 180), // 151-179
                5 => res.Where(pk => pk.IVTotal >= 180), // 180+
                6 => res.Where(pk => pk.IVTotal == 186), // == 186
                _ => res
            };
        }

        public static IEnumerable<PKM> FilterByMoves(IEnumerable<PKM> res, IEnumerable<int> Moves)
        {
            var moves = new HashSet<int>(Moves);
            int count = moves.Count;
            return res.Where(pk =>
                pk.Moves.Where(z => z > 0)
                    .Count(moves.Contains) == count
            );
        }

        public static IEnumerable<PKM> FilterByBatchInstruction(IEnumerable<PKM> res, IList<string> BatchInstructions)
        {
            if (BatchInstructions?.All(string.IsNullOrWhiteSpace) != false)
                return res; // none specified;

            var lines = BatchInstructions.Where(z => !string.IsNullOrWhiteSpace(z));
            var filters = StringInstruction.GetFilters(lines).ToArray();
            BatchEditing.ScreenStrings(filters);
            return res.Where(pkm => BatchEditing.IsFilterMatch(filters, pkm)); // Compare across all filters
        }

        public static Func<PKM, string> GetCloneDetectMethod(CloneDetectionMethod Clones)
        {
            return Clones switch
            {
                CloneDetectionMethod.HashPID => HashByPID,
                _ => HashByDetails,
            };
        }

        public static string HashByDetails(PKM pk)
        {
            return pk.Format switch
            {
                1 => $"{pk.Species:000}{((PK1) pk).DV16:X4}",
                2 => $"{pk.Species:000}{((PK2) pk).DV16:X4}",
                _ => $"{pk.Species:000}{pk.PID:X8}{string.Join(" ", pk.IVs)}{pk.AltForm:00}"
            };
        }

        public static string HashByPID(PKM pk)
        {
            return pk.Format switch
            {
                1 => $"{((PK1) pk).DV16:X4}",
                2 => $"{((PK2) pk).DV16:X4}",
                _ => $"{pk.PID:X8}"
            };
        }

        public static IEnumerable<PKM> GetClones(IEnumerable<PKM> res, CloneDetectionMethod type = CloneDetectionMethod.HashDetails)
        {
            var method = GetCloneDetectMethod(type);
            return GetClones(res, method);
        }

        public static IEnumerable<PKM> GetClones(IEnumerable<PKM> res, Func<PKM, string> method)
        {
            return res
                .GroupBy(method)
                .Where(grp => grp.Count() > 1)
                .SelectMany(z => z);
        }

        public static IEnumerable<PKM> GetExtraClones(IEnumerable<PKM> db)
        {
            return GetExtraClones(db, HashByDetails);
        }

        public static IEnumerable<PKM> GetExtraClones(IEnumerable<PKM> db, Func<PKM, string> method)
        {
            return db.GroupBy(method).Where(grp => grp.Count() > 1).SelectMany(z => z.Skip(1));
        }
    }
}