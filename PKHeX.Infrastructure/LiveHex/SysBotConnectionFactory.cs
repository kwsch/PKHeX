using PKHeX.Application.Abstractions.LiveHex;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>Produces real sys-botbase TCP connections.</summary>
public sealed class SysBotConnectionFactory : IConsoleConnectionFactory
{
    public IConsoleConnection Create() => new SysBotConnection();
}
