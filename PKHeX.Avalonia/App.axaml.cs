using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Infrastructure;
using PKHeX.Infrastructure.Configuration;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Avalonia.Views;

namespace PKHeX.Avalonia;

public partial class App : global::Avalonia.Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = BuildServiceProvider();

        // Register process-wide last-chance handlers first, before anything else can throw.
        GlobalExceptionHandler.Install(Services);

        // Initialize PKHeX Core
        // GameInfo is initialized via AppSettings in ConfigureServices

        // Apply the persisted theme preference before any window is created.
        Services.GetRequiredService<ThemeService>().Initialize();

        // Apply the persisted UI/data language before any window is created so both the game-data
        // strings (GameInfo) and the shell's UI-chrome strings (LocalizedStrings, synced by the main
        // ViewModel's ctor) render in the saved language from the first frame — no restart needed.
        var settings = Services.GetRequiredService<AppSettings>();
        Services.GetRequiredService<LanguageService>().SetLanguage(settings.DisplayLanguage);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            // Fire-and-forget: never awaited here, so the main window is shown immediately and is
            // never blocked by a slow, offline, or rate-limited GitHub API call. The coordinator
            // already stays silent on failure, but observe the task's exception anyway so a
            // faulted continuation can never surface as an UnobservedTaskException.
            _ = mainViewModel.CheckForUpdatesAsync().ContinueWith(
                t => Trace.TraceWarning($"Startup update check failed: {t.Exception?.GetBaseException().Message}"),
                TaskContinuationOptions.OnlyOnFaulted);
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Builds the application's dependency-injection container — the single source of truth for the
    /// object graph, shared by the real host startup (<see cref="OnFrameworkInitializationCompleted"/>)
    /// and the headless full-app UI test harness.
    /// </summary>
    /// <remarks>
    /// Production callers pass nothing: real OS config paths and the persisted settings store are used.
    /// Tests pass a throwaway <paramref name="paths"/>/<paramref name="settingsStore"/> so no real user
    /// config is read or written, and a <paramref name="configureOverrides"/> callback to swap host
    /// services (e.g. dialogs/windows) for deterministic test doubles. Overrides run last, so — with
    /// Microsoft.Extensions.DependencyInjection's last-registration-wins resolution — they replace the
    /// production registrations without the graph drifting from this single definition.
    /// </remarks>
    public static IServiceProvider BuildServiceProvider(
        IAppPaths? paths = null,
        ISettingsStore? settingsStore = null,
        AppSettings? settings = null,
        Action<IServiceCollection>? configureOverrides = null)
    {
        var services = new ServiceCollection();
        ConfigureServices(services, paths, settingsStore, settings);
        configureOverrides?.Invoke(services);
        return services.BuildServiceProvider();
    }

    private static void ConfigureServices(
        IServiceCollection services,
        IAppPaths? paths = null,
        ISettingsStore? settingsStore = null,
        AppSettings? settings = null)
    {
        // Inner layers (framework-free): use cases, ports + app-side impls, non-UI drivers.
        services.AddApplication();
        services.AddInfrastructure();

        // Config: resolve platform paths, load (migrating/recovering as needed) + seed Core
        // localization, then register the store and the loaded model for injection. Tests inject
        // temp paths/store (and often a fresh AppSettings) so nothing touches the real user config.
        paths ??= new AppPaths();
        settingsStore ??= new SettingsStore(paths);
        var config = settings ?? settingsStore.Load();
        config.InitializeCore();
        services.AddSingleton(paths);
        services.AddSingleton(settingsStore);
        services.AddSingleton(config);

        // Host (Frameworks & Drivers): Avalonia/Skia implementations of the Application ports.
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<ISpriteRenderer, AvaloniaSpriteRenderer>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IAppLifetime, AppLifetimeService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<IThemeService>(sp => sp.GetRequiredService<ThemeService>());

        // Update-check flow shared by the silent startup check and the manual "Check for Updates"
        // buttons (Settings/About). Singleton so the status-bar notification it raises persists for
        // the life of the main window regardless of which entry point ran the check.
        services.AddSingleton<UpdateCheckCoordinator>();

        // Root ViewModel (transient). Child/editor ViewModels are created by their parent presenter
        // with the current save, so they are not registered in the container.
        services.AddTransient<MainWindowViewModel>();
    }
}
