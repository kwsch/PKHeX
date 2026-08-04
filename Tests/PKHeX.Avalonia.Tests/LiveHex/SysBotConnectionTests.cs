using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Infrastructure.LiveHex;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>
/// Exercises the real <see cref="SysBotConnection"/> socket path against an in-process loopback
/// server that speaks the sys-botbase text protocol, plus the timeout/error paths.
/// </summary>
public class SysBotConnectionTests
{
    /// <summary>
    /// Connects with retries so the connect phase gets a generous overall budget even when a
    /// contended CI runner makes the loopback handshake occasionally exceed <paramref name="timeoutMs"/>.
    /// Each attempt still uses the caller's short <paramref name="timeoutMs"/>, so the socket's
    /// ReceiveTimeout/SendTimeout (set from that same value by <see cref="SysBotConnection.Connect"/>)
    /// stays tight for whatever read/write the test performs afterward - only the connect phase
    /// is made generous, via retrying rather than inflating the per-attempt timeout.
    /// </summary>
    private static void ConnectGenerously(SysBotConnection conn, string ip, int port, int timeoutMs, int connectBudgetMs = 10_000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(connectBudgetMs);
        while (true)
        {
            try
            {
                conn.Connect(ip, port, timeoutMs);
                return;
            }
            catch (LiveHexConnectionException) when (DateTime.UtcNow < deadline)
            {
                // Transient contention on the loopback handshake; retry until the generous budget is spent.
            }
        }
    }

    /// <summary>Minimal loopback sys-botbase server. Set <paramref name="mute"/> to never respond (timeout tests).</summary>
    private sealed class FakeBotbaseServer : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cts = new();
        public readonly List<string> Received = [];
        public int Port { get; }

        public FakeBotbaseServer(bool mute = false)
        {
            _listener = new TcpListener(IPAddress.Loopback, 0);
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _ = Task.Run(() => AcceptLoop(mute, _cts.Token));
        }

        private async Task AcceptLoop(bool mute, CancellationToken ct)
        {
            try
            {
                using var client = await _listener.AcceptTcpClientAsync(ct);
                using var stream = client.GetStream();
                while (!ct.IsCancellationRequested)
                {
                    var line = ReadLine(stream);
                    if (line is null)
                        return;
                    lock (Received) Received.Add(line);
                    if (mute)
                        continue;
                    Respond(stream, line);
                }
            }
            catch
            {
                // listener torn down
            }
        }

        private static string? ReadLine(NetworkStream stream)
        {
            var sb = new StringBuilder();
            var one = new byte[1];
            while (true)
            {
                int read;
                try { read = stream.Read(one, 0, 1); }
                catch { return null; }
                if (read == 0)
                    return sb.Length == 0 ? null : sb.ToString();
                if (one[0] == (byte)'\n')
                    return sb.ToString().TrimEnd('\r');
                sb.Append((char)one[0]);
            }
        }

        private static void Respond(NetworkStream stream, string command)
        {
            string reply;
            if (command.StartsWith("getTitleID"))
                reply = "0100ABF008968000\n";
            else if (command.StartsWith("getVersion"))
                reply = "2.5\n";
            else if (command.StartsWith("game "))
                reply = "1.3.2\n";
            else if (command.StartsWith("getHeapBase"))
                reply = "0000000012345678\n"; // 8 bytes big-endian
            else if (command.StartsWith("peek"))
            {
                // Echo a deterministic response of the requested length.
                var parts = command.Split(' ');
                int count = int.Parse(parts[^1]);
                var sb = new StringBuilder();
                for (int i = 0; i < count; i++)
                    sb.Append(((byte)(i & 0xFF)).ToString("X2"));
                sb.Append('\n');
                reply = sb.ToString();
            }
            else
                return; // poke commands have no response

            var bytes = Encoding.ASCII.GetBytes(reply);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _listener.Stop();
            _cts.Dispose();
        }
    }

    [Fact]
    public void Reads_and_decodes_a_peek_response()
    {
        using var server = new FakeBotbaseServer();
        using var conn = new SysBotConnection();
        ConnectGenerously(conn, "127.0.0.1", server.Port, 3000);

        var data = conn.ReadHeap(0x1000, 8);
        Assert.Equal(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }, data);
    }

    [Fact]
    public void Reads_info_commands()
    {
        using var server = new FakeBotbaseServer();
        using var conn = new SysBotConnection();
        ConnectGenerously(conn, "127.0.0.1", server.Port, 3000);

        Assert.Equal("0100ABF008968000", conn.GetTitleId());
        Assert.Equal("2.5", conn.GetBotbaseVersion());
        Assert.Equal("1.3.2", conn.GetGameInfo("version"));
        Assert.Equal(0x12345678UL, conn.GetHeapBase());
    }

    [Fact]
    public void Sends_correctly_formatted_poke_command()
    {
        using var server = new FakeBotbaseServer();
        using var conn = new SysBotConnection();
        ConnectGenerously(conn, "127.0.0.1", server.Port, 3000);

        conn.WriteHeap([0xAB, 0xCD], 0x2000);

        // Give the server a moment to record the line.
        var deadline = DateTime.UtcNow.AddSeconds(2);
        string? poke = null;
        while (DateTime.UtcNow < deadline && poke is null)
        {
            lock (server.Received)
                poke = server.Received.Find(l => l.StartsWith("poke"));
            if (poke is null) Thread.Sleep(10);
        }
        Assert.Equal("poke 0x0000000000002000 0xABCD", poke);
    }

    [Fact]
    public void Connect_to_dead_endpoint_throws_clear_error()
    {
        using var conn = new SysBotConnection();
        // Port 1 on loopback should refuse quickly; assert we surface a typed error, not a raw socket exception.
        var ex = Assert.Throws<LiveHexConnectionException>(() => conn.Connect("127.0.0.1", 1, 800));
        Assert.False(conn.Connected);
        Assert.False(string.IsNullOrWhiteSpace(ex.Message));
    }

    [Fact]
    public void Read_times_out_when_console_never_responds()
    {
        using var server = new FakeBotbaseServer(mute: true);
        using var conn = new SysBotConnection();
        // Connect phase gets a generous overall budget via retries (contended CI runners can make the
        // loopback handshake exceed the short timeout below); each attempt still uses 500ms, so
        // ReceiveTimeout/SendTimeout - set from that same value by Connect - stay short for the read below.
        ConnectGenerously(conn, "127.0.0.1", server.Port, 500);

        var ex = Assert.Throws<LiveHexConnectionException>(() => conn.ReadHeap(0x1000, 8));
        // Assert the read-timeout message specifically, not just "Timed out", so this test can't pass
        // by silently catching a connect-phase timeout instead of the read-phase timeout it targets.
        Assert.Contains("Timed out waiting for a response", ex.Message);
    }
}
