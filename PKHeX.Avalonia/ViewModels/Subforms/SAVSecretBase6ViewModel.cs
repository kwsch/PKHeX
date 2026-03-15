using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 ORAS Secret Base editor.
/// Edits base list, decorations, trainer info.
/// </summary>
public partial class SAVSecretBase6ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV6AO _sav;
    private bool _loading;

    public ObservableCollection<string> BaseNames { get; } = [];

    [ObservableProperty]
    private int _selectedBaseIndex = -1;

    // Base properties (displayed via PropertyGrid-like fields)
    [ObservableProperty]
    private string _trainerName = string.Empty;

    [ObservableProperty]
    private int _baseLocation;

    [ObservableProperty]
    private string _teamName = string.Empty;

    [ObservableProperty]
    private string _teamSlogan = string.Empty;

    // Decoration placement
    [ObservableProperty]
    private int _placementIndex;

    [ObservableProperty]
    private int _placementGood;

    [ObservableProperty]
    private int _placementX;

    [ObservableProperty]
    private int _placementY;

    [ObservableProperty]
    private int _placementRotation;

    [ObservableProperty]
    private uint _capturedRecord;

    private SecretBase6? _currentBase;
    private int _currentPlacementIndex = -1;

    public SAVSecretBase6ViewModel(SAV6AO sav) : base(sav)
    {
        _sav = (SAV6AO)(_origin = sav).Clone();

        CapturedRecord = (uint)_sav.Records.GetRecord(080);
        ReloadBaseList();

        if (BaseNames.Count > 0)
            SelectedBaseIndex = 0;
    }

    private void ReloadBaseList()
    {
        _loading = true;
        BaseNames.Clear();
        var block = _sav.SecretBase;
        var self = block.GetSecretBaseSelf();
        BaseNames.Add($"* {self.TrainerName}");
        for (int i = 0; i < SecretBase6Block.OtherSecretBaseCount; i++)
        {
            var other = block.GetSecretBaseOther(i);
            string name = other.TrainerName;
            if (string.IsNullOrWhiteSpace(name))
                name = "Empty";
            BaseNames.Add($"{i + 1:00} {name}");
        }
        _loading = false;
    }

    partial void OnSelectedBaseIndexChanged(int value)
    {
        if (value < 0 || _loading)
            return;
        SaveCurrentBase();
        LoadBase(value);
    }

    partial void OnPlacementIndexChanged(int value)
    {
        if (_currentBase == null || _loading)
            return;
        SavePlacement();
        LoadPlacement(value);
    }

    private void LoadBase(int index)
    {
        _loading = true;
        _currentPlacementIndex = -1;
        _currentBase = index == 0
            ? _sav.SecretBase.GetSecretBaseSelf()
            : _sav.SecretBase.GetSecretBaseOther(index - 1);

        TrainerName = _currentBase.TrainerName;
        BaseLocation = _currentBase.BaseLocation;
        TeamName = _currentBase.TeamName;
        TeamSlogan = _currentBase.TeamSlogan;

        PlacementIndex = 0;
        LoadPlacement(0);
        _loading = false;
    }

    private void LoadPlacement(int index)
    {
        if (_currentBase == null || index < 0 || index >= SecretBase6.COUNT_GOODS)
            return;
        _currentPlacementIndex = index;
        var p = _currentBase.GetPlacement(index);
        PlacementGood = p.Good;
        PlacementX = p.X;
        PlacementY = p.Y;
        PlacementRotation = p.Rotation;
    }

    private void SavePlacement()
    {
        if (_currentBase == null || _currentPlacementIndex < 0)
            return;
        var p = _currentBase.GetPlacement(_currentPlacementIndex);
        p.Good = (ushort)PlacementGood;
        p.X = (ushort)PlacementX;
        p.Y = (ushort)PlacementY;
        p.Rotation = (byte)PlacementRotation;
    }

    private void SaveCurrentBase()
    {
        if (_currentBase == null)
            return;
        SavePlacement();
        _currentBase.TrainerName = TrainerName;
        _currentBase.BaseLocation = BaseLocation;
        _currentBase.TeamName = TeamName;
        _currentBase.TeamSlogan = TeamSlogan;
    }

    [RelayCommand]
    private void GiveAllDecor()
    {
        _sav.SecretBase.GiveAllGoods();
    }

    [RelayCommand]
    private void DeleteBase()
    {
        if (SelectedBaseIndex < 1)
            return;
        int index = SelectedBaseIndex - 1;
        _sav.SecretBase.DeleteOther(index);
        _currentBase = null;
        _currentPlacementIndex = -1;
        ReloadBaseList();
        SelectedBaseIndex = index + 1;
    }

    [RelayCommand]
    private void Save()
    {
        SaveCurrentBase();
        _sav.Records.SetRecord(080, (int)CapturedRecord);
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
