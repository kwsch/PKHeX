using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
    public class StringTests
    {
        [Fact]
        public void Gen7ZHLengthCorrect()
        {
            StringConverter7ZH.Gen7_ZHRaw.Length.Should().Be(StringConverter7ZH.Gen7_ZHLength);
        }

        [Fact]
        public void EncodesOTNameCorrectly()
        {
            const string name_fabian = "Fabian♂";
            var pkm = new PK7 { OT_Name = name_fabian };
            var byte_fabian = new byte[]
            {
                0x46, 0x00, // F
                0x61, 0x00, // a
                0x62, 0x00, // b
                0x69, 0x00, // i
                0x61, 0x00, // a
                0x6E, 0x00, // n
                0x8E, 0xE0, // ♂
                0x00, 0x00, // \0 terminator
            };
            CheckStringGetSet(nameof(pkm.OT_Name), name_fabian, pkm.OT_Name, byte_fabian, pkm.OT_Trash);
        }

        [Fact]
        public void EncodesNicknameCorrectly()
        {
            const string name_nidoran = "ニドラン♀";
            var pkm = new PK7 { Nickname = name_nidoran };
            var byte_nidoran = new byte[]
            {
                0xCB, 0x30, // ニ
                0xC9, 0x30, // ド
                0xE9, 0x30, // ラ
                0xF3, 0x30, // ン
                0x40, 0x26, // ♀
                0x00, 0x00, // \0 terminator
            };
            CheckStringGetSet(nameof(pkm.Nickname), name_nidoran, pkm.Nickname, byte_nidoran, pkm.Nickname_Trash);
        }

        private static void CheckStringGetSet(string check, string instr, string outstr, byte[] indata, byte[] outdata)
        {
            instr.Should().BeEquivalentTo(outstr);

            outdata = outdata[..indata.Length];

            indata.SequenceEqual(outdata).Should()
                .BeTrue($"expected {check} to set properly, instead got {string.Join(", ", outdata.Select(z => $"{z:X2}"))}");
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
            StringConverter4.ConvertChar2ValueG4(g5).Should().Be(g4);
            StringConverter4.ConvertValue2CharG4(g4).Should().Be(g5);
        }
    }
}
