using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class DatabaseView : SubformWindow
{
    public DatabaseView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
