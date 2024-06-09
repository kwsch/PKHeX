using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Bundles raw string inputs into lists that can be used in data binding.
/// </summary>
public sealed class GameDataSource
{
    /// <summary>
    /// List of <see cref="Region3DSIndex"/> values to display.
    /// </summary>
    public static readonly IReadOnlyList<ComboItem> Regions = new List<ComboItem>
    {
        new ("Japan (日本)",      0),
        new ("Americas (NA/SA)",  1),
        new ("Europe (EU/AU)",    2),
        new ("China (中国大陆)",   4),
        new ("Korea (한국)",       5),
        new ("Taiwan (香港/台灣)", 6),
    };

    /// <summary>
    /// List of <see cref="LanguageID"/> values to display.
    /// </summary>
    private static readonly ComboItem[] LanguageList =
    [
        new ComboItem("JPN (日本語)",   (int)LanguageID.Japanese),
        new ComboItem("ENG (English)",  (int)LanguageID.English),
        new ComboItem("FRE (Français)", (int)LanguageID.French),
        new ComboItem("ITA (Italiano)", (int)LanguageID.Italian),
        new ComboItem("GER (Deutsch)",  (int)LanguageID.German),
        new ComboItem("ESP (Español)",  (int)LanguageID.Spanish),
        new ComboItem("KOR (한국어)",    (int)LanguageID.Korean),
        new ComboItem("CHS (简体中文)",  (int)LanguageID.ChineseS),
        new ComboItem("CHT (繁體中文)",  (int)LanguageID.ChineseT),
    ];

    /// <summary>
    /// Gets a list of languages to display based on the generation.
    /// </summary>
    /// <param name="generation">Generation to get the language list for.</param>
    /// <returns>List of languages to display.</returns>
    public static IReadOnlyList<ComboItem> LanguageDataSource(byte generation) => generation switch
    {
          3 => LanguageList[..6], // No Korean+
        < 7 => LanguageList[..7], // No Chinese+
          _ => [.. LanguageList],
    };

    public GameDataSource(GameStrings s)
    {
        Strings = s;
        BallDataSource = GetBalls(s.itemlist);
        SpeciesDataSource = Util.GetCBList(s.specieslist);
        NatureDataSource = Util.GetCBList(s.natures);
        AbilityDataSource = Util.GetCBList(s.abilitylist);
        GroundTileDataSource = Util.GetUnsortedCBList(s.groundtiletypes, GroundTileTypeExtensions.ValidTileTypes);

        var moves = Util.GetCBList(s.movelist);
        HaXMoveDataSource = moves;
        var legal = new List<ComboItem>(moves.Count);
        foreach (var m in moves)
        {
            if (MoveInfo.IsMoveKnowable((ushort)m.Value))
                legal.Add(m);
        }
        LegalMoveDataSource = legal;

        var games = GetVersionList(s);
        VersionDataSource = games;

        Met = new MetDataSource(s);

        Empty = new ComboItem(s.itemlist[0], 0);
        games[^1] = Empty;
    }

    /// <summary> Strings that this object's lists were generated with. </summary>
    public readonly GameStrings Strings;

    /// <summary> Contains Met Data lists to source lists from. </summary>
    public readonly MetDataSource Met;

    /// <summary> Represents "(None)", localized to this object's language strings. </summary>
    public readonly ComboItem Empty;

    public readonly IReadOnlyList<ComboItem> SpeciesDataSource;
    public readonly IReadOnlyList<ComboItem> BallDataSource;
    public readonly IReadOnlyList<ComboItem> NatureDataSource;
    public readonly IReadOnlyList<ComboItem> AbilityDataSource;
    public readonly IReadOnlyList<ComboItem> VersionDataSource;
    public readonly IReadOnlyList<ComboItem> LegalMoveDataSource;
    public readonly IReadOnlyList<ComboItem> HaXMoveDataSource;
    public readonly IReadOnlyList<ComboItem> GroundTileDataSource;

    /// <summary>
    /// Preferentially ordered list of <see cref="GameVersion"/> values to display in a list.
    /// </summary>
    /// <remarks>Most recent games are at the top, loosely following Generation groups.</remarks>
    private static ReadOnlySpan<byte> OrderedVersionArray =>
    [
        50, 51, // 9 S/V
        47,     // 8 PLA
        48, 49, // 8 BD/SP
        44, 45, // 8 SW/SH
        42, 43, // 7 LGP/E
        30, 31, // 7 S/M
        32, 33, // 7 US/UM
        24, 25, // 6 X/Y
        27, 26, // 6 OR/AS
        21, 20, // 5 B/W
        23, 22, // 5 B2/W2
        10, 11, 12, // 4 D/P/Pt
        07, 08, // 4 HG/SS
        02, 01, 03, // 3 R/S/E
        04, 05, // 3 FR/LG
        15,     // 3 Colosseum & XD

        39, 40, 41, // 7vc2 (Gen2 VC)
        35, 36, 37, 38, // 7vc1 (Gen1 VC)
        34, // 7go

        00,
    ];

    private static ComboItem[] GetBalls(ReadOnlySpan<string> itemList) => Util.GetVariedCBListBall(itemList, BallStoredIndexes, BallItemIDs);

    // Since Poké Ball (and Great Ball / Ultra Ball) are most common, any list should have them at the top. The rest can be sorted alphabetically.
    private static ReadOnlySpan<byte> BallStoredIndexes => [ 004, 003, 002, 001, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 0027, 0028, 0029, 0030, 0031, 0032, 0033, 0034, 0035, 0036, 0037 ];
    private static ReadOnlySpan<ushort> BallItemIDs     => [ 004, 003, 002, 001, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 492, 493, 494, 495, 496, 497, 498, 499, 576, 851, 1785, 1710, 1711, 1712, 1713, 1746, 1747, 1748, 1749, 1750, 1771 ];

    private static ComboItem[] GetVersionList(GameStrings s)
    {
        var list = s.gamelist;
        return Util.GetUnsortedCBList(list, OrderedVersionArray);
    }

    public List<ComboItem> GetItemDataSource(GameVersion game, EntityContext context, ReadOnlySpan<ushort> allowed, bool HaX = false)
    {
        var items = Strings.GetItemStrings(context, game);
        return HaX ? Util.GetCBList(items) : Util.GetCBList(items, allowed);
    }
}
