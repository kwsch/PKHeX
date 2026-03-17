using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class Pokedex4View : SubformWindow
{
    public Pokedex4View()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Pokedex4ViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private void OnEntryPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border { DataContext: Pokedex4EntryModel entry } &&
            DataContext is Pokedex4ViewModel vm)
        {
            vm.SelectedEntry = entry;
        }
    }

    private void OnAddFormClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not Pokedex4ViewModel vm)
            return;
        var listBox = this.FindControl<ListBox>("LB_NotSeenForms");
        if (listBox?.SelectedItem is string formName)
            vm.AddFormCommand.Execute(formName);
    }

    private void OnRemoveFormClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not Pokedex4ViewModel vm)
            return;
        var listBox = this.FindControl<ListBox>("LB_SeenForms");
        if (listBox?.SelectedItem is string formName)
            vm.RemoveFormCommand.Execute(formName);
    }
}
