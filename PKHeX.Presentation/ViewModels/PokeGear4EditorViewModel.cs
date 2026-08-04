using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokeGear4EditorViewModel : ViewModelBase
{
    private readonly SAV4HGSS? _hgss;

    public PokeGear4EditorViewModel(SaveFile sav)
    {
        _hgss = sav as SAV4HGSS;
        IsSupported = _hgss is not null;

        if (IsSupported)
            LoadCallers();
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<PokeGearCallerViewModel> _callers = [];

    private void LoadCallers()
    {
        Callers.Clear();
        if (_hgss is null) return;

        var rolodex = _hgss.GetPokeGearRoloDex();
        
        // Get all possible callers from the enum
        var values = Enum.GetValues<PokegearNumber>();
        foreach (var caller in values)
        {
            if (caller == PokegearNumber.None) continue;
            
            var isUnlocked = false;
            foreach (var entry in rolodex)
            {
                if (entry == caller)
                {
                    isUnlocked = true;
                    break;
                }
            }
            
            var name = FormatCallerName(caller.ToString());
            Callers.Add(new PokeGearCallerViewModel(caller, name, isUnlocked, OnCallerChanged));
        }
    }

    private static string FormatCallerName(string enumName)
    {
        // Convert "Professor_Elm" to "Professor Elm"
        return enumName.Replace('_', ' ');
    }

    private void OnCallerChanged(PokegearNumber caller, bool isUnlocked)
    {
        if (_hgss is null) return;
        
        var rolodex = _hgss.GetPokeGearRoloDex();
        
        if (isUnlocked)
        {
            // Find first empty slot and add caller
            for (int i = 0; i < rolodex.Length; i++)
            {
                if (rolodex[i] == PokegearNumber.None)
                {
                    rolodex[i] = caller;
                    break;
                }
            }
        }
        else
        {
            // Remove caller from rolodex
            for (int i = 0; i < rolodex.Length; i++)
            {
                if (rolodex[i] == caller)
                {
                    rolodex[i] = PokegearNumber.None;
                    break;
                }
            }
        }
    }

    [RelayCommand]
    private void UnlockAll()
    {
        _hgss?.PokeGearUnlockAllCallers();
        LoadCallers();
    }

    [RelayCommand]
    private void UnlockAllNoTrainers()
    {
        _hgss?.PokeGearUnlockAllCallersNoTrainers();
        LoadCallers();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _hgss?.PokeGearClearAllCallers();
        LoadCallers();
    }
}

public partial class PokeGearCallerViewModel : ViewModelBase
{
    private readonly Action<PokegearNumber, bool> _onChanged;

    public PokeGearCallerViewModel(PokegearNumber caller, string name, bool isUnlocked, Action<PokegearNumber, bool> onChanged)
    {
        Caller = caller;
        Name = name;
        _isUnlocked = isUnlocked;
        _onChanged = onChanged;
    }

    public PokegearNumber Caller { get; }
    public string Name { get; }

    [ObservableProperty]
    private bool _isUnlocked;

    partial void OnIsUnlockedChanged(bool value) => _onChanged(Caller, value);
}
