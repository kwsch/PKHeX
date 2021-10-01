using System;
using FluentAssertions;
using PKHeX.Core;
using System.IO;
using System.Linq;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class LegalityTest
    {
        static LegalityTest()
        {
            if (EncounterEvent.Initialized)
                return;

            RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
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

        [Theory]
        [InlineData("Legal", true)]
        [InlineData("Illegal", false)]
        public void TestPublicFiles(string name, bool isValid)
        {
            var folder = TestUtil.GetRepoPath();
            folder = Path.Combine(folder, "Legality");
            VerifyAll(folder, name, isValid);
        }

        [Theory]
        [InlineData("Legal", true)]
        [InlineData("Illegal", false)]
        [InlineData("PassingHacks", true)] // mad hacks, stuff to be flagged in the future
        [InlineData("FalseFlags", false)] // legal quirks, to be fixed in the future
        public void TestPrivateFiles(string name, bool isValid)
        {
            var folder = TestUtil.GetRepoPath();
            folder = Path.Combine(folder, "Legality", "Private");
            VerifyAll(folder, name, isValid, false);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void VerifyAll(string folder, string name, bool isValid, bool checkDir = true)
        {
            var path = Path.Combine(folder, name);
            bool exists = Directory.Exists(path);
            if (checkDir)
                exists.Should().BeTrue($"the specified test directory at '{path}' should exist");
            else if (!exists)
                return;

            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            var ctr = 0;
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                fi.Should().NotBeNull($"the test file '{file}' should be a valid file");
                PKX.IsPKM(fi.Length).Should().BeTrue($"the test file '{file}' should have a valid file length");

                var data = File.ReadAllBytes(file);
                var format = PKX.GetPKMFormatFromExtension(file[^1], -1);
                format.Should().BeLessOrEqualTo(PKX.Generation, "filename is expected to have a valid extension");

                var dn = fi.DirectoryName ?? string.Empty;
                ParseSettings.AllowGBCartEra = dn.Contains("GBCartEra");
                ParseSettings.AllowGen1Tradeback = dn.Contains("1 Tradeback");
                var pkm = PKMConverter.GetPKMfromBytes(data, prefer: format);
                pkm.Should().NotBeNull($"the PKM '{new FileInfo(file).Name}' should have been loaded");
                if (pkm == null)
                    continue;
                var legality = new LegalityAnalysis(pkm);
                if (legality.Valid == isValid)
                {
                    ctr++;
                    continue;
                }

                var fn = Path.Combine(dn, fi.Name);
                if (isValid)
                {
                    var info = legality.Info;
                    var result = legality.Results.Concat(info.Moves).Concat(info.Relearn);
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    var invalid = result.Where(z => z?.Valid == false);
                    var msg = string.Join(Environment.NewLine, invalid.Select(z => z.Comment));
                    legality.Valid.Should().BeTrue($"because the file '{fn}' should be Valid, but found:{Environment.NewLine}{msg}");
                }
                else
                {
                    legality.Valid.Should().BeFalse($"because the file '{fn}' should be invalid, but found Valid.");
                }
            }
            ctr.Should().BeGreaterThan(0);
        }
    }
}
