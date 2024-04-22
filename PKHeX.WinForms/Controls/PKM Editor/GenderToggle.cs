using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Automation;
using static PKHeX.WinForms.Properties.Resources;

namespace PKHeX.WinForms.Controls;

public partial class GenderToggle : UserControl, IGenderToggle
{
    public bool AllowClick { get; set; } = true;

    private int Value = -1; // Initial load will trigger gender to appear (-1 => 0)
    private string? InitialAccessible;
    public static int FocusBorderDeflate { get; set; }

    public byte Gender
    {
        get => (byte)Value;
        set => Value = SetGender(value);
    }

    public GenderToggle() => InitializeComponent();

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Focus();
        base.OnMouseDown(e);
    }

    protected override void OnEnter(EventArgs e)
    {
        Invalidate();
        base.OnEnter(e);
        AccessibilityObject.RaiseAutomationNotification(AutomationNotificationKind.Other,
            AutomationNotificationProcessing.All, AccessibleDescription ?? AccessibleName ?? "");
    }

    protected override void OnLeave(EventArgs e)
    {
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
        if (!Focused)
            return;
        var rc = ClientRectangle;
        rc.Inflate(-FocusBorderDeflate, -FocusBorderDeflate);
        ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
    }

    private static readonly Image[] GenderImages =
    [
        gender_0,
        gender_1,
        gender_2,
    ];

    private int SetGender(int value)
    {
        if ((uint)value > 2)
            value = 2;
        if (Value == value)
            return value;
        BackgroundImage = GenderImages[value];
        AccessibleName = (InitialAccessible ??= AccessibleName) + $" ({value})";
        AccessibleDescription = (InitialAccessible ??= AccessibleName) + $" ({value})";
        return value;
    }

    private void GenderToggle_Click(object sender, EventArgs e)
    {
        if (!AllowClick)
            return;
        TryToggle();
    }

    private void GenderToggle_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode is not (Keys.Enter or Keys.Space))
            return;

        TryToggle();
    }

    private void TryToggle()
    {
        if (AllowClick && ToggleGender().CanToggle)
        {
            AccessibilityObject.RaiseAutomationNotification(AutomationNotificationKind.Other,
                AutomationNotificationProcessing.All, $"Gender changed to {Gender}.");
            return;
        }
        AccessibilityObject.RaiseAutomationNotification(AutomationNotificationKind.Other,
            AutomationNotificationProcessing.All, $"Cannot change gender. Current value is {Gender}.");
    }

    public (bool CanToggle, int Value) ToggleGender()
    {
        if (CanToggle())
            return (true, Gender ^= 1);
        return (false, Gender);
    }

    public bool CanToggle() => (uint)Gender < 2;
}

public interface IGenderToggle
{
    /// <summary>
    /// Enables use of the built-in click action.
    /// </summary>
    bool AllowClick { get; set; }

    /// <summary>
    /// Get or set the value the control displays.
    /// </summary>
    byte Gender { get; set; }

    /// <summary>
    /// Manually flips the gender state if possible.
    /// </summary>
    /// <returns>True if the gender was toggled, and the current Gender value after the operation.</returns>
    (bool CanToggle, int Value) ToggleGender();
}
