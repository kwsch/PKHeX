using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class ExperienceBar : UserControl
{
    public EventHandler? ValueChanged;
    private bool IsDragging { get; set; }
    private string? HoverText { get; set; }
    private byte Growth { get; set; }
    private byte Level { get; set; }
    public uint EXP { get; private set; }

    public ExperienceBar() => InitializeComponent();

    private double CurrentPercent => Experience.GetEXPToLevelUpPercentage(Level, EXP, Growth);
    private void NotifyUpdate() => ValueChanged?.Invoke(this, EventArgs.Empty);
    private int RealWidth => BorderStyle == BorderStyle.None ? Width : Width - (SystemInformation.BorderSize.Width * 2);
    private int Border => BorderStyle == BorderStyle.None ? 0 : SystemInformation.BorderSize.Width;

    private uint GetEXPEdgeHigh()
    {
        var next = Experience.GetEXPToLevelUp(Level, Growth);
        if (next == 0)
            return EXP;
        return Experience.GetEXP(Level, Growth) + next - 1;
    }

    private uint GetEXPAtWidth(int width)
    {
        var start = Experience.GetEXP(Level, Growth);
        var range = Experience.GetEXPToLevelUp(Level, Growth);
        var maxWidth = RealWidth;
        if (range == 0 || maxWidth <= 0)
            return start;

        var progress = (uint)((width * range) / maxWidth);
        if (progress >= range)
            progress = range - 1;
        return start + progress;
    }

    private int GetRelativeX(object? sender, MouseEventArgs e) => sender == PAN_ExpPercent ? PAN_ExpPercent.Left + e.X : e.X;

    private uint GetHoverEXP(int x)
    {
        if (Level >= Experience.MaxLevel)
            return EXP;

        var maxWidth = RealWidth;
        if (maxWidth <= 0)
            return Experience.GetEXP(Level, Growth);

        var width = Math.Clamp(x - Border, 0, maxWidth);
        if (width == 0)
            return Experience.GetEXP(Level, Growth);
        if (width == maxWidth)
            return GetEXPEdgeHigh();
        return GetEXPAtWidth(width);
    }

    private string GetHoverText(int x)
    {
        var start = Experience.GetEXP(Level, Growth);
        var current = GetHoverEXP(x);
        if (ModifierKeys.HasFlag(Keys.Control))
            current = EXP;

        var gained = current - start;
        var range = Experience.GetEXPToLevelUp(Level, Growth);
        var remain = range - gained;
        return $"{gained}/{range} (-{remain})" + Environment.NewLine + $"{current} {((float)gained*100)/range:F0}%";
    }

    private void ShowHover(int x)
    {
        var text = GetHoverText(x);
        if (HoverText == text && Width > 0)
        {
            TT_Exp.Show(text, this, Math.Clamp(x, 0, Width - 1), Height + 2, TT_Exp.AutoPopDelay);
            return;
        }

        HoverText = text;
        var tooltipX = Width <= 0 ? 0 : Math.Clamp(x, 0, Width - 1);
        TT_Exp.Show(text, this, tooltipX, Height + 2, TT_Exp.AutoPopDelay);
    }

    private void HideHover()
    {
        HoverText = null;
        TT_Exp.Hide(this);
    }

    private void HandleMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        if (TryAction())
        {
            HideHover();
            return;
        }

        IsDragging = true;
        Capture = true;

        var x = GetRelativeX(sender, e);
        SetBoundedPixelPercent(x);
        ShowHover(x);
    }

    private int lastMouseMoveX = int.MinValue;

    private void HandleMouseMove(object? sender, MouseEventArgs e)
    {
        var x = GetRelativeX(sender, e);
        if (x == lastMouseMoveX)
            return;
        if (IsDragging && e.Button.HasFlag(MouseButtons.Left))
            SetBoundedPixelPercent(x);

        ShowHover(x);
        lastMouseMoveX = x;
    }

    private void HandleMouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        IsDragging = false;
        Capture = false;
        ShowHover(GetRelativeX(sender, e));
    }

    private void HandleMouseEnter(object? sender, EventArgs e) => ShowHover(PointToClient(MousePosition).X);

    private void HandleMouseLeave(object? sender, EventArgs e)
    {
        if (!IsDragging)
            HideHover();
    }

    private void HandleMouseCaptureChanged(object? sender, EventArgs e)
    {
        if (Capture)
            return;

        IsDragging = false;
        if (!ClientRectangle.Contains(PointToClient(MousePosition)))
            HideHover();
    }

    private bool TrySetEXPWithinLevel(int newWidth)
    {
        if (Level >= Experience.MaxLevel)
            return false;

        var maxWidth = RealWidth;
        if (maxWidth <= 0)
            return false;

        var width = Math.Clamp(newWidth, 0, maxWidth);
        var original = EXP;
        if (width == 0)
            EdgeLow();
        else if (width == maxWidth)
            EdgeHigh();
        else
            EXP = GetEXPAtWidth(width);

        return EXP != original;
    }

    private void SetBoundedPixelPercent(int x)
    {
        if (TrySetEXPWithinLevel(x - Border))
            NotifyUpdate();
    }

    /// <summary>
    /// Returns true if an action was taken, false if the caller should handle it otherwise.
    /// </summary>
    public bool TryAction()
    {
        if (ModifierKeys.HasFlag(Keys.Alt))
        {
            if (ModifierKeys.HasFlag(Keys.Control))
                DownlevelNoEXP();
            else if (EXP != Experience.GetEXP(Level, Growth))
                EdgeLow();
            else
                Underflow();
            NotifyUpdate();
            return true;
        }

        if (Level >= Experience.MaxLevel)
            return true;

        if (ModifierKeys.HasFlag(Keys.Shift))
        {
            if (!ModifierKeys.HasFlag(Keys.Control) && EXP != GetEXPEdgeHigh())
                EdgeHigh();
            else
                Overflow();
            NotifyUpdate();
            return true;
        }

        return false;
    }

    private void OnScroll(object? sender, MouseEventArgs e)
    {
        if ((Level >= Experience.MaxLevel && e.Delta > 0) || (Level <= Experience.MinLevel && e.Delta < 0 && PAN_ExpPercent.Width == 0))
            return;

        int value = 0;
        const int increment = 1;
        if (e.Delta > 0)
            value += increment;
        else if (e.Delta < 0)
            value -= increment;
        else
            return;

        SetNewPixelPercent(PAN_ExpPercent.Width + value, true);
        ShowHover(GetRelativeX(sender, e));
    }

    private void SetNewPixelPercent(int newWidth, bool scroll)
    {
        var currentWidth = PAN_ExpPercent.Width;
        if (newWidth == currentWidth)
            return; // unchanged, so do nothing

        var maxWidth = RealWidth;
        if (newWidth < 0)
        {
            Underflow();
        }
        else if (newWidth >= maxWidth)
        {
            Overflow();
        }
        else if (newWidth >= maxWidth + 1)
        {
            EdgeHigh();
        }
        else if (newWidth == 0)
        {
            EdgeLow();
        }
        else
        {
            var range = Experience.GetEXPToLevelUp(Level, Growth);
            var pixelsPerEXP = (double)maxWidth / range;

            double delta = newWidth - currentWidth;
            // If there aren't enough pixels to represent 1 EXP, round up to ensure at least 1 EXP is gained/lost per scroll increment.
            if (pixelsPerEXP > 1 && Math.Abs(delta) < pixelsPerEXP)
                delta = Math.Sign(delta) * pixelsPerEXP;

            var adjust = (uint)(int)(delta / pixelsPerEXP);
            var newEXP = unchecked(EXP + adjust);

            // don't allow clicking to change levels, in the event the user is trying to manually edge via clicking.
            // allow scrolling to change levels over/underflow.
            if (!scroll && Experience.GetLevel(newEXP, Growth) != Level)
                return;
            EXP = newEXP;
        }
        NotifyUpdate();
    }

    public void DownlevelNoEXP()
    {
        if (Level <= Experience.MinLevel)
            return;
        EXP = Experience.GetEXP((byte)(Level - 1), Growth);
    }

    public void Overflow()
    {
        if (Level >= Experience.MaxLevel)
            return;
        EXP = Experience.GetEXP((byte)(Level + 1), Growth);
    }

    public void Underflow()
    {
        if (Level <= Experience.MinLevel)
            return;
        EXP = Experience.GetEXP(Level, Growth) - 1;
    }

    public void EdgeLow()
    {
        EXP = Experience.GetEXP(Level, Growth);
    }

    public void EdgeHigh()
    {
        if (Level >= Experience.MaxLevel)
            return;
        EXP = GetEXPEdgeHigh();
    }

    public void Update(uint exp, byte growth) => Update(exp, growth, Experience.GetLevel(exp, growth));

    public void Update(uint exp, byte growth, byte level)
    {
        EXP = exp;
        Growth = growth;
        Level = level;

        if (level >= Experience.MaxLevel)
        {
            PAN_ExpPercent.Width = 0;
            return;
        }

        SetSizePercentFull();
    }

    private void SetSizePercentFull()
    {
        // If progress to next level is not entirely empty, round up to at least 1 pixel to show progress.
        // If we're off-by-one from the next level, the conversion from percent to int will auto round down to hide 1 pixel.
        var gainedPercent = CurrentPercent;
        var newWidth = (int)(gainedPercent * RealWidth);
        if (newWidth == 0 && gainedPercent != 0)
            newWidth = 1;
        PAN_ExpPercent.Width = newWidth;
    }
}
