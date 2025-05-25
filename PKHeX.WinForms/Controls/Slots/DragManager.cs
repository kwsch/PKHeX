using System.Drawing;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Manages drag-and-drop operations for slot controls.
/// </summary>
public sealed class DragManager
{
    /// <summary>
    /// Gets the current slot change information for drag-and-drop operations.
    /// </summary>
    public SlotChangeInfo<Cursor, PictureBox> Info { get; private set; } = new();

    /// <summary>
    /// Occurs when an external drag-and-drop operation is requested.
    /// </summary>
    public event DragEventHandler? RequestExternalDragDrop;

    /// <summary>
    /// Requests a drag-and-drop operation.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The drag event arguments.</param>
    public void RequestDD(object sender, DragEventArgs e) => RequestExternalDragDrop?.Invoke(sender, e);

    /// <summary>
    /// Sets the cursor for the specified form and updates the drag info.
    /// </summary>
    /// <param name="f">The form to set the cursor for.</param>
    /// <param name="z">The cursor to set.</param>
    public void SetCursor(Form? f, Cursor? z)
    {
        if (f is not null)
            f.Cursor = z;
        Info.Cursor = z;
    }

    /// <summary>
    /// Resets the cursor for the specified form to the default cursor.
    /// </summary>
    /// <param name="sender">The form to reset the cursor for.</param>
    public void ResetCursor(Form? sender)
    {
        SetCursor(sender, Cursors.Default);
    }

    /// <summary>
    /// Initializes the drag manager and resets the drag info.
    /// </summary>
    public void Initialize()
    {
        Info = new SlotChangeInfo<Cursor, PictureBox>();
    }

    /// <summary>
    /// Resets the drag manager's slot change info.
    /// </summary>
    public void Reset() => Info.Reset();

    /// <summary>
    /// Gets or sets the mouse down position for drag detection.
    /// </summary>
    public Point MouseDownPosition { private get; set; }

    /// <summary>
    /// Gets a value indicating whether a drag operation can be started.
    /// </summary>
    public bool CanStartDrag => Info.IsLeftMouseDown && !Cursor.Position.Equals(MouseDownPosition);
}
