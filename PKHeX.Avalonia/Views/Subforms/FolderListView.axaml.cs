using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class FolderListView : SubformWindow
{
    public FolderListView()
    {
        InitializeComponent();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
