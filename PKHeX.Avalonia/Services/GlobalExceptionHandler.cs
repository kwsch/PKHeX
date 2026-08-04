using System.Diagnostics;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Application.Abstractions;
using PKHeX.Presentation.Localization;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Installs process-wide last-chance handlers so an unhandled exception on the UI thread, a
/// thread-pool task, or any other thread is logged (and, where the UI is still alive, surfaced
/// via an error dialog) instead of silently crashing the whole process.
/// </summary>
/// <remarks>
/// The three handlers are subscribed to <b>static</b>/process-lifetime events
/// (<see cref="Dispatcher.UnhandledException"/>, <see cref="TaskScheduler.UnobservedTaskException"/>,
/// <see cref="AppDomain.UnhandledException"/>). <see cref="Install"/> is therefore idempotent — a second
/// call replaces the target service provider but never subscribes a second time — and the concrete
/// handler delegates are retained so <see cref="Uninstall"/> can unsubscribe them. This keeps repeated
/// host builds (and the headless test host, whose <c>App</c> is constructed once per assembly) from
/// accumulating dangling subscriptions that pin dead service providers.
/// </remarks>
public static class GlobalExceptionHandler
{
    private static readonly object Gate = new();
    private static bool _installed;
    private static IServiceProvider? _services;

    // Retained delegate instances so the exact same handlers can be removed in Uninstall (lambdas
    // subscribed inline cannot be unsubscribed).
    private static DispatcherUnhandledExceptionEventHandler? _uiThreadHandler;
    private static EventHandler<UnobservedTaskExceptionEventArgs>? _unobservedTaskHandler;
    private static UnhandledExceptionEventHandler? _appDomainHandler;

    public static void Install(IServiceProvider services)
    {
        lock (Gate)
        {
            // Idempotent: keep the latest provider (so a re-init points dialogs at the live graph) but
            // never subscribe the static events more than once.
            _services = services;
            if (_installed)
                return;

            // UI-thread exceptions (e.g. thrown from a binding, command, or event handler running on
            // the dispatcher): log, mark handled so Avalonia doesn't tear down the process, and tell
            // the user something went wrong.
            _uiThreadHandler = (_, e) =>
            {
                Trace.TraceError($"Unhandled UI-thread exception: {e.Exception}");
                e.Handled = true;
                var current = _services;
                if (current is not null)
                    ShowErrorDialog(current, e.Exception);
            };

            // Fire-and-forget Tasks whose exception was never awaited/observed: log and mark
            // observed so the finalizer thread doesn't escalate it into a process crash.
            _unobservedTaskHandler = (_, e) =>
            {
                Trace.TraceError($"Unobserved task exception: {e.Exception}");
                e.SetObserved();
            };

            // Last-chance handler for exceptions on any other thread. By the time this fires the
            // runtime is already terminating the process for non-UI threads, so this is log-only.
            _appDomainHandler = (_, e) =>
            {
                Trace.TraceError($"Unhandled AppDomain exception (terminating={e.IsTerminating}): {e.ExceptionObject}");
            };

            Dispatcher.UIThread.UnhandledException += _uiThreadHandler;
            TaskScheduler.UnobservedTaskException += _unobservedTaskHandler;
            AppDomain.CurrentDomain.UnhandledException += _appDomainHandler;
            _installed = true;
        }
    }

    /// <summary>
    /// Removes the process-wide handlers installed by <see cref="Install"/> and clears the retained
    /// service provider. Safe to call when nothing is installed (no-op). Idempotent.
    /// </summary>
    public static void Uninstall()
    {
        lock (Gate)
        {
            if (!_installed)
                return;

            if (_uiThreadHandler is not null)
                Dispatcher.UIThread.UnhandledException -= _uiThreadHandler;
            if (_unobservedTaskHandler is not null)
                TaskScheduler.UnobservedTaskException -= _unobservedTaskHandler;
            if (_appDomainHandler is not null)
                AppDomain.CurrentDomain.UnhandledException -= _appDomainHandler;

            _uiThreadHandler = null;
            _unobservedTaskHandler = null;
            _appDomainHandler = null;
            _services = null;
            _installed = false;
        }
    }

    private static void ShowErrorDialog(IServiceProvider services, Exception ex)
    {
        var dialogService = services.GetRequiredService<IDialogService>();
        _ = dialogService.ShowErrorAsync(
            LocalizedStrings.Instance["App_UnexpectedErrorTitle"],
            LocalizedStrings.Instance.Format("App_UnexpectedErrorMessage", ex.Message));
    }
}
