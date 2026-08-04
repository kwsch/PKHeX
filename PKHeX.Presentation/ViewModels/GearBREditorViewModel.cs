using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Editor for Battle Revolution (SAV4BR) Gear/Outfits.
/// </summary>
public partial class GearBREditorViewModel : ViewModelBase
{
    private readonly SAV4BR _sav;

    public GearBREditorViewModel(SAV4BR sav)
    {
        _sav = sav;
        LoadData();
    }

    #region Shiny Outfits

    [ObservableProperty] private bool _shinyGroudon;
    [ObservableProperty] private bool _shinyLucario;
    [ObservableProperty] private bool _shinyElectivire;
    [ObservableProperty] private bool _shinyKyogre;
    [ObservableProperty] private bool _shinyRoserade;
    [ObservableProperty] private bool _shinyPachirisu;

    private void LoadData()
    {
        ShinyGroudon = _sav.GearShinyGroudonOutfit;
        ShinyLucario = _sav.GearShinyLucarioOutfit;
        ShinyElectivire = _sav.GearShinyElectivireOutfit;
        ShinyKyogre = _sav.GearShinyKyogreOutfit;
        ShinyRoserade = _sav.GearShinyRoseradeOutfit;
        ShinyPachirisu = _sav.GearShinyPachirisuOutfit;
    }

    private void SaveData()
    {
        _sav.GearShinyGroudonOutfit = ShinyGroudon;
        _sav.GearShinyLucarioOutfit = ShinyLucario;
        _sav.GearShinyElectivireOutfit = ShinyElectivire;
        _sav.GearShinyKyogreOutfit = ShinyKyogre;
        _sav.GearShinyRoseradeOutfit = ShinyRoserade;
        _sav.GearShinyPachirisuOutfit = ShinyPachirisu;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void UnlockAll()
    {
        _sav.GearUnlock.UnlockAll();
        ShinyGroudon = true;
        ShinyLucario = true;
        ShinyElectivire = true;
        ShinyKyogre = true;
        ShinyRoserade = true;
        ShinyPachirisu = true;
    }

    [RelayCommand]
    private void ClearAll()
    {
        _sav.GearUnlock.Clear();
        ShinyGroudon = false;
        ShinyLucario = false;
        ShinyElectivire = false;
        ShinyKyogre = false;
        ShinyRoserade = false;
        ShinyPachirisu = false;
    }

    [RelayCommand]
    private void Save()
    {
        SaveData();
    }

    #endregion
}
