using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Android.Util;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application;
using PKHeX.Application.Services;
using PKHeX.Infrastructure;
using PKHeX.Presentation.Localization;

namespace PKHeX.Android;

public partial class App : global::Avalonia.Application
{
    private IServiceProvider? _services;

    public override void Initialize()
    {
        Log.Info("PKHEX_ANDROID", "App.Initialize: before XAML");
        AvaloniaXamlLoader.Load(this);
        Log.Info("PKHEX_ANDROID", "App.Initialize: after XAML");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: start");
        InitializeCoreLocalization();
        Log.Info("PKHEX_ANDROID", "App.OnFrameworkInitializationCompleted: localization ready");

        if (ApplicationLifetime is ISingleViewApplicationLifetime activity)
        {
            // Avalonia.Android 11.3.18 exposes ISingleViewApplicationLifetime rather than the
            // newer IActivityApplicationLifetime/MainViewFactory API. Assign a fresh view here;
            // MainActivity also refreshes this assignment after an Android activity recreation.
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

    private void InitializeCoreLocalization()
    {
        Log.Info("PKHEX_ANDROID", "InitializeCoreLocalization: building services");
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure();
        services.AddSingleton<AppSettings>();
        _services = services.BuildServiceProvider();
        Log.Info("PKHEX_ANDROID", "InitializeCoreLocalization: services built");

        var languageService = _services.GetRequiredService<LanguageService>();
        var settings = _services.GetRequiredService<AppSettings>();
        Log.Info("PKHEX_ANDROID", "InitializeCoreLocalization: setting language");
        languageService.SetLanguage(settings.DisplayLanguage);
        Log.Info("PKHEX_ANDROID", "InitializeCoreLocalization: setting presentation strings");
        LocalizedStrings.Instance.SetLanguage(languageService.CurrentLanguage);
        Log.Info("PKHEX_ANDROID", "InitializeCoreLocalization: complete");
    }

    internal static MainView CreateMainView()
    {
        Log.Info("PKHEX_ANDROID", "CreateMainView: start");
        var view = new MainView();
        Log.Info("PKHEX_ANDROID", "CreateMainView: complete");
        return view;
    }
}
