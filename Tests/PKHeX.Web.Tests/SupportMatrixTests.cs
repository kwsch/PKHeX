using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The release allowlist that both the loader and the About panel read.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SupportMatrixTests
{
    [Fact]
    public void OpensExactlyXYAndORAS()
    {
        // Widening the release is a deliberate change: it needs its own fixtures and evidence, and this test updated with it.
        SupportMatrix.Families.Select(f => f.SaveType).Should().Equal(typeof(SAV6XY), typeof(SAV6AO));
        SupportMatrix.Families.Should().OnlyContain(f => !string.IsNullOrWhiteSpace(f.Games));
    }

    [Fact]
    public void RelatedAndOtherSaveTypesAreNotEnabled()
    {
        SupportMatrix.IsEnabled(new SAV6XY()).Should().BeTrue();
        SupportMatrix.IsEnabled(new SAV6AO()).Should().BeTrue();
        SupportMatrix.IsEnabled(new SAV6AODemo()).Should().BeFalse("the ORAS demo is a separate family");
        SupportMatrix.IsEnabled(new SAV7SM()).Should().BeFalse();
        SupportMatrix.IsEnabled(new SAV5BW()).Should().BeFalse();
    }

    [Fact]
    public void LoaderRefusesSavesCoreRecognisesButTheReleaseDoesNotOpen()
    {
        var bytes = new SAV5BW().Write().ToArray();
        SaveUtil.GetSaveFile(bytes.ToArray()).Should().NotBeNull("Core recognises the file");
        var outcome = SaveLoader.Load(bytes);
        outcome.Failure.Should().Be(LoadFailure.RecognizedNotEnabled);
        outcome.Recognized!.SaveType.Should().Be<SAV5BW>();
        outcome.Session.Should().BeNull();
    }
}
