using PKHeX.Core;

namespace PKHeX.Web.Services;

/// <summary>
/// How a Pokémon's name follows its nickname flag and language, as the desktop editor applies it (<c>PKMEditor.UpdateIsNicknamed</c>,
/// <c>UpdateNickname</c> and <c>IsPossibleNotNicknamed</c> in PKHeX.WinForms), and what the game would show for it.
/// </summary>
/// <remarks>
/// Pure functions over Core's species names and fonts; they change nothing. Written for the formats this release edits (Generation 6):
/// the desktop's extra rules for eggs (their default name is the egg name) and for Generation 5 transfers (names in capitals) do not apply,
/// because eggs are not edited and no Generation 5 format is opened.
/// </remarks>
public static class NameRules
{
    /// <summary>
    /// True when <paramref name="name"/> is <paramref name="species"/>'s name in at least one of the context's languages, so the name may
    /// belong to a Pokémon that was never nicknamed. Mirrors <c>PKMEditor.IsPossibleNotNicknamed</c>.
    /// </summary>
    /// <param name="species">The species.</param>
    /// <param name="name">The name to check.</param>
    /// <param name="context">The format whose languages and spelling are checked.</param>
    public static bool IsPossiblyNotNicknamed(ushort species, string name, EntityContext context) =>
        !SpeciesName.IsNicknamedAnyLanguage(species, name, context);

    /// <summary>
    /// The nickname flag after the name was typed as <paramref name="name"/>: set when it already was, or when the name is not the species'
    /// name in any language. A typed species name never clears the flag. Mirrors <c>PKMEditor.UpdateIsNicknamed</c>.
    /// </summary>
    /// <param name="pk">The entity being edited, for its species, format and highest species number.</param>
    /// <param name="name">The typed name.</param>
    /// <param name="isNicknamed">The flag before the name was typed.</param>
    public static bool FlagAfterTyping(PKM pk, string name, bool isNicknamed)
    {
        if (isNicknamed || !HasSpeciesName(pk))
        {
            return isNicknamed;
        }
        return !IsPossiblyNotNicknamed(pk.Species, name, pk.Context);
    }

    /// <summary>
    /// The name of a Pokémon that is not nicknamed, after its flag was cleared or its language changed: the species' default name in
    /// <paramref name="language"/>, unless <paramref name="name"/> is already the species' name in some language, which is kept.
    /// Mirrors <c>PKMEditor.UpdateNickname</c>. A language the format has no names for (see <see cref="DefaultName"/>) also keeps the name,
    /// rather than writing an empty or foreign one.
    /// </summary>
    /// <param name="pk">The entity being edited, for its species, format and highest species number.</param>
    /// <param name="name">The current name.</param>
    /// <param name="language">The language whose default name is used.</param>
    public static string NameAfterReset(PKM pk, string name, int language)
    {
        if (!HasSpeciesName(pk))
        {
            // The desktop clears the name of an invalid species; this release keeps the stored text instead of writing an empty name.
            return name;
        }
        if (IsPossiblyNotNicknamed(pk.Species, name, pk.Context))
        {
            return name;
        }
        return DefaultName(pk, language) ?? name;
    }

    /// <summary>
    /// The species' default name in <paramref name="language"/>, spelled as the entity's format stores it, or null when there is none: the
    /// species is outside the format, or the language is not one of the format's game languages
    /// (<see cref="PKHeX.Core.Language.GetAvailableGameLanguages"/>), such as a stored 0, the unused 6, or a Chinese language in
    /// Generation 6. Core would give an empty or foreign name for those.
    /// </summary>
    public static string? DefaultName(PKM pk, int language)
    {
        if (!HasSpeciesName(pk) || language is < 0 or > byte.MaxValue || !PKHeX.Core.Language.GetAvailableGameLanguages(pk.Context).Contains((byte)language))
        {
            return null;
        }
        return SpeciesName.GetSpeciesNameGeneration(pk.Species, language, pk.Format);
    }

    /// <summary>
    /// What the name means for <paramref name="pk"/> as it stands: its default name, whether a name kept from another language stands
    /// in for it, and how the game's font shows it.
    /// </summary>
    /// <param name="pk">The entity to describe; it is read only.</param>
    /// <param name="saveLanguage">The save's language, which decides the font the game uses for the name.</param>
    public static NameStatus Describe(PKM pk, int saveLanguage)
    {
        var name = pk.Nickname;
        var defaultName = DefaultName(pk, pk.Language);
        var isDefault = name == defaultName;
        var keptOther = !pk.IsNicknamed && !isDefault && defaultName is not null && IsPossiblyNotNicknamed(pk.Species, name, pk.Context);
        var language = (LanguageID)pk.Language;
        var save = (LanguageID)saveLanguage;
        var undefined = StringFontUtil.HasUndefinedCharacters(name, pk.Context, language, save);
        var displayed = undefined ? StringFontUtil.ReplaceUndefinedCharacters(name, pk.Context, language, save) : name;
        return new NameStatus(defaultName, isDefault, keptOther, undefined, displayed);
    }

    /// <summary>True when the species is one the format can name; the desktop skips its name rules otherwise.</summary>
    private static bool HasSpeciesName(PKM pk) => pk.Species is not 0 && pk.Species <= pk.MaxSpeciesID;
}

/// <summary>What a Pokémon's name means, as described by <see cref="NameRules.Describe"/>.</summary>
/// <param name="DefaultName">
/// The species' default name in the Pokémon's language, or null when there is none (see <see cref="NameRules.DefaultName"/>).
/// </param>
/// <param name="IsDefault">True when the name is that default name.</param>
/// <param name="KeptOtherLanguageName">
/// True when the Pokémon is not nicknamed and keeps the species' name in another language, as the desktop does after a language change.
/// Core's legality check may report it.
/// </param>
/// <param name="HasUndefinedCharacters">True when the game's font cannot show some characters of the name.</param>
/// <param name="Displayed">The name as the game would show it, with undefined characters replaced by the font's placeholder.</param>
public sealed record NameStatus(string? DefaultName, bool IsDefault, bool KeptOtherLanguageName, bool HasUndefinedCharacters, string Displayed);
