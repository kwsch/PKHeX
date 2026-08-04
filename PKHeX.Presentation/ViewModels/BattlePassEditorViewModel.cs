using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class BattlePassEditorViewModel : ViewModelBase
{
    private readonly SAV4BR _sav;
    private readonly BattlePassAccessor _accessor;

    public BattlePassEditorViewModel(SaveFile sav)
    {
        _sav = (SAV4BR)sav;
        _accessor = _sav.BattlePasses;
        IsSupported = true;

        LoadPasses();
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<BattlePassEntryViewModel> _passes = [];

    [ObservableProperty]
    private BattlePassEntryViewModel? _selectedPass;

    private void LoadPasses()
    {
        Passes.Clear();
        for (int i = 0; i < BattlePassAccessor.PASS_COUNT; i++)
        {
            var pass = _accessor[i];
            Passes.Add(new BattlePassEntryViewModel(i, pass, _sav));
        }

        if (Passes.Count > 0)
            SelectedPass = Passes[0];
    }

    [RelayCommand]
    private void UnlockAll()
    {
        _accessor.UnlockAllCustomPasses();
        _accessor.UnlockAllRentalPasses();
        LoadPasses();
    }

    [RelayCommand]
    private void Save()
    {
        // Changes are made directly to the pass objects which point to the save data
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadPasses();
    }
}

public partial class BattlePassEntryViewModel : ViewModelBase
{
    private readonly BattlePass _pass;
    private readonly SAV4BR _sav;

    public BattlePassEntryViewModel(int index, BattlePass pass, SAV4BR sav)
    {
        Index = index;
        _pass = pass;
        _sav = sav;

        _name = pass.Name;
        _tid = pass.TID;
        _sid = pass.SID;
        _model = pass.Model;
        _skin = pass.Skin;
        
        _greeting = pass.Greeting;
        _sentOut = pass.SentOut;
        _win = pass.Win;
        _lose = pass.Lose;

        _battles = pass.Battles;
    }

    public int Index { get; }
    public string DisplayName => $"{Index + 1:00} - {Name}";

    [ObservableProperty] private string _name;
    partial void OnNameChanged(string value) => _pass.Name = value;

    [ObservableProperty] private ushort _tid;
    partial void OnTidChanged(ushort value) => _pass.TID = value;

    [ObservableProperty] private ushort _sid;
    partial void OnSidChanged(ushort value) => _pass.SID = value;

    [ObservableProperty] private int _model;
    partial void OnModelChanged(int value) => _pass.Model = value;

    [ObservableProperty] private int _skin;
    partial void OnSkinChanged(int value) => _pass.Skin = value;

    [ObservableProperty] private string _greeting;
    partial void OnGreetingChanged(string value) => _pass.Greeting = value;

    [ObservableProperty] private string _sentOut;
    partial void OnSentOutChanged(string value) => _pass.SentOut = value;

    [ObservableProperty] private string _win;
    partial void OnWinChanged(string value) => _pass.Win = value;

    [ObservableProperty] private string _lose;
    partial void OnLoseChanged(string value) => _pass.Lose = value;

    [ObservableProperty] private int _battles;
    partial void OnBattlesChanged(int value) => _pass.Battles = value;
}
