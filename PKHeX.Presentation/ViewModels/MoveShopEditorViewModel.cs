using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class MoveShopEditorViewModel : ViewModelBase
{
    private readonly IMoveShop8 _shop;
    private readonly IMoveShop8Mastery _mastery;
    private readonly PKM _pkm;
    
    public ObservableCollection<MoveShopItemViewModel> Moves { get; } = [];

    public Action? CloseRequested { get; set; }

    public MoveShopEditorViewModel(IMoveShop8 shop, IMoveShop8Mastery mastery, PKM pkm)
    {
        _shop = shop;
        _mastery = mastery;
        _pkm = pkm;

        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = _shop.Permit.RecordPermitIndexes;
        
        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var isValid = _shop.Permit.IsRecordPermitted(i);
            var type = MoveInfo.GetType(move, _pkm.Context);
            var name = names[move];

            var item = new MoveShopItemViewModel(i, move, name, type, isValid);
            item.IsPurchased = _shop.GetPurchasedRecordFlag(i);
            item.IsMastered = _mastery.GetMasteredRecordFlag(i);

            Moves.Add(item);
        }
    }

    [RelayCommand]
    private void Save()
    {
        foreach (var item in Moves)
        {
            _shop.SetPurchasedRecordFlag(item.Index, item.IsPurchased);
            _mastery.SetMasteredRecordFlag(item.Index, item.IsMastered);
        }
        CloseRequested?.Invoke();
    }

    [RelayCommand]
    private void SetAll()
    {
        // "All" logic from WinForms: 
        // Default (or just click): Set Mastered flags (SetMoveShopFlags)
        // Shift: Set All (SetMoveShopFlagsAll) - assumes permits?
        // Control: Clear Shop + Set Mastered
        
        // Simplifying for Avalonia UI: distinct buttons or logic?
        // Let's replicate standard "Give All" behavior which usually means Give All legal.
        
        _mastery.SetMoveShopFlags(_pkm);
        ReloadFlags();
    }

    [RelayCommand]
    private void SetNone()
    {
        _shop.ClearMoveShopFlags();
        ReloadFlags();
    }

    private void ReloadFlags()
    {
        foreach (var item in Moves)
        {
            item.IsPurchased = _shop.GetPurchasedRecordFlag(item.Index);
            item.IsMastered = _mastery.GetMasteredRecordFlag(item.Index);
        }
    }
}

public partial class MoveShopItemViewModel : ObservableObject
{
    public int Index { get; }
    public int MoveID { get; }
    public string Name { get; }
    public int Type { get; }
    public bool IsPermitted { get; }

    [ObservableProperty]
    private bool _isPurchased;

    [ObservableProperty]
    private bool _isMastered;
    
    // For sorting/display
    public string IndexDisplay => $"{Index + 1:00}";

    public MoveShopItemViewModel(int index, int moveId, string name, int type, bool isPermitted)
    {
        Index = index;
        MoveID = moveId;
        Name = name;
        Type = type;
        IsPermitted = isPermitted;
    }
}
