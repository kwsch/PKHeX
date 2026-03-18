using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using PKHeX.Avalonia.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class MainWindow : Window
{
    private bool _forceClose;
    private bool _isClosing;

    public MainWindow()
    {
        InitializeComponent();

        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        Closing += OnWindowClosing;
        Opened += OnWindowOpened;
    }

    private async void OnWindowOpened(object? sender, EventArgs e)
    {
        // Fire-and-forget update check on startup
        if (DataContext is MainWindowViewModel vm)
            await vm.CheckForUpdatesAsync();
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_forceClose)
            return;

        if (_isClosing)
        {
            e.Cancel = true;
            return;
        }

        if (DataContext is MainWindowViewModel vm && vm.HasUnsavedChanges)
        {
            e.Cancel = true;
            _isClosing = true;

            var confirmDialog = new Window
            {
                Title = "Unsaved Changes",
                Width = 360,
                Height = 140,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            var result = false;

            var cancelBtn = new Button { Content = "Cancel", Width = 80 };
            cancelBtn.Click += (_, _) => { result = false; confirmDialog.Close(); };

            var closeBtn = new Button { Content = "Close Anyway", Width = 100 };
            closeBtn.Click += (_, _) => { result = true; confirmDialog.Close(); };

            confirmDialog.Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "You have unsaved changes. Close without saving?",
                        TextWrapping = TextWrapping.Wrap,
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { cancelBtn, closeBtn },
                    },
                },
            };

            await confirmDialog.ShowDialog(this);
            _isClosing = false;

            if (result)
            {
                _forceClose = true;
                Close();
            }
        }
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

        App.Settings.Startup.DarkMode = Application.Current.RequestedThemeVariant == ThemeVariant.Dark;
    }
}
