namespace PKHeX.Application.Abstractions;

/// <summary>
/// Implemented by dialog ViewModels that can request their own window to close (e.g. a Save/Close
/// button). <see cref="IWindowService"/> wires <see cref="CloseRequested"/> to close the dialog,
/// so the ViewModel never touches the window.
/// </summary>
public interface ICloseableDialog
{
    Action? CloseRequested { get; set; }
}
