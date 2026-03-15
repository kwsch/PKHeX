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
        _bag = sav.Inventory;
        _itemNames = [.. GameInfo.Strings.GetItemStrings(sav.Context, sav.Version)];

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
        _bag.CopyTo(SAV);
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
        {
            var pouch = pouchModel.Pouch;
            int ctr = 0;

            foreach (var itemModel in pouchModel.Items)
            {
                var itemName = itemModel.ItemName;
                var itemId = Array.IndexOf(_itemNames, itemName);
                if (itemId < 0)
                    itemId = 0;

                var count = itemModel.Count;

                if (itemId == 0 && count == 0)
                    continue;

                // Validate count using bag rules
                _bag.IsQuantitySane(pouch.Type, itemId, ref count, hasNew: false);

                var item = pouch.GetEmpty(itemId, count);
                pouch.Items[ctr++] = item;
            }

            // Clear remaining slots
            for (int i = ctr; i < pouch.Items.Length; i++)
                pouch.Items[i] = pouch.GetEmpty();
        }
    }

    /// <summary>
    /// Sorts the currently selected pouch by item name.
    /// </summary>
    [RelayCommand]
    private void SortByName()
    {
        SelectedPouch?.Items.Sort(nameof(InventoryItemModel.ItemName));
    }

    /// <summary>
    /// Sorts the currently selected pouch by item count.
    /// </summary>
    [RelayCommand]
    private void SortByCount()
    {
        SelectedPouch?.Items.Sort(nameof(InventoryItemModel.Count), System.ComponentModel.ListSortDirection.Descending);
    }

    partial void OnSelectedPouchIndexChanged(int value)
    {
        if (value >= 0 && value < Pouches.Count)
            SelectedPouch = Pouches[value];
    }
}
