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
            switch (generation)
            {
                case 1:
                case 2: return res.Where(pk => pk.VC || pk.Format < 3);
                default:
                    return res.Where(pk => pk.GenNumber == generation);
            }
        }

        public static IEnumerable<PKM> FilterByLVL(IEnumerable<PKM> res, SearchComparison option, int level)
        {
            if (level > 100)
                return res;

            switch (option)
            {
                case SearchComparison.LessThanEquals:
                    return res.Where(pk => pk.Stat_Level <= level);
                case SearchComparison.Equals:
                    return res.Where(pk => pk.Stat_Level == level);
                case SearchComparison.GreaterThanEquals:
                    return res.Where(pk => pk.Stat_Level >= level);

                default:
                    return res; // Any (Do nothing)
            }
        }

        public static IEnumerable<PKM> FilterByEVs(IEnumerable<PKM> res, int option)
        {
            switch (option)
            {
                default: return res; // Any (Do nothing)
                case 1: // None (0)
                    return res.Where(pk => pk.EVTotal == 0);
                case 2: // Some (127-0)
                    return res.Where(pk => pk.EVTotal < 128);
                case 3: // Half (128-507)
                    return res.Where(pk => pk.EVTotal >= 128 && pk.EVTotal < 508);
                case 4: // Full (508+)
                    return res.Where(pk => pk.EVTotal >= 508);
            }
        }

        public static IEnumerable<PKM> FilterByIVs(IEnumerable<PKM> res, int option)
        {
            switch (option)
            {
                default: return res; // Do nothing
                case 1: // <= 90
                    return res.Where(pk => pk.IVTotal <= 90);
                case 2: // 91-120
                    return res.Where(pk => pk.IVTotal > 90 && pk.IVTotal <= 120);
                case 3: // 121-150
                    return res.Where(pk => pk.IVTotal > 120 && pk.IVTotal <= 150);
                case 4: // 151-179
                    return res.Where(pk => pk.IVTotal > 150 && pk.IVTotal < 180);
                case 5: // 180+
                    return res.Where(pk => pk.IVTotal >= 180);
                case 6: // == 186
                    return res.Where(pk => pk.IVTotal == 186);
            }
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
            switch (Clones)
            {
                default: return null;
                case CloneDetectionMethod.HashDetails:
                    return HashByDetails;
                case CloneDetectionMethod.HashPID:
                    return HashByPID;
            }
        }

        public static string HashByDetails(PKM pk)
        {
            switch (pk.Format)
            {
                case 1: return $"{pk.Species:000}{((PK1)pk).DV16:X4}";
                case 2: return $"{pk.Species:000}{((PK2)pk).DV16:X4}";
                default: return $"{pk.Species:000}{pk.PID:X8}{string.Join(" ", pk.IVs)}{pk.AltForm:00}";
            }
        }

        public static string HashByPID(PKM pk)
        {
            switch (pk.Format)
            {
                case 1: return $"{((PK1)pk).DV16:X4}";
                case 2: return $"{((PK2)pk).DV16:X4}";
                default: return $"{pk.PID:X8}";
            }
        }

        public static IEnumerable<PKM> GetClones(IEnumerable<PKM> res, CloneDetectionMethod type = CloneDetectionMethod.HashDetails)
        {
            var method = GetCloneDetectMethod(type);
            return method == null ? res : GetClones(res, method);
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