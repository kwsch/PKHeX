using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
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

    /// <summary>
    /// Next Pokémon visits every Pokémon the party and boxes show as openable, once each, in their order, and wraps; Previous visits them in
    /// reverse. Only counts are reported, never positions or contents.
    /// </summary>
    [TierTheory(TestCategory.RealSave)]
    [InlineData("XY")]
    [InlineData("ORAS")]
    public void StepsVisitEveryOpenablePokemonInOrder(string family)
    {
        var fixture = RealSaves.Read(family);
        var session = SaveFixtures.Open(fixture.Bytes);
        var openable = StorageView.Party(session)
            .Concat(Enumerable.Range(0, StorageView.BoxCount(session)).SelectMany(b => StorageView.Box(session, b).Slots))
            .Where(s => s.CanOpen).Select(s => s.Ref).ToArray();
        Assert.True(openable.Length > 1, "The fixture needs more than one Pokémon.");

        foreach (var direction in new[] { 1, -1 })
        {
            var expected = direction == 1 ? openable : [.. openable.Reverse()];
            var visited = new List<SlotRef>();
            var at = expected[0];
            for (var i = 0; i < expected.Length; i++)
            {
                at = StorageView.Neighbour(session, at, direction) ?? throw new InvalidOperationException("A step found no other Pokémon.");
                visited.Add(at);
            }
            Assert.True(visited.SequenceEqual([.. expected.Skip(1), expected[0]]),
                $"Steps ({direction}) do not visit the {expected.Length} openable positions in order (positions withheld).");
        }
        fixture.AssertUnchanged();
    }
}
