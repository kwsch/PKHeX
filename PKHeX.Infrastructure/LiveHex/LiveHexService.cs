using System;
using System.Threading;
using System.Threading.Tasks;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Core;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// Infrastructure implementation of <see cref="ILiveHexService"/>. Orchestrates a sys-botbase
/// connection (created via <see cref="IConsoleConnectionFactory"/>), validates the attached game,
/// and bridges box bytes between console RAM and a <see cref="SaveFile"/>.
/// </summary>
public sealed class LiveHexService : ILiveHexService
{
    private const int TimeoutMs = 8000;

    private readonly IConsoleConnectionFactory _factory;
    private IConsoleConnection? _connection;
    private LiveHexGameProfile? _profile;

    public LiveHexService(IConsoleConnectionFactory factory) => _factory = factory;

    public bool IsConnected => _connection is { Connected: true };

    public LiveHexSessionInfo? Session { get; private set; }

    public LiveHexGameSupport GetSupport(SaveFile sav)
    {
        var name = LiveHexGameProfiles.GetGameName(sav);
        if (LiveHexGameProfiles.IsSupported(sav))
            return new LiveHexGameSupport(true, name, $"Supported firmware: {LiveHexGameProfiles.GetSupportedVersions(sav)}");
        return new LiveHexGameSupport(false, name,
            "LiveHeX supports Sword/Shield, Brilliant Diamond/Shining Pearl, Legends: Arceus and Scarlet/Violet only.");
    }

    public Task ConnectAsync(string ip, int port, SaveFile sav, CancellationToken cancellationToken = default) =>
        Task.Run(() => Connect(ip, port, sav), cancellationToken);

    private void Connect(string ip, int port, SaveFile sav)
    {
        if (!LiveHexGameProfiles.IsSupported(sav))
            throw new LiveHexConnectionException("This save type is not supported by LiveHeX.");

        DisconnectInternal();

        var connection = _factory.Create();
        connection.Connect(ip, port, TimeoutMs); // throws LiveHexConnectionException on failure
        try
        {
            var botbaseVersion = connection.GetBotbaseVersion();
            var titleId = connection.GetTitleId();
            if (!LiveHexGameProfiles.TitleMatchesSave(sav, titleId))
            {
                throw new LiveHexConnectionException(
                    $"The console is running a different game (title {titleId}) than the loaded save " +
                    $"({LiveHexGameProfiles.GetGameName(sav)}). Open the matching save and reconnect.");
            }

            var gameVersion = connection.GetGameInfo("version");
            var profile = LiveHexGameProfiles.Resolve(sav, titleId, gameVersion)
                ?? throw new LiveHexConnectionException(
                    $"Unsupported game version '{gameVersion}'. Supported versions for " +
                    $"{LiveHexGameProfiles.GetGameName(sav)}: {LiveHexGameProfiles.GetSupportedVersions(sav)}.");

            _connection = connection;
            _profile = profile;
            Session = new LiveHexSessionInfo(titleId, botbaseVersion, gameVersion, profile.Label);
        }
        catch
        {
            connection.Dispose();
            _connection = null;
            _profile = null;
            Session = null;
            throw;
        }
    }

    public Task DisconnectAsync() => Task.Run(DisconnectInternal);

    private void DisconnectInternal()
    {
        _connection?.Dispose();
        _connection = null;
        _profile = null;
        Session = null;
    }

    public Task ReadBoxAsync(SaveFile sav, int box, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            var (connection, profile) = RequireSession(sav, box);
            var bytes = LiveHexBoxAddressing.ReadBox(connection, profile, box, sav.SIZE_BOXSLOT, sav.BoxSlotCount);
            if (!sav.SetBoxBinary(bytes, box))
            {
                throw new LiveHexConnectionException(
                    "The box data read from the console could not be applied (the box is overwrite-protected).");
            }
        }, cancellationToken);

    public Task WriteBoxAsync(SaveFile sav, int box, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            var (connection, profile) = RequireSession(sav, box);
            var bytes = sav.GetBoxBinary(box);
            LiveHexBoxAddressing.WriteBox(connection, profile, box, bytes, sav.SIZE_BOXSLOT, sav.BoxSlotCount);
        }, cancellationToken);

    private (IConsoleConnection Connection, LiveHexGameProfile Profile) RequireSession(SaveFile sav, int box)
    {
        if (_connection is not { Connected: true } connection || _profile is null)
            throw new LiveHexConnectionException("Not connected to a console.");
        if ((uint)box >= (uint)sav.BoxCount)
            throw new LiveHexConnectionException($"Box {box + 1} is out of range for this save (has {sav.BoxCount} boxes).");
        return (connection, _profile);
    }
}
