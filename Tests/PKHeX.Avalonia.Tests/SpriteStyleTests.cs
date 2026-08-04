using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PKHeX.Avalonia.Converters;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Avalonia.Views;
using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Avalonia.Tests;

public class SpriteStyleTests
{
    [Fact]
    public void GetSuggested_LegendsArceus_ReturnsMugshot()
        => Assert.Equal(SpriteStyle.Mugshot, SpriteStyleSelector.GetSuggested(new SAV8LA()));

    [Fact]
    public void GetSuggested_ScarletViolet_ReturnsArtwork()
        => Assert.Equal(SpriteStyle.Artwork, SpriteStyleSelector.GetSuggested(new SAV9SV()));

    [Fact]
    public void GetSuggested_LegendsZA_ReturnsArtwork()
        => Assert.Equal(SpriteStyle.Artwork, SpriteStyleSelector.GetSuggested(new SAV9ZA()));

    [Fact]
    public void GetSuggested_SwordShield_ReturnsClassic()
        => Assert.Equal(SpriteStyle.Classic, SpriteStyleSelector.GetSuggested(new SAV8SWSH()));

    [Theory]
    [InlineData(SpritePreference.ForceSprites, SpriteStyle.Classic)]
    [InlineData(SpritePreference.ForceMugshots, SpriteStyle.Mugshot)]
    [InlineData(SpritePreference.ForceArtwork, SpriteStyle.Artwork)]
    public void Resolve_ForcePreference_OverridesSave(SpritePreference pref, SpriteStyle expected)
        => Assert.Equal(expected, SpriteStyleSelector.Resolve(pref, new SAV9SV()));

    [Fact]
    public void Resolve_UseSuggested_DefersToSave()
        => Assert.Equal(SpriteStyle.Artwork, SpriteStyleSelector.Resolve(SpritePreference.UseSuggested, new SAV9SV()));

    [Fact]
    public void Loader_Classic_LoadsKnownSpecies()
    {
        var loader = new SpriteLoader { Style = SpriteStyle.Classic };
        using var bmp = loader.GetSprite(25, 0, 0, 0, false, EntityContext.Gen9);
        Assert.NotNull(bmp);
    }

    [Fact]
    public void Loader_Classic_ShinyDiffersFromNonShiny()
    {
        // Proves the s-suffix fix: b_25s.png loads instead of falling back to b_25.png.
        var loader = new SpriteLoader { Style = SpriteStyle.Classic };
        using var shiny = loader.GetSprite(25, 0, 0, 0, true, EntityContext.Gen9);
        using var normal = loader.GetSprite(25, 0, 0, 0, false, EntityContext.Gen9);
        Assert.NotNull(shiny);
        Assert.NotNull(normal);
        Assert.False(shiny!.Bytes.SequenceEqual(normal!.Bytes));
    }

    [Fact]
    public void Loader_Artwork_LoadsGen9Species()
    {
        var loader = new SpriteLoader { Style = SpriteStyle.Artwork };
        using var bmp = loader.GetSprite(906, 0, 0, 0, false, EntityContext.Gen9); // Sprigatito
        Assert.NotNull(bmp);
    }

    [Fact]
    public void Loader_Artwork_ShinyDiffersFromNonShiny()
    {
        var loader = new SpriteLoader { Style = SpriteStyle.Artwork };
        using var shiny = loader.GetSprite(6, 0, 0, 0, true, EntityContext.Gen9);   // Charizard, a_6s.png
        using var normal = loader.GetSprite(6, 0, 0, 0, false, EntityContext.Gen9); // a_6.png
        Assert.NotNull(shiny);
        Assert.NotNull(normal);
        // The new "Artwork Shiny Sprites" set now resolves to a distinct sprite
        // (before wiring the ShinyFolder, shiny Artwork fell back to the non-shiny image).
        Assert.False(shiny!.Bytes.SequenceEqual(normal!.Bytes));
    }

    [Fact]
    public void Loader_Classic_MissingGen9Species_ReturnsNull()
    {
        // Classic set has no Gen 9 sprites; loader returns null (renderer then draws a placeholder).
        var loader = new SpriteLoader { Style = SpriteStyle.Classic };
        using var bmp = loader.GetSprite(906, 0, 0, 0, false, EntityContext.Gen9);
        Assert.Null(bmp);
    }

    [AvaloniaFact]
    public void Renderer_ScarletViolet_RendersGen9SpeciesSprite()
    {
        // End-to-end: AppSettings-injected renderer + save-based style selection (Artwork for SV)
        // renders a Gen 9 species (artwork-only) to a real Avalonia bitmap.
        var renderer = new AvaloniaSpriteRenderer(new AppSettings()); // UseSuggested
        var sav = new SAV9SV();
        renderer.Initialize(sav);

        var pk = sav.BlankPKM;
        pk.Species = 1000; // Gholdengo (Gen 9, only present in the artwork set)
        var bmp = renderer.GetSprite(pk);

        Assert.NotNull(bmp);
    }

    [Theory]
    [InlineData(SpritePreference.UseSuggested, "Use Suggested")]
    [InlineData(SpritePreference.ForceSprites, "Force Sprites")]
    [InlineData(SpritePreference.ForceMugshots, "Force Mugshots")]
    [InlineData(SpritePreference.ForceArtwork, "Force Artwork")]
    public void Converter_MapsPreferenceToUpstreamLabel(SpritePreference pref, string expected)
    {
        var converter = new SpritePreferenceLabelConverter();
        var result = converter.Convert(pref, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    [AvaloniaFact]
    public void SettingsView_LoadsAndBindsSpritePreference()
    {
        // Proves the new Sprites card wires up at runtime: the XAML resource (converter),
        // the ComboBox ItemsSource, and the SelectedItem binding.
        var settings = new AppSettings();
        var vm = new SettingsViewModel(settings, new FakeSettingsStore(), new ThemeService(settings, new FakeSettingsStore()), new PKHeX.Application.Services.LanguageService(), UpdateTestDoubles.Coordinator());
        var view = new SettingsView { DataContext = vm };
        var window = new Window { Content = view, Width = 480, Height = 720 };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var combo = view.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault();
        Assert.NotNull(combo);
        Assert.Equal(4, combo!.ItemCount);
        Assert.Equal(vm.SpritePreference, combo.SelectedItem);
    }
}
