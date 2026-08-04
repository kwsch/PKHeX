
using System.Threading.Tasks;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Abstracted clipboard service for cross-platform access and testability.
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Gets text from the system clipboard asynchronously.
    /// </summary>
    Task<string?> GetTextAsync();

    /// <summary>
    /// Sets text to the system clipboard asynchronously.
    /// </summary>
    Task SetTextAsync(string text);
}
