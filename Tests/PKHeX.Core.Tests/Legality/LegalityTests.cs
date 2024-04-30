using System;
using System.Collections.Generic;
using FluentAssertions;
using System.IO;
using System.Linq;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class LegalityTest
{
    private static readonly string TestPath = TestUtil.GetRepoPath();
    static LegalityTest() => TestUtil.InitializeLegality();

    [Theory]
    [InlineData("censor")]
    [InlineData("buttnugget")]
    [InlineData("18넘")]
    [InlineData("inoffensive", false)]
    public void CensorsBadWords(string badword, bool value = true)
    {
        WordFilter.TryMatch(badword, out _).Should().Be(value, "the word should have been identified as a bad word");
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
            if (pk == null)
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
                legality.Valid.Should().BeTrue($"because the file '{fn}' should be Valid, but found:{Environment.NewLine}{string.Join(Environment.NewLine, GetIllegalLines(legality))}");
            }
            else
            {
                legality.Valid.Should().BeFalse($"because the file '{fn}' should be invalid, but found Valid.");
            }
        }
        ctr.Should().BeGreaterThan(0, "any amount of files should have been processed from a folder that exists.");
    }

    private static IEnumerable<string> GetIllegalLines(LegalityAnalysis legality)
    {
        foreach (var l in legality.Results.Where(z => !z.Valid))
            yield return l.Comment;

        var info = legality.Info;
        foreach (var m in info.Moves.Where(z => !z.Valid))
            yield return m.Summary(info.Entity, info.EvoChainsAllGens);
        foreach (var r in info.Relearn.Where(z => !z.Valid))
            yield return r.Summary(info.Entity, info.EvoChainsAllGens);
    }
}
