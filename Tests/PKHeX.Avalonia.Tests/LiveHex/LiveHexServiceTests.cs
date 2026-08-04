using System.Threading.Tasks;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Core;
using PKHeX.Infrastructure.LiveHex;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>Tests for <see cref="LiveHexService"/> connect validation and box bridging.</summary>
public class LiveHexServiceTests
{
    private sealed class FakeFactory(FakeSwitchMemory memory) : IConsoleConnectionFactory
    {
        public IConsoleConnection Create() => memory;
    }

    private static (LiveHexService Service, FakeSwitchMemory Memory) NewService()
    {
        var mem = new FakeSwitchMemory();
        return (new LiveHexService(new FakeFactory(mem)), mem);
    }

    [Fact]
    public void GetSupport_classifies_save_types()
    {
        var (service, _) = NewService();
        Assert.True(service.GetSupport(new SAV8SWSH()).Supported);
        Assert.True(service.GetSupport(new SAV9SV()).Supported);
        Assert.True(service.GetSupport(new SAV8BS()).Supported);
        Assert.True(service.GetSupport(new SAV8LA()).Supported);

        var unsupported = service.GetSupport(new SAV7SM());
        Assert.False(unsupported.Supported);
        Assert.False(string.IsNullOrWhiteSpace(unsupported.Detail));
    }

    [Fact]
    public async Task ConnectAsync_succeeds_and_populates_session_for_matching_game()
    {
        var (service, mem) = NewService();
        mem.TitleId = "0100ABF008968000"; // Sword
        mem.GameVersion = "1.3.2";

        await service.ConnectAsync("127.0.0.1", 6000, new SAV8SWSH());

        Assert.True(service.IsConnected);
        Assert.True(mem.ConnectCalled);
        Assert.NotNull(service.Session);
        Assert.Equal("1.3.2", service.Session!.Value.GameVersion);
        Assert.Contains("Sword", service.Session!.Value.ProfileLabel);
    }

    [Fact]
    public async Task ConnectAsync_rejects_wrong_game()
    {
        var (service, mem) = NewService();
        mem.TitleId = "0100A3D008C5C000"; // Scarlet
        mem.GameVersion = "4.0.0";

        var ex = await Assert.ThrowsAsync<LiveHexConnectionException>(
            () => service.ConnectAsync("127.0.0.1", 6000, new SAV8SWSH()));
        Assert.Contains("different game", ex.Message);
        Assert.False(service.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_rejects_unsupported_firmware()
    {
        var (service, mem) = NewService();
        mem.TitleId = "0100ABF008968000"; // Sword
        mem.GameVersion = "9.9.9";

        var ex = await Assert.ThrowsAsync<LiveHexConnectionException>(
            () => service.ConnectAsync("127.0.0.1", 6000, new SAV8SWSH()));
        Assert.Contains("Unsupported game version", ex.Message);
        Assert.False(service.IsConnected);
    }

    [Fact]
    public async Task WriteBox_then_ReadBox_roundtrips_through_console_and_save()
    {
        var (service, mem) = NewService();
        mem.TitleId = "0100ABF008968000";
        mem.GameVersion = "1.3.2";

        var source = new SAV8SWSH();
        // Seed a genuine (valid) Pokémon so the source box differs from a blank target box.
        var pk = source.BlankPKM;
        pk.Species = 25; // Pikachu
        pk.RefreshChecksum();
        source.SetBoxSlotAtIndex(pk, 0, 0);
        var sourceBox = source.GetBoxBinary(0);
        Assert.NotEqual(new SAV8SWSH().GetBoxBinary(0), sourceBox); // sanity: box is non-blank

        await service.ConnectAsync("127.0.0.1", 6000, source);
        await service.WriteBoxAsync(source, 0);

        // The console heap now holds the exact box bytes we wrote.
        Assert.Equal(sourceBox, mem.GetHeapBytes(0x45075880, sourceBox.Length));

        // Read the console box into a fresh (blank) save; its box binary must now match the source.
        var target = new SAV8SWSH();
        await service.ReadBoxAsync(target, 0);
        Assert.Equal(sourceBox, target.GetBoxBinary(0));
    }

    [Fact]
    public async Task Operations_before_connect_throw_not_connected()
    {
        var (service, _) = NewService();
        var ex = await Assert.ThrowsAsync<LiveHexConnectionException>(
            () => service.ReadBoxAsync(new SAV8SWSH(), 0));
        Assert.Contains("Not connected", ex.Message);
    }

    [Fact]
    public async Task ReadBox_rejects_out_of_range_box()
    {
        var (service, mem) = NewService();
        mem.TitleId = "0100ABF008968000";
        mem.GameVersion = "1.3.2";
        var sav = new SAV8SWSH();
        await service.ConnectAsync("127.0.0.1", 6000, sav);

        var ex = await Assert.ThrowsAsync<LiveHexConnectionException>(
            () => service.ReadBoxAsync(sav, sav.BoxCount + 5));
        Assert.Contains("out of range", ex.Message);
    }
}
