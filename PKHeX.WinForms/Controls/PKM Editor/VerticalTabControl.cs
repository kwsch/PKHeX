using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Aligns tabs to the left side of the control with text displayed horizontally.
/// </summary>
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
        if ((uint)index >= TabPages.Count)
            return;
        var bounds = GetTabRect(index);

        var graphics = e.Graphics;
        DrawBackground(e, bounds, graphics);

        using var flags = new StringFormat();
        flags.Alignment = StringAlignment.Center;
        flags.LineAlignment = StringAlignment.Center;
        using var text = new SolidBrush(ForeColor);
        var tab = TabPages[index];
        graphics.DrawString(tab.Text, Font, text, bounds, flags);
        base.OnDrawItem(e);
    }

    protected static void DrawBackground(DrawItemEventArgs e, Rectangle bounds, Graphics graphics)
    {
        if (e.State != DrawItemState.Selected)
        {
            e.DrawBackground();
            return;
        }

        var (c1, c2) = (SystemColors.ControlLightLight, SystemColors.ScrollBar);
        using var brush = new LinearGradientBrush(bounds, c1, c2, 90f);
        graphics.FillRectangle(brush, bounds);
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        base.ScaleControl(factor, specified);
        ItemSize = new((int)(ItemSize.Width * factor.Width), (int)(ItemSize.Height * factor.Height));
    }
}

/// <summary>
/// Specialized <see cref="VerticalTabControl"/> for displaying a <see cref="PKHeX.Core.PKM"/> editor tabs.
/// </summary>
public sealed class VerticalTabControlEntityEditor : VerticalTabControl
{
    /// <summary>
    /// Tab stripe colors based on Contest Stats.
    /// </summary>
    private static readonly Color[] SelectedTags =
    [
        ContestColor.Cool, // Main
        ContestColor.Beauty, // Met
        ContestColor.Cute, // Stats
        ContestColor.Clever, // Moves
        ContestColor.Tough, // Cosmetic
        Color.RosyBrown, // OT
    ];

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var index = e.Index;
        if ((uint)index >= TabPages.Count)
            return;
        var bounds = GetTabRect(index);

        var graphics = e.Graphics;
        DrawBackground(e, bounds, graphics);
        if (e.State == DrawItemState.Selected)
        {
            // draw colored pip on the left side of the tab
            using var pipBrush = new SolidBrush(SelectedTags[index]);
            var pip = GetTabRect(index) with { Width = bounds.Width / 8 };
            graphics.FillRectangle(pipBrush, pip);

            // shift text to the right to avoid pip overlap
            bounds = bounds with { Width = bounds.Width - pip.Width, X = bounds.X + pip.Width };
        }

        using var flags = new StringFormat();
        flags.Alignment = StringAlignment.Center;
        flags.LineAlignment = StringAlignment.Center;
        using var text = new SolidBrush(ForeColor);
        var tab = TabPages[index];
        graphics.DrawString(tab.Text, Font, text, bounds, flags);
    }
}
