using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Build provenance embedded by the <c>AddBuildProvenance</c> target and read by <see cref="BuildInfo"/>.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class BuildInfoTests
{
    /// <remarks>
    /// Needs a git checkout and a build of the current commit: committing after building and then testing with <c>--no-build</c> fails here on purpose,
    /// because the build under test no longer describes the checked-out source.
    /// </remarks>
    [Fact]
    public void SourceCommitIsTheCheckedOutCommit()
    {
        var head = GitHead();
        Assert.True(BuildInfo.SourceCommit == head, $"The build records commit {BuildInfo.SourceCommit} but HEAD is {head}; rebuild before testing.");
        Assert.Matches("^[0-9a-f]{40}$", BuildInfo.SourceCommit);
    }

    [Fact]
    public void WebVersionIsTheRepositoryVersion()
    {
        var informational = typeof(BuildInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        Assert.Equal(informational.Split('+')[0], BuildInfo.WebVersion);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void MissingOrBlankValuesAreUnknown(string? value)
    {
        AssemblyMetadataAttribute[] metadata = value is null ? [] : [new(BuildInfo.CommitKey, value)];
        Assert.Equal(BuildInfo.Unknown, BuildInfo.Read(metadata, BuildInfo.CommitKey));
    }

    [Fact]
    public void ReadsOnlyTheRequestedKey()
    {
        AssemblyMetadataAttribute[] metadata = [new(BuildInfo.VersionKey, "1.2.3"), new(BuildInfo.CommitKey, " abc ")];
        Assert.Equal("1.2.3", BuildInfo.Read(metadata, BuildInfo.VersionKey));
        Assert.Equal("abc", BuildInfo.Read(metadata, BuildInfo.CommitKey));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("false", false)]
    [InlineData("yes", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void SpritesAreIncludedOnlyWhenRecordedAsTrue(string? value, bool expected)
    {
        AssemblyMetadataAttribute[] metadata = value is null ? [] : [new(BuildInfo.SpritesKey, value)];
        Assert.Equal(expected, BuildInfo.ReadFlag(metadata, BuildInfo.SpritesKey));
    }

    [Fact]
    public void TestBuildsAreMadeWithoutSprites()
    {
        // The default; publishing with sprites is an explicit choice (-p:PKHeXWebSprites=true).
        Assert.False(BuildInfo.SpritesIncluded);
    }

    private static string GitHead()
    {
        var start = new ProcessStartInfo("git", "rev-parse HEAD")
        {
            WorkingDirectory = SaveFixtures.RepositoryRoot,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        Process git;
        try
        {
            git = Process.Start(start)!;
        }
        catch (Win32Exception)
        {
            throw new InvalidOperationException("git is not on PATH; the provenance test needs git and a git checkout.");
        }
        using (git)
        {
            var output = git.StandardOutput.ReadToEnd().Trim();
            git.WaitForExit();
            Assert.True(git.ExitCode == 0, "git rev-parse HEAD failed; the provenance test needs a git checkout.");
            return output;
        }
    }
}
