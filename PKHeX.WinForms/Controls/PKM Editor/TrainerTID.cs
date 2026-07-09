using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

internal partial class TrainerTID : UserControl, ITrainerIDControl
{
    private TrainerIDFormat Format { get; set; }
    public ToolTip? ToolTip { private get; set; }
    private bool IsSixDigit => Format is TrainerIDFormat.SixDigit;
    public event EventHandler? ValueChanged;

    public TrainerTID() => InitializeComponent();

    private void RaiseValueChanged(object? sender, EventArgs e) => ValueChanged?.Invoke(sender, e);

    public void LoadTrainer(ITrainerID32 trainer, TrainerIDFormat displayType)
    {
        Format = displayType;
        TB_Five.Visible = !IsSixDigit;
        TB_Six.Visible = IsSixDigit;

        if (IsSixDigit)
            TB_Six.Text = trainer.GetTrainerTID7().ToString(TrainerIDExtensions.TID7);
        else
            TB_Five.Text = trainer.TID16.ToString(TrainerIDExtensions.TID16);
    }

    public void SaveTrainer(ITrainerID32 trainer)
    {
        if (IsSixDigit)
            Save6(trainer);
        else 
            Save5(trainer);
    }
    public bool IsValueSame(ITrainerID32 trainer)
    {
        if (IsSixDigit)
            return trainer.GetTrainerTID7() == (uint.TryParse(TB_Six.Text, out var value) ? value : 0);
        else
            return trainer.TID16 == (ushort.TryParse(TB_Five.Text, out var value) ? value : 0);
    }

    private void Save5(ITrainerID32 trainer)
    {
        var box = TB_Five;
        if (box.Text.Length == 0)
        {
            trainer.TID16 = 0;
            return;
        }

        const ushort max = ushort.MaxValue;
        if (!uint.TryParse(box.Text, out var value))
        {
            value = 0;
        }
        else if (value > max)
        {
            value = max;
        }
        else
        {
            // Valid value. Set to object.
            trainer.TID16 = (ushort)value;
            return;
        }

        // Invalid; reset and try again.
        trainer.TID16 = (ushort)value;
        box.Text = value.ToString(TrainerIDExtensions.TID16);
    }

    private void Save6(ITrainerID32 trainer)
    {
        var box = TB_Six;
        if (box.Text.Length == 0)
        {
            trainer.SetTrainerTID7(0);
            return;
        }

        const uint max = 999_999u;
        if (!uint.TryParse(box.Text, out var value))
        {
            value = 0;
        }
        if (value > max)
        {
            value = max;
        }
        else if (value == max && (trainer.IsValidTrainerID7(sid7: value, trainer.GetTrainerTID7())))
        {
            value = max - 1;
        }
        else
        {
            // Valid value. Set to object.
            trainer.SetTrainerTID7(value);
            return;
        }

        // Invalid; reset and try again.
        trainer.SetTrainerTID7(value);
        box.Text = value.ToString(TrainerIDExtensions.TID7);
    }

    public void SetToolTip(string text)
    {
        ToolTip?.SetToolTip(TB_Five, text);
        ToolTip?.SetToolTip(TB_Six, text);
    }
}
