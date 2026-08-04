namespace PKHeX.Application.Abstractions.LiveHex;

/// <summary>
/// Creates fresh <see cref="IConsoleConnection"/> instances. Lets the LiveHeX service open a new
/// socket per session while remaining decoupled from the concrete Infrastructure socket type
/// (and trivially fakeable in tests).
/// </summary>
public interface IConsoleConnectionFactory
{
    /// <summary>Creates a new, unconnected console connection.</summary>
    IConsoleConnection Create();
}
