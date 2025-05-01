using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Enum for the different tokens used in battle templates.
/// </summary>
/// <remarks>
/// Each token represents a specific aspect of a Pokémon's battle template.
/// One token per line. Each token can have specific grammar rules depending on the language.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter<BattleTemplateToken>))]
public enum BattleTemplateToken : byte
{
    None = 0, // invalid, used as a magic value to signal that a token is not recognized

    // Standard tokens
    Shiny,
    Ability,
    Nature,
    Friendship,
    EVs,
    IVs,
    Level,
    DynamaxLevel,
    Gigantamax,
    TeraType,

    // Tokens that can appear multiple times
    Moves,

    // When present, first line will not contain values for these tokens (instead outputting on separate token line)
    // Not part of the standard export format, but can be recognized/optionally used in the program
    HeldItem,
    Nickname,
    Gender,

    // Manually appended, not stored or recognized on import
    AVs,
    GVs,

    // Future Showdown propositions
    AbilityHeldItem, // [Ability] Item
    EVsWithNature, // +/-
    EVsAppendNature, // +/- and .. (Nature)

    // Omitting the first line (species) shouldn't be done unless it is manually added in the presentation/export.
    FirstLine = byte.MaxValue, 
}
