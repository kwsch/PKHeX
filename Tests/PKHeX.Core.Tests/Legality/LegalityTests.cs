using FluentAssertions;
using PKHeX.Core;
using System.IO;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class LegalityTest
    {
        static LegalityTest()
        {
            if (!EncounterEvent.Initialized)
                EncounterEvent.RefreshMGDB();
        }

        [Theory]
        [InlineData("censor")]
        [InlineData("buttnugget")]
        [InlineData("18넘")]
        public void CensorsBadWords(string badword)
        {
            WordFilter.IsFiltered(badword, out _).Should().BeTrue("the word should have been identified as a bad word");
        }

        [Fact]
        public void TestFilesPassOrFailLegalityChecks()
        {
            var folder = TestUtil.GetRepoPath();
            folder = Path.Combine(folder, "Legality");
            ParseSettings.AllowGBCartEra = true;
            VerifyAll(folder, "Legal", true);
            VerifyAll(folder, "Illegal", false);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void VerifyAll(string folder, string name, bool isValid)
        {
            var path = Path.Combine(folder, name);
            Directory.Exists(path).Should().BeTrue($"the specified test directory at '{path}' should exist");
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            var ctr = 0;
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                fi.Should().NotBeNull($"the test file '{file}' should be a valid file");
                PKX.IsPKM(fi.Length).Should().BeTrue($"the test file '{file}' should have a valid file length");

                var data = File.ReadAllBytes(file);
                var format = PKX.GetPKMFormatFromExtension(file[file.Length - 1], -1);
                format.Should().BeLessOrEqualTo(PKX.Generation, "filename is expected to have a valid extension");
                var pkm = PKMConverter.GetPKMfromBytes(data, prefer: format);
                pkm.Should().NotBeNull($"the PKM '{new FileInfo(file).Name}' should have been loaded");

                ParseSettings.AllowGBCartEra = fi.DirectoryName.Contains("GBCartEra");
                ParseSettings.AllowGen1Tradeback = fi.DirectoryName.Contains("1 Tradeback");
                var legality = new LegalityAnalysis(pkm);
                legality.Valid.Should().Be(isValid, $"because the file '{fi.Directory.Name}\\{fi.Name}' should be {(isValid ? "Valid" : "Invalid")}");
                ctr++;
            }

            ctr.Should().BeGreaterThan(0);
        }
    }
}
