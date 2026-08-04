using System.Diagnostics;
using System.IO.Compression;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Updating;

/// <summary>
/// Linux install strategy. AppImage: the new AppImage file is already a complete, self-contained
/// executable, so installing it is just chmod +x and an atomic move over the current AppImage path
/// (via a detached shell helper that waits for our process to exit). Portable (extracted-zip) install:
/// same staging + swap dance as the Windows portable path, using a POSIX shell helper instead of cmd.
/// </summary>
internal sealed class LinuxUpdateStrategy : IPlatformUpdateStrategy
{
    public Task<UpdateInstallResult> InstallAsync(
        string downloadedFilePath, ReleaseAsset asset, InstallLocationInfo location,
        IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        return location.Kind == InstallKind.LinuxAppImage
            ? SwapAppImageAsync(downloadedFilePath, location, progress)
            : SwapPortableAsync(downloadedFilePath, location, progress, ct);
    }

    private static Task<UpdateInstallResult> SwapAppImageAsync(
        string newAppImagePath, InstallLocationInfo location, IProgress<UpdateProgress> progress)
    {
        progress.Report(new UpdateProgress(UpdatePhase.Swapping, 0, null));

        MakeExecutable(newAppImagePath);

        var targetPath = location.Root;
        var backupPath = targetPath + ".bak";
        var logPath = Path.Combine(Path.GetTempPath(), "pkhex-update-helper.log");
        var pid = Environment.ProcessId;

        var lines = new[]
        {
            "#!/bin/sh",
            "PID=\"$1\"",
            $"LOG=\"{logPath}\"",
            "echo \"$(date) waiting for pid $PID to exit\" >> \"$LOG\"",
            "while kill -0 \"$PID\" 2>/dev/null; do sleep 0.5; done",
            "echo \"$(date) swapping AppImage\" >> \"$LOG\"",
            $"rm -f \"{backupPath}\"",
            $"mv \"{targetPath}\" \"{backupPath}\" 2>>\"$LOG\"",
            $"if mv \"{newAppImagePath}\" \"{targetPath}\" 2>>\"$LOG\"; then",
            $"  chmod +x \"{targetPath}\"",
            $"  rm -f \"{backupPath}\"",
            "  echo \"$(date) update applied, relaunching\" >> \"$LOG\"",
            $"  \"{targetPath}\" &",
            "else",
            "  echo \"$(date) swap failed, restoring backup\" >> \"$LOG\"",
            $"  mv \"{backupPath}\" \"{targetPath}\" 2>>\"$LOG\"",
            $"  \"{targetPath}\" &",
            "fi",
        };

        var scriptPath = WriteScript(lines);
        StartDetached("/bin/sh", $"\"{scriptPath}\" {pid}");

        progress.Report(new UpdateProgress(UpdatePhase.Relaunching, 0, null));
        return Task.FromResult(new UpdateInstallResult(true, true, (string?)null));
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

        var backupDir = currentDir + ".bak";
        var logPath = Path.Combine(Path.GetTempPath(), "pkhex-update-helper.log");
        var pid = Environment.ProcessId;
        var exePath = Path.Combine(currentDir, "PKHeX.Avalonia");

        var lines = new[]
        {
            "#!/bin/sh",
            "PID=\"$1\"",
            $"LOG=\"{logPath}\"",
            "echo \"$(date) waiting for pid $PID to exit\" >> \"$LOG\"",
            "while kill -0 \"$PID\" 2>/dev/null; do sleep 0.5; done",
            "echo \"$(date) swapping install directory\" >> \"$LOG\"",
            $"rm -rf \"{backupDir}\"",
            $"cp -a \"{currentDir}\" \"{backupDir}\"",
            $"if cp -a \"{stagingDir}\"/. \"{currentDir}\"/ 2>>\"$LOG\"; then",
            $"  rm -rf \"{backupDir}\"",
            "  echo \"$(date) update applied, relaunching\" >> \"$LOG\"",
            "else",
            "  echo \"$(date) swap failed, restoring backup\" >> \"$LOG\"",
            $"  rm -rf \"{currentDir}\"",
            $"  mv \"{backupDir}\" \"{currentDir}\"",
            "fi",
            $"chmod +x \"{exePath}\" 2>/dev/null",
            $"\"{exePath}\" &",
        };

        var scriptPath = WriteScript(lines);
        StartDetached("/bin/sh", $"\"{scriptPath}\" {pid}");

        progress.Report(new UpdateProgress(UpdatePhase.Relaunching, 0, null));
        return new UpdateInstallResult(true, true, null);
    }

    private static string WriteScript(IReadOnlyList<string> lines)
    {
        var scriptPath = Path.Combine(Path.GetTempPath(), $"pkhex-update-helper-{Guid.NewGuid():N}.sh");
        File.WriteAllText(scriptPath, string.Join('\n', lines) + "\n");
        MakeExecutable(scriptPath);
        return scriptPath;
    }

    private static void MakeExecutable(string path)
    {
        var psi = new ProcessStartInfo("chmod") { UseShellExecute = false, CreateNoWindow = true };
        psi.ArgumentList.Add("+x");
        psi.ArgumentList.Add(path);
        using var chmod = Process.Start(psi);
        chmod?.WaitForExit();
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
}
