using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVBoxListView : SubformWindow
{
    public SAVBoxListView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
