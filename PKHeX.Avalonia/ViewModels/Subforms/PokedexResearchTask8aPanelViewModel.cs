using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for a single Legends Arceus research task panel.
/// Displays thresholds and current value for a research task.
/// </summary>
public partial class PokedexResearchTask8aPanelViewModel : ObservableObject
{
    public ushort Species { get; private set; }
    public PokedexResearchTask8a Task { get; private set; } = new();
    public int ReportedCount { get; private set; }

    public string TaskLabel { get; private set; } = string.Empty;
    public int PointsPerLevel => Task.PointsSingle + Task.PointsBonus;
    public bool HasBonus => Task.PointsBonus != 0;
    public bool CanSetCurrentValue => Task.Task.CanSetCurrentValue();

    [ObservableProperty] private int _currentValue;

    [ObservableProperty] private string _threshold1 = string.Empty;
    [ObservableProperty] private string _threshold2 = string.Empty;
    [ObservableProperty] private string _threshold3 = string.Empty;
    [ObservableProperty] private string _threshold4 = string.Empty;
    [ObservableProperty] private string _threshold5 = string.Empty;

    private string[] _taskDescriptions = [];
    private string[] _speciesQuests = [];
    private string[] _timeTaskDescriptions = [];

    public void SetStrings(string[] tasks, string[] speciesQuests, string[] timeTasks)
    {
        _taskDescriptions = tasks;
        _speciesQuests = speciesQuests;
        _timeTaskDescriptions = timeTasks;
    }

    public void SetTask(ushort species, PokedexResearchTask8a task, int reportedLevel)
    {
        Species = species;
        Task = task;
        ReportedCount = reportedLevel - 1;

        TaskLabel = task.GetTaskLabelString(_taskDescriptions, _timeTaskDescriptions, _speciesQuests);

        var thresholds = task.TaskThresholds;
        Threshold1 = thresholds.Length > 0 ? thresholds[0].ToString() : string.Empty;
        Threshold2 = thresholds.Length > 1 ? thresholds[1].ToString() : string.Empty;
        Threshold3 = thresholds.Length > 2 ? thresholds[2].ToString() : string.Empty;
        Threshold4 = thresholds.Length > 3 ? thresholds[3].ToString() : string.Empty;
        Threshold5 = thresholds.Length > 4 ? thresholds[4].ToString() : string.Empty;

        OnPropertyChanged(nameof(TaskLabel));
        OnPropertyChanged(nameof(HasBonus));
        OnPropertyChanged(nameof(CanSetCurrentValue));
        OnPropertyChanged(nameof(PointsPerLevel));
    }
}
