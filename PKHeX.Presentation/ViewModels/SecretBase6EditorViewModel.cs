using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class SecretBase6EditorViewModel : ViewModelBase
{
    private readonly SAV6AO _sav;
    private readonly SecretBase6Block _block;

    public SecretBase6EditorViewModel(SaveFile sav)
    {
        _sav = (SAV6AO)sav;
        _block = _sav.SecretBase;
        
        LoadBases();
    }

    [ObservableProperty] private ObservableCollection<SecretBase6EntryViewModel> _bases = [];
    [ObservableProperty] private SecretBase6EntryViewModel? _selectedBase;
    [ObservableProperty] private int _capturedFlags;

    private void LoadBases()
    {
        Bases.Clear();
        
        // Self
        var self = _block.GetSecretBaseSelf();
        Bases.Add(new SecretBase6EntryViewModel(self, true));
        
        // Others
        for (int i = 0; i < SecretBase6Block.OtherSecretBaseCount; i++)
        {
            var other = _block.GetSecretBaseOther(i);
            if (other.IsEmpty) continue;
            Bases.Add(new SecretBase6EntryViewModel(other, false, i));
        }
        
        SelectedBase = Bases.FirstOrDefault();
        CapturedFlags = _sav.Records.GetRecord(80);
    }

    [RelayCommand]
    private void GiveAllGoods()
    {
        _block.GiveAllGoods();
    }

    [RelayCommand]
    private void DeleteSelectedOther()
    {
        if (SelectedBase == null || SelectedBase.IsSelf || SelectedBase.OtherIndex == -1) return;
        
        _block.DeleteOther(SelectedBase.OtherIndex);
        LoadBases();
    }

    [RelayCommand]
    private void Save()
    {
        _sav.Records.SetRecord(80, CapturedFlags);
        
        // Selected base changes are already applied via viewmodels if we use direct binding to model in simple cases, 
        // but here we might need to sync back if we are using temporary properties.
        // Actually, SecretBase6EntryViewModel should handle its own saving if needed.
        
        foreach (var entry in Bases)
        {
            entry.Save();
        }

        _sav.State.Edited = true;
    }
}

public partial class SecretBase6EntryViewModel : ViewModelBase
{
    private readonly SecretBase6 _base;
    public bool IsSelf { get; }
    public int OtherIndex { get; }

    public SecretBase6EntryViewModel(SecretBase6 b, bool isSelf, int otherIndex = -1)
    {
        _base = b;
        IsSelf = isSelf;
        OtherIndex = otherIndex;
        
        TrainerName = _base.TrainerName;
        BaseLocation = _base.BaseLocation;
        Rank = (int)_base.Rank;
        FlagsFriends = _base.TotalFlagsFromFriends;
        FlagsOthers = _base.TotalFlagsFromOther;
        
        Placements = new ObservableCollection<SecretBase6PlacementViewModel>(
            Enumerable.Range(0, SecretBase6.COUNT_GOODS)
                .Select(i => new SecretBase6PlacementViewModel(i, _base.GetPlacement(i)))
        );

        if (_base is SecretBase6Other other)
        {
            Team = new ObservableCollection<SecretBase6PKMViewModel>(
                Enumerable.Range(0, SecretBase6Other.COUNT_TEAM)
                    .Select(i => new SecretBase6PKMViewModel(i, other.GetParticipant(i)))
            );
        }
    }

    [ObservableProperty] private string _trainerName;
    [ObservableProperty] private int _baseLocation;
    [ObservableProperty] private int _rank;
    [ObservableProperty] private uint _flagsFriends;
    [ObservableProperty] private uint _flagsOthers;
    
    public ObservableCollection<SecretBase6PlacementViewModel> Placements { get; }
    public ObservableCollection<SecretBase6PKMViewModel>? Team { get; }

    public void Save()
    {
        _base.TrainerName = TrainerName;
        _base.BaseLocation = BaseLocation;
        _base.Rank = (SecretBase6Rank)Rank;
        _base.TotalFlagsFromFriends = FlagsFriends;
        _base.TotalFlagsFromOther = FlagsOthers;
        
        foreach (var p in Placements) p.Save();
        
        if (_base is SecretBase6Other other && Team != null)
        {
            for (int i = 0; i < Team.Count; i++)
            {
                other.SetParticipant(i, Team[i].GetModel());
            }
        }
    }

    public string DisplayName => IsSelf ? $"* {TrainerName}" : $"{OtherIndex + 1:00} {TrainerName}";
}

public partial class SecretBase6PlacementViewModel : ViewModelBase
{
    private readonly SecretBase6GoodPlacement _model;
    public int Index { get; }

    public SecretBase6PlacementViewModel(int index, SecretBase6GoodPlacement model)
    {
        Index = index;
        _model = model;
        GoodId = _model.Good;
        X = _model.X;
        Y = _model.Y;
        Rotation = _model.Rotation;
    }

    [ObservableProperty] private ushort _goodId;
    [ObservableProperty] private ushort _x;
    [ObservableProperty] private ushort _y;
    [ObservableProperty] private byte _rotation;

    public void Save()
    {
        _model.Good = GoodId;
        _model.X = X;
        _model.Y = Y;
        _model.Rotation = Rotation;
    }
}

public partial class SecretBase6PKMViewModel : ViewModelBase
{
    private readonly SecretBase6PKM _model;
    public int Index { get; }

    public SecretBase6PKMViewModel(int index, SecretBase6PKM model)
    {
        Index = index;
        _model = model;
        
        Species = _model.Species;
        Level = _model.CurrentLevel;
        IsShiny = _model.IsShiny;
        // Add more basic info as needed
    }

    [ObservableProperty] private ushort _species;
    [ObservableProperty] private byte _level;
    [ObservableProperty] private bool _isShiny;

    public SecretBase6PKM GetModel()
    {
        _model.Species = Species;
        _model.CurrentLevel = Level;
        _model.IsShiny = IsShiny;
        return _model;
    }
}
