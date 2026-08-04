using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class DaycareEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IDaycareStorage? _storage;
    private readonly IDaycareEggState? _eggState;
    private readonly IDaycareExperience? _experience;

    public DaycareEditorViewModel(SaveFile sav, ISpriteRenderer spriteRenderer)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        
        _storage = sav as IDaycareStorage;
        _eggState = sav as IDaycareEggState;
        _experience = sav as IDaycareExperience;

        LoadDaycareData();
    }

    public bool HasDaycare => _storage is not null;
    public bool HasEggState => _eggState is not null;
    public bool HasExperience => _experience is not null;
    public int SlotCount => _storage?.DaycareSlotCount ?? 0;

    [ObservableProperty]
    private ObservableCollection<DaycareSlotViewModel> _slots = [];

    [ObservableProperty]
    private bool _isEggAvailable;

    partial void OnIsEggAvailableChanged(bool value)
    {
        if (_eggState is not null)
            _eggState.IsEggAvailable = value;
    }

    private void LoadDaycareData()
    {
        Slots.Clear();
        
        if (_storage is null) return;

        for (int i = 0; i < _storage.DaycareSlotCount; i++)
        {
            var mem = _storage.GetDaycareSlot(i);
            var pk = EntityFormat.GetFromBytes(mem.ToArray(), _sav.Context);
            var occupied = _storage.IsDaycareOccupied(i);
            var exp = _experience?.GetDaycareEXP(i) ?? 0;
            
            var vm = new DaycareSlotViewModel(i, pk, occupied, exp, _spriteRenderer);
            vm.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(DaycareSlotViewModel.IsOccupied))
                    _storage.SetDaycareOccupied(vm.Index, vm.IsOccupied);
                else if (e.PropertyName == nameof(DaycareSlotViewModel.Experience) && _experience is not null)
                    _experience.SetDaycareEXP(vm.Index, vm.Experience);
            };
            Slots.Add(vm);
        }

        if (_eggState is not null)
            IsEggAvailable = _eggState.IsEggAvailable;
    }

    [RelayCommand]
    private void RefreshDaycare()
    {
        LoadDaycareData();
    }
}

public partial class DaycareSlotViewModel : ViewModelBase
{
    private readonly ISpriteRenderer _spriteRenderer;

    public DaycareSlotViewModel(int index, PKM? pk, bool occupied, uint exp, ISpriteRenderer spriteRenderer)
    {
        Index = index;
        _pk = pk;
        _isOccupied = occupied;
        _experience = exp;
        _spriteRenderer = spriteRenderer;
    }

    public int Index { get; }
    
    [ObservableProperty]
    private PKM? _pk;

    [ObservableProperty]
    private bool _isOccupied;

    [ObservableProperty]
    private uint _experience;

    public string Species => Pk?.Species > 0 ? StringResourceLookup.Species(Pk.Species) : "(Empty)";
    public string Level => Pk is not null && Pk.Species > 0 ? $"Lv. {Pk.CurrentLevel}" : "";
    public byte[]? Sprite => Pk is not null && Pk.Species > 0 ? _spriteRenderer.GetSprite(Pk) : null;
}
