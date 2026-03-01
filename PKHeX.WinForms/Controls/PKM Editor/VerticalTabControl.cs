using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PKHeX.Drawing;
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
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;

        using var bgBrush = new SolidBrush(BackColor);
        g.FillRectangle(bgBrush, ClientRectangle);

        for (int i = 0; i < TabCount; i++)
            DrawTab(g, TabPages[i], i);
    }

    private void DrawTab(Graphics g, TabPage page, int index)
    {
        var bounds = GetTabRect(index);

        // Draw tab background
        if (index == SelectedIndex)
        {
            var (c1, c2) = (DefaultBackColor, ColorUtil.Blend(DefaultBackColor, SystemColors.ScrollBar, 0.4));
            using var brush = new LinearGradientBrush(bounds, c1, c2, 90f);
            g.FillRectangle(brush, bounds);
        }

        // Draw dark grey border around tab
        ControlPaint.DrawBorder(g, bounds, SystemColors.ActiveBorder, Application.IsDarkModeEnabled ? ButtonBorderStyle.Outset : ButtonBorderStyle.Solid);

        // Draw text
        const TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
        TextRenderer.DrawText(g, page.Text, Font, bounds, ForeColor, flags);
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

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;

        // Fill background
        using (var bgBrush = new SolidBrush(BackColor))
            g.FillRectangle(bgBrush, ClientRectangle);

        // Draw each tab
        for (int i = 0; i < TabCount; i++)
            DrawCustomTab(g, TabPages[i], i);
    }

    private void DrawCustomTab(Graphics g, TabPage page, int index)
    {
        var bounds = GetTabRect(index);

        // Draw tab background
        if (index == SelectedIndex)
        {
            var (c1, c2) = (DefaultBackColor, ColorUtil.Blend(DefaultBackColor, SystemColors.ScrollBar, 0.4));
            using var brush = new LinearGradientBrush(bounds, c1, c2, 90f);
            g.FillRectangle(brush, bounds);

            // Draw colored pip on the left side of the tab
            using var pipBrush = new SolidBrush(SelectedTags[index]);
            var pip = bounds with { Width = bounds.Width / 8 };
            g.FillRectangle(pipBrush, pip);

            // Shift text to the right to avoid pip overlap
            bounds = bounds with { Width = bounds.Width - pip.Width, X = bounds.X + pip.Width };
        }

        // Draw dark grey border around tab
        ControlPaint.DrawBorder(g, GetTabRect(index), SystemColors.ActiveBorder, Application.IsDarkModeEnabled ? ButtonBorderStyle.Outset : ButtonBorderStyle.Solid);

        // Draw text
        const TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
        TextRenderer.DrawText(g, page.Text, Font, bounds, ForeColor, flags);
    }
}
