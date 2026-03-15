using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVPokedexGGView : SubformWindow
{
    public SAVPokedexGGView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SAVPokedexGGViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
