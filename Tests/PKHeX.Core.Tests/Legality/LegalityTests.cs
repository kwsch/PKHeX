using System;
using System.Collections.Generic;
using FluentAssertions;
using PKHeX.Core;
using System.IO;
using System.Linq;
using Xunit;

namespace PKHeX.Tests.Legality;

public class LegalityTest
{
    private static readonly string TestPath = TestUtil.GetRepoPath();
    private static readonly object InitLock = new();
    private static bool IsInitialized;

    private static void Init()
    {
        lock (InitLock)
        {
            if (IsInitialized)
                return;
            RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
            if (EncounterEvent.Initialized)
                return;
            EncounterEvent.RefreshMGDB();
            IsInitialized = true;
        }
    }

    static LegalityTest() => Init();

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
        RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
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
        if (!isValid)
            Init();
        var folder = Path.Combine(TestPath, "Legality", "Private");
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
            EntityDetection.IsSizePlausible(fi.Length).Should().BeTrue($"the test file '{file}' should have a valid file length");

            var data = File.ReadAllBytes(file);
            var prefer = EntityFileExtension.GetContextFromExtension(file);
            prefer.IsValid().Should().BeTrue("filename is expected to have a valid extension");

            var dn = fi.DirectoryName ?? string.Empty;
            ParseSettings.AllowGBCartEra = dn.Contains("GBCartEra");
            ParseSettings.AllowGen1Tradeback = dn.Contains("1 Tradeback");
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
        ctr.Should().BeGreaterThan(0);
    }

    private static IEnumerable<string> GetIllegalLines(LegalityAnalysis legality)
    {
        foreach (var l in legality.Results.Where(z => !z.Valid))
            yield return l.Comment;

        var info = legality.Info;
        foreach (var m in info.Moves.Where(z => !z.Valid))
            yield return m.Summary(legality.Info.Entity, legality.Info.EvoChainsAllGens);
        foreach (var r in info.Relearn.Where(z => !z.Valid))
            yield return r.Summary(legality.Info.Entity, legality.Info.EvoChainsAllGens);
    }
}
