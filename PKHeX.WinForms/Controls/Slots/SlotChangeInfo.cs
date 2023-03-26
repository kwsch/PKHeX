using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public sealed class SlotChangeInfo<TCursor, TImageSource> where TCursor : class where TImageSource : class
{
    public bool LeftMouseIsDown { get; set; }
    public bool DragDropInProgress { get; set; }

    public TCursor? Cursor { get; set; }
    public string? CurrentPath { get; set; }

    public SlotViewInfo<TImageSource>? Source { get; set; }
    public SlotViewInfo<TImageSource>? Destination { get; set; }

    public void Reset()
    {
        LeftMouseIsDown = DragDropInProgress = false;
        CurrentPath = null;
        Cursor = default;
    }

    public bool SameLocation => (Destination != null) && (Source?.Equals(Destination) ?? false);

    private bool SourceIsParty => Source?.Slot is SlotInfoParty;
    private bool DestinationIsParty => Destination?.Slot is SlotInfoParty;

    /// <summary>
    /// Used to indicate if the changes will alter the player's party data state.
    /// </summary>
    public bool DragIsParty => SourceIsParty || DestinationIsParty;

    public bool DragIsSwap => Source is not null && Destination is not null;
}
