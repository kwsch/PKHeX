namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Abstraction over a box viewer that the seek tool drives: it reads the current
/// position and asks the viewer to jump to a matching slot. Keeps
/// <see cref="EntitySeekViewModel"/> decoupled from the box viewer's internals
/// (and independently testable with a stub).
/// </summary>
public interface IBoxNavigator
{
    /// <summary>Number of boxes in the save.</summary>
    int BoxCount { get; }

    /// <summary>Slots per box in the save.</summary>
    int SlotsPerBox { get; }

    /// <summary>The currently displayed box index.</summary>
    int CurrentBox { get; }

    /// <summary>The currently selected slot index within the current box.</summary>
    int CurrentSlot { get; }

    /// <summary>Selects the given box/slot, loading the box if needed and highlighting the slot.</summary>
    void NavigateTo(int box, int slot);
}
