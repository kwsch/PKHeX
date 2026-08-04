using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Event Work editor for SAV7b (Let's Go).
/// Handles specific integer work variables and flags.
/// </summary>
public partial class EventWorkEditorViewModel : ViewModelBase
{
    private readonly dynamic? _eventWork; // Use dynamic to bypass restrictive interface checks if needed
    private readonly bool _supported;

    public EventWorkEditorViewModel(SaveFile sav)
    {
        if (sav is SAV7b s7b)
        {
            _eventWork = s7b.Blocks.EventWork;
            _supported = true;
            
            // Get counts safely
            WorkCount = _eventWork?.CountWork ?? 0;
            FlagCount = _eventWork?.CountFlag ?? 0;
        }
        else
        {
            _supported = false;
        }
    }

    public bool IsSupported => _supported;
    public int WorkCount { get; }
    public int FlagCount { get; }

    [ObservableProperty] private int _selectedWorkIndex;
    [ObservableProperty] private int _selectedWorkValue;

    [ObservableProperty] private int _selectedFlagIndex;
    [ObservableProperty] private bool _selectedFlagValue;

    partial void OnSelectedWorkIndexChanged(int value)
    {
        if (_supported && value >= 0 && value < WorkCount)
        {
            SelectedWorkValue = (int)_eventWork!.GetWork(value);
        }
    }

    partial void OnSelectedFlagIndexChanged(int value)
    {
        if (_supported && value >= 0 && value < FlagCount)
        {
            SelectedFlagValue = (bool)_eventWork!.GetFlag(value);
        }
    }

    [RelayCommand]
    private void ApplyWork()
    {
        if (_supported && SelectedWorkIndex >= 0 && SelectedWorkIndex < WorkCount)
        {
            _eventWork!.SetWork(SelectedWorkIndex, SelectedWorkValue);
        }
    }

    [RelayCommand]
    private void ApplyFlag()
    {
        if (_supported && SelectedFlagIndex >= 0 && SelectedFlagIndex < FlagCount)
        {
            _eventWork!.SetFlag(SelectedFlagIndex, SelectedFlagValue);
        }
    }
}
