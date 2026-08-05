using Moq;
using PKHeX.Application.Abstractions.GiftRecords;
using PKHeX.Application.Abstractions.LiveHex;
using PKHeX.Application.Services;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public sealed class PKHaXModeTests
{
    [Fact]
    public void MainWindow_PropagatesHaXModeToSaveEditors()
    {
        var saveGateway = new Mock<ISaveFileGateway>();
        var settings = new AppSettings { HaXMode = true };
        var vm = new MainWindowViewModel(
            saveGateway.Object,
            new Mock<IDialogService>().Object,
            new Mock<IWindowService>().Object,
            new Mock<ISpriteRenderer>().Object,
            new Mock<ISlotService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IQrCodeService>().Object,
            UpdateTestDoubles.Coordinator(),
            new Mock<ISaveBackupService>().Object,
            settings,
            new FakeSettingsStore(),
            new Mock<IThemeService>().Object,
            new UndoRedoService(),
            new LanguageService(),
            new Mock<IAutoLegalityService>().Object,
            new Mock<ILiveHexService>().Object,
            new Mock<ILivingDexService>().Object,
            new Mock<IGiftRecordProvider>().Object);

        var save = new SAV6XY();
        saveGateway.Raise(m => m.SaveFileChanged += null, save);

        Assert.True(vm.IsHaXMode);
        Assert.StartsWith("PKHaX Avalonia", vm.WindowTitle);
        Assert.True(vm.CurrentPokemonEditor?.IsHaXMode == true);
        Assert.NotNull(vm.BoxViewer);
        Assert.NotNull(vm.PartyViewer);
        Assert.NotNull(vm.InventoryEditor);
    }

    private static PokemonEditorViewModel CreateEditor(bool haxMode)
    {
        var sav = new SAV6XY();
        return new PokemonEditorViewModel(
            sav.BlankPKM, sav,
            new Mock<ISpriteRenderer>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IWindowService>().Object,
            haxMode);
    }

    [Fact]
    public void AbilityList_IsSpeciesRestricted_WhenNotHaX()
    {
        var editor = CreateEditor(haxMode: false);
        editor.Species = (int)Species.Pikachu;

        Assert.False(editor.ShowFullAbilityList);
        // One entry per ability slot the format defines (slot 1/2/hidden), not the whole ability table.
        Assert.Equal(editor.PreparePKM().PersonalInfo.AbilityCount, editor.AbilityList.Count);
        Assert.DoesNotContain(editor.AbilityList, a => a.Value == (int)Ability.Levitate);
    }

    [Fact]
    public void AbilityList_OffersEveryAbility_WhenHaX()
    {
        var editor = CreateEditor(haxMode: true);
        editor.Species = (int)Species.Pikachu;

        Assert.True(editor.ShowFullAbilityList);
        Assert.Contains(editor.AbilityList, a => a.Value == (int)Ability.Levitate);

        // A foreign ability survives the round trip to the entity.
        editor.Ability = (int)Ability.Levitate;
        var pk = editor.PreparePKM();
        Assert.Equal((int)Ability.Levitate, pk.Ability);
    }

    [Fact]
    public void AbilityNumber_IsWrittenOnlyInHaX()
    {
        var hax = CreateEditor(haxMode: true);
        Assert.True(hax.ShowAbilityNumber); // SAV6XY entities are format 6

        hax.Species = (int)Species.Pikachu;
        hax.AbilityNumber = 4; // hidden-ability slot
        Assert.Equal(4, hax.PreparePKM().AbilityNumber);

        var plain = CreateEditor(haxMode: false);
        Assert.False(plain.ShowAbilityNumber);
    }

    [Fact]
    public void FormEntry_KeepsUndefinedFormValue_WhenHaX()
    {
        var editor = CreateEditor(haxMode: true);
        Assert.True(editor.ShowFormEntry);

        // Bulbasaur has no alternate forms; PKHaX still keeps a typed form value.
        editor.Species = (int)Species.Bulbasaur;
        editor.Form = 3;

        Assert.Equal(3, editor.Form);
        Assert.Equal(3, editor.PreparePKM().Form);
    }

    [Fact]
    public void FormValue_IsClampedToTheSpeciesList_WhenNotHaX()
    {
        var editor = CreateEditor(haxMode: false);
        Assert.False(editor.ShowFormEntry);

        editor.Species = (int)Species.Bulbasaur;
        editor.Form = 3;

        // Changing species repopulates the list, which drops the undefined form.
        editor.Species = (int)Species.Ivysaur;
        Assert.Equal(0, editor.Form);
    }

    [Fact]
    public void HackedStats_AreOnlyWritableWhileEnabled()
    {
        var editor = CreateEditor(haxMode: true);
        Assert.True(editor.CanHackStats);
        Assert.False(editor.HackedStats);

        editor.Species = (int)Species.Pikachu;
        editor.Level = 50;
        var derived = editor.Stat_ATK;

        // Off: the setter is inert and the derived value stands.
        editor.Stat_ATK = 999;
        Assert.Equal(derived, editor.Stat_ATK);

        // On: the typed value is kept, and recalculation no longer overwrites it.
        editor.HackedStats = true;
        editor.Stat_ATK = 999;
        editor.IvATK = 0;
        Assert.Equal(999, editor.Stat_ATK);

        // Off again: the derived value comes back.
        editor.HackedStats = false;
        Assert.NotEqual(999, editor.Stat_ATK);
    }

    [Fact]
    public void HackedStats_AreNotOfferedOutsideHaX()
    {
        var editor = CreateEditor(haxMode: false);
        Assert.False(editor.CanHackStats);

        editor.Species = (int)Species.Pikachu;
        var derived = editor.Stat_SPE;
        editor.HackedStats = true; // nothing gates the property itself; the UI hides it
        editor.Stat_SPE = 777;
        editor.HackedStats = false;

        Assert.Equal(derived, editor.Stat_SPE);
    }
}
