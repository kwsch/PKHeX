using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class DonutEditor9aView : SubformWindow
{
    public DonutEditor9aView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
