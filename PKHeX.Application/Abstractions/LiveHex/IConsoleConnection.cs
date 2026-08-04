using System;

namespace PKHeX.Application.Abstractions.LiveHex;

/// <summary>
/// Low-level, framework-free abstraction over a single console memory-injection connection
/// (sys-botbase over TCP). This is the Application-layer port for all network IO; the concrete
/// socket implementation lives in the Infrastructure layer and this interface is fully mockable
/// so protocol/box logic can be unit-tested against a fake console.
/// </summary>
/// <remarks>
/// All members are synchronous because the sys-botbase wire protocol is a strict
/// request/response exchange over a single socket. Higher layers marshal these onto a background
/// thread to keep the UI responsive.
/// </remarks>
public interface IConsoleConnection : IDisposable
{
    /// <summary>True while the underlying socket is connected.</summary>
    bool Connected { get; }

    /// <summary>Opens the socket to <paramref name="ip"/>:<paramref name="port"/>.</summary>
    /// <param name="ip">Console IP address.</param>
    /// <param name="port">sys-botbase port (default 6000).</param>
    /// <param name="timeoutMs">Connect/read/write timeout in milliseconds.</param>
    /// <exception cref="LiveHexConnectionException">Thrown when the connection cannot be established.</exception>
    void Connect(string ip, int port, int timeoutMs);

    /// <summary>Closes the socket. Safe to call when already disconnected.</summary>
    void Disconnect();

    /// <summary>Reads <paramref name="length"/> bytes from the heap-relative <paramref name="offset"/>.</summary>
    byte[] ReadHeap(ulong offset, int length);

    /// <summary>Writes <paramref name="data"/> to the heap-relative <paramref name="offset"/>.</summary>
    void WriteHeap(ReadOnlySpan<byte> data, ulong offset);

    /// <summary>Reads <paramref name="length"/> bytes from the main-NSO-relative <paramref name="offset"/>.</summary>
    byte[] ReadMain(ulong offset, int length);

    /// <summary>Reads <paramref name="length"/> bytes from the absolute <paramref name="offset"/>.</summary>
    byte[] ReadAbsolute(ulong offset, int length);

    /// <summary>Writes <paramref name="data"/> to the absolute <paramref name="offset"/>.</summary>
    void WriteAbsolute(ReadOnlySpan<byte> data, ulong offset);

    /// <summary>Requests the heap base address of the attached process.</summary>
    ulong GetHeapBase();

    /// <summary>Requests the title id (16 hex chars) of the attached process.</summary>
    string GetTitleId();

    /// <summary>Requests the sys-botbase version string.</summary>
    string GetBotbaseVersion();

    /// <summary>Requests a piece of running-game information (e.g. <c>"version"</c>).</summary>
    string GetGameInfo(string info);
}
