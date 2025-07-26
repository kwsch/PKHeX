using System;
using FluentAssertions;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class LegalityTest
{
    private static readonly string TestPath = TestUtil.GetRepoPath();
    static LegalityTest() => TestUtil.InitializeLegality();

    [Theory]
    [InlineData("Ass")]
    [InlineData("Ａｓｓ")]
    [InlineData("9/11")]
    [InlineData("９／１１", false)]
    [InlineData("baise")]
    [InlineData("baisé", false)]
    [InlineData("BAISÉ", false)]
    [InlineData("scheiße")]
    [InlineData("SCHEISSE", false)]
    [InlineData("RICCHIONE ")]
    [InlineData("RICCHIONE", false)]
    [InlineData("せっくす")]
    [InlineData("セックス")]
    [InlineData("ふぁっく", false)]
    [InlineData("ファック", false)]
    [InlineData("kofagrigus", false)]
    [InlineData("cofagrigus", false)]
    public void CensorsBadWordsGen5(string badword, bool value = true)
    {
        var result = WordFilter5.IsFiltered(badword, out _);
        result.Should().Be(value, $"the word {(value ? "should" : "should not")} have been identified as a bad word");
    }

    [Theory]
    [InlineData("kofagrigus")]
    [InlineData("cofagrigus")]
    [InlineData("Cofagrigus", false)]
    public void CensorsBadWordsGen6(string badword, bool value = true)
    {
        var result = WordFilter3DS.IsFilteredGen6(badword, out _);
        result.Should().Be(value, $"the word {(value ? "should" : "should not")} have been identified as a bad word");
    }

    [Theory]
    [InlineData("badword")]
    [InlineData("butt nuggets")]
    [InlineData("18년")]
    [InlineData("ふぁっく")]
    [InlineData("ｇｃｄ")]
    [InlineData("Ｐ０ＲＮ")]
    [InlineData("gmail.com")]
    [InlineData("kofagrigus")]
    [InlineData("cofagrigus", false)]
    [InlineData("Cofagrigus", false)]
    [InlineData("inoffensive", false)]
    public void CensorsBadWordsGen7(string badword, bool value = true)
    {
        var result = WordFilter3DS.IsFilteredGen7(badword, out _);
        result.Should().Be(value, $"the word {(value ? "should" : "should not")} have been identified as a bad word");
    }

    [Theory]
    [InlineData("badword")]
    [InlineData("butt nuggets")]
    [InlineData("18넘")]
    [InlineData("ふぁっく")]
    [InlineData("ｳﾞｧｷﾞﾅ")]
    [InlineData("ｵｯﾊﾟｲ")]
    [InlineData("ﾌｧｯﾞｸ")]
    [InlineData("ﾌｧﾂﾞｸ", false)]
    [InlineData("ﾌｧｯｸﾞ", false)]
    [InlineData("sh!t")]
    [InlineData("sh！t", false)]
    [InlineData("abu$e")]
    [InlineData("kofagrigus")]
    [InlineData("cofagrigus", false)]
    [InlineData("Cofagrigus", false)]
    [InlineData("inoffensive", false)]
    public void CensorsBadWordsSwitch(string badword, bool value = true)
    {
        var result = WordFilterNX.IsFiltered(badword, out _, EntityContext.Gen9);
        result.Should().Be(value, $"the word {(value ? "should" : "should not")} have been identified as a bad word");
    }

    [Theory]
    [InlineData("Legal", true)]
    [InlineData("Illegal", false)]
    public void TestPublicFiles(string subFolder, bool isValid)
    {
        var folder = Path.Combine(TestPath, "Legality");
        VerifyAll(folder, subFolder, isValid);
    }

    [Theory]
    [InlineData("Legal", true)]
    [InlineData("Illegal", false)]
    [InlineData("PassingHacks", true)] // mad hacks, stuff to be flagged in the future
    [InlineData("FalseFlags", false)] // legal quirks, to be fixed in the future
    public void TestPrivateFiles(string subFolder, bool isValid)
    {
        var folder = Path.Combine(TestPath, "Legality", "Private");
        VerifyAll(folder, subFolder, isValid, false);
    }

    // ReSharper disable once UnusedParameter.Local
    private static void VerifyAll(string folder, string subFolder, bool isValid, bool checkDir = true)
    {
        var path = Path.Combine(folder, subFolder);
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
            EntityDetection.IsSizePlausible(fi.Length).Should().BeTrue($"the test file '{file}' should have a valid file length");

            var data = File.ReadAllBytes(file);
            var prefer = EntityFileExtension.GetContextFromExtension(file);
            prefer.IsValid().Should().BeTrue("filename is expected to have a valid extension");

            var dn = fi.DirectoryName ?? string.Empty;
            ParseSettings.AllowGBCartEra = dn.Contains("GBCartEra");
            ParseSettings.Settings.Tradeback.AllowGen1Tradeback = dn.Contains("1 Tradeback");
            var pk = EntityFormat.GetFromBytes(data, prefer);
            pk.Should().NotBeNull($"the PKM '{new FileInfo(file).Name}' should have been loaded");
            if (pk is null)
                continue;
            var legality = new LegalityAnalysis(pk);
            if (legality.Valid == isValid)
            {
                ctr++;
                continue;
            }

            var fn = Path.Combine(dn, fi.Name);
            if (isValid)
            {
                legality.Valid.Should().BeTrue($"because the file '{fn}' should be Valid, but found:{Environment.NewLine}{GetIllegalLines(legality)}");
            }
            else
            {
                legality.Valid.Should().BeFalse($"because the file '{fn}' should be invalid, but found Valid.");
            }
        }
        ctr.Should().BeGreaterThan(0, "any amount of files should have been processed from a folder that exists.");
    }

    // Simple info-dump for illegal lines in a LegalityAnalysis.
    private static string GetIllegalLines(LegalityAnalysis legality)
    {
        var localizer = new LegalityLocalizationContext
        {
            Analysis = legality,
            Settings = LegalityLocalizationSet.GetLocalization(GameLanguage.DefaultLanguage),
        };

        StringBuilder result = new();
        foreach (var l in legality.Results.Where(z => !z.Valid))
            result.AppendLine(localizer.Humanize(l));

        var info = legality.Info;
        foreach (var m in info.Moves.Where(z => !z.Valid))
            result.AppendLine(m.Summary(localizer));
        foreach (var r in info.Relearn.Where(z => !z.Valid))
            result.AppendLine(r.Summary(localizer));

        return result.ToString();
    }
}
