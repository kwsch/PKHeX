using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static PKHeX.Core.EncountersWC3;

namespace PKHeX.Core;

/// <summary>
/// Helper class that stores references to all the event data templates.
/// </summary>
public static class EncounterEvent
{
    #region Pickle Data
    /// <summary>Event Database for Generation 3</summary>
    public static WC3[] MGDB_G3 => Encounter_WC3;

    /// <summary>Event Database for Generation 4</summary>
    public static readonly PCD[] MGDB_G4 = GetPCDDB(Util.GetBinaryResource("wc4.pkl"));

    /// <summary>Event Database for Generation 5</summary>
    public static readonly PGF[] MGDB_G5 = GetPGFDB(Util.GetBinaryResource("pgf.pkl"));

    /// <summary>Event Database for Generation 6</summary>
    public static readonly WC6[] MGDB_G6 = GetWC6DB(Util.GetBinaryResource("wc6.pkl"), Util.GetBinaryResource("wc6full.pkl"));

    /// <summary>Event Database for Generation 7</summary>
    public static readonly WC7[] MGDB_G7 = GetWC7DB(Util.GetBinaryResource("wc7.pkl"), Util.GetBinaryResource("wc7full.pkl"));

    /// <summary>Event Database for Generation 7 <see cref="GameVersion.GG"/></summary>
    public static readonly WB7[] MGDB_G7GG = GetWB7DB(Util.GetBinaryResource("wb7full.pkl"));

    /// <summary>Event Database for Generation 8</summary>
    public static readonly WC8[] MGDB_G8 = GetWC8DB(Util.GetBinaryResource("wc8.pkl"));

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.PLA"/></summary>
    public static readonly WA8[] MGDB_G8A = GetWA8DB(Util.GetBinaryResource("wa8.pkl"));

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.BDSP"/></summary>
    public static readonly WB8[] MGDB_G8B = GetWB8DB(Util.GetBinaryResource("wb8.pkl"));

    /// <summary>Event Database for Generation 9 <see cref="GameVersion.SV"/></summary>
    public static readonly WC9[] MGDB_G9 = GetWC9DB(Util.GetBinaryResource("wc9.pkl"));
    #endregion

    #region Locally Loaded Data
    /// <summary>Event Database for Generation 4</summary>
    public static PCD[] EGDB_G4 { get; private set; } = [];

    /// <summary>Event Database for Generation 5</summary>
    public static PGF[] EGDB_G5 { get; private set; } = [];

    /// <summary>Event Database for Generation 6</summary>
    public static WC6[] EGDB_G6 { get; private set; } = [];

    /// <summary>Event Database for Generation 7</summary>
    public static WC7[] EGDB_G7 { get; private set; } = [];

    /// <summary>Event Database for Generation 7 <see cref="GameVersion.GG"/></summary>
    public static WB7[] EGDB_G7GG { get; private set; } = [];

    /// <summary>Event Database for Generation 8</summary>
    public static WC8[] EGDB_G8 { get; private set; } = [];

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.PLA"/></summary>
    public static WA8[] EGDB_G8A { get; private set; } = [];

    /// <summary>Event Database for Generation 8 <see cref="GameVersion.BDSP"/></summary>
    public static WB8[] EGDB_G8B { get; private set; } = [];

    /// <summary>Event Database for Generation 9 <see cref="GameVersion.SV"/></summary>
    public static WC9[] EGDB_G9 { get; private set; } = [];
    #endregion

    private static PCD[] GetPCDDB(ReadOnlySpan<byte> bin) => Get(bin, PCD.Size, static d => new PCD(d));
    private static PGF[] GetPGFDB(ReadOnlySpan<byte> bin) => Get(bin, PGF.Size, static d => new PGF(d));

    private static WC6[] GetWC6DB(ReadOnlySpan<byte> wc6bin, ReadOnlySpan<byte> wc6full) => WC6Full.GetArray(wc6full, wc6bin);
    private static WC7[] GetWC7DB(ReadOnlySpan<byte> wc7bin, ReadOnlySpan<byte> wc7full) => WC7Full.GetArray(wc7full, wc7bin);

    private static WB7[] GetWB7DB(ReadOnlySpan<byte> bin) => Get(bin, WB7.Size, static d => new WB7(d));
    private static WC8[] GetWC8DB(ReadOnlySpan<byte> bin) => Get(bin, WC8.Size, static d => new WC8(d));
    private static WB8[] GetWB8DB(ReadOnlySpan<byte> bin) => Get(bin, WB8.Size, static d => new WB8(d));
    private static WA8[] GetWA8DB(ReadOnlySpan<byte> bin) => Get(bin, WA8.Size, static d => new WA8(d));
    private static WC9[] GetWC9DB(ReadOnlySpan<byte> bin) => Get(bin, WC9.Size, static d => new WC9(d));

    private static T[] Get<T>(ReadOnlySpan<byte> bin, int size, Func<byte[], T> ctor)
    {
        // bin is a multiple of size
        // bin.Length % size == 0
        var result = new T[bin.Length / size];
        Debug.Assert(result.Length * size == bin.Length);
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * size;
            var slice = bin.Slice(offset, size).ToArray();
            result[i] = ctor(slice);
        }
        return result;
    }

    /// <summary>
    /// Reloads the locally stored event templates.
    /// </summary>
    /// <param name="paths">External folder(s) to source individual mystery gift template files from.</param>
    public static void RefreshMGDB(params string[] paths)
    {
        // If no paths are provided, clear the arrays. See the bottom of this method.
        HashSet<PCD>? g4 = null; List<PCD>? lg4 = null;
        HashSet<PGF>? g5 = null; List<PGF>? lg5 = null;
        HashSet<WC6>? g6 = null; List<WC6>? lg6 = null;
        HashSet<WC7>? g7 = null; List<WC7>? lg7 = null;
        HashSet<WB7>? b7 = null; List<WB7>? lb7 = null;
        HashSet<WC8>? g8 = null; List<WC8>? lg8 = null;
        HashSet<WB8>? b8 = null; List<WB8>? lb8 = null;
        HashSet<WA8>? a8 = null; List<WA8>? la8 = null;
        HashSet<WC9>? g9 = null; List<WC9>? lg9 = null;

        // Load external files
        // For each file, load the gift object into the appropriate list.
        foreach (var path in paths)
        {
            if (!Directory.Exists(path))
                continue;
            var gifts = MysteryUtil.GetGiftsFromFolder(path);
            foreach (var gift in gifts)
            {
                var added = gift switch
                {
                    PCD pcd => AddOrExpand(ref g4, ref lg4, pcd),
                    PGF pgf => AddOrExpand(ref g5, ref lg5, pgf),
                    WC6 wc6 => AddOrExpand(ref g6, ref lg6, wc6),
                    WC7 wc7 => AddOrExpand(ref g7, ref lg7, wc7),
                    WB7 wb7 => AddOrExpand(ref b7, ref lb7, wb7),
                    WC8 wc8 => AddOrExpand(ref g8, ref lg8, wc8),
                    WB8 wb8 => AddOrExpand(ref b8, ref lb8, wb8),
                    WA8 wa8 => AddOrExpand(ref a8, ref la8, wa8),
                    WC9 wc9 => AddOrExpand(ref g9, ref lg9, wc9),
                    _ => false,
                };
                if (!added)
                    Trace.WriteLine($"Failed to add gift in {Path.GetDirectoryName(path)}: {gift.FileName}");

                static bool AddOrExpand<T>(ref HashSet<T>? arr, ref List<T>? extra, T obj)
                {
                    if (arr is null)
                    {
                        // Most users won't be adding more than 1-2 gifts
                        // Save memory by initializing the HashSet and List minimally.
                        arr = new HashSet<T>(1);
                        extra = new List<T>(1);
                    }
                    if (arr.Add(obj))
                        extra!.Add(obj);
                    return true;
                }
            }
            EGDB_G4 = SetArray(lg4);
            EGDB_G5 = SetArray(lg5);
            EGDB_G6 = SetArray(lg6);
            EGDB_G7 = SetArray(lg7);
            EGDB_G7GG = SetArray(lb7);
            EGDB_G8 = SetArray(lg8);
            EGDB_G8A = SetArray(la8);
            EGDB_G8B = SetArray(lb8);
            EGDB_G9 = SetArray(lg9);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T[] SetArray<T>(List<T>? update) => update is null ? [] : update.ToArray();
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
            MGDB_G3,
            MGDB_G4,       EGDB_G4,
            MGDB_G5,       EGDB_G5,
            MGDB_G6,       EGDB_G6,
            MGDB_G7,       EGDB_G7,
            MGDB_G7GG,     EGDB_G7GG,
            MGDB_G8,       EGDB_G8,
            MGDB_G8A,      EGDB_G8A,
            MGDB_G8B,      EGDB_G8B,
            MGDB_G9,       EGDB_G9,
        }.SelectMany(z => z);
        var result = regular.Where(mg => mg is { IsItem: false, IsEntity: true, Species: not 0 });
        if (sorted)
            result = result.OrderBy(mg => mg.Species);
        return result;
    }
}
