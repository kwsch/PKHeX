using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class ChatterEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IChatter? _chatter;

    public ChatterEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        _chatter = GetChatter(sav);

        IsSupported = _chatter is not null;

        if (IsSupported)
            LoadData();
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private bool _initialized;

    partial void OnInitializedChanged(bool value)
    {
        if (_chatter is not null)
        {
            _chatter.Initialized = value;
            OnPropertyChanged(nameof(ConfusionChance));
        }
    }

    public int ConfusionChance => _chatter?.ConfusionChance ?? 0;

    [ObservableProperty]
    private bool _hasRecording;

    private void LoadData()
    {
        if (_chatter is null) return;

        Initialized = _chatter.Initialized;
        HasRecording = _chatter.Initialized || !IsRecordingEmpty();
    }

    private bool IsRecordingEmpty()
    {
        if (_chatter is null) return true;
        foreach (var b in _chatter.Recording)
        {
            if (b != 0) return false;
        }
        return true;
    }

    [RelayCommand]
    private void ClearRecording()
    {
        if (_chatter is null) return;

        var recording = _chatter.Recording;
        for (int i = 0; i < recording.Length; i++)
            recording[i] = 0;

        _chatter.Initialized = false;
        LoadData();
    }

    [RelayCommand]
    private void Refresh() => LoadData();

    private static IChatter? GetChatter(SaveFile sav)
    {
        try
        {
            return sav switch
            {
                SAV4 sav4 => sav4.Chatter,
                SAV5 sav5 => sav5.Chatter,
                _ => null
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            // SAV4.Chatter accesses Buffer which is empty for blank saves
            return null;
        }
    }
}
