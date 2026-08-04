using System.Text.Json;
using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers the theme-system logic that doesn't require a live Avalonia window: preference
/// persistence (round-tripping through AppSettings' JSON store) and the Settings picker wiring
/// (that selecting a theme calls through to IThemeService, and that loading existing state does not).
/// </summary>
public class ThemeTests
{
    [Theory]
    [InlineData(AppTheme.Dark)]
    [InlineData(AppTheme.Light)]
    [InlineData(AppTheme.HighContrast)]
    [InlineData(AppTheme.System)]
    public void AppSettings_RoundTripsThemePreference_ThroughJson(AppTheme theme)
    {
        var settings = new AppSettings { Theme = new AppSettings.ThemeSettings { Selected = theme } };

        var json = JsonSerializer.Serialize(settings);
        var restored = JsonSerializer.Deserialize<AppSettings>(json);

        Assert.NotNull(restored);
        Assert.Equal(theme, restored!.Theme.Selected);
    }

    [Fact]
    public void AppSettings_DefaultsToDarkTheme()
    {
        var settings = new AppSettings();
        Assert.Equal(AppTheme.Dark, settings.Theme.Selected);
    }

    [Fact]
    public void ThemeService_ApplyTheme_UpdatesCurrentThemeAndPersists()
    {
        var settings = new AppSettings();
        var store = new FakeSettingsStore();
        var service = new ThemeService(settings, store);

        service.ApplyTheme(AppTheme.Light);

        Assert.Equal(AppTheme.Light, service.CurrentTheme);
        Assert.Equal(AppTheme.Light, settings.Theme.Selected);
        Assert.Same(settings, store.Saved);
    }

    [Fact]
    public void SettingsViewModel_Load_DoesNotReapplyTheme()
    {
        var settings = new AppSettings { Theme = new AppSettings.ThemeSettings { Selected = AppTheme.HighContrast } };
        var themeServiceMock = new Mock<IThemeService>();
        themeServiceMock.SetupGet(t => t.CurrentTheme).Returns(AppTheme.HighContrast);

        var vm = new SettingsViewModel(settings, new FakeSettingsStore(), themeServiceMock.Object, new PKHeX.Application.Services.LanguageService(), UpdateTestDoubles.Coordinator());

        Assert.Equal(AppTheme.HighContrast, vm.SelectedTheme);
        themeServiceMock.Verify(t => t.ApplyTheme(It.IsAny<AppTheme>()), Times.Never);
    }

    [Fact]
    public void SettingsViewModel_ChangingSelectedTheme_AppliesThroughThemeService()
    {
        var settings = new AppSettings();
        var themeServiceMock = new Mock<IThemeService>();
        themeServiceMock.SetupGet(t => t.CurrentTheme).Returns(AppTheme.Dark);

        var vm = new SettingsViewModel(settings, new FakeSettingsStore(), themeServiceMock.Object, new PKHeX.Application.Services.LanguageService(), UpdateTestDoubles.Coordinator());

        vm.SelectedTheme = AppTheme.Light;

        themeServiceMock.Verify(t => t.ApplyTheme(AppTheme.Light), Times.Once);
    }

    [Fact]
    public void AppThemeVariants_HighContrast_InheritsFromDark()
    {
        Assert.Equal("HighContrast", AppThemeVariants.HighContrast.Key);
        Assert.Equal(global::Avalonia.Styling.ThemeVariant.Dark, AppThemeVariants.HighContrast.InheritVariant);
    }
}
