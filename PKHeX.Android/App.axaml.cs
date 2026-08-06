using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Android.Util;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Core;
using PKHeX.Infrastructure;
using PKHeX.Presentation.Localization;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Android;

public partial class App : global::Avalonia.Application
{
    private IServiceProvider? _services;
    private MainWindowViewModel? _mainViewModel;

    public override void Initialize()
    {
        Log.Info("PKHEX_ANDROID", "App.Initialize: before XAML");
        AvaloniaXamlLoader.Load(this);
        // After the XAML palette is in place, so the system accents override the bundled defaults.
        AndroidDynamicColor.Apply(this);
        Log.Info("PKHEX_ANDROID", "App.Initialize: after XAML");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: start");
        InitializeFullApplication();
        Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: localization ready");

        if (ApplicationLifetime is ISingleViewApplicationLifetime activity)
        {
            activity.MainView = CreateMainView();
            Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: main view assigned");
        }
        else
        {
            Log.Warn("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: no single-view lifetime available");
        }

        base.OnFrameworkInitializationCompleted();
        Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: complete");
    }

    private void InitializeFullApplication()
    {
        Log.Info("PKHEX_ANDROID", "InitializeFullApplication: building services");

        // Let the settings store load as it does on desktop. Passing a hand-built AppSettings here
        // told BuildServiceProvider to skip the store entirely, so every preference reverted on
        // launch — most visibly the display language, which reset to English each time even though
        // changing it does persist. PKHaX is forced on afterwards, since that is this host's point.
        _services = global::PKHeX.Avalonia.App.BuildServiceProvider(
            configureOverrides: services =>
            {
                // Desktop registrations create Avalonia Windows, which Android does not expose.
                // Keep the shared Presentation graph intact and replace only the host adapters.
                services.AddSingleton<SaveFileService>();
                services.AddSingleton<ISaveFileGateway>(sp =>
                    new AndroidSaveFileGateway(sp.GetRequiredService<SaveFileService>()));
                services.AddSingleton<IDialogService, AndroidDialogService>();
                services.AddSingleton<IWindowService, AndroidWindowService>();
                services.AddSingleton<IClipboardService, AndroidClipboardService>();
            });
        Log.Info("PKHEX_ANDROID", "InitializeFullApplication: services built");

        var settings = _services.GetRequiredService<AppSettings>();

        // First run only — no settings file yet — start in the phone's language instead of English.
        // Keyed on the file's absence rather than the language value, so someone who deliberately
        // picks English on a Chinese phone is not flipped back on every launch. Persisting it here
        // also means this branch never runs for them again.
        var paths = _services.GetRequiredService<IAppPaths>();
        if (!File.Exists(paths.ConfigFilePath))
        {
            settings.DisplayLanguage = AndroidSystemLanguage.Resolve(LocalizedStrings.SupportedLanguages);
            Log.Info("PKHEX_ANDROID", $"First run: adopting system language {settings.DisplayLanguage}");
            _services.GetRequiredService<ISettingsStore>().Save(settings);
        }

        settings.HaXMode = true;
        settings.Startup.ForceHaXOnLaunch = true;
        StartupUtil.ReloadSettings(settings);
        EntityConverter.AllowIncompatibleConversion = EntityCompatibilitySetting.AllowIncompatibleAll;

        var languageService = _services.GetRequiredService<LanguageService>();
        Log.Info("PKHEX_ANDROID", "InitializeFullApplication: setting language");
        languageService.SetLanguage(settings.DisplayLanguage);
        LocalizedStrings.Instance.SetLanguage(languageService.CurrentLanguage);
        _services.GetRequiredService<global::PKHeX.Avalonia.Services.ThemeService>().Initialize();
        _mainViewModel = _services.GetRequiredService<MainWindowViewModel>();
        Log.Info("PKHEX_ANDROID", $"InitializeFullApplication: PKHaX={_mainViewModel.IsHaXMode}");
    }

    internal MainView CreateMainView()
    {
        Log.Info("PKHEX_ANDROID", "CreateMainView: start");
        if (_mainViewModel is null)
            throw new InvalidOperationException("The full PKHeX application was not initialized.");

        var content = new AndroidMainView { DataContext = _mainViewModel };
        var view = new MainView(content);
        AndroidHostContext.SetMainView(view);
        Log.Info("PKHEX_ANDROID", "CreateMainView: complete");
        return view;
    }
}
