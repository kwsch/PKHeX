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
}
