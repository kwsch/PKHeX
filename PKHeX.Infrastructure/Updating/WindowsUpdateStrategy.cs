using System.Diagnostics;
using System.IO.Compression;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Updating;

/// <summary>
/// Windows install strategy. Two very different paths depending on <see cref="InstallLocationInfo.Kind"/>:
/// a portable (extracted-zip) install extracts the verified zip to staging and hands off to a detached
/// cmd helper that waits for this process to exit, swaps the directory (with a <c>.bak</c> rollback on
/// failure), and relaunches; an installer-owned install (Program Files) just launches the verified
/// Inno Setup installer, which handles its own UAC elevation prompt, and exits.
/// </summary>
internal sealed class WindowsUpdateStrategy : IPlatformUpdateStrategy
{
    public async Task<UpdateInstallResult> InstallAsync(
        string downloadedFilePath, ReleaseAsset asset, InstallLocationInfo location,
        IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        if (location.Kind == InstallKind.WindowsInstaller)
            return LaunchInstaller(downloadedFilePath);

        return await SwapPortableAsync(downloadedFilePath, location, progress, ct).ConfigureAwait(false);
    }

    private static UpdateInstallResult LaunchInstaller(string installerPath)
    {
        try
        {
            // Inno Setup installers self-elevate (UAC) when needed via their embedded manifest.
            Process.Start(new ProcessStartInfo(installerPath) { UseShellExecute = true });
            return new UpdateInstallResult(true, true, null);
        }
        catch (Exception)
        {
            return new UpdateInstallResult(false, false, "Update_Error_NeedsAdmin");
        }
    }

    private static async Task<UpdateInstallResult> SwapPortableAsync(
        string zipPath, InstallLocationInfo location, IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        var currentDir = location.Root.TrimEnd(Path.DirectorySeparatorChar);
        var stagingDir = Path.Combine(Path.GetTempPath(), $"pkhex-update-stage-{Guid.NewGuid():N}");

        progress.Report(new UpdateProgress(UpdatePhase.Extracting, 0, null));
        try
        {
            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, stagingDir), ct).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return new UpdateInstallResult(false, false, "Update_Error_SwapFailed");
        }

        progress.Report(new UpdateProgress(UpdatePhase.Swapping, 0, null));

        var exeName = "PKHeX.Avalonia.exe";
        var scriptPath = WriteHelperScript(currentDir, stagingDir, exeName);

        Process.Start(new ProcessStartInfo("cmd.exe", $"/c start \"\" /min \"{scriptPath}\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
        });

        progress.Report(new UpdateProgress(UpdatePhase.Relaunching, 0, null));
        return new UpdateInstallResult(true, true, null);
    }

    /// <summary>
    /// Writes a self-contained .cmd helper that waits for our pid to exit, backs up the current
    /// install directory's contents, copies the staged files in (restoring the backup on failure),
    /// and relaunches — logging every step to a temp log file.
    /// </summary>
    private static string WriteHelperScript(string currentDir, string stagingDir, string exeName)
    {
        var pid = Environment.ProcessId;
        var logPath = Path.Combine(Path.GetTempPath(), "pkhex-update-helper.log");
        var backupDir = currentDir + ".bak";
        var exePath = Path.Combine(currentDir, exeName);

        var lines = new[]
        {
            "@echo off",
            $"echo %date% %time% waiting for pid {pid} to exit >> \"{logPath}\"",
            $":waitloop",
            $"tasklist /fi \"PID eq {pid}\" | find \"{pid}\" >nul",
            "if not errorlevel 1 (",
            "  timeout /t 1 /nobreak >nul",
            "  goto waitloop",
            ")",
            $"echo %date% %time% swapping install directory >> \"{logPath}\"",
            $"if exist \"{backupDir}\" rmdir /s /q \"{backupDir}\"",
            $"mkdir \"{backupDir}\"",
            $"xcopy \"{currentDir}\" \"{backupDir}\" /e /i /h /y >> \"{logPath}\" 2>&1",
            $"xcopy \"{stagingDir}\" \"{currentDir}\" /e /i /h /y >> \"{logPath}\" 2>&1",
            "if errorlevel 1 (",
            $"  echo %date% %time% swap failed, restoring backup >> \"{logPath}\"",
            $"  xcopy \"{backupDir}\" \"{currentDir}\" /e /i /h /y >> \"{logPath}\" 2>&1",
            ") else (",
            $"  rmdir /s /q \"{backupDir}\"",
            $"  echo %date% %time% update applied, relaunching >> \"{logPath}\"",
            ")",
            $"start \"\" \"{exePath}\"",
        };

        var scriptPath = Path.Combine(Path.GetTempPath(), $"pkhex-update-helper-{Guid.NewGuid():N}.cmd");
        File.WriteAllText(scriptPath, string.Join("\r\n", lines) + "\r\n");
        return scriptPath;
    }
}
