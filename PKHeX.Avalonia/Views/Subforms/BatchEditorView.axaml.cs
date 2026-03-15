using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class BatchEditorView : SubformWindow
{
    public BatchEditorView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is BatchEditorViewModel vm && vm.Modified)
            CloseWithResult(true);
        else
            CloseWithResult(false);
    }
}
