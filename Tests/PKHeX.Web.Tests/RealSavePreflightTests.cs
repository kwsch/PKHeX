using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>Native Core validation of the private real fixtures before any browser run.</summary>
[Trait(TestCategory.Name, TestCategory.RealSave)]
public sealed class RealSavePreflightTests
{
    [Theory]
    [InlineData("XY")]
    [InlineData("ORAS")]
    public void RealSaveNativePreflight(string family)
    {
        var fixture = RealSaves.Read(family);
        var native = fixture.Native;
        var index = SaveFixtures.WritableSlot(native);
        Assert.True(SaveFixtures.Slot(native, index).Read(native) is PK6 { ChecksumValid: true });

        // The Web session algorithm must agree with native Core on the same bytes.
        var session = SaveLoader.Load(fixture.Bytes);
        Assert.True(session.Family == family);
        // Not Assert.Contains: its failure message would list the private save's occupied slots.
        Assert.True(session.OccupiedSlots.Contains(index), "Session does not list the native writable slot as occupied.");
        Assert.True(SaveExporter.Export(session, null).AsSpan().SequenceEqual(native.Clone().Write().Span), "Session no-op export differs from native.");
        fixture.AssertUnchanged();
    }
}
