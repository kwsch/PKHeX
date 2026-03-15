using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class BoxExporterView : SubformWindow
{
    public BoxExporterView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
