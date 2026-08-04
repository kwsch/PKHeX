using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class PKMDatabaseView : UserControl
{
    private double _lastFilterWidth = 250;

    public PKMDatabaseView()
    {
        InitializeComponent();
    }

    private const double FilterMinWidth = 180;

    private void OnToggleFilters(object? sender, RoutedEventArgs e)
    {
        var column = RootGrid.ColumnDefinitions[0];
        if (column.Width.Value <= 0)
        {
            column.MinWidth = FilterMinWidth;
            column.Width = new GridLength(_lastFilterWidth);
            FilterSplitter.IsVisible = true;
            ToggleFiltersButton.Content = "◀ Filters";
        }
        else
        {
            _lastFilterWidth = column.Width.Value;
            column.MinWidth = 0;
            column.Width = new GridLength(0);
            FilterSplitter.IsVisible = false;
            ToggleFiltersButton.Content = "▶ Filters";
        }
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is PKMDatabaseViewModel vm && sender is DataGrid grid && grid.SelectedItem is PKMDatabaseEntry entry)
        {
            vm.SelectPokemonCommand.Execute(entry);
            
            // Close dialog if we are in one
            var window = VisualRoot as Window;
            window?.Close();
        }
    }
}
