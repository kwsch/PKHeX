using System;
using System.Text;
using PKHeX.Infrastructure.LiveHex;

namespace PKHeX.Avalonia.Tests.LiveHex;

/// <summary>Unit tests for the clean-room sys-botbase protocol encoding/decoding and pointer walk.</summary>
public class LiveHexProtocolTests
{
    private static string Ascii(byte[] b) => Encoding.ASCII.GetString(b);

    [Fact]
    public void Peek_formats_heap_read_command()
    {
        Assert.Equal("peek 0x0000000000001234 16\r\n", Ascii(SwitchCommand.Peek(0x1234, 16)));
    }

    [Fact]
    public void PeekMain_and_PeekAbsolute_use_correct_verbs()
    {
        Assert.Equal("peekMain 0x00000000000000A0 8\r\n", Ascii(SwitchCommand.PeekMain(0xA0, 8)));
        Assert.Equal("peekAbsolute 0x0000000000000010 4\r\n", Ascii(SwitchCommand.PeekAbsolute(0x10, 4)));
    }

    [Fact]
    public void Poke_formats_payload_as_uppercase_hex()
    {
        Assert.Equal("poke 0x0000000000000010 0xABCD\r\n", Ascii(SwitchCommand.Poke(0x10, [0xAB, 0xCD])));
        Assert.Equal("pokeAbsolute 0x0000000000000020 0x00FF\r\n", Ascii(SwitchCommand.PokeAbsolute(0x20, [0x00, 0xFF])));
    }

    [Fact]
    public void Info_commands_are_encoded()
    {
        Assert.Equal("getHeapBase\r\n", Ascii(SwitchCommand.GetHeapBase()));
        Assert.Equal("getTitleID\r\n", Ascii(SwitchCommand.GetTitleId()));
        Assert.Equal("getVersion\r\n", Ascii(SwitchCommand.GetBotbaseVersion()));
        Assert.Equal("game version\r\n", Ascii(SwitchCommand.GetGameInfo("version")));
    }

    [Fact]
    public void HexCodec_roundtrips_bytes()
    {
        byte[] data = [0x00, 0x0A, 0xFF, 0x7F, 0x80];
        var hex = HexCodec.ToHex(data);
        Assert.Equal("000AFF7F80", hex);

        var dest = new byte[data.Length];
        HexCodec.DecodeInto(Encoding.ASCII.GetBytes(hex), dest);
        Assert.Equal(data, dest);
    }

    [Fact]
    public void HexCodec_rejects_wrong_length_response()
    {
        var dest = new byte[4];
        Assert.Throws<FormatException>(() => HexCodec.DecodeInto("0AFF"u8, dest)); // 4 chars for 4 bytes
    }

    [Fact]
    public void PointerResolver_walks_chain_and_returns_heap_relative_address()
    {
        var mem = new FakeSwitchMemory { HeapBase = 0x1000 };
        mem.SetMainPointer(0x100, 0x5000);      // ReadMain(0x100) = 0x5000
        mem.SetAbsolutePointer(0x5008, 0x9000); // ReadAbsolute(0x5000 + 8) = 0x9000

        // "[[main+100]+8]+4" => ReadAbsolute(ReadMain(0x100)+8) + 4 - HeapBase
        var resolved = PointerResolver.Resolve(mem, "[[main+100]+8]+4");
        Assert.Equal(0x9000UL + 4 - 0x1000, resolved);
    }

    [Fact]
    public void PointerResolver_can_return_absolute_address()
    {
        var mem = new FakeSwitchMemory { HeapBase = 0x1000 };
        mem.SetMainPointer(0x100, 0x5000);
        mem.SetAbsolutePointer(0x5008, 0x9000);

        var resolved = PointerResolver.Resolve(mem, "[[main+100]+8]+4", heapRelative: false);
        Assert.Equal(0x9004UL, resolved);
    }

    [Fact]
    public void PointerResolver_rejects_arithmetic_expressions()
    {
        var mem = new FakeSwitchMemory();
        Assert.Equal(PointerResolver.InvalidPointer, PointerResolver.Resolve(mem, "[main-100]"));
        Assert.Equal(PointerResolver.InvalidPointer, PointerResolver.Resolve(mem, ""));
    }
}
