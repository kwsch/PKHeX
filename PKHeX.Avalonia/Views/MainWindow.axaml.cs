using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using PKHeX.Avalonia.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        var files = e.Data.GetFiles()?.Select(f => f.Path.LocalPath).ToArray();
        if (files is { Length: > 0 })
            vm.HandleFileDrop(files);
    }

    private void OnToggleThemeClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Application.Current is null)
            return;

        var current = Application.Current.RequestedThemeVariant;
        Application.Current.RequestedThemeVariant =
            current == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
    }
}
