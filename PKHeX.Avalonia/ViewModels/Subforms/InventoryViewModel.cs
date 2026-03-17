using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Controls;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Inventory (bag) editor subform.
/// Loads all pouches from the save file and exposes them as tabs with editable item rows.
/// </summary>
public partial class InventoryViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SaveFile _clone;
    private readonly PlayerBag _bag;
    private readonly string[] _itemNames;

    [ObservableProperty]
    private int _selectedPouchIndex;

    [ObservableProperty]
    private InventoryPouchModel? _selectedPouch;

    /// <summary>
    /// All pouch tabs.
    /// </summary>
    public ObservableCollection<InventoryPouchModel> Pouches { get; } = [];

    public InventoryViewModel(SaveFile sav) : base(sav)
    {
        _origin = sav;
        _clone = sav.Clone();
        _bag = _clone.Inventory;
        _itemNames = [.. GameInfo.Strings.GetItemStrings(_clone.Context, _clone.Version)];

        // Fill in blanks for unnamed items
        for (int i = 0; i < _itemNames.Length; i++)
        {
            if (string.IsNullOrEmpty(_itemNames[i]))
                _itemNames[i] = $"(Item #{i:000})";
        }

        LoadPouches();

        if (Pouches.Count > 0)
        {
            SelectedPouchIndex = 0;
            SelectedPouch = Pouches[0];
        }
    }

    private void LoadPouches()
    {
        foreach (var pouch in _bag.Pouches)
        {
            pouch.Sanitize(_itemNames.Length - 1);

            var validItemIds = pouch.GetAllItems();
            var validNames = GetValidItemNames(validItemIds);
            var items = new SortableList<InventoryItemModel>();

            foreach (var item in pouch.Items)
            {
                items.Add(new InventoryItemModel(item, _itemNames, validNames));
            }

            var pouchModel = new InventoryPouchModel(pouch.Type.ToString(), pouch, items);
            Pouches.Add(pouchModel);
        }
    }

    private List<string> GetValidItemNames(ReadOnlySpan<ushort> validItemIds)
    {
        var names = new List<string>(validItemIds.Length + 1);
        names.Add(_itemNames[0]); // "(None)" entry

        foreach (var id in validItemIds)
        {
            if (id < _itemNames.Length)
                names.Add(_itemNames[id]);
        }

        names.Sort(StringComparer.CurrentCulture);
        return names;
    }

    /// <summary>
    /// Saves edits back to the save file's inventory.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        WriteBackAllPouches();
        _bag.CopyTo(_clone);
        _origin.CopyChangesFrom(_clone);
        Modified = true;
    }

    /// <summary>
    /// Cancels and discards edits.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        Modified = false;
    }

    private void WriteBackAllPouches()
    {
        foreach (var pouchModel in Pouches)
            WriteBackPouch(pouchModel);
    }

    private void WriteBackPouch(InventoryPouchModel pouchModel)
    {
        var pouch = pouchModel.Pouch;
        bool hasNew = pouchModel.HasNew;
        int ctr = 0;

        foreach (var itemModel in pouchModel.Items)
        {
            var itemId = itemModel.Index;
            var count = itemModel.Count;

            if (itemId == 0 && count == 0 && !hasNew)
                continue;

            // Validate count using bag rules
            _bag.IsQuantitySane(pouch.Type, itemId, ref count, hasNew: hasNew);

            var item = pouch.GetEmpty(itemId, count);

            // Write back flag data
            if (item is IItemFavorite fav)
                fav.IsFavorite = itemModel.IsFavorite;
            if (item is IItemNewFlag nf)
                nf.IsNew = itemModel.IsNew;
            if (item is IItemFreeSpace fs)
                fs.IsFreeSpace = itemModel.IsFreeSpace;
            if (item is IItemFreeSpaceIndex fsi)
                fsi.FreeSpaceIndex = itemModel.FreeSpaceIndex;
            if (item is IItemNewShopFlag ns)
                ns.IsNewShop = itemModel.IsNewShop;
            if (item is IItemHeldFlag hf)
                hf.IsHeld = itemModel.IsHeld;

            pouch.Items[ctr++] = item;
        }

        // Clear remaining slots
        for (int i = ctr; i < pouch.Items.Length; i++)
            pouch.Items[i] = pouch.GetEmpty();
    }

    /// <summary>
    /// Reloads the UI model items from the underlying pouch data.
    /// Called after modifying the pouch items array directly.
    /// </summary>
    private void ReloadPouch(InventoryPouchModel pouchModel)
    {
        var pouch = pouchModel.Pouch;
        var validItemIds = pouch.GetAllItems();
        var validNames = GetValidItemNames(validItemIds);

        pouchModel.Items.Clear();
        foreach (var item in pouch.Items)
        {
            pouchModel.Items.Add(new InventoryItemModel(item, _itemNames, validNames));
        }
    }

    /// <summary>
    /// Writes back the selected pouch, executes a modification, then reloads the pouch.
    /// Mirrors the WinForms ModifyPouch pattern.
    /// </summary>
    private void ModifyPouch(Action<InventoryPouch> func)
    {
        var pouchModel = SelectedPouch;
        if (pouchModel is null)
            return;

        WriteBackPouch(pouchModel);
        func(pouchModel.Pouch);
        ReloadPouch(pouchModel);
    }

    // ===== Sort Commands =====

    /// <summary>
    /// Sorts the currently selected pouch by item name (ascending).
    /// </summary>
    [RelayCommand]
    private void SortByName() => ModifyPouch(p => p.SortByName(_itemNames));

    /// <summary>
    /// Sorts the currently selected pouch by item name (descending).
    /// </summary>
    [RelayCommand]
    private void SortByNameReverse() => ModifyPouch(p => p.SortByName(_itemNames, reverse: true));

    /// <summary>
    /// Sorts the currently selected pouch by item count (ascending).
    /// </summary>
    [RelayCommand]
    private void SortByCount() => ModifyPouch(p => p.SortByCount());

    /// <summary>
    /// Sorts the currently selected pouch by item count (descending).
    /// </summary>
    [RelayCommand]
    private void SortByCountReverse() => ModifyPouch(p => p.SortByCount(reverse: true));

    /// <summary>
    /// Sorts the currently selected pouch by item index (ascending).
    /// </summary>
    [RelayCommand]
    private void SortByIndex() => ModifyPouch(p => p.SortByIndex());

    /// <summary>
    /// Sorts the currently selected pouch by item index (descending).
    /// </summary>
    [RelayCommand]
    private void SortByIndexReverse() => ModifyPouch(p => p.SortByIndex(reverse: true));

    // ===== Give / Remove / Modify Commands =====

    /// <summary>
    /// Gives all legal items in the selected pouch at max count.
    /// </summary>
    [RelayCommand]
    private void GiveAllItems()
    {
        var pouchModel = SelectedPouch;
        if (pouchModel is null)
            return;

        WriteBackPouch(pouchModel);
        var pouch = pouchModel.Pouch;
        var items = pouch.GetAllItems().ToArray();
        pouch.GiveAllItems(_bag, items);
        ReloadPouch(pouchModel);
    }

    /// <summary>
    /// Removes all items from the selected pouch.
    /// </summary>
    [RelayCommand]
    private void RemoveAllItems() => ModifyPouch(p => p.RemoveAll());

    /// <summary>
    /// Sets all non-empty items in the selected pouch to max count.
    /// </summary>
    [RelayCommand]
    private void ModifyAllCount() => ModifyPouch(p => p.ModifyAllCount(_bag));

    partial void OnSelectedPouchIndexChanged(int value)
    {
        if (value >= 0 && value < Pouches.Count)
            SelectedPouch = Pouches[value];
    }
}
