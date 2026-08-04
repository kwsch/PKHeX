using System;
using System.IO;
using System.Linq;
using System.Text;
using PKHeX.Infrastructure;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Covers the automatic save-backup behaviour required by issue #135: timestamped backups, retention
/// pruning, and graceful handling of corrupt/unreadable backup files. Uses a throwaway temp directory
/// so nothing touches the real user data location.
/// </summary>
public sealed class SaveBackupServiceTests : IDisposable
{
    private readonly string _root;
    private readonly SaveBackupService _service;

    public SaveBackupServiceTests()
    {
        var paths = new FakeAppPaths();
        _root = Path.GetDirectoryName(paths.DataDirectory)!;
        _service = new SaveBackupService(paths);
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); }
        catch { /* best effort */ }
    }

    [Fact]
    public void CreateBackup_WritesTimestampedFile_UnderIdentityFolder()
    {
        var identity = "my-save_abc12345";
        var data = Encoding.UTF8.GetBytes("save-bytes");

        var entry = _service.CreateBackup(identity, data, maxBackups: 10);

        Assert.NotNull(entry);
        Assert.True(File.Exists(entry!.Value.FilePath));
        Assert.Equal(_service.GetBackupFolder(identity), Path.GetDirectoryName(entry.Value.FilePath));
        Assert.Equal(data, File.ReadAllBytes(entry.Value.FilePath));
        Assert.Equal(data.Length, entry.Value.SizeBytes);
    }

    [Fact]
    public void CreateBackup_ThenListBackups_ReturnsTheEntry()
    {
        var identity = "identity-1";
        _service.CreateBackup(identity, [1, 2, 3], maxBackups: 10);

        var result = _service.ListBackups(identity);

        Assert.Single(result.Entries);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void ListBackups_ForUnknownIdentity_ReturnsEmpty_NotThrows()
    {
        var result = _service.ListBackups("never-backed-up");

        Assert.Empty(result.Entries);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void ListBackups_OrdersNewestFirst()
    {
        var identity = "ordering-test";
        var folder = _service.GetBackupFolder(identity);
        Directory.CreateDirectory(folder);

        // Write backups directly with explicit timestamps to avoid depending on real-clock timing.
        WriteFakeBackup(folder, "20240101_000000_000.bak", DateTime.UtcNow.AddDays(-2));
        WriteFakeBackup(folder, "20240102_000000_000.bak", DateTime.UtcNow.AddDays(-1));
        WriteFakeBackup(folder, "20240103_000000_000.bak", DateTime.UtcNow);

        var result = _service.ListBackups(identity);

        Assert.Equal(3, result.Entries.Count);
        Assert.True(result.Entries[0].TimestampUtc >= result.Entries[1].TimestampUtc);
        Assert.True(result.Entries[1].TimestampUtc >= result.Entries[2].TimestampUtc);
    }

    [Fact]
    public void CreateBackup_PrunesOldestBeyondMaxBackups()
    {
        var identity = "retention-test";
        const int max = 3;

        for (int i = 0; i < 5; i++)
        {
            _service.CreateBackup(identity, Encoding.UTF8.GetBytes($"version-{i}"), max);
        }

        var result = _service.ListBackups(identity);

        Assert.Equal(max, result.Entries.Count);
        // The newest `max` backups should be the ones retained (last written == "version-4").
        var newest = result.Entries[0];
        Assert.Equal(Encoding.UTF8.GetBytes("version-4"), _service.ReadBackup(newest));
    }

    [Fact]
    public void ListBackups_SkipsEmptyCorruptFile_AndWarns_NeverThrows()
    {
        var identity = "corrupt-test";
        var folder = _service.GetBackupFolder(identity);
        Directory.CreateDirectory(folder);

        // A healthy backup...
        _service.CreateBackup(identity, Encoding.UTF8.GetBytes("healthy"), maxBackups: 10);
        // ...and a zero-byte one simulating a truncated/corrupt write.
        var corruptPath = Path.Combine(folder, "20200101_000000_000.bak");
        File.WriteAllBytes(corruptPath, []);

        var result = _service.ListBackups(identity);

        Assert.Single(result.Entries); // only the healthy one is listed
        Assert.Single(result.Warnings);
        Assert.Contains("corrupt", result.Warnings[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeleteBackup_RemovesFile()
    {
        var identity = "delete-test";
        var entry = _service.CreateBackup(identity, [1, 2, 3], maxBackups: 10)!.Value;

        _service.DeleteBackup(entry);

        Assert.False(File.Exists(entry.FilePath));
        Assert.Empty(_service.ListBackups(identity).Entries);
    }

    [Fact]
    public void DeleteBackup_MissingFile_DoesNotThrow()
    {
        var fakeEntry = new PKHeX.Application.Abstractions.SaveBackupEntry(
            Path.Combine(_root, "does-not-exist.bak"), "does-not-exist.bak", DateTime.UtcNow, 0);

        var ex = Record.Exception(() => _service.DeleteBackup(fakeEntry));

        Assert.Null(ex);
    }

    private static void WriteFakeBackup(string folder, string fileName, DateTime timestampUtc)
    {
        var path = Path.Combine(folder, fileName);
        File.WriteAllBytes(path, [1, 2, 3]);
        File.SetLastWriteTimeUtc(path, timestampUtc);
    }
}
