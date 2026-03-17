using System;
using System.IO;
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
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
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

        var language = Settings.Startup.Language;
        LocalizeUtil.InitializeStrings(language);
    }

    private static string GetConfigPath() => Path.Combine(WorkingDirectory, "cfg.json");
}
