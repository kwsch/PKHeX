using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVAccessorView : SubformWindow
{
    public SAVAccessorView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
