using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static PKHeX.Core.EncountersWC3;

namespace PKHeX.Core
{
    public static class EncounterEvent
    {
        /// <summary>Event Database for Generation 3</summary>
        public static IReadOnlyList<WC3> MGDB_G3 { get; private set; } = Array.Empty<WC3>();

        /// <summary>Event Database for Generation 4</summary>
        public static IReadOnlyList<PCD> MGDB_G4 { get; private set; } = Array.Empty<PCD>();

        /// <summary>Event Database for Generation 5</summary>
        public static IReadOnlyList<PGF> MGDB_G5 { get; private set; } = Array.Empty<PGF>();

        /// <summary>Event Database for Generation 6</summary>
        public static IReadOnlyList<WC6> MGDB_G6 { get; private set; } = Array.Empty<WC6>();

        /// <summary>Event Database for Generation 7</summary>
        public static IReadOnlyList<WC7> MGDB_G7 { get; private set; } = Array.Empty<WC7>();

        /// <summary>Event Database for Generation 7 <see cref="GameVersion.GG"/></summary>
        public static IReadOnlyList<WB7> MGDB_G7GG { get; private set; } = Array.Empty<WB7>();

        /// <summary>Event Database for Generation 8</summary>
        public static IReadOnlyList<WC8> MGDB_G8 { get; private set; } = Array.Empty<WC8>();

        /// <summary>Indicates if the databases are initialized.</summary>
        public static bool Initialized => MGDB_G3.Count != 0;

        private static PCD[] GetPCDDB(byte[] bin) => Get(bin, PCD.Size, d => new PCD(d));
        private static PGF[] GetPGFDB(byte[] bin) => Get(bin, PGF.Size, d => new PGF(d));

        private static WC6[] GetWC6DB(byte[] wc6bin, byte[] wc6full) => WC6Full.GetArray(wc6full, wc6bin);
        private static WC7[] GetWC7DB(byte[] wc7bin, byte[] wc7full) => WC7Full.GetArray(wc7full, wc7bin);

        private static WB7[] GetWB7DB(byte[] bin) => Get(bin, WB7.SizeFull, d => new WB7(d));
        private static WC8[] GetWC8DB(byte[] bin) => Get(bin, WC8.Size, d => new WC8(d));

        private static T[] Get<T>(byte[] bin, int size, Func<byte[], T> ctor)
        {
            var result = new T[bin.Length / size];
            System.Diagnostics.Debug.Assert(result.Length * size == bin.Length);
            for (int i = 0; i < result.Length; i++)
            {
                var offset = i * size;
                var slice = bin.Slice(offset, size);
                result[i] = ctor(slice);
            }
            return result;
        }

        public static void RefreshMGDB(params string[] paths)
        {
            ICollection<PCD> g4 = GetPCDDB(Util.GetBinaryResource("wc4.pkl"));
            ICollection<PGF> g5 = GetPGFDB(Util.GetBinaryResource("pgf.pkl"));
            ICollection<WC6> g6 = GetWC6DB(Util.GetBinaryResource("wc6.pkl"), Util.GetBinaryResource("wc6full.pkl"));
            ICollection<WC7> g7 = GetWC7DB(Util.GetBinaryResource("wc7.pkl"), Util.GetBinaryResource("wc7full.pkl"));
            ICollection<WB7> b7 = GetWB7DB(Util.GetBinaryResource("wb7full.pkl"));
            ICollection<WC8> g8 = GetWC8DB(Util.GetBinaryResource("wc8.pkl"));

            foreach (var gift in paths.Where(Directory.Exists).SelectMany(MysteryUtil.GetGiftsFromFolder))
            {
                static void AddOrExpand<T>(ref ICollection<T> arr, T obj)
                {
                    if (arr is HashSet<T> h)
                        h.Add(obj);
                    else
                        arr = new HashSet<T>(arr) {obj};
                }
                switch (gift)
                {
                    case PCD pcd: AddOrExpand(ref g4, pcd); continue;
                    case PGF pgf: AddOrExpand(ref g5, pgf); continue;
                    case WC6 wc6: AddOrExpand(ref g6, wc6); continue;
                    case WC7 wc7: AddOrExpand(ref g7, wc7); continue;
                    case WB7 wb7: AddOrExpand(ref b7, wb7); continue;
                    case WC8 wc8: AddOrExpand(ref g8, wc8); continue;
                }
            }

            static T[] SetArray<T>(ICollection<T> arr)
            {
                if (arr is T[] x)
                    return x;

                // rather than use Linq to build an array, just do it the quick way directly.
                var result = new T[arr.Count];
                ((HashSet<T>)arr).CopyTo(result, 0);
                return result;
            }

            MGDB_G3 = Encounter_WC3; // hardcoded
            MGDB_G4 = SetArray(g4);
            MGDB_G5 = SetArray(g5);
            MGDB_G6 = SetArray(g6);
            MGDB_G7 = SetArray(g7);
            MGDB_G7GG = SetArray(b7);
            MGDB_G8 = SetArray(g8);
        }

        public static IEnumerable<MysteryGift> GetAllEvents(bool sorted = true)
        {
            var regular = new IReadOnlyList<MysteryGift>[]
            {
                MGDB_G4,
                MGDB_G5,
                MGDB_G6,
                MGDB_G7,
                MGDB_G7GG,
                MGDB_G8,
            }.SelectMany(z => z);
            regular = regular.Where(mg => !mg.IsItem && mg.IsPokémon && mg.Species > 0);
            var result = MGDB_G3.Concat(regular);
            if (sorted)
                result = result.OrderBy(mg => mg.Species);
            return result;
        }
    }
}
