using System.Drawing;
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
    public DoubleBufferedDataGridView()
    {
        if (Application.IsDarkModeEnabled) // NET10
        {
            EnableHeadersVisualStyles = false;
            BorderStyle = BorderStyle.None;
            RowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Drawing.ColorUtil.Blend(SystemColors.ControlLight, SystemColors.ControlLightLight, 0.45) };
        }

        AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = SystemColors.ControlLight };
    }

    protected override CreateParams CreateParams
    {
        get
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            return base.CreateParams;
        }
    }
}
