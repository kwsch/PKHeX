using System.Buffers.Binary;
using System.Collections.Generic;
using PKHeX.Application.Abstractions.LiveHex;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>
/// In-memory fake console that models three address spaces (main, absolute, heap) so LiveHeX
/// protocol, pointer-resolution and box-addressing logic can be exercised end-to-end without a
/// real console. Heap and absolute spaces are independent sparse maps; unset bytes read as 0.
/// </summary>
internal sealed class FakeSwitchMemory : IConsoleConnection
{
    private readonly Dictionary<ulong, byte> _heap = new();
    private readonly Dictionary<ulong, byte> _absolute = new();
    private readonly Dictionary<ulong, byte> _main = new();

    public ulong HeapBase { get; set; } = 0x8000000;
    public string TitleId { get; set; } = "0100ABF008968000"; // Sword by default
    public string GameVersion { get; set; } = "1.3.2";
    public string BotbaseVersion { get; set; } = "2.5";

    public bool Connected { get; private set; } = true;
    public bool ConnectCalled { get; private set; }

    public void Connect(string ip, int port, int timeoutMs) { ConnectCalled = true; Connected = true; }
    public void Disconnect() => Connected = false;
    public void Dispose() => Disconnect();

    public byte[] ReadHeap(ulong offset, int length) => Read(_heap, offset, length);
    public void WriteHeap(System.ReadOnlySpan<byte> data, ulong offset) => Write(_heap, data, offset);
    public byte[] ReadMain(ulong offset, int length) => Read(_main, offset, length);
    public byte[] ReadAbsolute(ulong offset, int length) => Read(_absolute, offset, length);
    public void WriteAbsolute(System.ReadOnlySpan<byte> data, ulong offset) => Write(_absolute, data, offset);
    public ulong GetHeapBase() => HeapBase;
    public string GetTitleId() => TitleId;
    public string GetBotbaseVersion() => BotbaseVersion;
    public string GetGameInfo(string info) => GameVersion;

    // --- test setup helpers ---
    public void SetMainPointer(ulong offset, ulong value) => Write(_main, U64(value), offset);
    public void SetAbsolutePointer(ulong offset, ulong value) => Write(_absolute, U64(value), offset);
    public void SetAbsoluteBytes(ulong offset, byte[] data) => Write(_absolute, data, offset);
    public byte[] GetAbsoluteBytes(ulong offset, int length) => Read(_absolute, offset, length);
    public void SetHeapBytes(ulong offset, byte[] data) => Write(_heap, data, offset);
    public byte[] GetHeapBytes(ulong offset, int length) => Read(_heap, offset, length);

    private static byte[] Read(Dictionary<ulong, byte> space, ulong offset, int length)
    {
        var result = new byte[length];
        for (int i = 0; i < length; i++)
            space.TryGetValue(offset + (ulong)i, out result[i]);
        return result;
    }

    private static void Write(Dictionary<ulong, byte> space, System.ReadOnlySpan<byte> data, ulong offset)
    {
        for (int i = 0; i < data.Length; i++)
            space[offset + (ulong)i] = data[i];
    }

    private static byte[] U64(ulong value)
    {
        var b = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(b, value);
        return b;
    }
}
