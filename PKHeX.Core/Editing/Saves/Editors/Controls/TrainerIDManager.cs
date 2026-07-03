using System;

namespace PKHeX.Core;

/// <summary>
/// Wrapper for managing Trainer ID and Secret ID controls, ensuring synchronization and proper event handling between them.
/// </summary>
public sealed class TrainerIDManager : ITrainerIDControl
{
    private bool _loading;
    private readonly ITrainerIDControl _tid;
    private readonly ITrainerIDControl _sid;

    /// <summary>
    /// Cached value used to prepare the Trainer Shiny Value tooltip.
    /// </summary>
    private byte Generation { get; set; }

    public event EventHandler? ValueChanged;

    /// <summary>
    /// Gets or sets the current <see cref="ITrainerID32"/> instance.
    /// Throws an <see cref="InvalidOperationException"/> if accessed before initialization.
    /// </summary>
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

/// <summary>
/// Interface for controls that manage Trainer ID and Secret ID values, providing methods for loading, saving, and comparing trainer information, as well as setting tooltips.
/// </summary>
public interface ITrainerIDControl
{
    /// <summary>
    /// Event that is raised when the Trainer ID or Secret ID value changes. This event is triggered after the internal values are synchronized and saved.
    /// </summary>
    event EventHandler? ValueChanged;

    void LoadTrainer(ITrainerID32 trainer, TrainerIDFormat displayType);
    void SaveTrainer(ITrainerID32 trainer);
    bool IsValueSame(ITrainerID32 trainer);
    void SetToolTip(string text);
}
