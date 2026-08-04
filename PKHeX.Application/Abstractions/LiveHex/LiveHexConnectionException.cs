using System;

namespace PKHeX.Application.Abstractions.LiveHex;

/// <summary>
/// Raised when a LiveHeX operation fails in a way that should be surfaced to the user with a clear,
/// actionable message (unreachable console, wrong game, unsupported firmware, timeout, protocol
/// error). Carrying a dedicated type lets the ViewModel render the message without leaking raw
/// socket/format exceptions.
/// </summary>
public sealed class LiveHexConnectionException : Exception
{
    public LiveHexConnectionException(string message) : base(message) { }
    public LiveHexConnectionException(string message, Exception inner) : base(message, inner) { }
}
