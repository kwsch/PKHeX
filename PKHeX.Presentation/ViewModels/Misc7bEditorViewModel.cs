using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Misc editor for Let's Go Pikachu/Eevee (SAV7b) saves.
/// </summary>
public partial class Misc7bEditorViewModel : ViewModelBase
{
    private readonly SAV7b _sav;

    public Misc7bEditorViewModel(SAV7b sav)
    {
        _sav = sav;
        LoadData();
    }

    #region Trainer Info

    [ObservableProperty] private uint _money;
    [ObservableProperty] private string _rivalName = string.Empty;

    private void LoadData()
    {
        Money = _sav.Blocks.Misc.Money;
        RivalName = _sav.Blocks.Misc.RivalName;
    }

    private void SaveData()
    {
        _sav.Blocks.Misc.Money = Money;
        _sav.Blocks.Misc.RivalName = RivalName;
    }

    [RelayCommand]
    private void MaxMoney()
    {
        Money = 9_999_999;
    }

    #endregion

    #region Go Park

    public int GoParkCount => GoParkStorage.Count;
    public int GoParkSlotCount => GoParkStorage.SlotsPerArea;
    public int GoParkAreaCount => GoParkStorage.Count / GoParkStorage.SlotsPerArea;

    [RelayCommand]
    private void DeleteAllGoPark()
    {
        _sav.Park.DeleteAll();
    }

    #endregion

    #region Unlocks

    [RelayCommand]
    private void UnlockAllTrainerTitles()
    {
        _sav.Blocks.EventWork.UnlockAllTitleFlags();
    }

    [RelayCommand]
    private void UnlockAllFashion()
    {
        _sav.Blocks.FashionPlayer.UnlockAllAccessoriesPlayer();
        _sav.Blocks.FashionStarter.UnlockAllAccessoriesStarter();
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Save()
    {
        SaveData();
    }

    #endregion
}
