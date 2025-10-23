using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
        if (e.State == DrawItemState.Selected)
        {
            using var brush = new LinearGradientBrush(bounds, Color.White, Color.LightGray, 90f);
            graphics.FillRectangle(brush, bounds);
        }
        else
        {
            e.DrawBackground();
        }

        using var flags = new StringFormat();
        flags.Alignment = StringAlignment.Center;
        flags.LineAlignment = StringAlignment.Center;
        using var text = new SolidBrush(ForeColor);
        var tab = TabPages[index];
        graphics.DrawString(tab.Text, Font, text, bounds, flags);
        base.OnDrawItem(e);
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
        Color.FromArgb(248, 152, 096),
        Color.FromArgb(128, 152, 248),
        Color.FromArgb(248, 168, 208),
        Color.FromArgb(112, 224, 112),
        Color.FromArgb(248, 240, 056),
        Color.RosyBrown,
    ];

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var index = e.Index;
        if ((uint)index >= TabPages.Count)
            return;
        var bounds = GetTabRect(index);

        var graphics = e.Graphics;
        if (e.State == DrawItemState.Selected)
        {
            Color c1 = Color.White;
            Color c2 = Color.LightGray;

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                try
                {
                    if (Main.Settings?.Draw is { } settings)
                    {
                        c1 = settings.VerticalSelectPrimary;
                        c2 = settings.VerticalSelectSecondary;
                    }

                    if (IsDarkMode(BackColor))
                    {
                        if (c1.GetBrightness() > 0.6f) c1 = Color.FromArgb(62, 62, 66);
                        if (c2.GetBrightness() > 0.6f) c2 = Color.FromArgb(45, 45, 48);
                    }
                }
                catch { }
            }

            using var brush = new LinearGradientBrush(bounds, c1, c2, 90f);
            graphics.FillRectangle(brush, bounds);

            using var pipBrush = new SolidBrush(SelectedTags[index]);
            var pip = GetTabRect(index) with { Width = bounds.Width / 8 };
            graphics.FillRectangle(pipBrush, pip);
            bounds = bounds with { Width = bounds.Width - pip.Width, X = bounds.X + pip.Width };
        }
        else
        {
            // no need to shift text
            e.DrawBackground();
        }

        using var flags = new StringFormat();
        flags.Alignment = StringAlignment.Center;
        flags.LineAlignment = StringAlignment.Center;
        using var text = new SolidBrush(ForeColor);
        var tab = TabPages[index];
        graphics.DrawString(tab.Text, Font, text, bounds, flags);
    }

    private static bool IsDarkMode(Color backColor) => backColor.GetBrightness() < 0.5f;
}
