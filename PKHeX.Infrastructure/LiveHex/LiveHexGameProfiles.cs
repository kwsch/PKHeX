using System;
using System.Collections.Generic;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>How a game lays out its box data in console RAM.</summary>
internal enum BoxAddressingMode
{
    /// <summary>Boxes are a contiguous region at a fixed heap offset (SwSh).</summary>
    HeapFlat,

    /// <summary>Boxes are a contiguous region behind a pointer chain (SV, PLA).</summary>
    PointerContiguous,

    /// <summary>Each stored Pokémon is behind its own pointer (BDSP managed heap objects).</summary>
    PointerScatter,
}

/// <summary>
/// Immutable addressing profile for one (game, firmware) pair. Sizes are taken from the loaded
/// <see cref="SaveFile"/> at runtime; this record only carries the console-RAM addressing data.
/// </summary>
internal sealed record LiveHexGameProfile(
    string GameName,
    string GameVersion,
    BoxAddressingMode Mode,
    ulong HeapBoxStart,      // HeapFlat
    string PointerExpr,      // PointerContiguous / PointerScatter box-list pointer
    int ScatterPointerCount) // PointerScatter: entries in the box-list pointer table
{
    public string Label => $"{GameName} v{GameVersion}";
}

/// <summary>
/// The LiveHeX game-support matrix. Every constant here is factual console-RAM offset data mirrored
/// from the upstream PKHeX-Plugins <c>RamOffsets</c>/<c>LP*</c> tables at the pinned commit
/// (see <c>NOTICE.LiveHeX.md</c>). Keys are the sys-botbase title id plus the reported game version.
/// </summary>
internal static class LiveHexGameProfiles
{
    // --- Switch title ids (from sys-botbase getTitleID) ---
    private const string Sword = "0100ABF008968000";
    private const string Shield = "01008DB008C2C000";
    private const string BrilliantDiamond = "0100000011D90000";
    private const string ShiningPearl = "010018E011D92000";
    private const string LegendsArceus = "01001F5010DFA000";
    private const string Scarlet = "0100A3D008C5C000";
    private const string Violet = "01008F6008C5E000";

    private static readonly HashSet<string> SwShTitles = [Sword, Shield];
    private static readonly HashSet<string> BdspTitles = [BrilliantDiamond, ShiningPearl];
    private static readonly HashSet<string> SvTitles = [Scarlet, Violet];

    /// <summary>Whether the save type is supported by LiveHeX at all.</summary>
    public static bool IsSupported(SaveFile sav) => sav is SAV8SWSH or SAV8BS or SAV8LA or SAV9SV;

    public static string GetGameName(SaveFile sav) => sav switch
    {
        SAV8SWSH => "Sword/Shield",
        SAV8BS => "Brilliant Diamond/Shining Pearl",
        SAV8LA => "Legends: Arceus",
        SAV9SV => "Scarlet/Violet",
        _ => sav.Version.ToString(),
    };

    /// <summary>Human-readable list of firmware versions supported for the given save type.</summary>
    public static string GetSupportedVersions(SaveFile sav) => sav switch
    {
        SAV8SWSH => "1.1.1, 1.2.1, 1.3.2",
        SAV8BS => "1.0.0, 1.1.0, 1.1.1, 1.1.2, 1.1.3, 1.2.0, 1.3.0",
        SAV8LA => "1.0.0, 1.0.1, 1.0.2, 1.1.1",
        SAV9SV => "1.0.1, 1.1.0, 1.2.0, 1.3.0, 1.3.1, 1.3.2, 2.0.1, 2.0.2, 3.0.0, 3.0.1, 4.0.0",
        _ => "none",
    };

    /// <summary>Whether the console-reported <paramref name="titleId"/> belongs to the save's game family.</summary>
    public static bool TitleMatchesSave(SaveFile sav, string titleId) => sav switch
    {
        SAV8SWSH => SwShTitles.Contains(titleId),
        SAV8BS => BdspTitles.Contains(titleId),
        SAV8LA => titleId == LegendsArceus,
        SAV9SV => SvTitles.Contains(titleId),
        _ => false,
    };

    /// <summary>
    /// Resolves the addressing profile for the attached console, or <see langword="null"/> when the
    /// firmware version is not in the support matrix.
    /// </summary>
    public static LiveHexGameProfile? Resolve(SaveFile sav, string titleId, string gameVersion)
    {
        var v = gameVersion.Trim();
        return sav switch
        {
            SAV8SWSH => SwSh(v),
            SAV8BS => Bdsp(titleId, v),
            SAV8LA => Pla(v),
            SAV9SV => Sv(v),
            _ => null,
        };
    }

    private static LiveHexGameProfile? SwSh(string v)
    {
        ulong start = v switch
        {
            "1.1.1" => 0x4293D8B0,
            "1.2.1" => 0x4506D890,
            "1.3.0" or "1.3.1" or "1.3.2" => 0x45075880,
            _ => 0,
        };
        return start == 0 ? null : new("Sword/Shield", v, BoxAddressingMode.HeapFlat, start, string.Empty, 0);
    }

    private static LiveHexGameProfile? Sv(string v)
    {
        string ptr = v switch
        {
            "1.0.1" => "[[[main+42DA8E8]+128]+9B0]",
            "1.1.0" => "[[[main+4384B18]+128]+9B0]",
            "1.2.0" => "[[[main+44A98C8]+130]+9B0]",
            "1.3.0" or "1.3.1" => "[[[main+44BFBA8]+130]+9B0]",
            "1.3.2" => "[[[main+44C1C18]+130]+9B0]",
            "2.0.1" => "[[[[main+4622A30]+198]+30]+9D0]",
            "2.0.2" => "[[[[main+4623A30]+198]+30]+9D0]",
            "3.0.0" or "3.0.1" or "4.0.0" => "[[[[main+47350d8]+1C0]+30]+9D0]",
            _ => string.Empty,
        };
        return ptr.Length == 0 ? null : new("Scarlet/Violet", v, BoxAddressingMode.PointerContiguous, 0, ptr, 0);
    }

    private static LiveHexGameProfile? Pla(string v)
    {
        string ptr = v switch
        {
            "1.0.0" => "[[main+4275470]+1F0]+68",
            "1.0.1" => "[[main+427B470]+1F0]+68",
            "1.0.2" => "[[main+427C470]+1F0]+68",
            "1.1.0" or "1.1.1" => "[[main+42BA6B0]+1F0]+68",
            _ => string.Empty,
        };
        return ptr.Length == 0 ? null : new("Legends: Arceus", v, BoxAddressingMode.PointerContiguous, 0, ptr, 0);
    }

    private static LiveHexGameProfile? Bdsp(string titleId, string v)
    {
        bool sp = titleId == ShiningPearl;
        string ptr = (sp, v) switch
        {
            (_, "1.0.0") => "[[[main+4C0ABD8]+520]+C0]+5E0",
            (_, "1.1.0") => "[[[main+4E27C50]+B8]+170]+20",
            (false, "1.1.1") => "[[[[main+4C1DCF8]+B8]+10]+A0]+20",
            (true, "1.1.1") => "[[[[main+4E34DD0]+B8]+10]+A0]+20",
            (_, "1.1.2") => "[[[[main+4E34DD0]+B8]+10]+A0]+20",
            (_, "1.1.3") => "[[[[main+4E59E60]+B8]+10]+A0]+20",
            (_, "1.2.0") => "[[[[main+4E36C58]+B8]+10]+A0]+20",
            (false, "1.3.0") => "[[[[main+4C64DC0]+B8]+10]+A0]+20",
            (true, "1.3.0") => "[[[[main+4E7BE98]+B8]+10]+A0]+20",
            _ => string.Empty,
        };
        var name = sp ? "Shining Pearl" : "Brilliant Diamond";
        return ptr.Length == 0 ? null : new(name, v, BoxAddressingMode.PointerScatter, 0, ptr, 40);
    }
}
