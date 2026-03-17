using System;
using Avalonia.Controls;

namespace PKHeX.Avalonia.Views;

/// <summary>
/// Base class for subform windows that use an OK/Cancel pattern.
/// </summary>
public class SubformWindow : Window
{
    /// <summary>
    /// Indicates whether the subform made modifications that should be persisted.
    /// </summary>
    public bool Modified { get; protected set; }

    /// <summary>
    /// Closes the window, optionally marking it as having saved changes.
    /// </summary>
    protected void CloseWithResult(bool save)
    {
        Modified = save;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        (DataContext as IDisposable)?.Dispose();
    }
}
