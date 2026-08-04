using System;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class ShowdownIntegrationTests : IDisposable
{
    private readonly Mock<ISaveFileGateway> _saveFileServiceMock = new();
    private readonly Mock<IDialogService> _dialogServiceMock = new();
    private readonly Mock<ISpriteRenderer> _spriteRendererMock = new();
    private readonly Mock<ISlotService> _slotServiceMock = new();
    private readonly Mock<IClipboardService> _clipboardServiceMock = new();

    public ShowdownIntegrationTests()
    {
        PKHeX.Core.GameInfo.CurrentLanguage = "en";
        PKHeX.Core.GameInfo.Strings = PKHeX.Core.GameInfo.GetStrings("en");
    }

    public void Dispose()
    {
        PKHeX.Core.GameInfo.CurrentLanguage = "en";
        PKHeX.Core.GameInfo.Strings = PKHeX.Core.GameInfo.GetStrings("en");
    }

    private MainWindowViewModel CreateViewModel(SaveFile sav)
    {
        var vm = new MainWindowViewModel(
            _saveFileServiceMock.Object,
            _dialogServiceMock.Object,
            new Mock<IWindowService>().Object,
            _spriteRendererMock.Object,
            _slotServiceMock.Object,
            _clipboardServiceMock.Object,
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
            new Mock<PKHeX.Application.Abstractions.GiftRecords.IGiftRecordProvider>().Object
        );
        
        // Simulate loading a save
        // We can't easily trigger the event from outside if we don't own the service logic,
        // but we can manually call the handler if it was public, or just set the property if it was public.
        // Actually, MainWindowViewModel subscribes to _saveFileService.SaveFileChanged.
        // So we raise that event.
        
        _saveFileServiceMock.Raise(m => m.SaveFileChanged += null, sav);
        
        return vm;
    }

    [AvaloniaFact]
    public async Task ExportShowdown_CopiesTextToClipboard()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var vm = CreateViewModel(sav);
        
        // Setup a Pokemon in the editor
        var pkm = new PK8 
        { 
            Species = (int)Species.Charizard,
            Move1 = (int)Move.Flamethrower,
            Nature = Nature.Timid,
            StatAlignment = Nature.Timid, // Gen 8 uses StatAlignment for stats/export
            IV_HP = 31, IV_ATK = 0, IV_DEF = 31, IV_SPA = 31, IV_SPD = 31, IV_SPE = 31,
            EV_SPA = 252, EV_SPE = 252,
            HeldItem = 270 // Life Orb
        };
        pkm.SetPIDGender(0); // Ensure valid PID for gender
        
        Assert.NotNull(vm.CurrentPokemonEditor);
        vm.CurrentPokemonEditor.LoadPKM(pkm);
        
        // Act
        await vm.ExportShowdownCommand.ExecuteAsync(null);
        
        // Assert
        _clipboardServiceMock.Verify(c => c.SetTextAsync(It.Is<string>(s => 
            s.Contains("Charizard") && 
            s.Contains("Life Orb") && 
            s.Contains("Flamethrower") &&
            s.Contains("Timid Nature")
        )), Times.Once);
    }

    [AvaloniaFact]
    public async Task ImportShowdown_LoadsPokemonIntoEditor()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var vm = CreateViewModel(sav);
        // Notes:
        // Item: Light Ball (236)
        // Nature: Jolly
        // Move: Volt Tackle
        var showdownText = @"
Pikachu @ Light Ball
Ability: Static
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail
- Fake Out
- Thunder Punch
";

        _clipboardServiceMock.Setup(c => c.GetTextAsync()).ReturnsAsync(showdownText);
        
        // Act
        await vm.ImportShowdownCommand.ExecuteAsync(null);

        // Assert
        Assert.NotNull(vm.CurrentPokemonEditor);
        // Verify Editor state
        Assert.Equal((int)Species.Pikachu, vm.CurrentPokemonEditor.Species);
        Assert.Equal(236, vm.CurrentPokemonEditor.HeldItem); // Light Ball
        Assert.Equal((int)Nature.Jolly, vm.CurrentPokemonEditor.Nature);
        Assert.Equal(252, vm.CurrentPokemonEditor.EvATK);
        // Moves might differ in ID based on game, but let's check basic property
        Assert.NotEqual(0, vm.CurrentPokemonEditor.Move1); 
    }

    [AvaloniaFact]
    public async Task ImportShowdown_ShowsError_OnEmptyClipboard()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var vm = CreateViewModel(sav);
        
        _clipboardServiceMock.Setup(c => c.GetTextAsync()).ReturnsAsync(string.Empty);
        
        // Act
        await vm.ImportShowdownCommand.ExecuteAsync(null);
        
        // Assert
        _clipboardServiceMock.Verify(c => c.GetTextAsync(), Times.Once);
        // Verify Dialog showed error
        _dialogServiceMock.Verify(d => d.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        
        // Verify Editor species didn't change (still default/empty)
        Assert.Equal(0, vm.CurrentPokemonEditor?.Species ?? 0);
    }
}
