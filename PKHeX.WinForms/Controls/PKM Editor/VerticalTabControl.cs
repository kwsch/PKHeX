using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

public class VerticalTabControl : TabControl
{
    public VerticalTabControl()
    {
        Alignment = TabAlignment.Right;
        DrawMode = TabDrawMode.OwnerDrawFixed;
        SizeMode = TabSizeMode.Fixed;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var index = e.Index;
        var tab = TabPages[index];
        var bounds = GetTabRect(index);

        var graphics = e.Graphics;
        if (e.State == DrawItemState.Selected)
        {
            using var brush = new LinearGradientBrush(bounds, Color.White, Color.LightGray, 90f);
            graphics.FillRectangle(brush, bounds);
        }
        else
        {
            e.DrawBackground();
        }

        using var flags = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };
        using var text = new SolidBrush(ForeColor);
        graphics.DrawString(tab.Text, Font, text, bounds, flags);
        base.OnDrawItem(e);
    }
}
