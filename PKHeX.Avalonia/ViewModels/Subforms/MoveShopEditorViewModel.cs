using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single move shop entry.
/// </summary>
public partial class MoveShopEntryModel : ObservableObject
{
    public int Index { get; }
    public ushort MoveId { get; }
    public string MoveName { get; }
    public string DisplayIndex { get; }
    public bool IsPermitted { get; }

    [ObservableProperty]
    private bool _isPurchased;

    [ObservableProperty]
    private bool _isMastered;

    public MoveShopEntryModel(int index, ushort moveId, string moveName, bool isPermitted, bool isPurchased, bool isMastered)
    {
        Index = index;
        MoveId = moveId;
        MoveName = moveName;
        IsPermitted = isPermitted;
        DisplayIndex = (index + 1).ToString("00");
        _isPurchased = isPurchased;
        _isMastered = isMastered;
    }
}

/// <summary>
/// ViewModel for the Move Shop Editor subform.
/// Edits purchased and mastered flags for Pokemon implementing <see cref="IMoveShop8"/> and <see cref="IMoveShop8Mastery"/>.
/// </summary>
public partial class MoveShopEditorViewModel : ObservableObject
{
    private readonly IMoveShop8 _shop;
    private readonly IMoveShop8Mastery _master;
    private readonly PKM _entity;

    [ObservableProperty]
    private bool _modified;

    /// <summary>All move shop entries.</summary>
    public ObservableCollection<MoveShopEntryModel> Records { get; } = [];

    public MoveShopEditorViewModel(IMoveShop8 shop, IMoveShop8Mastery master, PKM pk)
    {
        _shop = shop;
        _master = master;
        _entity = pk;
        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = _shop.Permit.RecordPermitIndexes;

        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var moveName = move < names.Count ? names[move] : $"Move {move}";
            bool isPermitted = _shop.Permit.IsRecordPermitted(i);
            bool isPurchased = _shop.GetPurchasedRecordFlag(i);
            bool isMastered = _master.GetMasteredRecordFlag(i);

            Records.Add(new MoveShopEntryModel(i, move, moveName, isPermitted, isPurchased, isMastered));
        }
    }

    /// <summary>
    /// Saves all flags back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        foreach (var rec in Records)
        {
            _shop.SetPurchasedRecordFlag(rec.Index, rec.IsPurchased);
            _master.SetMasteredRecordFlag(rec.Index, rec.IsMastered);
        }
        Modified = true;
    }

    /// <summary>
    /// Sets all permitted flags using legal move shop logic.
    /// </summary>
    [RelayCommand]
    private void SetAll()
    {
        Save();
        _master.SetMoveShopFlagsAll(_entity);
        ReloadFromEntity();
        Modified = true;
    }

    /// <summary>
    /// Clears all move shop flags.
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        _shop.ClearMoveShopFlags();
        ReloadFromEntity();
        Modified = true;
    }

    private void ReloadFromEntity()
    {
        foreach (var rec in Records)
        {
            rec.IsPurchased = _shop.GetPurchasedRecordFlag(rec.Index);
            rec.IsMastered = _master.GetMasteredRecordFlag(rec.Index);
        }
    }
}
