using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

internal partial class TrainerSID : UserControl, ITrainerIDControl
{
    private TrainerIDFormat Format { get; set; }
    public ToolTip? ToolTip { private get; set; }
    private bool IsFourDigit => Format is TrainerIDFormat.SixDigit;
    private bool IsFiveDigit => Format is TrainerIDFormat.SixteenBit;

    public event EventHandler? ValueChanged;

    public TrainerSID() => InitializeComponent();

    private void RaiseValueChanged(object? sender, EventArgs e) => ValueChanged?.Invoke(sender, e);

    public void LoadTrainer(ITrainerID32 trainer, TrainerIDFormat displayType)
    {
        Format = displayType;
        TB_Five.Visible = IsFiveDigit;
        TB_Four.Visible = IsFourDigit;
        LoadTrainer(trainer);
    }

    private void LoadTrainer(ITrainerID32 trainer)
    {
        if (IsFourDigit)
            TB_Four.Text = trainer.GetTrainerSID7().ToString(TrainerIDExtensions.SID7);
        else if (IsFiveDigit)
            TB_Five.Text = trainer.SID16.ToString(TrainerIDExtensions.SID16);
    }

    public void SaveTrainer(ITrainerID32 trainer)
    {
        if (IsFourDigit)
            Save4(trainer);
        else if (IsFiveDigit)
            Save5(trainer);
    }

    public bool IsValueSame(ITrainerID32 trainer)
    {
        if (IsFourDigit)
            return trainer.GetTrainerSID7() == (uint.TryParse(TB_Four.Text, out var value) ? value : 0);
        if (IsFiveDigit)
            return trainer.SID16 == (ushort.TryParse(TB_Five.Text, out var value) ? value : 0);
        return true;
    }

    private void Save4(ITrainerID32 trainer)
    {
        var box = TB_Four;
        if (box.Text.Length == 0)
        {
            trainer.SetTrainerSID7(0);
            return;
        }

        const uint max = 4294u;
        if (!uint.TryParse(box.Text, out var value))
        {
            value = 0;
        }
        else if (value > max)
        {
            value = max;
            if (!trainer.IsValidTrainerID7(sid7: value, trainer.GetTrainerTID7()))
                value = max - 1;
        }
        else
        {
            if (trainer.IsValidTrainerID7(sid7: value, trainer.GetTrainerTID7()))
            {
                trainer.SetTrainerSID7(value);
                return;
            }
            value = max - 1;
        }

        // Invalid.
        trainer.SetTrainerSID7(value);
        box.Text = value.ToString(TrainerIDExtensions.SID7);
    }

    private void Save5(ITrainerID32 trainer)
    {
        var box = TB_Five;
        if (box.Text.Length == 0)
        {
            trainer.SID16 = 0;
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
            trainer.SID16 = (ushort)value;
            return;
        }

        // Invalid; reset and try again.
        trainer.SID16 = (ushort)value;
        box.Text = value.ToString(TrainerIDExtensions.SID16);
    }

    public void SetToolTip(string text)
    {
        ToolTip?.SetToolTip(TB_Five, text);
        ToolTip?.SetToolTip(TB_Four, text);
    }
}
