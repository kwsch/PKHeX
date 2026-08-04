using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class FestivalPlazaEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly JoinFesta7? _festa;

    public FestivalPlazaEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV7 sav7)
        {
            _festa = sav7.Festa;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private string _plazaName = string.Empty;

    partial void OnPlazaNameChanged(string value)
    {
        if (_festa is not null)
            _festa.FestivalPlazaName = value;
    }

    [ObservableProperty]
    private int _currentFC;

    partial void OnCurrentFCChanged(int value)
    {
        if (_festa is not null)
            _festa.FestaCoins = value;
        OnPropertyChanged(nameof(TotalFC));
    }

    [ObservableProperty]
    private int _usedFC;

    partial void OnUsedFCChanged(int value)
    {
        OnPropertyChanged(nameof(TotalFC));
    }

    public int TotalFC => CurrentFC + UsedFC;

    [ObservableProperty]
    private int _rank;

    partial void OnRankChanged(int value)
    {
        if (_festa is not null)
            _festa.FestaRank = (ushort)value;
        OnPropertyChanged(nameof(RankFCRange));
    }

    public string RankFCRange => GetRankText(Rank);

    [ObservableProperty]
    private ObservableCollection<FacilityViewModel> _facilities = [];

    [ObservableProperty]
    private FacilityViewModel? _selectedFacility;

    private void LoadData()
    {
        if (_festa is null) return;

        PlazaName = _festa.FestivalPlazaName;
        CurrentFC = _festa.FestaCoins;
        Rank = _festa.FestaRank;

        // Load facilities
        Facilities.Clear();
        for (int i = 0; i < JoinFesta7.FestaFacilityCount; i++)
        {
            var facility = _festa.GetFestaFacility(i);
            Facilities.Add(new FacilityViewModel(i, facility));
        }

        if (Facilities.Count > 0)
            SelectedFacility = Facilities[0];
    }

    private static string GetRankText(int rank)
    {
        if (rank < 1) return string.Empty;
        if (rank == 1) return "0 - 5";
        if (rank == 2) return "6 - 15";
        if (rank == 3) return "16 - 30";
        if (rank <= 10)
        {
            int i = ((rank - 1) * (rank - 2) * 5) + 1;
            return $"{i} - {i + ((rank - 1) * 10) - 1}";
        }
        if (rank <= 20)
        {
            int i = (rank * 100) - 649;
            return $"{i} - {i + 99}";
        }
        if (rank <= 70)
        {
            int j = (rank - 1) / 10;
            int i = (rank * ((j * 30) + 60)) - ((j * j * 150) + (j * 180) + 109);
            return $"{i} - {i + (j * 30) + 59}";
        }
        if (rank <= 100)
        {
            int i = (rank * 270) - 8719;
            return $"{i} - {i + 269}";
        }
        if (rank <= 998)
        {
            int i = (rank * 300) - 11749;
            return $"{i} - {i + 299}";
        }
        if (rank == 999)
            return "287951+";
        return string.Empty;
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadData();
    }
}

public partial class FacilityViewModel : ViewModelBase
{
    private readonly FestaFacility _facility;

    public FacilityViewModel(int index, FestaFacility facility)
    {
        Index = index;
        _facility = facility;
        _type = facility.Type;
        _color = facility.Color;
        _ownerName = facility.OriginalTrainerName;
        _isIntroduced = facility.IsIntroduced;
    }

    public int Index { get; }
    public string DisplayName => $"Facility {Index + 1}";

    [ObservableProperty]
    private int _type;

    partial void OnTypeChanged(int value) => _facility.Type = value;

    [ObservableProperty]
    private int _color;

    partial void OnColorChanged(int value) => _facility.Color = (byte)value;

    [ObservableProperty]
    private string _ownerName;

    partial void OnOwnerNameChanged(string value) => _facility.OriginalTrainerName = value;

    [ObservableProperty]
    private bool _isIntroduced;

    partial void OnIsIntroducedChanged(bool value) => _facility.IsIntroduced = value;
}
