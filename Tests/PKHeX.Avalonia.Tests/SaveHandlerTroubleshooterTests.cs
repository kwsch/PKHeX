using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PKHeX.Application.Abstractions;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class SaveHandlerTroubleshooterTests
{
    private static SaveHandlerTroubleshooterViewModel NewVM(out FakeGateway gateway)
    {
        gateway = new FakeGateway();
        var dialog = new Mock<IDialogService>();
        return new SaveHandlerTroubleshooterViewModel(dialog.Object, gateway);
    }

    // SAV5B2W2's blank .Write() bytes round-trip cleanly through SaveUtil under BOTH auto-detect and an
    // explicit forced type, so it is the known-good blank fixture for the force-load path. (Notably,
    // a blank SAV8SWSH does NOT round-trip — its .Write() can't be re-decrypted — so it is unsuitable.)
    private static byte[] KnownGoodBlank() => new SAV5B2W2().Write().ToArray();

    [Fact]
    public void Lists_ArePopulated_WithDefaultsFirst()
    {
        var vm = NewVM(out _);

        // Save types: Default (auto) entry plus every concrete SaveFileType.
        Assert.True(vm.SaveTypes.Count > 1);
        Assert.Equal(SaveFileType.None, vm.SaveTypes[0].Value);
        Assert.Contains(vm.SaveTypes, t => t.Value == SaveFileType.B2W2);
        Assert.DoesNotContain(vm.SaveTypes.Skip(1), t => t.Value == SaveFileType.None);

        // Handlers: Default (no special handling) entry plus every registered handler.
        Assert.Equal(SaveUtil.Handlers.Count + 1, vm.Handlers.Count);
        Assert.All(vm.Handlers, h => Assert.NotNull(h.Handler));

        // Languages: every LanguageID value.
        Assert.Equal(Enum.GetValues<LanguageID>().Length, vm.Languages.Count);
        Assert.Contains(vm.Languages, l => l.Value == LanguageID.English);

        // Default selections are sane.
        Assert.NotNull(vm.SelectedSaveType);
        Assert.NotNull(vm.SelectedHandler);
        Assert.NotNull(vm.SelectedLanguage);
        Assert.NotNull(vm.SelectedVersion);
    }

    [Fact]
    public void SelectingType_FiltersVersionChoices_ToThatType()
    {
        var vm = NewVM(out _);

        vm.SelectedSaveType = vm.SaveTypes.First(t => t.Value == SaveFileType.B2W2);

        // First entry is always "Any"; the rest must map to the chosen type.
        Assert.Equal(GameVersion.Any, vm.Versions[0].Value);
        Assert.True(vm.Versions.Count > 1);
        Assert.All(vm.Versions.Skip(1), v => Assert.Equal(SaveFileType.B2W2, v.Value.SaveFileType));
        Assert.Contains(vm.Versions, v => v.Value is GameVersion.B2 or GameVersion.W2);
    }

    [Fact]
    public void ForceLoad_AutoDefault_LoadsKnownGoodBlank()
    {
        var vm = NewVM(out _);

        // Defaults: Save Type = "Default (auto-detect)", Handler = "Default".
        var sav = vm.ForceLoad(KnownGoodBlank(), path: null);

        Assert.NotNull(sav);
        Assert.IsType<SAV5B2W2>(sav);
    }

    [Fact]
    public void ForceLoad_ExplicitTypeAndHandler_LoadsKnownGoodBlank()
    {
        var vm = NewVM(out _);

        vm.SelectedSaveType = vm.SaveTypes.First(t => t.Value == SaveFileType.B2W2);
        vm.SelectedHandler = vm.Handlers[0]; // Default (no special handling)
        vm.SelectedVersion = vm.Versions[0]; // Any

        var sav = vm.ForceLoad(KnownGoodBlank(), path: null);

        Assert.NotNull(sav);
        Assert.IsType<SAV5B2W2>(sav);
    }

    [Fact]
    public void ForceLoad_ExplicitType_RecoversSaveThatAutoDetectionMisses()
    {
        // Core scenario the tool exists for: a blank SAV1 is not recognized by auto-detection, but
        // forcing the RBY type loads it. This proves the forced-type path adds capability over the
        // normal loader.
        var data = new SAV1().Write().ToArray();
        Assert.Null(SaveUtil.GetSaveFile(data)); // auto-detection fails

        var vm = NewVM(out _);
        vm.SelectedSaveType = vm.SaveTypes.First(t => t.Value == SaveFileType.RBY);
        vm.SelectedVersion = vm.Versions[0];

        var sav = vm.ForceLoad(data, path: null);

        Assert.NotNull(sav);
        Assert.IsType<SAV1>(sav);
    }

    [Fact]
    public void ForceLoad_InvalidBytes_ReturnsNull()
    {
        var vm = NewVM(out _);
        var junk = new byte[64]; // far too small to be any real save

        Assert.Null(vm.ForceLoad(junk, path: null));
    }

    [Fact]
    public void ForceLoad_WrongForcedType_FailsToBuild()
    {
        var vm = NewVM(out _);
        // Valid B2W2 bytes, but forced to be read as a Gen-1 (RBY) save: the size/shape mismatch must
        // not yield a valid B2W2 save. ForceLoad itself does not swallow exceptions (the Load command
        // does), so a wrong forced type may either throw or return null — both are "failed to build".
        var data = KnownGoodBlank();
        vm.SelectedSaveType = vm.SaveTypes.First(t => t.Value == SaveFileType.RBY);
        vm.SelectedVersion = vm.Versions[0];

        SaveFile? result = null;
        var ex = Record.Exception(() => result = vm.ForceLoad(data, path: null));

        Assert.True(ex is not null || result is null or not SAV5B2W2);
    }

    [Fact]
    public async Task Load_OnSuccess_HandsSaveToGatewayAndExposesResult()
    {
        var vm = NewVM(out var gateway);

        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            $"sht_b2w2_{Guid.NewGuid():N}.sav");
        await System.IO.File.WriteAllBytesAsync(path, KnownGoodBlank());
        try
        {
            var closed = false;
            vm.CloseRequested = () => closed = true;
            vm.FilePath = path;

            await vm.LoadCommand.ExecuteAsync(null);

            Assert.True(vm.LastLoadSucceeded);
            Assert.NotNull(vm.LoadedSave);
            Assert.IsType<SAV5B2W2>(vm.LoadedSave);
            Assert.Same(vm.LoadedSave, gateway.LastOpened);
            Assert.Equal(path, gateway.LastPath);
            Assert.True(closed);
            Assert.Contains("Success", vm.Status);
        }
        finally
        {
            System.IO.File.Delete(path);
        }
    }

    [Fact]
    public async Task Load_OnInvalidFile_ReportsFailure_AndDoesNotIntegrate()
    {
        var vm = NewVM(out var gateway);

        var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            $"sht_junk_{Guid.NewGuid():N}.sav");
        await System.IO.File.WriteAllBytesAsync(path, new byte[64]);
        try
        {
            vm.FilePath = path;

            await vm.LoadCommand.ExecuteAsync(null);

            Assert.False(vm.LastLoadSucceeded);
            Assert.Null(vm.LoadedSave);
            Assert.Null(gateway.LastOpened);
            Assert.Contains("failed", vm.Status, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            System.IO.File.Delete(path);
        }
    }

    [Fact]
    public void LoadCommand_CannotExecute_WithoutFilePath()
    {
        var vm = NewVM(out _);
        Assert.False(vm.LoadCommand.CanExecute(null));

        vm.FilePath = "/some/path.sav";
        Assert.True(vm.LoadCommand.CanExecute(null));
    }

    /// <summary>Minimal gateway capturing the integration hand-off without touching any UI.</summary>
    private sealed class FakeGateway : ISaveFileGateway
    {
        public SaveFile? CurrentSave { get; private set; }
        public bool HasSave => CurrentSave is not null;
        public string? CurrentPath => LastPath;
        public SaveFile? LastOpened { get; private set; }
        public string? LastPath { get; private set; }

        public event Action<SaveFile?>? SaveFileChanged;

        public Task<bool> LoadSaveFileAsync(string path) => Task.FromResult(false);

        public void OpenLoadedSave(SaveFile sav, string? path = null)
        {
            CurrentSave = sav;
            LastOpened = sav;
            LastPath = path;
            SaveFileChanged?.Invoke(sav);
        }

        public Task<bool> SaveFileAsync(string? path = null) => Task.FromResult(true);

        public void CloseSave()
        {
            CurrentSave = null;
            SaveFileChanged?.Invoke(null);
        }
    }
}
