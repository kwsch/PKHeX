using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Represents information about a slot change during drag-and-drop operations.
/// </summary>
/// <typeparam name="TCursor">The type of the cursor object.</typeparam>
/// <typeparam name="TImageSource">The type of the image source object.</typeparam>
public sealed class SlotChangeInfo<TCursor, TImageSource>
    where TCursor : class
    where TImageSource : class
{
    /// <summary>
    /// Gets or sets a value indicating whether the left mouse button is down.
    /// </summary>
    public bool IsLeftMouseDown { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether a drag-and-drop operation is in progress.
    /// </summary>
    public bool IsDragDropInProgress { get; set; }
    /// <summary>
    /// Gets or sets the current cursor.
    /// </summary>
    public TCursor? Cursor { get; set; }
    /// <summary>
    /// Gets or sets the current file path involved in the drag-and-drop operation.
    /// </summary>
    public string? CurrentPath { get; set; }

    /// <summary>
    /// Slot that is being dragged from.
    /// </summary>
    public SlotViewInfo<TImageSource>? Source { get; set; }

    /// <summary>
    /// Slot that is being dragged to.
    /// </summary>
    public SlotViewInfo<TImageSource>? Destination { get; set; }

    /// <summary>
    /// Resets the slot change information to its default state.
    /// </summary>
    public void Reset()
    {
        IsLeftMouseDown = IsDragDropInProgress = false;
        CurrentPath = null;
        Cursor = null;
    }

    private bool IsSourceParty => Source?.Slot is SlotInfoParty;
    private bool IsDestinationParty => Destination?.Slot is SlotInfoParty;

    /// <summary>
    /// Used to indicate if the changes will alter the player's party data state.
    /// </summary>
    public bool IsDragParty => IsSourceParty || IsDestinationParty;

    /// <summary>
    /// Used to indicate if the changes will involve two slots within the program.
    /// </summary>
    public bool IsDragSwap => Source is not null && Destination is not null;

    /// <summary>
    /// Used to indicate if the changes will involve two slots within the same location.
    /// </summary>
    public bool IsDragSameLocation => (Destination is not null) && (Source?.Equals(Destination) ?? false);
}
