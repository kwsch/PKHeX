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
        public static WC3[] MGDB_G3 { get; private set; } = Array.Empty<WC3>();

        /// <summary>Event Database for Generation 4</summary>
        public static PCD[] MGDB_G4 { get; private set; } = Array.Empty<PCD>();

        /// <summary>Event Database for Generation 5</summary>
        public static PGF[] MGDB_G5 { get; private set; } = Array.Empty<PGF>();

        /// <summary>Event Database for Generation 6</summary>
        public static WC6[] MGDB_G6 { get; private set; } = Array.Empty<WC6>();

        /// <summary>Event Database for Generation 7</summary>
        public static WC7[] MGDB_G7 { get; private set; } = Array.Empty<WC7>();

        /// <summary>Event Database for Generation 7 <see cref="GameVersion.GG"/></summary>
        public static WB7[] MGDB_G7GG { get; private set; } = Array.Empty<WB7>();

        /// <summary>Indicates if the databases are initialized.</summary>
        public static bool Initialized => MGDB_G3.Length != 0;

        private static IEnumerable<byte[]> GetData(byte[] bin, int size)
        {
            for (int i = 0; i < bin.Length; i += size)
            {
                byte[] data = new byte[size];
                Buffer.BlockCopy(bin, i, data, 0, size);
                yield return data;
            }
        }

        private static HashSet<PCD> GetPCDDB(byte[] bin) => new HashSet<PCD>(GetData(bin, PCD.Size).Select(d => new PCD(d)));

        private static HashSet<PGF> GetPGFDB(byte[] bin) => new HashSet<PGF>(GetData(bin, PGF.Size).Select(d => new PGF(d)));

        private static HashSet<WC6> GetWC6DB(byte[] wc6bin, byte[] wc6full) => new HashSet<WC6>(
            GetData(wc6full, WC6.SizeFull).Select(d => new WC6(d))
            .Concat(GetData(wc6bin, WC6.Size).Select(d => new WC6(d))));

        private static HashSet<WC7> GetWC7DB(byte[] wc7bin, byte[] wc7full) => new HashSet<WC7>(
            GetData(wc7full, WC7.SizeFull).Select(d => new WC7(d))
            .Concat(GetData(wc7bin, WC7.Size).Select(d => new WC7(d))));

        private static HashSet<WB7> GetWB7DB(byte[] wc7full) => new HashSet<WB7>(GetData(wc7full, WB7.SizeFull).Select(d => new WB7(d)));

        public static void RefreshMGDB(params string[] paths)
        {
            var g4 = GetPCDDB(Util.GetBinaryResource("wc4.pkl"));
            var g5 = GetPGFDB(Util.GetBinaryResource("pgf.pkl"));
            var g6 = GetWC6DB(Util.GetBinaryResource("wc6.pkl"), Util.GetBinaryResource("wc6full.pkl"));
            var g7 = GetWC7DB(Util.GetBinaryResource("wc7.pkl"), Util.GetBinaryResource("wc7full.pkl"));
            var b7 = GetWB7DB(Util.GetBinaryResource("wb7full.pkl"));

            foreach (var path in paths.Where(Directory.Exists))
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (!MysteryGift.IsMysteryGift(fi.Length))
                        continue;

                    var gift = MysteryGift.GetMysteryGift(File.ReadAllBytes(file), fi.Extension);
                    switch (gift)
                    {
                        case PCD pcd: g4.Add(pcd); continue;
                        case PGF pgf: g5.Add(pgf); continue;
                        case WC6 wc6: g6.Add(wc6); continue;
                        case WC7 wc7: g7.Add(wc7); continue;
                        case WB7 wb7: b7.Add(wb7); continue;
                    }
                }
            }

            MGDB_G3 = Encounter_WC3; // hardcoded
            MGDB_G4 = g4.ToArray();
            MGDB_G5 = g5.ToArray();
            MGDB_G6 = g6.ToArray();
            MGDB_G7 = g7.ToArray();
            MGDB_G7GG = b7.ToArray();
        }

        public static IEnumerable<MysteryGift> GetAllEvents(bool sorted = true)
        {
            var regular = new MysteryGift[][]
            {
                MGDB_G4,
                MGDB_G5,
                MGDB_G6,
                MGDB_G7,
                MGDB_G7GG,
            }.SelectMany(z => z);
            regular = regular.Where(mg => !mg.IsItem && mg.IsPokémon && mg.Species > 0);
            var result = MGDB_G3.Concat(regular);
            if (sorted)
                result = result.OrderBy(mg => mg.Species);
            return result;
        }
    }
}
