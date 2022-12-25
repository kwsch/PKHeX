using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PKHeX.Core.EncountersWC3;

namespace PKHeX.Core;

/// <summary>
/// Helper class that stores references to all the event data templates.
/// </summary>
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

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.PLA"/></summary>
    public static IReadOnlyList<WA8> MGDB_G8A { get; private set; } = Array.Empty<WA8>();

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.BDSP"/></summary>
    public static IReadOnlyList<WB8> MGDB_G8B { get; private set; } = Array.Empty<WB8>();

    /// <summary>Event Database for Generation 9 <see cref="GameVersion.SV"/></summary>
    public static IReadOnlyList<WC9> MGDB_G9 { get; private set; } = Array.Empty<WC9>();

    /// <summary>Indicates if the databases are initialized.</summary>
    public static bool Initialized => MGDB_G3.Count != 0;

    private static PCD[] GetPCDDB(ReadOnlySpan<byte> bin) => Get(bin, PCD.Size, static d => new PCD(d));
    private static PGF[] GetPGFDB(ReadOnlySpan<byte> bin) => Get(bin, PGF.Size, static d => new PGF(d));

    private static WC6[] GetWC6DB(ReadOnlySpan<byte> wc6bin, ReadOnlySpan<byte> wc6full) => WC6Full.GetArray(wc6full, wc6bin);
    private static WC7[] GetWC7DB(ReadOnlySpan<byte> wc7bin, ReadOnlySpan<byte> wc7full) => WC7Full.GetArray(wc7full, wc7bin);

    private static WB7[] GetWB7DB(ReadOnlySpan<byte> bin) => Get(bin, WB7.SizeFull, static d => new WB7(d));
    private static WC8[] GetWC8DB(ReadOnlySpan<byte> bin) => Get(bin, WC8.Size, static d => new WC8(d));
    private static WB8[] GetWB8DB(ReadOnlySpan<byte> bin) => Get(bin, WB8.Size, static d => new WB8(d));
    private static WA8[] GetWA8DB(ReadOnlySpan<byte> bin) => Get(bin, WA8.Size, static d => new WA8(d));
    private static WC9[] GetWC9DB(ReadOnlySpan<byte> bin) => Get(bin, WC9.Size, static d => new WC9(d));

    private static T[] Get<T>(ReadOnlySpan<byte> bin, int size, Func<byte[], T> ctor)
    {
        // bin is a multiple of size
        // bin.Length % size == 0
        var result = new T[bin.Length / size];
        System.Diagnostics.Debug.Assert(result.Length * size == bin.Length);
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * size;
            var slice = bin.Slice(offset, size).ToArray();
            result[i] = ctor(slice);
        }
        return result;
    }

    /// <summary>
    /// Reloads the stored event templates.
    /// </summary>
    /// <param name="paths">External folder(s) to source individual mystery gift template files from.</param>
    public static void RefreshMGDB(params string[] paths)
    {
        ICollection<PCD> g4 = GetPCDDB(Util.GetBinaryResource("wc4.pkl"));
        ICollection<PGF> g5 = GetPGFDB(Util.GetBinaryResource("pgf.pkl"));
        ICollection<WC6> g6 = GetWC6DB(Util.GetBinaryResource("wc6.pkl"), Util.GetBinaryResource("wc6full.pkl"));
        ICollection<WC7> g7 = GetWC7DB(Util.GetBinaryResource("wc7.pkl"), Util.GetBinaryResource("wc7full.pkl"));
        ICollection<WB7> b7 = GetWB7DB(Util.GetBinaryResource("wb7full.pkl"));
        ICollection<WC8> g8 = GetWC8DB(Util.GetBinaryResource("wc8.pkl"));
        ICollection<WB8> b8 = GetWB8DB(Util.GetBinaryResource("wb8.pkl"));
        ICollection<WA8> a8 = GetWA8DB(Util.GetBinaryResource("wa8.pkl"));
        ICollection<WC9> g9 = GetWC9DB(Util.GetBinaryResource("wc9.pkl"));

        // Load external files
        // For each file, load the gift object into the appropriate list.
        var gifts = GetGifts(paths);
        foreach (var gift in gifts)
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
                case WB8 wb8: AddOrExpand(ref b8, wb8); continue;
                case WA8 wa8: AddOrExpand(ref a8, wa8); continue;
                case WC9 wc9: AddOrExpand(ref g9, wc9); continue;
            }
        }

        static T[] SetArray<T>(ICollection<T> arr)
        {
            if (arr is T[] x)
                return x;

            // rather than use Linq to build an array, just do it the quick way directly.
            var result = new T[arr.Count];
            ((IReadOnlySet<T>)arr).CopyTo(result);
            return result;
        }

        MGDB_G3 = Encounter_WC3; // hardcoded
        MGDB_G4 = SetArray(g4);
        MGDB_G5 = SetArray(g5);
        MGDB_G6 = SetArray(g6);
        MGDB_G7 = SetArray(g7);
        MGDB_G7GG = SetArray(b7);
        MGDB_G8 = SetArray(g8);
        MGDB_G8A = SetArray(a8);
        MGDB_G8B = SetArray(b8);
        MGDB_G9 = SetArray(g9);
    }

    private static IEnumerable<MysteryGift> GetGifts(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (!Directory.Exists(path))
                continue;
            var gifts = MysteryUtil.GetGiftsFromFolder(path);
            foreach (var gift in gifts)
                yield return gift;
        }
    }

    /// <summary>
    /// Gets all event gifts.
    /// </summary>
    /// <param name="sorted">If true, the result will be sorted by ascending species ID. Otherwise, will be ordered by generation ascending.</param>
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
            MGDB_G8A,
            MGDB_G8B,
            MGDB_G9,
        }.SelectMany(z => z);
        regular = regular.Where(mg => mg is { IsItem: false, IsEntity: true, Species: > 0 });
        var result = MGDB_G3.Concat(regular);
        if (sorted)
            result = result.OrderBy(mg => mg.Species);
        return result;
    }
}
