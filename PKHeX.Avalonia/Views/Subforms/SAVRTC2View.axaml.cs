using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVRTC2View : SubformWindow
{
    public SAVRTC2View()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SAVRTC2ViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
