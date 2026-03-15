using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class EntitySearchSetupView : SubformWindow
{
    public EntitySearchSetupView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
