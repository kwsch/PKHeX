using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class CGearImage5View : SubformWindow
{
    public CGearImage5View()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
