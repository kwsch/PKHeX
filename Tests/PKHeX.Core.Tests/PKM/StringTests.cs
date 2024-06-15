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
        var pk = new PK7 { OriginalTrainerName = name_fabian };
        ReadOnlySpan<byte> byte_fabian =
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
        CheckStringGetSet(nameof(pk.OriginalTrainerName), name_fabian, pk.OriginalTrainerName, byte_fabian, pk.OriginalTrainerTrash);
    }

    [Fact]
    public void EncodesNicknameCorrectly()
    {
        const string name_nidoran = "ニドラン♀";
        var pk = new PK7 { Nickname = name_nidoran };
        ReadOnlySpan<byte> byte_nidoran =
        [
            0xCB, 0x30, // ニ
            0xC9, 0x30, // ド
            0xE9, 0x30, // ラ
            0xF3, 0x30, // ン
            0x40, 0x26, // ♀
            0x00, 0x00, // \0 terminator
        ];
        CheckStringGetSet(nameof(pk.Nickname), name_nidoran, pk.Nickname, byte_nidoran, pk.NicknameTrash);
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
        var sb = new System.Text.StringBuilder(outdata.Length*3);
        foreach (var b in outdata)
            sb.Append($"{b:X2} ");
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
        var len = StringConverter1.SetString(b12, g12, g12.Length, true);
        var result = StringConverter12Transporter.GetString(b12[..len], true);
        result.Should().Be(g7);
    }

    [Theory]
    [InlineData(Species.MrMime, "MR․MIME")]
    public static void ConvertStringG1(Species species, string expect)
    {
        const bool jp = false;
        const int lang = (int)LanguageID.English;
        // Ensure the API returns the correct Generation 1 name string.
        var name = SpeciesName.GetSpeciesNameGeneration((ushort)species, lang, 1);
        name.Should().Be(expect);

        // Ensure the API converts it back and forth correctly.
        Span<byte> convert = stackalloc byte[expect.Length + 1];
        var len = StringConverter1.SetString(convert, name, name.Length, jp);
        len.Should().Be(expect.Length + 1);
        var gen1Name = StringConverter1.GetString(convert, jp);
        gen1Name.Should().Be(expect);

        // Truncated name transferred with Virtual Console rules isn't the same as the Generation 7 name.
        var vcName = StringConverter12Transporter.GetString(convert[..len], jp);
        var gen7Name = SpeciesName.GetSpeciesNameGeneration((ushort)species, lang, 7);
        vcName.Should().NotBe(gen7Name);
    }
}
