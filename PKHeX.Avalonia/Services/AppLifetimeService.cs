using Avalonia.Controls.ApplicationLifetimes;
using PKHeX.Application.Abstractions;

namespace PKHeX.Avalonia.Services;

/// <summary>Avalonia implementation of <see cref="IAppLifetime"/>, over the classic desktop lifetime.</summary>
public sealed class AppLifetimeService : IAppLifetime
{
    public void Shutdown()
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}
