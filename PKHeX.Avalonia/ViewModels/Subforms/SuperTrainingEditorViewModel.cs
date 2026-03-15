using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single super training regimen flag.
/// </summary>
public partial class SuperTrainEntryModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }
    public bool IsDistribution { get; }

    [ObservableProperty]
    private bool _isCompleted;

    public SuperTrainEntryModel(int index, string name, bool isDistribution, bool isCompleted)
    {
        Index = index;
        Name = name;
        IsDistribution = isDistribution;
        _isCompleted = isCompleted;
    }
}

/// <summary>
/// ViewModel for the Super Training Editor subform.
/// Edits Gen 6 super training regimen flags for Pokemon implementing <see cref="ISuperTrainRegimen"/>.
/// </summary>
public partial class SuperTrainingEditorViewModel : ObservableObject
{
    private readonly ISuperTrainRegimen _entity;

    [ObservableProperty]
    private bool _modified;

    [ObservableProperty]
    private bool _secretUnlocked;

    [ObservableProperty]
    private bool _secretComplete;

    /// <summary>Regular super training entries.</summary>
    public ObservableCollection<SuperTrainEntryModel> RegularEntries { get; } = [];

    /// <summary>Distribution super training entries.</summary>
    public ObservableCollection<SuperTrainEntryModel> DistributionEntries { get; } = [];

    public SuperTrainingEditorViewModel(ISuperTrainRegimen pk)
    {
        _entity = pk;
        SecretUnlocked = pk.SecretSuperTrainingUnlocked;
        SecretComplete = pk.SuperTrainSupremelyTrained;
        LoadRegimens();
    }

    private void LoadRegimens()
    {
        for (int i = 0; i < SuperTrainRegimenExtensions.CountRegimen; i++)
        {
            var name = SuperTrainRegimenExtensions.GetRegimenName(i);
            RegularEntries.Add(new SuperTrainEntryModel(i, name, false, _entity.GetRegimenState(i)));
        }

        for (int i = 0; i < SuperTrainRegimenExtensions.CountRegimenDistribution; i++)
        {
            var name = SuperTrainRegimenExtensions.GetRegimenNameDistribution(i);
            DistributionEntries.Add(new SuperTrainEntryModel(i, name, true, _entity.GetRegimenStateDistribution(i)));
        }
    }

    /// <summary>
    /// Saves all flags back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        foreach (var entry in RegularEntries)
            _entity.SetRegimenState(entry.Index, entry.IsCompleted);
        foreach (var entry in DistributionEntries)
            _entity.SetRegimenStateDistribution(entry.Index, entry.IsCompleted);

        _entity.SecretSuperTrainingUnlocked = SecretUnlocked;
        _entity.SuperTrainSupremelyTrained = SecretComplete;
        Modified = true;
    }

    /// <summary>
    /// Sets all regimen flags to completed.
    /// </summary>
    [RelayCommand]
    private void SetAll()
    {
        SecretUnlocked = true;
        SecretComplete = true;

        foreach (var entry in RegularEntries)
            entry.IsCompleted = true;
        foreach (var entry in DistributionEntries)
            entry.IsCompleted = true;
    }

    /// <summary>
    /// Clears all regimen flags.
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        SecretUnlocked = false;
        SecretComplete = false;

        foreach (var entry in RegularEntries)
            entry.IsCompleted = false;
        foreach (var entry in DistributionEntries)
            entry.IsCompleted = false;
    }
}
