using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class InventoryView : SubformWindow
{
    public InventoryView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is InventoryViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Column visibility is handled per-DataGrid in OnDataGridLoaded
    }

    /// <summary>
    /// Called when each DataGrid inside a tab's DataTemplate is loaded.
    /// Adds columns dynamically based on the pouch's flag capabilities.
    /// </summary>
    private void OnDataGridLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not DataGrid dg)
            return;

        // Only configure columns once
        if (dg.Columns.Count > 0)
            return;

        var pouch = dg.DataContext as InventoryPouchModel;
        if (pouch is null)
            return;

        // Always add Item and Count columns
        dg.Columns.Add(new DataGridTextColumn
        {
            Header = "Item",
            Binding = new Binding(nameof(InventoryItemModel.ItemName)),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
        });
        dg.Columns.Add(new DataGridTextColumn
        {
            Header = "Count",
            Binding = new Binding(nameof(InventoryItemModel.Count)),
            Width = new DataGridLength(60),
        });

        // Conditionally add flag columns based on the pouch's item type
        if (pouch.HasFavorite)
        {
            dg.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = "Fav",
                Binding = new Binding(nameof(InventoryItemModel.IsFavorite)),
                Width = new DataGridLength(45),
            });
        }
        if (pouch.HasNew)
        {
            dg.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = "New",
                Binding = new Binding(nameof(InventoryItemModel.IsNew)),
                Width = new DataGridLength(45),
            });
        }
        if (pouch.HasFreeSpace)
        {
            dg.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = "Free",
                Binding = new Binding(nameof(InventoryItemModel.IsFreeSpace)),
                Width = new DataGridLength(45),
            });
        }
        if (pouch.HasFreeSpaceIndex)
        {
            dg.Columns.Add(new DataGridTextColumn
            {
                Header = "Free#",
                Binding = new Binding(nameof(InventoryItemModel.FreeSpaceIndex)),
                Width = new DataGridLength(50),
            });
        }
        if (pouch.HasNewShop)
        {
            dg.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = "Shop",
                Binding = new Binding(nameof(InventoryItemModel.IsNewShop)),
                Width = new DataGridLength(45),
            });
        }
        if (pouch.HasHeld)
        {
            dg.Columns.Add(new DataGridCheckBoxColumn
            {
                Header = "Held",
                Binding = new Binding(nameof(InventoryItemModel.IsHeld)),
                Width = new DataGridLength(45),
            });
        }
    }
}
