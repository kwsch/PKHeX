using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for Gen 3 Secret Bases (RSE).
/// </summary>
public partial class SecretBase3EditorViewModel : ViewModelBase
{
    private readonly SAV3 _sav;
    private readonly SecretBaseManager3 _manager;

    public SecretBase3EditorViewModel(SAV3 sav)
    {
        _sav = sav;
        _manager = (sav switch { SAV3RS rs => (ISaveBlock3LargeHoenn)rs.LargeBlock, SAV3E e => e.LargeBlock, _ => throw new ArgumentException("Not a Hoenn Gen3 save") }).SecretBases;
        LoadBases();
    }

    public ObservableCollection<SecretBase3ViewModel> Bases { get; } = [];

    [ObservableProperty] private SecretBase3ViewModel? _selectedBase;

    private void LoadBases()
    {
        Bases.Clear();
        foreach (var b in _manager.Bases)
        {
            Bases.Add(new SecretBase3ViewModel(b));
        }
    }

    [RelayCommand]
    private void Save()
    {
        // Copy data back from ViewModels
        var list = new System.Collections.Generic.List<SecretBase3>();
        foreach (var vm in Bases)
        {
            vm.ApplyChanges();
            list.Add(vm.Base);
        }
        _manager.Bases = list;
        _manager.Save();
    }

    [RelayCommand]
    private void ClearSelected()
    {
        if (SelectedBase is null) return;
        
        // Clear the selected base
        SelectedBase.Name = string.Empty;
        SelectedBase.Tid = 0;
        SelectedBase.Sid = 0;
        SelectedBase.TimesEntered = 0;
    }
}

public partial class SecretBase3ViewModel : ViewModelBase
{
    public SecretBase3 Base { get; }

    public SecretBase3ViewModel(SecretBase3 secretBase)
    {
        Base = secretBase;
        _name = secretBase.OriginalTrainerName;
        _tid = secretBase.TID16;
        _sid = secretBase.SID16;
        _timesEntered = secretBase.TimesEntered;
        _battledToday = secretBase.BattledToday;
        _registered = secretBase.RegistryStatus == 1;
        _gender = secretBase.OriginalTrainerGender;
    }

    [ObservableProperty] private string _name;
    [ObservableProperty] private ushort _tid;
    [ObservableProperty] private ushort _sid;
    [ObservableProperty] private byte _timesEntered;
    [ObservableProperty] private bool _battledToday;
    [ObservableProperty] private bool _registered;
    [ObservableProperty] private byte _gender;

    public string GenderSymbol => Gender == 0 ? "♂" : "♀";

    public void ApplyChanges()
    {
        Base.OriginalTrainerName = Name;
        Base.TID16 = Tid;
        Base.SID16 = Sid;
        Base.TimesEntered = TimesEntered;
        Base.BattledToday = BattledToday;
        Base.RegistryStatus = Registered ? 1 : 0;
        Base.OriginalTrainerGender = Gender;
    }
}
