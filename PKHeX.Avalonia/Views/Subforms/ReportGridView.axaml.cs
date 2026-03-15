using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class ReportGridView : SubformWindow
{
    public ReportGridView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
