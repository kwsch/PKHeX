using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

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

    private void OnDataGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;
        if (grid.SelectedItem is not SaveFileEntry entry)
            return;
        if (DataContext is not FolderListViewModel vm)
            return;

        vm.OpenSaveFileCommand.Execute(entry);

        // Close the dialog after the file is opened
        if (vm.FileOpened)
            CloseWithResult(true);
    }
}
