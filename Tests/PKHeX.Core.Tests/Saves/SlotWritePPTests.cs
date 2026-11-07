using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Saves;

public class SlotWritePPTests
{
    private const int Tackle = 33; // 35 PP
    private const int Growl = 45; // 40 PP

    private static readonly EntityImportSettings Adapt = new(EntityImportOption.Enable, EntityImportOption.Disable, EntityImportOption.Disable);

    private static T GetUsedPP<T>() where T : PKM, new()
    {
        var pk = new T
        {
            Species = (ushort)Species.Bulbasaur,
            Move1 = Tackle,
            Move2 = Growl,
            Move1_PPUps = 1,
            Move1_PP = 1,
            Move2_PP = 0,
        };
        pk.ResetPartyStats();
        return pk;
    }

    private static void AssertHealed(PKM pk)
    {
        pk.Move1_PP.Should().Be(42); // 35 base, +1 PP Up
        pk.Move2_PP.Should().Be(40);
        pk.Move3_PP.Should().Be(0);
        pk.Move4_PP.Should().Be(0);
    }

    [Fact]
    public void BoxWriteHealsPP5()
    {
        var sav = new SAV5BW();
        var pk = GetUsedPP<PK5>();
        sav.SetBoxSlotAtIndex(pk, 0, 0, Adapt);
        AssertHealed(sav.GetBoxSlotAtIndex(0, 0));
    }

    [Fact]
    public void PartyWriteKeepsPP5()
    {
        var sav = new SAV5BW();
        var pk = GetUsedPP<PK5>();
        sav.SetPartySlotAtIndex(pk, 0, Adapt);
        var result = sav.GetPartySlotAtIndex(0);
        result.Move1_PP.Should().Be(1);
        result.Move2_PP.Should().Be(0);
    }

    [Fact]
    public void BoxWriteHealsPP4()
    {
        var sav = new SAV4Pt();
        var pk = GetUsedPP<PK4>();
        sav.SetBoxSlotAtIndex(pk, 0, 0, Adapt);
        AssertHealed(sav.GetBoxSlotAtIndex(0, 0));
    }

    [Fact]
    public void BoxWriteKeepsPPWhenNotAdapting()
    {
        var sav = new SAV5BW();
        var pk = GetUsedPP<PK5>();
        sav.SetBoxSlotAtIndex(pk, 0, 0, EntityImportSettings.None);
        var result = sav.GetBoxSlotAtIndex(0, 0);
        result.Move1_PP.Should().Be(1);
        result.Move2_PP.Should().Be(0);
    }
}
