using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class TrainerID : UserControl
{
    private readonly TrainerIDManager _manager;
    public event EventHandler? UpdatedID;

    public TrainerID()
    {
        InitializeComponent();
        TIDFields.ToolTip = SIDFields.ToolTip = TSVTooltip;
        _manager = new TrainerIDManager(TIDFields, SIDFields);
        _manager.ValueChanged += (sender, e) => UpdatedID?.Invoke(sender, e);
    }

    public void LoadTrainer<T>(T trainer) where T : ITrainerID32, IGeneration
    {
        _manager.LoadTrainer(trainer);
        Label_SID.Visible = trainer.Generation >= 3;
    }

    public void LoadTrainer(ITrainerID32 trainer, byte generation)
    {
        _manager.LoadTrainer(trainer, generation);
        Label_SID.Visible = generation >= 3;
    }

    public void LoadTrainer() => _manager.LoadTrainer();
    public void SetToolTip() => _manager.SetToolTip();
}
