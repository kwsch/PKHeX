using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Fallback for OS files dropped anywhere on the window that weren't already handled by a
    // more specific target (a box/party slot, or the editor panel) — e.g. a save file dropped
    // over the trainer/inventory tabs. Save files open (same path as File > Open); Pokémon
    // entity files load into the current editor.
    private async void OnWindowDrop(object? sender, DragEventArgs e)
    {
        if (e.Handled || DataContext is not MainWindowViewModel vm)
            return;

        var files = e.DataTransfer.TryGetFiles();
        if (files is not { Length: > 0 })
            return;

        var paths = files.Select(f => f.TryGetLocalPath()).OfType<string>().ToList();
        if (paths.Count == 0)
            return;

        e.Handled = true;
        await vm.HandleWindowFileDropAsync(paths);
    }
}
