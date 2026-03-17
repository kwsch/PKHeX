using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Controls;
using PKHeX.Avalonia.ViewModels;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views;

public partial class SlotControl : UserControl
{
    /// <summary>Minimum distance (pixels) the pointer must move before initiating a drag.</summary>
    private const double DragThreshold = 6;

    private Point _pressPosition;
    private bool _isPressed;

    public SlotControl()
    {
        InitializeComponent();

        // Pointer events — for click-to-view and drag initiation
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;

        // DragDrop events — for receiving drops
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DropEvent, OnDrop);

        // Context menu handlers — wired in code-behind because ContextMenu
        // opens in a popup overlay disconnected from the visual tree,
        // so $parent[ItemsControl] XAML bindings cannot resolve.
        MenuView.Click += OnMenuViewClick;
        MenuSet.Click += OnMenuSetClick;
        MenuDelete.Click += OnMenuDeleteClick;
        MenuLegality.Click += OnMenuLegalityClick;
    }

    private void OnMenuViewClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SlotModel slot)
            return;
        FindSAVEditorViewModel()?.ViewSlotCommand.Execute(slot);
    }

    private void OnMenuSetClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SlotModel slot)
            return;
        FindSAVEditorViewModel()?.SetSlotCommand.Execute(slot);
    }

    private void OnMenuDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SlotModel slot)
            return;
        FindSAVEditorViewModel()?.DeleteSlotCommand.Execute(slot);
    }

    private void OnMenuLegalityClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SlotModel slot)
            return;
        FindSAVEditorViewModel()?.CheckLegalitySlotCommand.Execute(slot);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        _pressPosition = e.GetPosition(this);
        _isPressed = true;
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isPressed)
            return;

        var currentPos = e.GetPosition(this);
        var delta = currentPos - _pressPosition;
        if (Math.Abs(delta.X) < DragThreshold && Math.Abs(delta.Y) < DragThreshold)
            return;

        // We've moved past the threshold — initiate drag
        _isPressed = false;

        if (DataContext is not SlotModel slot)
            return;

        var manager = FindSlotChangeManager();
        if (manager is null)
            return;

        await manager.StartDragAsync(slot, e);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isPressed)
            return;

        _isPressed = false;

        // This was a click (not a drag) — view the slot
        if (e.InitialPressMouseButton != MouseButton.Left)
            return;

        if (DataContext is not SlotModel slot)
            return;

        var vm = FindSAVEditorViewModel();
        if (vm is null)
            return;

        // Modifier key shortcuts: Ctrl+click = View, Shift+click = Set, Alt+click = Delete
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            vm.ViewSlotCommand.Execute(slot);
        else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            vm.SetSlotCommand.Execute(slot);
        else if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
            vm.DeleteSlotCommand.Execute(slot);
        else
            vm.ViewSlotCommand.Execute(slot);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var manager = FindSlotChangeManager();
        if (manager is not null)
        {
            manager.HandleDragOver(e);

            // Visual feedback: highlight the border
            if (e.DragEffects != DragDropEffects.None)
            {
                SlotBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215));
                e.Handled = true; // prevent bubbling to MainWindow
            }
        }
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        // Reset border to default
        SlotBorder.ClearValue(Border.BorderBrushProperty);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        // Reset border to default
        SlotBorder.ClearValue(Border.BorderBrushProperty);

        if (DataContext is not SlotModel slot)
            return;

        var manager = FindSlotChangeManager();
        if (manager is not null)
        {
            manager.HandleDrop(slot, e);
            e.Handled = true; // prevent bubbling to MainWindow
        }
    }

    /// <summary>
    /// Walks up the visual tree to find the <see cref="SAVEditorViewModel"/>.
    /// Falls back to reaching it through <see cref="BoxViewerViewModel.SlotManager"/>
    /// when the slot lives inside a BoxViewer window.
    /// </summary>
    private SAVEditorViewModel? FindSAVEditorViewModel()
    {
        var itemsControl = this.FindAncestorOfType<ItemsControl>();
        if (itemsControl?.DataContext is SAVEditorViewModel savVm)
            return savVm;

        // BoxViewer path: reach the main editor through the shared SlotChangeManager
        if (itemsControl?.DataContext is BoxViewerViewModel boxVm)
            return boxVm.SlotManager?.Editor;

        return null;
    }

    /// <summary>
    /// Finds the <see cref="SlotChangeManager"/> from either the <see cref="SAVEditorViewModel"/>
    /// or a <see cref="BoxViewerViewModel"/> ancestor.
    /// </summary>
    private SlotChangeManager? FindSlotChangeManager()
    {
        var itemsControl = this.FindAncestorOfType<ItemsControl>();

        if (itemsControl?.DataContext is SAVEditorViewModel savVm)
            return savVm.SlotManager;

        if (itemsControl?.DataContext is BoxViewerViewModel boxVm)
            return boxVm.SlotManager;

        return null;
    }
}
