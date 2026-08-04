using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PKHeX.Application.Abstractions.LiveHex;

namespace PKHeX.Infrastructure.LiveHex;

/// <summary>
/// sys-botbase over TCP implementation of <see cref="IConsoleConnection"/>. Owns a single blocking
/// socket and performs synchronous request/response exchanges guarded by a lock so overlapping
/// calls from a background worker cannot interleave on the wire.
/// </summary>
/// <remarks>
/// Clean-room implementation; the wire protocol grammar mirrors the public sys-botbase protocol.
/// See <c>NOTICE.LiveHeX.md</c>.
/// </remarks>
public sealed class SysBotConnection : IConsoleConnection
{
    private readonly Lock _sync = new();
    private Socket? _socket;
    private int _timeoutMs = 8000;

    public bool Connected => _socket is { Connected: true };

    public void Connect(string ip, int port, int timeoutMs)
    {
        lock (_sync)
        {
            _timeoutMs = timeoutMs;
            DisconnectInternal();
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = timeoutMs,
                SendTimeout = timeoutMs,
                NoDelay = true,
            };
            try
            {
                var async = socket.BeginConnect(ip, port, null, null);
                if (!async.AsyncWaitHandle.WaitOne(timeoutMs))
                {
                    socket.Close();
                    throw new LiveHexConnectionException(
                        $"Timed out connecting to {ip}:{port}. Check the IP address, that the console is on the same Wi-Fi network, and that sys-botbase is running.");
                }
                socket.EndConnect(async);
            }
            catch (LiveHexConnectionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                socket.Dispose();
                throw new LiveHexConnectionException(
                    $"Could not connect to {ip}:{port}. {ex.Message}", ex);
            }
            _socket = socket;
        }
    }

    public void Disconnect()
    {
        lock (_sync)
            DisconnectInternal();
    }

    private void DisconnectInternal()
    {
        if (_socket is null)
            return;
        try
        {
            if (_socket.Connected)
                _socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
            // best-effort; socket may already be torn down
        }
        _socket.Dispose();
        _socket = null;
    }

    public byte[] ReadHeap(ulong offset, int length) => ReadBytes(SwitchCommand.Peek(offset, length), length);
    public byte[] ReadMain(ulong offset, int length) => ReadBytes(SwitchCommand.PeekMain(offset, length), length);
    public byte[] ReadAbsolute(ulong offset, int length) => ReadBytes(SwitchCommand.PeekAbsolute(offset, length), length);

    public void WriteHeap(ReadOnlySpan<byte> data, ulong offset) => Send(SwitchCommand.Poke(offset, data));
    public void WriteAbsolute(ReadOnlySpan<byte> data, ulong offset) => Send(SwitchCommand.PokeAbsolute(offset, data));

    public ulong GetHeapBase()
    {
        var data = ReadBytes(SwitchCommand.GetHeapBase(), 8);
        return BinaryPrimitives.ReadUInt64BigEndian(data);
    }

    public string GetTitleId() => ReadLine(SwitchCommand.GetTitleId());
    public string GetBotbaseVersion() => ReadLine(SwitchCommand.GetBotbaseVersion());
    public string GetGameInfo(string info) => ReadLine(SwitchCommand.GetGameInfo(info));

    private byte[] ReadBytes(byte[] command, int length)
    {
        lock (_sync)
        {
            var socket = RequireSocket();
            SendAll(socket, command);
            var hex = ReadUntilNewline(socket);
            var result = new byte[length];
            HexCodec.DecodeInto(hex, result);
            return result;
        }
    }

    private string ReadLine(byte[] command)
    {
        lock (_sync)
        {
            var socket = RequireSocket();
            SendAll(socket, command);
            var line = ReadUntilNewline(socket);
            return Encoding.ASCII.GetString(line).Trim('\0', '\r', ' ', '\t');
        }
    }

    private void Send(byte[] command)
    {
        lock (_sync)
            SendAll(RequireSocket(), command);
    }

    private Socket RequireSocket()
    {
        if (_socket is not { Connected: true } socket)
            throw new LiveHexConnectionException("Not connected to a console.");
        return socket;
    }

    private void SendAll(Socket socket, byte[] command)
    {
        try
        {
            int sent = 0;
            while (sent < command.Length)
                sent += socket.Send(command, sent, command.Length - sent, SocketFlags.None);
        }
        catch (SocketException ex)
        {
            throw new LiveHexConnectionException($"Lost connection while sending a command: {ex.SocketErrorCode}.", ex);
        }
    }

    // sys-botbase terminates every response with '\n'; hex responses contain no other control chars.
    private static byte[] ReadUntilNewline(Socket socket)
    {
        var buffer = new List<byte>(512);
        var chunk = new byte[4096];
        while (true)
        {
            int read;
            try
            {
                read = socket.Receive(chunk, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode is SocketError.TimedOut or SocketError.WouldBlock)
                    throw new LiveHexConnectionException("Timed out waiting for a response from the console.", ex);
                throw new LiveHexConnectionException($"Lost connection while reading a response: {ex.SocketErrorCode}.", ex);
            }

            if (read == 0)
                throw new LiveHexConnectionException("The console closed the connection unexpectedly.");

            for (int i = 0; i < read; i++)
            {
                if (chunk[i] == (byte)'\n')
                {
                    for (int j = 0; j < i; j++)
                        buffer.Add(chunk[j]);
                    // trim a trailing carriage return if present
                    if (buffer.Count > 0 && buffer[^1] == (byte)'\r')
                        buffer.RemoveAt(buffer.Count - 1);
                    return buffer.ToArray();
                }
            }
            for (int i = 0; i < read; i++)
                buffer.Add(chunk[i]);
        }
    }

    public void Dispose() => Disconnect();
}
