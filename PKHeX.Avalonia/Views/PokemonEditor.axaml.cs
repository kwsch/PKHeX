using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class PokemonEditor : UserControl
{
    public PokemonEditor()
    {
        InitializeComponent();
    }

    // Only OS file drops are meaningful here (no in-app slot payload), so always show the "copy" cursor.
    private void OnEditorDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.TryGetFiles() is { Length: > 0 }
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    // Dropping an entity file directly onto the editor loads it without writing to a box/party
    // slot; dropping a save file routes through the host's "open save" path.
    private async void OnEditorDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not PokemonEditorViewModel vm)
            return;

        var files = e.DataTransfer.TryGetFiles();
        if (files is not { Length: > 0 })
            return;

        e.Handled = true;
        var paths = files.Select(f => f.TryGetLocalPath()).OfType<string>().ToList();
        if (paths.Count == 0)
            return;

        await vm.HandleFileDropAsync(paths);
    }

    private void ExpBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border bar)
            return;
        if (!e.GetCurrentPoint(bar).Properties.IsLeftButtonPressed)
            return;
        ApplyExpFromPointer(bar, e);
    }

    private void ExpBar_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Border bar)
            return;
        // Only react while the left button is held (drag).
        if (!e.GetCurrentPoint(bar).Properties.IsLeftButtonPressed)
            return;
        ApplyExpFromPointer(bar, e);
    }

    private void ApplyExpFromPointer(Border bar, PointerEventArgs e)
    {
        if (DataContext is not PokemonEditorViewModel vm)
            return;

        var width = bar.Bounds.Width;
        if (width <= 0)
            return;

        // Holding Shift or Ctrl snaps to the current level's high edge (mirrors upstream's modifier behavior).
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) || e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            vm.SetExpToLevelEdgeHigh();
            return;
        }

        var x = e.GetPosition(bar).X;
        var fraction = x / width;
        vm.SetExpFromFraction(fraction);
    }
}
