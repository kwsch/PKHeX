using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PKHeX.Core;

/// <summary>
/// Logic for startup initialization and argument parsing.
/// </summary>
public static class StartupUtil
{
    public static void ReloadSettings(IProgramSettings settings)
    {
        var backup = settings.Backup;
        SaveFinder.CustomBackupPaths.Clear();
        SaveFinder.CustomBackupPaths.AddRange(backup.OtherBackupPaths.Where(Directory.Exists));

        settings.SaveLanguage.Apply();

        var write = settings.SlotWrite;
        SaveFile.SetUpdateDex = write.SetUpdateDex ? EntityImportOption.Enable : EntityImportOption.Disable;
        SaveFile.SetUpdatePKM = write.SetUpdatePKM ? EntityImportOption.Enable : EntityImportOption.Disable;
        SaveFile.SetUpdateRecords = write.SetUpdateRecords ? EntityImportOption.Enable : EntityImportOption.Disable;
        CommonEdits.ShowdownSetIVMarkings = settings.Import.ApplyMarkings;
        CommonEdits.ShowdownSetBehaviorNature = settings.Import.ApplyNature;
        ParseSettings.Initialize(settings.Legality);

        var converter = settings.Converter;
        EntityConverter.AllowIncompatibleConversion = converter.AllowIncompatibleConversion;
        EntityConverter.RejuvenateHOME = converter.AllowGuessRejuvenateHOME;
        EntityConverter.VirtualConsoleSourceGen1 = converter.VirtualConsoleSourceGen1;
        EntityConverter.VirtualConsoleSourceGen2 = converter.VirtualConsoleSourceGen2;
        EntityConverter.RetainMetDateTransfer45 = converter.RetainMetDateTransfer45;

        var mgdb = settings.LocalResources.GetMGDatabasePath();
        if (!Directory.Exists(mgdb))
            return;
        new Task(() => EncounterEvent.RefreshMGDB(mgdb)).Start();
    }

    public static ProgramInit FormLoadInitialActions(ReadOnlySpan<string> args, IStartupSettings startup, BackupSettings backup, Version currentVersion)
    {
        // Check if there is an update available
        var showChangelog = GetShowChangelog(currentVersion, startup);
        // Remember the current version for next run

        // HaX behavior requested
        var hax = startup.ForceHaXOnLaunch || GetIsHaX(args);

        // Prompt to create a backup folder
        var showAskBackupFolderCreate = !backup.BAKPrompt;
        if (showAskBackupFolderCreate)
            backup.BAKPrompt = true; // Never prompt after this run, unless changed in settings

        startup.Version = currentVersion.ToString();
        return new ProgramInit(showChangelog, showAskBackupFolderCreate, hax);
    }

    private static bool GetShowChangelog(Version currentVersion, IStartupSettings startup)
    {
        if (!startup.ShowChangelogOnUpdate)
            return false;
        if (!Version.TryParse(startup.Version, out var lastRun))
            return false;
        return lastRun < currentVersion;
    }

    public static StartupArguments GetStartup(ReadOnlySpan<string> args, IStartupSettings startup, LocalResourceSettings localResource)
    {
        var result = new StartupArguments();
        try
        {
            result.ReadArguments(args);
            result.ReadSettings(startup);
            result.ReadTemplateIfNoEntity(localResource.GetTemplatePath());
        } catch (Exception ex)
        {
            // If an error occurs, store it in the result for later handling
            result.Error = ex;
        }
        return result;
    }

    private static bool GetIsHaX(ReadOnlySpan<string> args)
    {
        foreach (var x in args)
        {
            var arg = x.AsSpan().Trim('-');
            if (arg.Equals("HaX", StringComparison.CurrentCultureIgnoreCase))
                return true;
        }

        ReadOnlySpan<char> path = Environment.ProcessPath!;
        return Path.GetFileNameWithoutExtension(path).EndsWith("HaX");
    }
}

public readonly record struct ProgramInit(bool ShowChangelog, bool BackupPrompt, bool HaX);
