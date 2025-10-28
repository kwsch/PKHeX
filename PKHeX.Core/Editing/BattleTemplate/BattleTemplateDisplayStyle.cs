namespace PKHeX.Core;

/// <summary>
/// Token order for displaying the battle template.
/// </summary>
public enum BattleTemplateDisplayStyle : sbyte
{
    Custom = -1,
    Showdown = 0, // default
    Legacy,
    Brief, // default preview hover style
}
