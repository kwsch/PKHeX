using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class GroupViewerView : SubformWindow
{
    public GroupViewerView()
    {
        InitializeComponent();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
