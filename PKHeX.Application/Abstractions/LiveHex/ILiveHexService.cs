using System.Threading;
using System.Threading.Tasks;
using PKHeX.Core;

namespace PKHeX.Application.Abstractions.LiveHex;

/// <summary>
/// Game-support classification for a loaded <see cref="SaveFile"/>.
/// </summary>
/// <param name="Supported">Whether LiveHeX box read/write is available for this save type.</param>
/// <param name="GameName">Human-readable game name (e.g. "Sword/Shield").</param>
/// <param name="Detail">Supplementary note (supported firmware versions, or why it is unsupported).</param>
public readonly record struct LiveHexGameSupport(bool Supported, string GameName, string Detail);

/// <summary>
/// Details about the currently attached console, populated on a successful connect.
/// </summary>
/// <param name="TitleId">Attached process title id.</param>
/// <param name="BotbaseVersion">Reported sys-botbase version.</param>
/// <param name="GameVersion">Reported running-game version (e.g. "1.3.2").</param>
/// <param name="ProfileLabel">The matched offset-profile label used for box addressing.</param>
public readonly record struct LiveHexSessionInfo(string TitleId, string BotbaseVersion, string GameVersion, string ProfileLabel);

/// <summary>
/// High-level, ViewModel-facing LiveHeX port: connect to a console over Wi-Fi (sys-botbase TCP),
/// detect game support, and read/write the current box between the console and a
/// <see cref="SaveFile"/>. All network IO happens behind this port; the implementation lives in
/// the Infrastructure layer and is injectable/mockable.
/// </summary>
public interface ILiveHexService
{
    /// <summary>True while a console session is open.</summary>
    bool IsConnected { get; }

    /// <summary>Session details once connected; <see langword="null"/> otherwise.</summary>
    LiveHexSessionInfo? Session { get; }

    /// <summary>Classifies whether the given save type is supported by LiveHeX.</summary>
    LiveHexGameSupport GetSupport(SaveFile sav);

    /// <summary>
    /// Opens a sys-botbase connection and validates that the attached game matches
    /// <paramref name="sav"/> and runs a supported firmware version.
    /// </summary>
    /// <exception cref="LiveHexConnectionException">On any connect/validation failure.</exception>
    Task ConnectAsync(string ip, int port, SaveFile sav, CancellationToken cancellationToken = default);

    /// <summary>Closes the console connection. Safe to call when not connected.</summary>
    Task DisconnectAsync();

    /// <summary>
    /// Reads box <paramref name="box"/> from the console and applies it to <paramref name="sav"/>
    /// (via <see cref="SaveFile.SetBoxBinary"/>).
    /// </summary>
    /// <exception cref="LiveHexConnectionException">On any IO/protocol failure.</exception>
    Task ReadBoxAsync(SaveFile sav, int box, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes box <paramref name="box"/> from <paramref name="sav"/> (via
    /// <see cref="SaveFile.GetBoxBinary"/>) back to the console.
    /// </summary>
    /// <remarks>
    /// This performs no confirmation of its own; callers MUST obtain explicit user confirmation
    /// before invoking it (see the LiveHeX ViewModel).
    /// </remarks>
    /// <exception cref="LiveHexConnectionException">On any IO/protocol failure.</exception>
    Task WriteBoxAsync(SaveFile sav, int box, CancellationToken cancellationToken = default);
}
