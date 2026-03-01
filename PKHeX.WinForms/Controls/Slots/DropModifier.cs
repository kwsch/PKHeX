namespace PKHeX.WinForms.Controls;

/// <summary>
/// Specifies the modifier for a <see cref="Core.PKM"/> drag-and-drop operation.
/// </summary>
public enum DropModifier
{
    /// <summary>
    /// No modifier is applied.
    /// </summary>
    None,
    /// <summary>
    /// Overwrite the target slot.
    /// </summary>
    Overwrite,
    /// <summary>
    /// Clone the source slot.
    /// </summary>
    Clone,
}
