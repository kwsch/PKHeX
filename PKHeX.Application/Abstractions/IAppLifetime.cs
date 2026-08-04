namespace PKHeX.Application.Abstractions;

/// <summary>
/// Thin port over the host application's shutdown mechanism. Lets Presentation-layer ViewModels
/// (e.g. after a self-update swap is staged and a relaunch helper is waiting) trigger a clean exit
/// without referencing Avalonia's application lifetime types directly.
/// </summary>
public interface IAppLifetime
{
    /// <summary>Requests a normal application shutdown.</summary>
    void Shutdown();
}
