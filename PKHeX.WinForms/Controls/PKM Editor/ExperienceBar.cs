using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class ExperienceBar : UserControl
{
    public EventHandler? ValueChanged;
    private byte Growth { get; set; }
    private byte Level { get; set; }
    public uint EXP { get; private set; }

    public ExperienceBar() => InitializeComponent();

    private double CurrentPercent => Experience.GetEXPToLevelUpPercentage(Level, EXP, Growth);
    private void NotifyUpdate() => ValueChanged?.Invoke(this, EventArgs.Empty);
    private int RealWidth => BorderStyle == BorderStyle.None ? Width : Width - SystemInformation.BorderSize.Width * 2;
    private int Border => BorderStyle == BorderStyle.None ? 0 : SystemInformation.BorderSize.Width;

    private uint GetEXPEdgeHigh()
    {
        var next = Experience.GetEXPToLevelUp(Level, Growth);
        if (next == 0)
            return EXP;
        return Experience.GetEXP(Level, Growth) + next - 1;
    }

    private void HandleClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return; // ignore lol

        if (TryAction())
            return;

        var x = e.X;
        var border = Border;
        if (border != 0)
            x = Math.Max(0, x - border);
        SetNewPixelPercent(x, false);
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
    }

    private void SetNewPixelPercent(int newWidth, bool scroll)
    {
        var currentWidth = PAN_ExpPercent.Width;
        if (newWidth == currentWidth)
            return; // unchanged, so do nothing

        var maxWidth = RealWidth;
        // Recalculate EXP, trigger the event, which will trigger another Update.
        if (newWidth < 0)
            Underflow();
        else if (newWidth >= maxWidth)
            Overflow();
        else if (newWidth >= maxWidth + 1)
            EdgeHigh();
        else if (newWidth == 0)
            EdgeLow();
        else // somewhere in between
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
