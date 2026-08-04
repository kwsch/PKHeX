using System.Diagnostics;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Updating;

/// <summary>
/// macOS install strategy: mounts the downloaded DMG, copies the bundled <c>.app</c> to a staging
/// directory, strips the quarantine flag, then hands off to a detached shell helper that waits for
/// this process to exit before swapping the staged bundle over the running one and relaunching.
/// </summary>
internal sealed class MacOsUpdateStrategy : IPlatformUpdateStrategy
{
    public async Task<UpdateInstallResult> InstallAsync(
        string downloadedFilePath, ReleaseAsset asset, InstallLocationInfo location,
        IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        var currentAppPath = location.Root.TrimEnd(Path.DirectorySeparatorChar);
        var parentDir = Path.GetDirectoryName(currentAppPath);
        if (string.IsNullOrEmpty(parentDir) || !IsWritable(parentDir))
            return new UpdateInstallResult(false, false, "Update_Error_NeedsAdmin");

        progress.Report(new UpdateProgress(UpdatePhase.Extracting, 0, null));

        var mountPoint = Path.Combine(Path.GetTempPath(), $"pkhex-update-mount-{Guid.NewGuid():N}");
        var stagingDir = Path.Combine(Path.GetTempPath(), $"pkhex-update-stage-{Guid.NewGuid():N}");
        Directory.CreateDirectory(mountPoint);
        Directory.CreateDirectory(stagingDir);

        try
        {
            if (await RunAsync("hdiutil", ["attach", "-nobrowse", "-readonly", "-mountpoint", mountPoint, downloadedFilePath], ct).ConfigureAwait(false) != 0)
                return new UpdateInstallResult(false, false, "Update_Error_SwapFailed");

            try
            {
                var mountedAppPath = Directory.GetDirectories(mountPoint, "*.app").FirstOrDefault();
                if (mountedAppPath is null)
                    return new UpdateInstallResult(false, false, "Update_Error_SwapFailed");

                var stagedAppPath = Path.Combine(stagingDir, Path.GetFileName(mountedAppPath));
                if (await RunAsync("ditto", [mountedAppPath, stagedAppPath], ct).ConfigureAwait(false) != 0)
                    return new UpdateInstallResult(false, false, "Update_Error_SwapFailed");

                await RunAsync("xattr", ["-dr", "com.apple.quarantine", stagedAppPath], ct).ConfigureAwait(false);

                progress.Report(new UpdateProgress(UpdatePhase.Swapping, 0, null));

                var scriptPath = WriteHelperScript(currentAppPath, stagedAppPath);
                StartDetached("/bin/sh", $"\"{scriptPath}\" {Environment.ProcessId}");

                progress.Report(new UpdateProgress(UpdatePhase.Relaunching, 0, null));
                return new UpdateInstallResult(true, true, null);
            }
            finally
            {
                await RunAsync("hdiutil", ["detach", mountPoint, "-quiet"], CancellationToken.None).ConfigureAwait(false);
            }
        }
        finally
        {
            TryDeleteDirectory(mountPoint);
        }
    }

    /// <summary>
    /// Writes a self-contained POSIX shell helper that: waits for our process (pid, passed as $1) to
    /// exit, backs up the current bundle, swaps in the staged one (restoring the backup on failure),
    /// strips quarantine on the new bundle, relaunches it, and logs every step to a temp log file.
    /// </summary>
    private static string WriteHelperScript(string currentAppPath, string stagedAppPath)
    {
        var logPath = Path.Combine(Path.GetTempPath(), "pkhex-update-helper.log");
        var backupPath = currentAppPath + ".bak";

        var lines = new[]
        {
            "#!/bin/sh",
            "PID=\"$1\"",
            $"LOG=\"{logPath}\"",
            "echo \"$(date) waiting for pid $PID to exit\" >> \"$LOG\"",
            "while kill -0 \"$PID\" 2>/dev/null; do sleep 0.5; done",
            "echo \"$(date) swapping app bundle\" >> \"$LOG\"",
            $"rm -rf \"{backupPath}\"",
            $"mv \"{currentAppPath}\" \"{backupPath}\" 2>>\"$LOG\"",
            $"if mv \"{stagedAppPath}\" \"{currentAppPath}\" 2>>\"$LOG\"; then",
            $"  xattr -dr com.apple.quarantine \"{currentAppPath}\" 2>>\"$LOG\"",
            $"  rm -rf \"{backupPath}\"",
            "  echo \"$(date) update applied, relaunching\" >> \"$LOG\"",
            $"  open \"{currentAppPath}\"",
            "else",
            "  echo \"$(date) swap failed, restoring backup\" >> \"$LOG\"",
            $"  mv \"{backupPath}\" \"{currentAppPath}\" 2>>\"$LOG\"",
            $"  open \"{currentAppPath}\"",
            "fi",
        };

        var scriptPath = Path.Combine(Path.GetTempPath(), $"pkhex-update-helper-{Guid.NewGuid():N}.sh");
        File.WriteAllText(scriptPath, string.Join('\n', lines) + "\n");

        // chmod +x
        var psi = new ProcessStartInfo("chmod", $"+x \"{scriptPath}\"") { UseShellExecute = false, CreateNoWindow = true };
        using var chmod = Process.Start(psi);
        chmod?.WaitForExit();

        return scriptPath;
    }

    private static void StartDetached(string fileName, string arguments)
    {
        var psi = new ProcessStartInfo(fileName, arguments)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        Process.Start(psi);
    }

    private static async Task<int> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken ct)
    {
        var psi = new ProcessStartInfo(fileName) { UseShellExecute = false, CreateNoWindow = true };
        foreach (var arg in arguments)
            psi.ArgumentList.Add(arg);

        using var process = Process.Start(psi);
        if (process is null)
            return -1;

        await process.WaitForExitAsync(ct).ConfigureAwait(false);
        return process.ExitCode;
    }

    private static bool IsWritable(string directory)
    {
        try
        {
            var probePath = Path.Combine(directory, $".pkhex-update-write-probe-{Guid.NewGuid():N}.tmp");
            File.WriteAllBytes(probePath, [0]);
            File.Delete(probePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
