using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>Native Core validation of the private real fixtures before any browser run.</summary>
[Trait(TestCategory.Name, TestCategory.RealSave)]
public sealed class RealSavePreflightTests
{
    [TierTheory(TestCategory.RealSave)]
    [InlineData("XY")]
    [InlineData("ORAS")]
    public void RealSaveNativePreflight(string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        var index = SaveFixtures.WritableSlot(native);
        Assert.True(SaveFixtures.Slot(native, index).Read(native) is PK6 { ChecksumValid: true });

        // The Web session algorithm must agree with native Core on the same bytes.
        var session = SaveFixtures.Open(fixture.Bytes);
        // RealSaves.Read has already checked that the native type matches the family.
        Assert.True(session.Working.GetType() == native.GetType());
        // The session opens the same entity native Core reads there; the messages keep the private slot contents out.
        var summary = StorageView.Box(session, index.Box).Slots[index.Slot];
        Assert.True(summary.CanOpen, "Session does not show the native writable slot as an openable entity.");
        Assert.True(session.Select(index).Nickname == SaveFixtures.Slot(native, index).Read(native).Nickname, "Session draft differs from the native entity (value withheld).");
        Assert.True(SaveExporter.Export(session, null).AsSpan().SequenceEqual(native.Clone().Write().Span), "Session no-op export differs from native.");
        fixture.AssertUnchanged();
    }
}
