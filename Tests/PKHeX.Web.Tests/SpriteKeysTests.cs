extern alias atlas;

using FluentAssertions;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.Web.Services.Sprites;
using atlas::PKHeX.Web.SpriteAtlas;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Sprites are chosen as the desktop chooses them for X/Y and Omega Ruby/Alpha Sapphire saves.
/// </summary>
/// <remarks>
/// Expected resource names are written out literally, as the desktop's resx names them, so a change in the shared naming is noticed.
/// </remarks>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SpriteKeysTests
{
    private static SpriteLayers? Resolve(ushort species, byte form = 0, byte gender = 0, bool shiny = false, bool egg = false, bool item = false)
        => SpriteKeys.Resolve(species, form, gender, shiny, egg, item, SpriteFixtures.Index.Contains);

    [Theory]
    [InlineData(25, 0, 0, false, "b_25")]
    [InlineData(25, 1, 1, false, "b_25_1c")] // Pikachu's Generation 6 forms are the cosplay ones
    [InlineData(25, 1, 1, true, "b_25_1cs")]
    [InlineData(668, 0, 1, false, "b_668f")] // Pyroar has a female sprite
    [InlineData(668, 0, 0, false, "b_668")]
    [InlineData(678, 1, 1, false, "b_678_1")] // Meowstic's female is a form, not a gendered sprite
    [InlineData(201, 5, 2, false, "b_201_5")] // Unown F
    [InlineData(666, 3, 0, true, "b_666_3s")] // Vivillon
    [InlineData(670, 5, 1, true, "b_670_5s")] // Floette's Eternal Flower
    [InlineData(720, 1, 2, false, "b_720_1")] // Hoopa Unbound
    public void ChoosesTheDesktopResource(ushort species, byte form, byte gender, bool shiny, string expected)
    {
        var layers = Resolve(species, form, gender, shiny);

        layers.Should().Be(new SpriteLayers(expected, false, EggLayer.None, null, shiny));
    }

    [Fact]
    public void AnEmptyPositionHasNoSprite()
    {
        Resolve(0).Should().BeNull();
    }

    [Fact]
    public void AFormWithoutASpriteShowsTheSpeciesWithTheUnknownMark()
    {
        Resolve(1, form: 5).Should().Be(new SpriteLayers("b_1", true, EggLayer.None, null, false));
    }

    [Fact]
    public void ASpeciesWithoutAnySpriteShowsTheUnknownMarkAlone()
    {
        SpriteKeys.Resolve(1, 0, 0, false, false, false, _ => false).Should().Be(new SpriteLayers(SpriteKeys.Unknown, false, EggLayer.None, null, false));
    }

    [Fact]
    public void AShinyWithoutAShinySpriteUsesTheRegularOne()
    {
        SpriteKeys.Resolve(5, 0, 0, true, false, false, k => k == "b_5").Should().Be(new SpriteLayers("b_5", false, EggLayer.None, null, true));
    }

    [Fact]
    public void TheSecondarySetIsTriedBeforeFallingBack()
    {
        SpriteKeys.Resolve(5, 0, 0, true, false, false, k => k is "b_5" or "c_5s").Should().Be(new SpriteLayers("c_5s", false, EggLayer.None, null, true));
    }

    [Theory]
    [InlineData(false, EggLayer.AsItem)]
    [InlineData(true, EggLayer.OverSpecies)]
    public void AnEggShowsItsSpeciesWithTheEggIcon(bool holdsItem, EggLayer mode)
    {
        Resolve(25, egg: true, item: holdsItem).Should().Be(new SpriteLayers("b_25", false, mode, SpriteKeys.Egg, false));
    }

    [Fact]
    public void AManaphyEggHasItsOwnIcon()
    {
        Resolve(490, egg: true)!.Egg.Should().Be("b_490_e");
    }

    [Fact]
    public void ShinySpritesAreEnabledAsOnTheDesktop()
    {
        _ = Resolve(25);
        SpriteName.AllowShinySprite.Should().BeTrue();
    }

    /// <summary>
    /// Every species, form, gender and shininess a valid X/Y or Omega Ruby/Alpha Sapphire entity can have gets its own sprite:
    /// nothing falls back to the unknown mark, and every chosen image is in the atlas.
    /// </summary>
    [Fact]
    public void EveryGenerationSixFormHasItsOwnSprite()
    {
        var packed = SpriteSelection.Select(SpriteFixtures.Index).ToHashSet();
        var all = SpriteSelection.Enumerate(SpriteFixtures.Index.Contains).ToArray();

        all.Should().HaveCountGreaterThan(4000);
        all.Where(l => l.UnknownForm || l.Base == SpriteKeys.Unknown).Should().BeEmpty();
        all.Select(l => l.Base).Should().OnlyContain(k => packed.Contains(k));
        // Each species' default sprite is packed as well, for forms outside the listed range.
        Enumerable.Range(1, PersonalTable.AO.MaxSpeciesID).Should().OnlyContain(s => packed.Contains($"b_{s}"));
        packed.Should().Contain(SpriteKeys.Fixed);
    }

    [Fact]
    public void ShinySpritesAreChosenWhereTheDesktopHasThem()
    {
        var shiny = SpriteSelection.Enumerate(SpriteFixtures.Index.Contains).Where(l => l.Shiny).ToArray();

        shiny.Where(l => !l.Base.EndsWith('s')).Select(l => l.Base).Distinct().Should().BeEmpty("every Generation 6 form has a shiny sprite");
    }
}
