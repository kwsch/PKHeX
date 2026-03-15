using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class DonutFlavorProfile9aView : SubformWindow
{
    public DonutFlavorProfile9aView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
