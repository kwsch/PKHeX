using System;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic to detect and update the language and version of a save file.
/// </summary>
/// <remarks>Useful for save files that do not have a language or version saved in the data, or impossible to infer without file names.</remarks>
public static class SaveLanguage
{
    public static LanguageID OverrideLanguageGen1 { get; set; } = English;
    public static GameVersion OverrideVersionGen1 { get; set; } = RB;
    public static LanguageID OverrideLanguageGen2 { get; set; } = English;
    public static GameVersion OverrideVersionGen2 { get; set; } = GS;
    public static LanguageID OverrideLanguageGen3RS { get; set; } = English;
    public static GameVersion OverrideVersionGen3RS { get; set; } = RS;
    public static LanguageID OverrideLanguageGen3FRLG { get; set; } = English;
    public static GameVersion OverrideVersionGen3FRLG { get; set; } = FRLG;

    /// <summary>
    /// Tries to infer the language and version of the save file.
    /// </summary>
    /// <param name="sav">Save file to infer from</param>
    /// <param name="result">Result of the inference. Only use if the result is true.</param>
    /// <returns>True if the language and version could be inferred, false otherwise</returns>
    public static bool TryGetResult(SaveFile sav, out SaveLanguageResult result) => result = sav switch
    {
        SAV1 s1 => s1.InferFrom(),
        SAV2 s2 => s2.InferFrom(),
        SAV3 s3 => s3.InferFrom(),
        _ => default,
    };

    /// <summary>
    /// Updates the language and version of the save file if possible.
    /// </summary>
    /// <param name="sav">Save file to update</param>
    /// <returns>True if the language and version were updated, false otherwise</returns>
    public static bool TryRevise(SaveFile sav)
    {
        if (!TryGetResult(sav, out var result))
            return false;

        sav.Language = (int)result.Language;
        sav.Version = result.Version;

        if (sav is SAV3FRLG s3)
            s3.ResetPersonal(result.Version);

        if (sav.Metadata.FilePath is { } path)
            sav.Metadata.SetExtraInfo(path);

        return true;
    }

    /// <summary>
    /// Infer the language and version of the save file based on the file name.
    /// </summary>
    public static SaveLanguageResult InferFrom(this SAV1 sav)
    {
        if (sav.Metadata.FileName is not { } x)
            return GetFallback(sav);

        var result = InferFrom1(x, sav.Version);
        if (!IsResultValid(result, sav))
            return GetFallback(sav);

        return result;
    }

    private static bool IsResultValid(in SaveLanguageResult result, SAV1 sav)
    {
        if (result == default)
            return false;
        if (sav.Japanese)
            return result.Language == Japanese;
        return result.Language is not Japanese;
    }

    /// <inheritdoc cref="InferFrom(SAV1)"/>
    public static SaveLanguageResult InferFrom(this SAV2 sav)
    {
        if (sav.Metadata.FileName is not { } x)
            return GetFallback(sav);

        var result = InferFrom2(x, sav.Version);
        if (!IsResultValid(result, sav))
            return GetFallback(sav);

        return result;
    }

    private static bool IsResultValid(in SaveLanguageResult result, SAV2 sav)
    {
        if (result == default)
            return false;
        if (sav.Japanese)
            return result.Language == Japanese;
        if (sav.Korean)
            return result.Language == Korean;
        return result.Language is not (Japanese or Korean);
    }

    /// <inheritdoc cref="InferFrom(SAV1)"/>
    public static SaveLanguageResult InferFrom(this SAV3 sav)
    {
        if (sav.Metadata.FileName is not { } x)
            return GetFallback(sav);

        var ver = sav.Version;
        if (ver is FR)
            ver = FRLG; // Reset since the SAV3 impl assumes FR by default
        var result = InferFrom3(x, ver);
        if (!IsResultValid(result, sav))
            return GetFallback(sav);

        return result;
    }

    private static bool IsResultValid(in SaveLanguageResult result, SAV3 sav)
    {
        if (result == default)
            return false;
        if (sav.Japanese != (result.Language == Japanese))
            return false;

        return sav switch
        {
            SAV3RS => result.Version is R or S,
            SAV3E => result.Version is E,
            SAV3FRLG => result.Version is FR or LG,
            _ => false,
        };
    }

    private static bool Contains(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
        => span.Contains(value, StringComparison.OrdinalIgnoreCase);

    private static bool ContainsSpU(ReadOnlySpan<char> span, ReadOnlySpan<char> value)
    {
        if (Contains(span, value))
            return true;

        // Check for underscores too; replace the input w/ spaces to underscore
        Span<char> tmp = stackalloc char[value.Length];
        value.Replace(tmp, ' ', '_');
        return Contains(span, tmp);
    }

    /// <inheritdoc cref="InferFrom(SAV1)"/>
    public static SaveLanguageResult InferFrom1(ReadOnlySpan<char> name, GameVersion hint = Any)
    {
        // Red
        if (MaybeRD(hint)) {
            if (Contains(name, "red")) return (English, RD);
            if (Contains(name, "roug")) return (French, RD);
            if (Contains(name, "rot")) return (German, RD);
            if (Contains(name, "ross")) return (Italian, RD);
            if (Contains(name, "rojo")) return (Spanish, RD);

            if (Contains(name, "赤")) return (Japanese, RD);
            if (Contains(name, "aka")) return (Japanese, RD);
        }
        if (MaybeGN(hint)) {
            if (Contains(name, "blue")) return (English, GN);
            if (Contains(name, "bleu")) return (French, GN);
            if (Contains(name, "blau")) return (German, GN);
            if (Contains(name, "blu")) return (Italian, GN);
            if (Contains(name, "azul")) return (Spanish, GN);

            if (Contains(name, "緑")) return (Japanese, GN);
            if (Contains(name, "mid")) return (Japanese, GN);
        }
        if (MaybeYW(hint)) {
            if (Contains(name, "yell")) return (English, YW);
            if (Contains(name, "jaun")) return (French, YW);
            if (Contains(name, "gelb")) return (German, YW);
            if (Contains(name, "gial")) return (Italian, YW);
            if (Contains(name, "amar")) return (Spanish, YW);

            if (Contains(name, "ピ")) return (Japanese, YW);
            if (Contains(name, "黄")) return (Japanese, YW);
            if (Contains(name, "pika")) return (Japanese, YW);
        }
        if (MaybeBU(hint)) {
            if (Contains(name, "青")) return (Japanese, BU);
            if (Contains(name, "ao")) return (Japanese, BU);
        }
        return default;

        static bool MaybeRD(GameVersion game) => game is RD or RB or RBY or Any;
        static bool MaybeGN(GameVersion game) => game is GN or RB or RBY or Any;
        static bool MaybeBU(GameVersion game) => game is BU or RBY or Any;
        static bool MaybeYW(GameVersion game) => game is YW or RBY or Any;
    }

    /// <inheritdoc cref="InferFrom(SAV1)"/>
    public static SaveLanguageResult InferFrom2(ReadOnlySpan<char> name, GameVersion hint = Any)
    {
        if (MaybeGD(hint)) {
            if (Contains(name, "golde")) return (German, GD);
            if (Contains(name, "gold")) return (English, GD);
            if (ContainsSpU(name, "e oro")) return (Italian, GD);
            if (Contains(name, "oro")) return (Spanish, GD);
            if (ContainsSpU(name, "n or")) return (French, GD);
            if (Contains(name, "金")) return (Japanese, GD);
            if (Contains(name, "금")) return (Korean, GD);

            if (Contains(name, "kin")) return (Japanese, GD);
            if (Contains(name, "geum")) return (Korean, GD);
        }
        if (MaybeSI(hint)) {
            if (Contains(name, "silv")) return (English, SI);
            if (Contains(name, "silb")) return (German, SI);
            if (Contains(name, "plat")) return (Spanish, SI);
            if (ContainsSpU(name, "e arg")) return (Italian, SI);
            if (Contains(name, "arge")) return (French, SI);
            if (Contains(name, "銀")) return (Japanese, SI);
            if (Contains(name, "은")) return (Korean, SI);

            if (Contains(name, "gin")) return (Japanese, SI);
            if (Contains(name, "eun")) return (Korean, SI);
        }
        if (MaybeC(hint)) {
            if (Contains(name, "cry")) return (English, C);
            if (Contains(name, "kri")) return (German, C);
            if (Contains(name, "cristall")) return (Italian, C);
            if (ContainsSpU(name, "on cri")) return (French, C);
            if (ContainsSpU(name, "ón crist")) return (Spanish, C);
            if (Contains(name, "クリ")) return (Japanese, C);
        }

        // Codes
        if (MaybeGD(hint) && Contains(name, "gd")) return (OverrideLanguageGen2, GD);
        if (MaybeSI(hint) && Contains(name, "si")) return (OverrideLanguageGen2, SI);
        return default;

        static bool MaybeGD(GameVersion game) => game is GD or GS or Any;
        static bool MaybeSI(GameVersion game) => game is SI or GS or Any;
        static bool MaybeC(GameVersion game) => game is C or Any;
    }

    /// <inheritdoc cref="InferFrom(SAV1)"/>
    public static SaveLanguageResult InferFrom3(ReadOnlySpan<char> name, GameVersion hint = Any)
    {
        if (MaybeFR(hint)) {
            if (Contains(name, "fir")) return (English, FR);
            if (Contains(name, "feu")) return (French, FR);
            if (Contains(name, "feuer")) return (German, FR);
            if (Contains(name, "fuoco")) return (Italian, FR);
            if (Contains(name, "fueg")) return (Spanish, FR);
            if (Contains(name, "ファイアレッド")) return (Japanese, FR);
        }
        if (MaybeLG(hint)) {
            if (Contains(name, "lea")) return (English, LG);
            if (Contains(name, "vert")) return (French, LG);
            if (Contains(name, "blatt")) return (German, LG);
            if (Contains(name, "fogli")) return (Italian, LG);
            if (Contains(name, "hoja")) return (Spanish, LG);
            if (Contains(name, "リーフグリーン")) return (Japanese, LG);
        }
        if (MaybeR(hint)) {
            if (Contains(name, "ruby")) return (English, R);
            if (Contains(name, "rubí")) return (Spanish, R);
            if (Contains(name, "rubino")) return (Italian, R);
            if (Contains(name, "rubis")) return (French, R);
            if (Contains(name, "rubin")) return (German, R);
            if (Contains(name, "ルビー")) return (Japanese, R);
        }
        if (MaybeS(hint)) {
            if (Contains(name, "sapp")) return (English, S);
            if (Contains(name, "saphir")) return (French, S);
            if (Contains(name, "safir")) return (German, S);
            if (Contains(name, "zafir")) return (Spanish, S);
            if (Contains(name, "zaffiro")) return (Italian, S);
            if (Contains(name, "サファ")) return (Japanese, S);
        }
        if (MaybeE(hint)) {
            if (Contains(name, "esm")) return (Spanish, E);
            if (Contains(name, "smar")) return (German, E);
            if (Contains(name, "smer")) return (Italian, E);
            if (Contains(name, "merau") || ContainsSpU(name, "ion emerald de")) return (French, E);
            if (Contains(name, "emer")) return (English, E);
            if (Contains(name, "エメ")) return (Japanese, E);
        }

        // Codes
        if (MaybeS(hint)) {
            if (Contains(name, "axpj")) return (Japanese, S);
            if (Contains(name, "axp")) return (OverrideLanguageGen3RS, S);
        }
        if (MaybeR(hint)) {
            if (Contains(name, "axvj")) return (Japanese, R);
            if (Contains(name, "axv")) return (OverrideLanguageGen3RS, R);
        }
        if (MaybeE(hint)) {
            if (Contains(name, "bpej")) return (Japanese, E);
            if (Contains(name, "bpe")) return (OverrideLanguageGen3RS, E);
        }
        if (MaybeFR(hint)) {
            if (Contains(name, "bprj")) return (Japanese, FR);
            if (Contains(name, "bpr") || Contains(name, "fr") || Contains(name, "fire")) return (OverrideLanguageGen3FRLG, FR);
        }
        if (MaybeLG(hint)) {
            if (Contains(name, "bpgj")) return (Japanese, LG);
            if (Contains(name, "bpg") || Contains(name, "lg") || Contains(name, "leaf")) return (OverrideLanguageGen3FRLG, LG);
        }
        return default;

        static bool MaybeFR(GameVersion game) => game is FR or FRLG or Any;
        static bool MaybeLG(GameVersion game) => game is LG or FRLG or Any;
        static bool MaybeR(GameVersion game) => game is R or RS or Any;
        static bool MaybeS(GameVersion game) => game is S or RS or Any;
        static bool MaybeE(GameVersion game) => game is E or RS or Any;
    }

    /// <summary>
    /// Gets a safe fallback language and version for the save file.
    /// </summary>
    private static SaveLanguageResult GetFallback(SAV1 sav)
    {
        bool jp = sav.Japanese;
        bool yw = sav.Yellow;
        var lang = jp ? Japanese : OverrideLanguageGen1;
        var ver = yw ? YW : OverrideVersionGen1;
        return (lang, ver);
    }

    /// <inheritdoc cref="GetFallback(SAV1)"/>"/>
    private static SaveLanguageResult GetFallback(SAV2 sav)
    {
        bool jp = sav.Japanese;
        bool kor = sav.Korean;
        var lang = jp ? Japanese : kor ? Korean : OverrideLanguageGen2;
        var ver = sav.Version == C ? C : OverrideVersionGen2;
        return (lang, ver);
    }

    /// <inheritdoc cref="GetFallback(SAV1)"/>"/>
    private static SaveLanguageResult GetFallback(SAV3 sav)
    {
        bool jp = sav.Japanese;
        if (sav is SAV3FRLG)
            return (jp ? Japanese : OverrideLanguageGen3FRLG, OverrideVersionGen3FRLG);
        var lang = jp ? Japanese : OverrideLanguageGen3RS;
        var ver = sav.Version == E ? E : OverrideVersionGen3RS;
        return (lang, ver);
    }
}

/// <summary>
/// Result of the language and version inference.
/// </summary>
/// <param name="Language">Language ID for the save file.</param>
/// <param name="Version">Version ID for the save file.</param>
public readonly record struct SaveLanguageResult(LanguageID Language, GameVersion Version)
{
    public static implicit operator bool(SaveLanguageResult r) => r != default;
    public static implicit operator SaveLanguageResult((LanguageID Language, GameVersion Version) r) => new(r.Language, r.Version);
}
