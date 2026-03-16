using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels;

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

        // Attach drag handlers to the sprite image
        var sprite = this.FindControl<Image>("PB_Sprite");
        if (sprite is not null)
        {
            sprite.PointerPressed += OnSpritePointerPressed;
            sprite.PointerMoved += OnSpritePointerMoved;
            sprite.PointerReleased += OnSpritePointerReleased;
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
        if (!_spritePressed)
            return;

        var pos = e.GetPosition(this);
        if (Math.Abs(pos.X - _spritePressPos.X) < 5 && Math.Abs(pos.Y - _spritePressPos.Y) < 5)
            return;

        _spritePressed = false;

        if (DataContext is not PKMEditorViewModel vm || vm.Entity is null)
            return;

        vm.PreparePKM();

        var data = vm.Entity.DecryptedBoxData;
        var ext = vm.Entity.Extension;
        var tempPath = Path.Combine(Path.GetTempPath(), $"pkhex_export.{ext}");
        await File.WriteAllBytesAsync(tempPath, data);

        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Files, new[] { tempPath });
        await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
    }

    private void OnSpritePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _spritePressed = false;
    }
}
