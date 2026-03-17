using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PKHeX.Avalonia.ViewModels;
using PKHeX.Core;

namespace PKHeX.Avalonia.Views;

public partial class PKMEditorView : UserControl
{
    private Point _spritePressPos;
    private bool _spritePressed;

    public PKMEditorView()
    {
        InitializeComponent();

        // Tunnel handler so we intercept wheel events before the NumericUpDown handles them
        this.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);

        // Drop handlers for loading PKM files by drag-and-drop onto the editor
        AddHandler(DragDrop.DropEvent, OnEditorDrop);
        AddHandler(DragDrop.DragOverEvent, OnEditorDragOver);

        // Attach drag handlers to the sprite image
        var sprite = this.FindControl<Image>("PB_Sprite");
        if (sprite is not null)
        {
            sprite.PointerPressed += OnSpritePointerPressed;
            sprite.PointerMoved += OnSpritePointerMoved;
            sprite.PointerReleased += OnSpritePointerReleased;
        }
    }

    private void OnEditorDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private async void OnEditorDrop(object? sender, DragEventArgs e)
    {
        try
        {
            if (!e.Data.Contains(DataFormats.Files))
                return;

            var files = e.Data.GetFiles();
            if (files is null)
                return;

            foreach (var file in files)
            {
                var path = file.Path.LocalPath;
                if (!File.Exists(path))
                    continue;

                var data = await File.ReadAllBytesAsync(path);
                var pk = EntityFormat.GetFromBytes(data);
                if (pk is null)
                    continue;

                if (DataContext is PKMEditorViewModel vm)
                {
                    vm.PopulateFields(pk);
                    break;
                }
            }

            e.Handled = true;
        }
        catch
        {
            // Drop operation failed silently
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Source is NumericUpDown nud && nud.IsFocused)
        {
            var delta = e.Delta.Y > 0 ? 1 : -1;
            nud.Value = Math.Clamp((nud.Value ?? 0) + delta, nud.Minimum, nud.Maximum);
            e.Handled = true;
        }
    }

    private void OnSpritePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _spritePressPos = e.GetPosition(this);
            _spritePressed = true;
        }
    }

    private async void OnSpritePointerMoved(object? sender, PointerEventArgs e)
    {
        try
        {
            if (!_spritePressed)
                return;

            var pos = e.GetPosition(this);
            if (Math.Abs(pos.X - _spritePressPos.X) < 5 && Math.Abs(pos.Y - _spritePressPos.Y) < 5)
                return;

            _spritePressed = false;

            if (DataContext is not PKMEditorViewModel vm || vm.Entity is null)
                return;

            vm.PreparePKM();

            var pkData = vm.Entity.DecryptedBoxData;
            var ext = vm.Entity.Extension;
            var tempPath = Path.Combine(Path.GetTempPath(), $"pkhex_export.{ext}");
            await File.WriteAllBytesAsync(tempPath, pkData);

            // Use IStorageItem so the OS file manager can accept the drop
            var topLevel = TopLevel.GetTopLevel(this);
            var storageFile = await topLevel!.StorageProvider.TryGetFileFromPathAsync(tempPath);

            var dataObject = new DataObject();
            if (storageFile is not null)
                dataObject.Set(DataFormats.Files, new[] { storageFile });
            else
                dataObject.Set(DataFormats.Files, new[] { tempPath }); // fallback
            await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
        }
        catch
        {
            // Drag operation failed silently
        }
    }

    private void OnSpritePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _spritePressed = false;
    }
}
