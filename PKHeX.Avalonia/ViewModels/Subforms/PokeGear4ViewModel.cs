using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single PokeGear contact entry.
/// </summary>
public partial class PokeGearEntryModel : ObservableObject
{
    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private int _callerId;

    public PokeGearEntryModel(int index, string name, int callerId)
    {
        Index = index;
        Name = name;
        _callerId = callerId;
    }
}

/// <summary>
/// ViewModel for the PokeGear contact editor (Gen 4 HGSS).
/// Edits the PokeGear rolodex.
/// </summary>
public partial class PokeGear4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4HGSS _origin;
    private readonly SAV4HGSS SAV4H;

    public ObservableCollection<PokeGearEntryModel> Contacts { get; } = [];

    public PokeGear4ViewModel(SAV4HGSS sav) : base(sav)
    {
        _origin = sav;
        SAV4H = (SAV4HGSS)sav.Clone();
        LoadContacts();
    }

    private void LoadContacts()
    {
        Contacts.Clear();
        var rolodex = SAV4H.GetPokeGearRoloDex();
        for (int i = 0; i < rolodex.Length; i++)
        {
            var caller = rolodex[i];
            var name = caller.ToString().Replace('_', ' ');
            Contacts.Add(new PokeGearEntryModel(i, name, (int)caller));
        }
    }

    [RelayCommand]
    private void UnlockAll()
    {
        SAV4H.PokeGearUnlockAllCallers();
        LoadContacts();
    }

    [RelayCommand]
    private void UnlockAllNoTrainers()
    {
        SAV4H.PokeGearUnlockAllCallersNoTrainers();
        LoadContacts();
    }

    [RelayCommand]
    private void ClearAll()
    {
        SAV4H.PokeGearClearAllCallers();
        LoadContacts();
    }

    [RelayCommand]
    private void Save()
    {
        var rolodex = new PokegearNumber[Contacts.Count];
        for (int i = 0; i < Contacts.Count; i++)
            rolodex[i] = (PokegearNumber)Contacts[i].CallerId;
        SAV4H.SetPokeGearRoloDex(rolodex);
        _origin.CopyChangesFrom(SAV4H);
        Modified = true;
    }
}
