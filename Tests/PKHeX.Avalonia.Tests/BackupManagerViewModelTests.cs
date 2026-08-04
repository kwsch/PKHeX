using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers the Backup Manager restore/delete flows required by issue #135: restoring asks for
/// confirmation, itself creates a pre-restore backup of the current file, and reloads the save;
/// a corrupt backup is rejected gracefully instead of corrupting the working file.
/// </summary>
public sealed class BackupManagerViewModelTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _savePath;
    private readonly SaveBackupService _backupService;
    private readonly AppSettings _settings;
    private readonly Mock<ISaveFileGateway> _saveFileService = new();
    private readonly Mock<IDialogService> _dialogService = new();

    public BackupManagerViewModelTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "pkhex-backupmgr-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _savePath = Path.Combine(_tempDir, "game.sav");

        var sav = SaveFileFactory.CreateBlankSave(GameVersion.W2);
        File.WriteAllBytes(_savePath, sav.Write().ToArray());

        _backupService = new SaveBackupService(new FakeAppPaths());
        _settings = new AppSettings();
        _saveFileService.SetupGet(s => s.CurrentPath).Returns(_savePath);
        _saveFileService.Setup(s => s.LoadSaveFileAsync(It.IsAny<string>())).ReturnsAsync(true);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort */ }
    }

    private BackupEntryRow CreateOneBackup()
    {
        var identity = SaveIdentity.Compute(_savePath);
        var entry = _backupService.CreateBackup(identity, File.ReadAllBytes(_savePath), _settings.SaveBackup.MaxBackupsPerSave)!.Value;
        return new BackupEntryRow(entry);
    }

    [Fact]
    public void Refresh_NoCurrentPath_ShowsGuidanceStatus_DoesNotThrow()
    {
        _saveFileService.SetupGet(s => s.CurrentPath).Returns((string?)null);
        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings);

        Assert.Empty(vm.Backups);
        Assert.Contains("backup", vm.StatusText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RestoreSelectedAsync_UserConfirms_CreatesPreRestoreBackup_AndReloadsSave()
    {
        var row = CreateOneBackup();
        _dialogService.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings)
        {
            SelectedBackup = row,
        };

        await vm.RestoreSelectedCommand.ExecuteAsync(null);

        // Restore itself must be backed up first: the identity folder should now hold the original
        // backup we restored from *and* a fresh pre-restore snapshot of what was on disk just before.
        var identity = SaveIdentity.Compute(_savePath);
        var backups = _backupService.ListBackups(identity);
        Assert.Equal(2, backups.Entries.Count);

        _saveFileService.Verify(s => s.LoadSaveFileAsync(_savePath), Times.Once);
    }

    [Fact]
    public async Task RestoreSelectedAsync_UserCancels_DoesNotTouchDiskOrReload()
    {
        var row = CreateOneBackup();
        _dialogService.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings)
        {
            SelectedBackup = row,
        };

        await vm.RestoreSelectedCommand.ExecuteAsync(null);

        var identity = SaveIdentity.Compute(_savePath);
        Assert.Single(_backupService.ListBackups(identity).Entries); // only the original backup, no pre-restore snapshot
        _saveFileService.Verify(s => s.LoadSaveFileAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RestoreSelectedAsync_CorruptBackup_ShowsError_AndDoesNotOverwriteCurrentFile()
    {
        var identity = SaveIdentity.Compute(_savePath);
        var folder = _backupService.GetBackupFolder(identity);
        Directory.CreateDirectory(folder);
        var corruptPath = Path.Combine(folder, "20200101_000000_000.bak");
        File.WriteAllBytes(corruptPath, [1, 2, 3, 4]); // not a valid save file, but non-empty so it's still listed

        var corruptEntry = new BackupEntryRow(new PKHeX.Application.Abstractions.SaveBackupEntry(
            corruptPath, Path.GetFileName(corruptPath), DateTime.UtcNow, 4));

        _dialogService.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var originalBytes = File.ReadAllBytes(_savePath);
        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings)
        {
            SelectedBackup = corruptEntry,
        };

        await vm.RestoreSelectedCommand.ExecuteAsync(null);

        _dialogService.Verify(d => d.ShowErrorAsync(It.IsAny<string>(), It.Is<string>(m => m.Contains("corrupt", StringComparison.OrdinalIgnoreCase))), Times.Once);
        _saveFileService.Verify(s => s.LoadSaveFileAsync(It.IsAny<string>()), Times.Never);
        Assert.Equal(originalBytes, File.ReadAllBytes(_savePath)); // never overwritten
    }

    [Fact]
    public async Task DeleteSelectedAsync_UserConfirms_RemovesBackup()
    {
        var row = CreateOneBackup();
        _dialogService.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings)
        {
            SelectedBackup = row,
        };

        await vm.DeleteSelectedCommand.ExecuteAsync(null);

        Assert.False(File.Exists(row.FilePath));
        Assert.Empty(vm.Backups);
    }

    [Fact]
    public async Task DeleteSelectedAsync_UserCancels_KeepsBackup()
    {
        var row = CreateOneBackup();
        _dialogService.Setup(d => d.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var vm = new BackupManagerViewModel(_backupService, _saveFileService.Object, _dialogService.Object, _settings)
        {
            SelectedBackup = row,
        };

        await vm.DeleteSelectedCommand.ExecuteAsync(null);

        Assert.True(File.Exists(row.FilePath));
    }
}
