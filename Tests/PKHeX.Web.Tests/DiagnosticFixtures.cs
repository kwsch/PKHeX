using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Interop;
using PKHeX.Web.Services.Diagnostics;

namespace PKHeX.Web.Tests;

/// <summary>Diagnostic logs for tests, with a fixed clock and a logger that keeps what would reach the browser console.</summary>
internal static class DiagnosticFixtures
{
    /// <summary>When a fixture clock starts.</summary>
    public static readonly DateTimeOffset Start = new(2026, 10, 3, 12, 0, 0, TimeSpan.Zero);

    /// <summary>A new log on a fixed clock; <paramref name="console"/> receives every line it writes.</summary>
    public static DiagnosticLog NewLog(out RecordingLogger<DiagnosticLog> console, TimeProvider? clock = null)
    {
        console = new RecordingLogger<DiagnosticLog>();
        return new DiagnosticLog(console, clock ?? new FakeTimeProvider(Start));
    }

    /// <summary>A new log on a fixed clock.</summary>
    public static DiagnosticLog NewLog() => NewLog(out _);

    /// <summary>
    /// Registers what the diagnostic panel needs in a bUnit context: <paramref name="log"/>, the browser services and a fixed clock (unless one
    /// is registered already).
    /// </summary>
    public static void AddDiagnostics(IServiceCollection services, DiagnosticLog log)
    {
        services.AddSingleton(log);
        services.AddScoped<BrowserPage>();
        services.AddScoped<BrowserFileService>();
        if (!services.Any(d => d.ServiceType == typeof(TimeProvider)))
        {
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(Start));
        }
    }
}

/// <summary>Keeps every formatted log line and the exception passed with it, so tests can check what reaches the browser console.</summary>
internal sealed class RecordingLogger<T> : ILogger<T>
{
    /// <summary>The formatted messages, in order.</summary>
    public List<string> Messages { get; } = [];

    /// <summary>Exceptions passed to the logger; the diagnostic log never passes one.</summary>
    public List<Exception> Exceptions { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Messages.Add(formatter(state, exception));
        if (exception is not null)
        {
            Exceptions.Add(exception);
        }
    }
}
