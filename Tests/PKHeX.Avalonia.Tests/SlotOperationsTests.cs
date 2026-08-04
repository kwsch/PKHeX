using System.Reflection;
using Moq;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Regression coverage for GitHub issue #163: setting a box/party slot with a PKM prepared from a
/// cross-format editor (e.g. a PK3 loaded via the Encounter Database into a Gen-4 save) must not
/// throw. <see cref="SaveFile.SetBoxSlot"/> throws <see cref="ArgumentException"/> when the PKM's
/// concrete type doesn't match <see cref="SaveFile.PKMType"/>, so the VM must convert or reject
/// (with a dialog) before ever calling into the save file.
/// </summary>
public class SlotOperationsTests
{
    private static MainWindowViewModel CreateViewModel(Mock<IDialogService> dialogServiceMock)
    {
        return new MainWindowViewModel(
            new Mock<ISaveFileGateway>().Object,
            dialogServiceMock.Object,
            new Mock<IWindowService>().Object,
            new Mock<ISpriteRenderer>().Object,
            new Mock<ISlotService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IQrCodeService>().Object,
            UpdateTestDoubles.Coordinator(),
            new Mock<ISaveBackupService>().Object,
            new AppSettings(),
            new FakeSettingsStore(),
            new Mock<IThemeService>().Object,
            new UndoRedoService(),
            new LanguageService(),
            new Mock<IAutoLegalityService>().Object,
            new Mock<PKHeX.Application.Abstractions.LiveHex.ILiveHexService>().Object,
            new Mock<ILivingDexService>().Object,
            new Mock<PKHeX.Application.Abstractions.GiftRecords.IGiftRecordProvider>().Object);
    }

    private static void InvokeOnBoxSetSlot(MainWindowViewModel vm, int box, int slot)
    {
        var method = typeof(MainWindowViewModel).GetMethod("OnBoxSetSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        method!.Invoke(vm, [box, slot]);
    }

    [Fact]
    public void OnBoxSetSlot_with_foreign_format_pkm_does_not_throw()
    {
        // Currently open save is Gen-4, but the editor holds a PK3 (e.g. loaded from the Encounter
        // Database), simulating the crash scenario from issue #163.
        var sav4 = SaveFileFactory.CreateBlankSave(GameVersion.Pt);
        var sav3 = SaveFileFactory.CreateBlankSave(GameVersion.E);
        var pk3 = SaveFileFactory.CreateTestPKM(sav3);

        var (editorVm, _, _) = TestHelpers.CreateTestViewModel(pk3, sav3);

        var dialogServiceMock = new Mock<IDialogService>();
        dialogServiceMock
            .Setup(d => d.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var vm = CreateViewModel(dialogServiceMock);
        vm.CurrentSave = sav4;
        vm.CurrentPokemonEditor = editorVm;

        var exception = Record.Exception(() => InvokeOnBoxSetSlot(vm, 0, 0));

        Assert.Null(exception);

        var slotPk = sav4.GetBoxSlotAtIndex(0, 0);
        var converted = slotPk.Species != 0 && slotPk.GetType() == sav4.PKMType;
        var rejected = slotPk.Species == 0;

        // Either the PK3 was converted into a valid PK4 written to the slot, or the conversion
        // failed and the VM surfaced an error dialog instead of writing/crashing.
        Assert.True(converted || rejected);
        if (rejected)
            dialogServiceMock.Verify(d => d.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
