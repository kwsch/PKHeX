using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

internal class DoubleBufferedDataGridView : DataGridView
{
    public DoubleBufferedDataGridView() => SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
}
