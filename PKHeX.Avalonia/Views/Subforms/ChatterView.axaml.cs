using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class ChatterView : SubformWindow
{
    public ChatterView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ChatterViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private async void OnImportPcmClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatterViewModel vm)
            return;

        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import PCM File",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("PCM Files") { Patterns = ["*.pcm"] }],
        });

        var file = files.FirstOrDefault();
        if (file is not null)
            vm.ImportPcmCommand.Execute(file.Path.LocalPath);
    }

    private async void OnExportPcmClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not ChatterViewModel vm)
            return;

        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export PCM File",
            SuggestedFileName = "Recording.pcm",
            FileTypeChoices = [new FilePickerFileType("PCM Files") { Patterns = ["*.pcm"] }],
        });

        if (file is not null)
            vm.ExportPcmCommand.Execute(file.Path.LocalPath);
    }
}
