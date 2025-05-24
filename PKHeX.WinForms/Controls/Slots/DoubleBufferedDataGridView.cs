using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Provides a DataGridView with double buffering enabled to reduce flicker.
/// </summary>
internal class DoubleBufferedDataGridView : DataGridView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleBufferedDataGridView"/> class.
    /// </summary>
    public DoubleBufferedDataGridView() => SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
}
