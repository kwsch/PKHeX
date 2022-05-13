using System.Drawing;
using System.Windows.Forms;
using static PKHeX.WinForms.Properties.Resources;

namespace PKHeX.WinForms.Controls;

public partial class GenderToggle : UserControl, IGenderToggle
{
    public bool AllowClick { get; set; } = true;

    private int Value = -1; // Initial load will trigger gender to appear (-1 => 0)

    public int Gender
    {
        get => Value;
        set => Value = SetGender(value);
    }

    public GenderToggle() => InitializeComponent();

    private static readonly Image[] GenderImages =
    {
        gender_0,
        gender_1,
        gender_2,
    };

    private int SetGender(int value)
    {
        if ((uint)value > 2)
            value = 2;
        if (Value != value)
            BackgroundImage = GenderImages[value];
        return value;
    }

    private void GenderToggle_Click(object sender, System.EventArgs e)
    {
        if (!AllowClick)
            return;
        ToggleGender();
    }

    public (bool CanToggle, int Value) ToggleGender()
    {
        if ((uint)Gender < 2)
            return (true, Gender ^= 1);
        return (false, Gender);
    }
}

public interface IGenderToggle
{
    /// <summary>
    /// Enables use of the built in click action.
    /// </summary>
    bool AllowClick { get; set; }

    /// <summary>
    /// Get or set the value the control displays.
    /// </summary>
    int Gender { get; set; }

    /// <summary>
    /// Manually flips the gender state if possible.
    /// </summary>
    /// <returns>True if can toggle, and the resulting value.</returns>
    (bool CanToggle, int Value) ToggleGender();
}
