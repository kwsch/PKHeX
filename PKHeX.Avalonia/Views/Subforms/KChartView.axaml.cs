using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class KChartView : SubformWindow
{
    public KChartView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
