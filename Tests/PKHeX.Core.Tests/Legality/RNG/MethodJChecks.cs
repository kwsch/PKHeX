using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

public static class MethodJChecks
{
    [Theory]
    [InlineData(0x00000000, 0x64e86d5e, 05)]
    [InlineData(0x00000001, 0x1250e055, 14)]
    [InlineData(0x00000002, 0x746d2fae, 10)]
    public static void CheckRangeJ(uint prePID, uint seed0, int frames)
    {
        var pid = ClassicEraRNG.GetSequentialPID(prePID);
        var nature = (byte)(pid % 25);
        var count = MethodJ.GetReversalWindow(prePID, nature);
        var origin = LCRNG.Reverse(prePID, count * 2);

        count.Should().Be(frames);
        origin.Should().Be(seed0);
    }

    // Seeds that generate well-known PIDs.
    private const uint TimidFlawless = 0xC69FB838;
    private const uint ModestFlawless = TimidFlawless ^ 0x80000000u;

    [Theory]
    [InlineData(TimidFlawless, 0)]
    [InlineData(TimidFlawless, 1)]
    [InlineData(TimidFlawless, 2)]
    [InlineData(TimidFlawless, 3)]
    [InlineData(TimidFlawless, 4)]
    [InlineData(TimidFlawless, 5)]
    [InlineData(TimidFlawless, 7)]
    [InlineData(TimidFlawless, 8)]
    [InlineData(ModestFlawless, 0)]
    [InlineData(ModestFlawless, 1)]
    [InlineData(ModestFlawless, 2)]
    [InlineData(ModestFlawless, 4, LeadRequired.SynchronizeFail)]
    [InlineData(ModestFlawless, 5)]
    [InlineData(ModestFlawless, 8)]
    [InlineData(ModestFlawless, 9)]
    [InlineData(ModestFlawless, 10)]
    [InlineData(ModestFlawless, 6, LeadRequired.None)] // Not Sync
    public static void CheckPossibleJ(uint prePID, byte slotIndex,
        LeadRequired expectLead = LeadRequired.Synchronize)
    {
        var fake = new MockSlot4(slotIndex);
        var (seed, lead) = MethodJ.GetSeed(fake, prePID, 4);
        lead.Should().Be(expectLead);
        seed.Should().BeInRange(0, uint.MaxValue);
    }

    [Theory]
    [InlineData(TimidFlawless, 6)]
    [InlineData(TimidFlawless, 9)]
    [InlineData(TimidFlawless, 10)]
    [InlineData(TimidFlawless, 11)]
    [InlineData(ModestFlawless, 3)]
    [InlineData(ModestFlawless, 7)]
    [InlineData(ModestFlawless, 11)]
    public static void CheckImpossibleJ(uint prePID, byte slotIndex)
    {
        var fake = new MockSlot4(slotIndex);
        var lead = MethodJ.GetSeed(fake, prePID, 4);
        lead.IsValid().Should().BeFalse();
        lead.Seed.Should().BeInRange(0, uint.MaxValue);
    }
}
