using System;

namespace PKHeX.Core;

public sealed class TrainerIDManager : ITrainerIDControl
{
    private bool _loading;
    private readonly ITrainerIDControl _tid;
    private readonly ITrainerIDControl _sid;
    public byte Generation { private get; set; }

    public event EventHandler? ValueChanged;

    public ITrainerID32 Trainer
    {
        get => field ?? throw new InvalidOperationException("Trainer is not initialized.");
        set;
    }


    public TrainerIDManager(ITrainerIDControl tid, ITrainerIDControl sid)
    {
        _tid = tid;
        _sid = sid;

        tid.ValueChanged += RaiseValueChanged;
        sid.ValueChanged += RaiseValueChanged;
    }

    private void RaiseValueChanged(object? sender, EventArgs e)
    {
        if (_loading)
            return;
        _loading = true;
        SaveTrainer(Trainer);
        if (!_sid.IsValueSame(Trainer))
            _sid.LoadTrainer(Trainer, Trainer.TrainerIDDisplayFormat);
        SetToolTip();
        ValueChanged?.Invoke(this, EventArgs.Empty);
        _loading = false;
    }

    public void LoadTrainer<T>(T trainer) where T : ITrainerID32, IGeneration
    {
        Generation = trainer.Generation;
        LoadTrainer(trainer, trainer.GetTrainerIDFormat());
    }

    public void LoadTrainer(ITrainerID32 trainer, byte generation)
    {
        Generation = generation;
        LoadTrainer(trainer, trainer.GetTrainerIDFormat());
    }

    public void LoadTrainer(ITrainerID32 trainer, TrainerIDFormat displayType)
    {
        Trainer = trainer;
        LoadTrainer();
    }

    public void LoadTrainer()
    {
        _loading = true;
        try
        {
            var trainer = Trainer;
            var format = Trainer.TrainerIDDisplayFormat;
            _tid.LoadTrainer(trainer, format);
            _sid.LoadTrainer(trainer, format);
            SetToolTip();
        }
        finally
        {
            _loading = false;
        }
    }

    public void SaveTrainer(ITrainerID32 trainer)
    {
        _sid.SaveTrainer(trainer);
        _tid.SaveTrainer(trainer);
    }

    public bool IsValueSame(ITrainerID32 trainer) => _tid.IsValueSame(trainer) && _sid.IsValueSame(trainer);

    public void SetToolTip()
    {
        var tsv = Trainer.GetTSV(Generation);
        var text = tsv > ushort.MaxValue
            ? string.Empty
            : $"TSV: {tsv:D4}{Environment.NewLine}{Trainer.GetTextRepresentation()}";

        SetToolTip(text);
    }

    public void SetToolTip(string text)
    {
        _tid.SetToolTip(text);
        _sid.SetToolTip(text);
    }
}

public interface ITrainerIDControl
{
    event EventHandler? ValueChanged;

    void LoadTrainer(ITrainerID32 trainer, TrainerIDFormat displayType);
    void SaveTrainer(ITrainerID32 trainer);
    bool IsValueSame(ITrainerID32 trainer);
    void SetToolTip(string text);
}
