using System;
using System.Text.Json.Serialization;

namespace PKHeX.Core;

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
}
