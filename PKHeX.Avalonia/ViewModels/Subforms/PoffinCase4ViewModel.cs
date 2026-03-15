using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Poffin entry.
/// </summary>
public partial class PoffinEntryModel : ObservableObject
{
    public int Index { get; }

    [ObservableProperty]
    private string _label;

    [ObservableProperty]
    private int _type;

    [ObservableProperty]
    private byte _boostSpicy;

    [ObservableProperty]
    private byte _boostDry;

    [ObservableProperty]
    private byte _boostSweet;

    [ObservableProperty]
    private byte _boostBitter;

    [ObservableProperty]
    private byte _boostSour;

    [ObservableProperty]
    private byte _smoothness;

    public PoffinEntryModel(int index, string label, Poffin4 poffin)
    {
        Index = index;
        _label = label;
        _type = (int)poffin.Type;
        _boostSpicy = poffin.BoostSpicy;
        _boostDry = poffin.BoostDry;
        _boostSweet = poffin.BoostSweet;
        _boostBitter = poffin.BoostBitter;
        _boostSour = poffin.BoostSour;
        _smoothness = poffin.Smoothness;
    }

    public void ApplyTo(Poffin4 poffin)
    {
        poffin.Type = (PoffinFlavor4)Type;
        poffin.BoostSpicy = BoostSpicy;
        poffin.BoostDry = BoostDry;
        poffin.BoostSweet = BoostSweet;
        poffin.BoostBitter = BoostBitter;
        poffin.BoostSour = BoostSour;
        poffin.Smoothness = Smoothness;
    }
}

/// <summary>
/// ViewModel for the Poffin Case editor (Gen 4 Sinnoh).
/// Edits the poffin case contents.
/// </summary>
public partial class PoffinCase4ViewModel : SaveEditorViewModelBase
{
    private readonly PoffinCase4 Case;

    public ObservableCollection<PoffinEntryModel> Poffins { get; } = [];

    [ObservableProperty]
    private PoffinEntryModel? _selectedPoffin;

    public PoffinCase4ViewModel(SAV4Sinnoh sav) : base(sav)
    {
        Case = new PoffinCase4(sav);
        LoadPoffins();
    }

    private void LoadPoffins()
    {
        Poffins.Clear();
        for (int i = 0; i < Case.Poffins.Length; i++)
        {
            var poffin = Case.Poffins[i];
            var name = GetPoffinName(poffin.Type);
            Poffins.Add(new PoffinEntryModel(i, $"{i + 1:000} - {name}", poffin));
        }
        if (Poffins.Count > 0)
            SelectedPoffin = Poffins[0];
    }

    private static string GetPoffinName(PoffinFlavor4 flavor)
    {
        return flavor.ToString().Replace('_', '-');
    }

    [RelayCommand]
    private void FillAll()
    {
        Case.FillCase();
        LoadPoffins();
    }

    [RelayCommand]
    private void DeleteAll()
    {
        Case.DeleteAll();
        LoadPoffins();
    }

    [RelayCommand]
    private void Save()
    {
        for (int i = 0; i < Poffins.Count && i < Case.Poffins.Length; i++)
            Poffins[i].ApplyTo(Case.Poffins[i]);
        Case.Save();

        SAV.State.Edited = true;
        Modified = true;
    }
}
