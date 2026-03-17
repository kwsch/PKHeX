using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using PKHeX.Avalonia.Settings;
using PKHeX.Avalonia.ViewModels;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;

namespace PKHeX.Avalonia;

public partial class App : Application
{
    public static PKHeXSettings Settings { get; private set; } = new();
    public static Version CurrentVersion { get; } = typeof(App).Assembly.GetName().Version ?? new Version(1, 0, 0);
    public static string WorkingDirectory { get; } = Path.GetDirectoryName(Environment.ProcessPath) ?? "";
    public static bool HaX { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        LoadSettings();

        if (Settings.Startup.DarkMode)
            Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
            desktop.MainWindow = mainWindow;

            // Auto-load save file on startup, after the window is shown
            mainWindow.Opened += (_, _) =>
            {
                try
                {
                    var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
                    var startup = StartupUtil.GetStartup(args, Settings);
                    if (startup.SAV is { } sav)
                    {
                        var path = sav.Metadata.FilePath ?? string.Empty;
                        mainViewModel.LoadInitialSave(sav, startup.Entity, path);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Auto-load on startup failed: {ex.Message}");
                }
            };

            desktop.ShutdownRequested += (_, _) =>
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(Settings, PKHeXSettings.SerializerOptions);
                    File.WriteAllText(GetConfigPath(), json);
                }
                catch { }
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void LoadSettings()
    {
        var configPath = GetConfigPath();
        Settings = PKHeXSettings.GetSettings(configPath);
        StartupUtil.ReloadSettings(Settings);
        Settings.LocalResources.SetLocalPath(WorkingDirectory);
        SpriteBuilder.LoadSettings(Settings.Sprite);

        // Handle HaX and version tracking (mirrors WinForms FormLoadInitialActions)
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        var init = StartupUtil.FormLoadInitialActions(args, Settings, CurrentVersion);
        HaX = init.HaX;

        var language = Settings.Startup.Language;
        LocalizeUtil.InitializeStrings(language);
    }

    private static string GetConfigPath() => Path.Combine(WorkingDirectory, "cfg.json");
}
