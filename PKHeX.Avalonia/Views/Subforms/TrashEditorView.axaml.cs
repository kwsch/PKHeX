using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class TrashEditorView : SubformWindow
{
    public TrashEditorView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        // TrashEditorViewModel writes directly to the byte span;
        // closing with true marks Modified so the caller knows to keep changes.
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
