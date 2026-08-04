using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PoketchEditorViewModel : ViewModelBase
{
    private readonly SAV4Sinnoh? _sav;

    private static readonly string[] AppNames =
    [
        "Digital Watch", "Calculator", "Memo Pad", "Pedometer", "Party",
        "Friendship Checker", "Dowsing Machine", "Berry Searcher", "Daycare", "History",
        "Counter", "Analog Watch", "Marking Map", "Link Searcher", "Coin Toss",
        "Move Tester", "Calendar", "Dot Artist", "Roulette", "Trainer Counter",
        "Kitchen Timer", "Color Changer", "Matchup Checker", "Stopwatch", "Alarm Clock"
    ];

    private static readonly string[] ColorNames = ["Green", "Yellow", "Orange", "Red", "Purple", "Blue", "Cyan", "White"];

    public PoketchEditorViewModel(SaveFile sav)
    {
        if (sav is SAV4Sinnoh sinnoh)
        {
            _sav = sinnoh;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => _sav?.Version.ToString() ?? "Unknown";

    [ObservableProperty]
    private bool _poketchEnabled;

    [ObservableProperty]
    private int _currentApp;

    [ObservableProperty]
    private int _colorIndex;

    [ObservableProperty]
    private uint _stepCounter;

    [ObservableProperty]
    private ObservableCollection<PoketchAppViewModel> _apps = [];

    public string[] Colors => ColorNames;

    private void LoadData()
    {
        if (_sav is null) return;

        PoketchEnabled = _sav.PoketchEnabled;
        CurrentApp = _sav.CurrentPoketchApp;
        ColorIndex = (int)_sav.PoketchColor;
        StepCounter = _sav.PoketchStepCounter;

        Apps.Clear();
        for (int i = 0; i < 25; i++)
        {
            var app = (PoketchApp)i;
            var unlocked = _sav.GetPoketchAppUnlocked(app);
            var name = i < AppNames.Length ? AppNames[i] : app.ToString();
            Apps.Add(new PoketchAppViewModel(i, name, unlocked));
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (_sav is null) return;

        _sav.PoketchEnabled = PoketchEnabled;
        _sav.CurrentPoketchApp = (sbyte)CurrentApp;
        _sav.PoketchColor = (PoketchColor)ColorIndex;
        _sav.PoketchStepCounter = StepCounter;

        foreach (var app in Apps)
            _sav.SetPoketchAppUnlocked((PoketchApp)app.Index, app.IsUnlocked);
    }

    [RelayCommand]
    private void UnlockAll()
    {
        foreach (var app in Apps)
            app.IsUnlocked = true;
    }

    [RelayCommand]
    private void ResetStepCounter()
    {
        StepCounter = 0;
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public partial class PoketchAppViewModel : ViewModelBase
{
    public PoketchAppViewModel(int index, string name, bool isUnlocked)
    {
        Index = index;
        Name = name;
        _isUnlocked = isUnlocked;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;
}
