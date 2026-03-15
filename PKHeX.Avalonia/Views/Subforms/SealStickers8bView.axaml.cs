using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SealStickers8bView : SubformWindow
{
    public SealStickers8bView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SealStickers8bViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
