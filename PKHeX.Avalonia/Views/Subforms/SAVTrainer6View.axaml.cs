using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class SAVTrainer6View : SubformWindow
{
    public SAVTrainer6View()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SAVTrainer6ViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private async void OnExportJpegClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SAVTrainer6ViewModel vm)
            return;

        var jpegBytes = vm.GetJpegBytes();
        if (jpegBytes.Length == 0)
        {
            vm.JpegExportStatus = "No picture data found in the save file!";
            return;
        }

        var storage = GetTopLevel(this)?.StorageProvider;
        if (storage is null)
            return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Trainer Photo",
            SuggestedFileName = vm.JpegFileName,
            FileTypeChoices = [new FilePickerFileType("JPEG") { Patterns = ["*.jpeg"] }],
        });

        if (file is null)
            return;

        await File.WriteAllBytesAsync(file.Path.LocalPath, jpegBytes);
        vm.JpegExportStatus = "Exported!";
    }
}
