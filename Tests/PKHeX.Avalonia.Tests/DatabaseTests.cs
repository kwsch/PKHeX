using System;
using Xunit;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using PKHeX.Avalonia.Services;
using Moq;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests for database, search, and language features.
/// Restores GameInfo.CurrentLanguage to "en" after each test to avoid cross-test pollution.
/// </summary>
public class DatabaseTests : IDisposable
{
    public DatabaseTests()
    {
        GameInfo.CurrentLanguage = "en";
        GameInfo.Strings = GameInfo.GetStrings("en");
    }

    public void Dispose()
    {
        GameInfo.CurrentLanguage = "en";
        GameInfo.Strings = GameInfo.GetStrings("en");
    }

    [Fact]
    public void Verify_Database_Species_Names()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();

        var vm = new PKMDatabaseViewModel(sav, spriteMock.Object, dialogMock.Object);

        var pk = new PK9 { Species = 1000 };
        var entry = new PKMDatabaseEntry(pk, spriteMock.Object);

        Assert.Equal("Gholdengo", entry.SpeciesName);

        GameInfo.CurrentLanguage = "de";
        GameInfo.Strings = GameInfo.GetStrings("de");

        Assert.Equal("Monetigo", entry.SpeciesName);
    }

    [Fact]
    public void Verify_Language_Refresh_PokemonEditor()
    {
        var sav = BlankSaveFile.Get(GameVersion.E);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();

        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

        var vm = new PokemonEditorViewModel(sav.BlankPKM, sav, spriteMock.Object, dialogMock.Object, new Mock<IWindowService>().Object);

        var engName = vm.SpeciesList.First(x => x.Value == 1).Text;
        Assert.Equal("Bulbasaur", engName);

        GameInfo.CurrentLanguage = "de";
        GameInfo.Strings = GameInfo.GetStrings("de");
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

        vm.RefreshLanguage();

        var nameAfterChange = vm.SpeciesList.First(x => x.Value == 1).Text;
        Assert.Equal("Bisasam", nameAfterChange);
    }

    [Fact]
    public void Verify_Language_Refresh_InventoryEditor()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);

        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
        var pouch = vm.Pouches.First(p => p.PouchName == "Items");

        var firstItem = pouch.ItemList.First(x => x.Value > 0);
        var itemId = firstItem.Value;
        var enName = firstItem.Text;
        Assert.False(string.IsNullOrEmpty(enName));

        GameInfo.CurrentLanguage = "de";
        GameInfo.Strings = GameInfo.GetStrings("de");
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

        vm.RefreshLanguage();

        var refreshedItem = pouch.ItemList.First(x => x.Value == itemId);
        Assert.False(string.IsNullOrEmpty(refreshedItem.Text));
        Assert.NotEqual(enName, refreshedItem.Text);
    }

    [Fact]
    public void Verify_Database_Search_Populates_Correctly()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);

        var pkm = sav.BlankPKM;
        pkm.Species = 25;
        pkm.CurrentLevel = 50;
        pkm.Nature = (Nature)3;
        sav.SetBoxSlotAtIndex(pkm, 0, 0);

        var settings = new PKHeX.Core.Searching.SearchSettings
        {
            Species = 0,
            Context = sav.Context
        };

        var allPkms = sav.BoxData.Concat(sav.PartyData);
        var matches = settings.Search(allPkms).ToList();

        var pikachuMatch = matches.FirstOrDefault(p => p.Species == 25);
        Assert.NotNull(pikachuMatch);
        Assert.Equal(50, pikachuMatch.CurrentLevel);
        Assert.Equal((Nature)3, pikachuMatch.Nature);

        var emptyMatches = matches.Where(p => p.Species == 0).ToList();
        Assert.NotEmpty(emptyMatches);

        var vmMatches = matches.Where(p => p.Species != 0).ToList();
        var emptyVmMatches = vmMatches.Where(p => p.Species == 0).ToList();
        Assert.Empty(emptyVmMatches);

        var settingsSpecific = new PKHeX.Core.Searching.SearchSettings
        {
            Species = 25,
            Context = sav.Context
        };
        var matchesSpecific = settingsSpecific.Search(allPkms).Where(p => p.Species != 0).ToList();
        Assert.Single(matchesSpecific);
        Assert.Equal(25, matchesSpecific[0].Species);

        var settingsMismatch = new PKHeX.Core.Searching.SearchSettings
        {
            Species = 268,
            Context = sav.Context
        };
        var matchesMismatch = settingsMismatch.Search(allPkms).Where(p => p.Species != 0).ToList();
        Assert.Empty(matchesMismatch);

        var spriteMock = new Mock<ISpriteRenderer>();
        var entry = new PKMDatabaseEntry(pikachuMatch, spriteMock.Object);

        Assert.Equal("Pikachu", entry.SpeciesName);
        Assert.Equal("50", entry.Level);
        Assert.Contains("Adamant", entry.NatureName);
    }

    [Fact]
    public void Verify_Database_Reacts_To_Language_Message()
    {
        var sav = BlankSaveFile.Get(GameVersion.E);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();

        var vm = new PKMDatabaseViewModel(sav, spriteMock.Object, dialogMock.Object);
        string initialText = vm.Filter.SpeciesList.First(x => x.Value == 1).Text;
        Assert.Equal("Bulbasaur", initialText);

        GameInfo.CurrentLanguage = "de";
        GameInfo.Strings = GameInfo.GetStrings("de");
        GameInfo.FilteredSources = new FilteredGameDataSource(sav, GameInfo.Sources);

        WeakReferenceMessenger.Default.Send(new LanguageChangedMessage("de"));

        string newText = vm.Filter.SpeciesList.First(x => x.Value == 1).Text;
        Assert.Equal("Bisasam", newText);
    }

    [Fact]
    public void Verify_Database_Search_Cascoon()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);

        var pkm = sav.BlankPKM;
        pkm.Species = 268;
        pkm.CurrentLevel = 15;
        pkm.Nature = (Nature)1;

        var clone = pkm.Clone();
        sav.SetBoxSlotAtIndex(clone, 0, 0);

        var loaded = sav.GetBoxSlotAtIndex(0, 0);
        Assert.Equal((Nature)1, loaded.Nature);
    }

    [Fact]
    public void Verify_Search_Wildcards()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var pkm = sav.BlankPKM;
        pkm.Species = 268;
        pkm.Nature = (Nature)1;
        pkm.Ability = 50;
        pkm.HeldItem = 10;

        sav.SetBoxSlotAtIndex(pkm, 0, 0);
        var allPkms = sav.BoxData.Concat(sav.PartyData);

        var setNatureRandom = new PKHeX.Core.Searching.SearchSettings { Species = 0, Context = sav.Context, Nature = Nature.Random };
        var matchNatureRandom = setNatureRandom.Search(allPkms).Where(p => p.Species != 0).Count();

        var setAbilityWild = new PKHeX.Core.Searching.SearchSettings { Species = 0, Context = sav.Context, Ability = -1 };
        var matchAbilityWild = setAbilityWild.Search(allPkms).Where(p => p.Species != 0).Count();

        var setItemWild = new PKHeX.Core.Searching.SearchSettings { Species = 0, Context = sav.Context, Item = -1 };
        var matchItemWild = setItemWild.Search(allPkms).Where(p => p.Species != 0).Count();

        Assert.True(matchNatureRandom == 1, $"Nature.Random (25) failed. Count: {matchNatureRandom}");
        Assert.True(matchAbilityWild == 1, $"Ability -1 failed. Count: {matchAbilityWild}");
        Assert.True(matchItemWild == 1, $"Item -1 failed. Count: {matchItemWild}");
    }

    [Fact]
    public void Verify_French_Language()
    {
        var strings = GameInfo.GetStrings("fr");
        Assert.NotNull(strings);
        Assert.NotEmpty(strings.Species);

        var name = strings.Species[1];
        Assert.Equal("Bulbizarre", name);
    }

    private static PKMDatabaseViewModel CreateDatabaseVm()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();
        return new PKMDatabaseViewModel(sav, spriteMock.Object, dialogMock.Object);
    }

    [Fact]
    public void Verify_Database_Default_Filters_Map_To_Any()
    {
        var vm = CreateDatabaseVm();
        var settings = vm.Filter.GetSearchSettings();

        // Existing wildcard expectations.
        Assert.Equal(0, settings.Species);
        Assert.Equal(PKHeX.Core.Nature.Random, settings.Nature);
        Assert.Equal(-1, settings.Ability);
        Assert.Equal(-1, settings.Item);
        Assert.Null(settings.SearchShiny);
        Assert.Null(settings.SearchLegal);

        // New filters default to "any"/disabled.
        Assert.Null(settings.SearchEgg);
        Assert.Equal(-1, settings.HiddenPowerType);
        Assert.Null(settings.Level);
        Assert.Equal(PKHeX.Core.Searching.SearchComparison.None, settings.SearchLevel);
        Assert.Equal(0, settings.IVType);
        Assert.Equal(0, settings.EVType);
        Assert.Null(settings.ESV);
    }

    [Fact]
    public void Verify_Database_HiddenPower_Filter_Maps()
    {
        var vm = CreateDatabaseVm();

        // "Any" sentinel is present and selected by default.
        Assert.Contains(vm.Filter.HiddenPowerList, x => x.Value == -1);
        Assert.Equal(PKHeX.Core.HiddenPower.TypeCount, vm.Filter.HiddenPowerList.Count - 1);

        vm.Filter.HiddenPowerType = 3; // arbitrary HP type index
        Assert.Equal(3, vm.Filter.GetSearchSettings().HiddenPowerType);
    }

    [Fact]
    public void Verify_Database_Egg_Tristate_Maps()
    {
        var vm = CreateDatabaseVm();

        vm.Filter.IsEgg = true;
        Assert.True(vm.Filter.GetSearchSettings().SearchEgg);

        vm.Filter.IsEgg = false;
        Assert.False(vm.Filter.GetSearchSettings().SearchEgg);

        vm.Filter.IsEgg = null;
        Assert.Null(vm.Filter.GetSearchSettings().SearchEgg);
    }

    [Fact]
    public void Verify_Database_Level_Filter_Maps()
    {
        var vm = CreateDatabaseVm();

        // With no comparison selected, Level is not applied even if a value is set.
        vm.Filter.Level = 50;
        vm.Filter.LevelComparison = (int)PKHeX.Core.Searching.SearchComparison.None;
        var none = vm.Filter.GetSearchSettings();
        Assert.Null(none.Level);
        Assert.Equal(PKHeX.Core.Searching.SearchComparison.None, none.SearchLevel);

        // With a comparison selected, Level + SearchLevel flow through.
        vm.Filter.LevelComparison = (int)PKHeX.Core.Searching.SearchComparison.GreaterThanEquals;
        var ge = vm.Filter.GetSearchSettings();
        Assert.Equal((byte)50, ge.Level);
        Assert.Equal(PKHeX.Core.Searching.SearchComparison.GreaterThanEquals, ge.SearchLevel);
    }

    [Fact]
    public void Verify_Database_IV_EV_Type_Filters_Map()
    {
        var vm = CreateDatabaseVm();

        vm.Filter.IvType = 6; // Flawless (186)
        vm.Filter.EvType = 1; // None (0)
        var settings = vm.Filter.GetSearchSettings();
        Assert.Equal(6, settings.IVType);
        Assert.Equal(1, settings.EVType);
    }

    [Fact]
    public void Verify_Database_ESV_Filter_Maps()
    {
        var vm = CreateDatabaseVm();

        // Disabled => null regardless of value.
        vm.Filter.Esv = 1234;
        vm.Filter.EsvEnabled = false;
        Assert.Null(vm.Filter.GetSearchSettings().ESV);

        // Enabled => value flows through.
        vm.Filter.EsvEnabled = true;
        Assert.Equal(1234, vm.Filter.GetSearchSettings().ESV);
    }

    [Fact]
    public void Verify_Database_New_Filters_Actually_Search()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();

        var pkm = sav.BlankPKM;
        pkm.Species = 25;
        pkm.CurrentLevel = 50;
        sav.SetBoxSlotAtIndex(pkm, 0, 0);

        var vm = new PKMDatabaseViewModel(sav, spriteMock.Object, dialogMock.Object);
        var allPkms = sav.BoxData.Concat(sav.PartyData);

        // Level >= 50 should match the level-50 Pikachu.
        vm.Filter.Level = 50;
        vm.Filter.LevelComparison = (int)PKHeX.Core.Searching.SearchComparison.GreaterThanEquals;
        var hit = vm.Filter.GetSearchSettings().Search(allPkms).Where(p => p.Species != 0).ToList();
        Assert.Single(hit);
        Assert.Equal(25, hit[0].Species);

        // Level <= 10 should exclude it.
        vm.Filter.LevelComparison = (int)PKHeX.Core.Searching.SearchComparison.LessThanEquals;
        vm.Filter.Level = 10;
        var miss = vm.Filter.GetSearchSettings().Search(allPkms).Where(p => p.Species != 0).ToList();
        Assert.Empty(miss);
    }
}
