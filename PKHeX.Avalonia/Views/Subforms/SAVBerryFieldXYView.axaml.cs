using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVBerryFieldXYView : SubformWindow
{
    public SAVBerryFieldXYView()
    {
        InitializeComponent();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
