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
        var settings = new AppSettings
        {
            DisplayLanguage = "en",
            HaXMode = true,
        };
        settings.Startup.ForceHaXOnLaunch = true;
        StartupUtil.ReloadSettings(settings);
        EntityConverter.AllowIncompatibleConversion = EntityCompatibilitySetting.AllowIncompatibleAll;

        _services = global::PKHeX.Avalonia.App.BuildServiceProvider(
            settings: settings,
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
