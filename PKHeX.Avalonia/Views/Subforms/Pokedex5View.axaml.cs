using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class Pokedex5View : SubformWindow
{
    public Pokedex5View()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Pokedex5ViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private void OnEntryPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { DataContext: Pokedex5EntryModel entry } &&
            DataContext is Pokedex5ViewModel vm)
        {
            vm.SelectedEntry = entry;
        }
    }
}
