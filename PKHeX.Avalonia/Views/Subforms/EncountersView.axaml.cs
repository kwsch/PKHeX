using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class EncountersView : SubformWindow
{
    public EncountersView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
