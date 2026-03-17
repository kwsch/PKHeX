using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class QRDialogView : SubformWindow
{
    public QRDialogView()
    {
        InitializeComponent();
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not QRDialogViewModel vm)
                return;

            var bytes = vm.GetQRImageBytes();
            if (bytes is null)
                return;

            var topLevel = GetTopLevel(this);
            if (topLevel is null)
                return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save QR Code",
                SuggestedFileName = "qrcode.png",
                FileTypeChoices =
                [
                    new FilePickerFileType("PNG Image") { Patterns = ["*.png"] },
                ],
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                await stream.WriteAsync(bytes.AsMemory());
            }
        }
        catch
        {
            // File save failed silently
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
