namespace PKHeX.Core;

/// <summary>
/// Exposes <see cref="OT_Affection"/> and <see cref="HT_Affection"/> properties used by Gen6/7.
/// </summary>
public interface IAffection
{
    byte OT_Affection { get; set; }
    byte HT_Affection { get; set; }
}
