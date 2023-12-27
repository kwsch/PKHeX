using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class StringTests
{
    [Fact]
    public void EncodesOTNameCorrectly()
    {
        const string name_fabian = "Fabian♂";
        var pk = new PK7 { OT_Name = name_fabian };
        Span<byte> byte_fabian =
        [
            0x46, 0x00, // F
            0x61, 0x00, // a
            0x62, 0x00, // b
            0x69, 0x00, // i
            0x61, 0x00, // a
            0x6E, 0x00, // n
            0x8E, 0xE0, // ♂
            0x00, 0x00, // \0 terminator
        ];
        CheckStringGetSet(nameof(pk.OT_Name), name_fabian, pk.OT_Name, byte_fabian, pk.OT_Trash);
    }

    [Fact]
    public void EncodesNicknameCorrectly()
    {
        const string name_nidoran = "ニドラン♀";
        var pk = new PK7 { Nickname = name_nidoran };
        Span<byte> byte_nidoran =
        [
            0xCB, 0x30, // ニ
            0xC9, 0x30, // ド
            0xE9, 0x30, // ラ
            0xF3, 0x30, // ン
            0x40, 0x26, // ♀
            0x00, 0x00, // \0 terminator
        ];
        CheckStringGetSet(nameof(pk.Nickname), name_nidoran, pk.Nickname, byte_nidoran, pk.Nickname_Trash);
    }

    private static void CheckStringGetSet(string check, string instr, string outstr, ReadOnlySpan<byte> indata, ReadOnlySpan<byte> outdata)
    {
        instr.Should().BeEquivalentTo(outstr);

        outdata = outdata[..indata.Length];

        indata.SequenceEqual(outdata).Should()
            .BeTrue($"expected {check} to set properly, instead got {Hex(outdata)}");
    }

    private static string Hex(ReadOnlySpan<byte> outdata)
    {
        var sb = new System.Text.StringBuilder(outdata.Length);
        foreach (var b in outdata)
            sb.Append(b.ToString("X2")).Append(' ');
        return sb.ToString();
    }

    [Theory]
    [InlineData(0x0F5, 0xFF5E)] // ～
    [InlineData(0x0FA, 0x2660)] // ♠
    [InlineData(0x0FB, 0x2663)] // ♣
    [InlineData(0x0FC, 0x2665)] // ♥
    [InlineData(0x0FD, 0x2666)] // ♦
    [InlineData(0x0FE, 0x2605)] // ★
    [InlineData(0x105, 0x266A)] // ♪
    public static void Encode45(ushort g4, char g5)
    {
        StringConverter4Util.ConvertChar2ValueG4(g5).Should().Be(g4);
        StringConverter4Util.ConvertValue2CharG4(g4).Should().Be(g5);
    }

    [Theory]
    [InlineData("ぐリお", "ぐりお")]
    public static void ConvertStringVC(string g12, string g7)
    {
        Span<byte> b12 = stackalloc byte[g12.Length];
        var len = StringConverter12.SetString(b12, g12, g12.Length, true);
        var result = StringConverter12Transporter.GetString(b12[..len], true);
        result.Should().Be(g7);
    }
}
