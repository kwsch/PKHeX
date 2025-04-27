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
    None = 0, // invalid
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

    Moves,

    HeldItem,
    Nickname,
    Gender,
}
