using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PKHeX.Core;

/// <summary>
/// Logic object that assembles parameters used for starting up an editing environment.
/// </summary>
public sealed class StartupArguments
{
    public PKM? Entity { get; private set; }
    public SaveFile? SAV { get; private set; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public Exception? Error { get; internal set; }
    public readonly List<object> Extra = [];

    /// <summary>
    /// Step 1: Reads in command line arguments.
    /// </summary>
    public void ReadArguments(ReadOnlySpan<string> args)
    {
        foreach (var path in args)
        {
            var other = FileUtil.GetSupportedFile(path, SAV);
            if (other is SaveFile s)
                (SAV = s).Metadata.SetExtraInfo(path);
            else if (other is PKM pk)
                Entity = pk;
            else if (other is not null)
                Extra.Add(other);
        }
    }

    /// <summary>
    /// Step 2: Reads settings config.
    /// </summary>
    public void ReadSettings(IStartupSettings startup)
    {
        if (SAV is not null)
            return;

        if (Entity is { } x)
            SAV = ReadSettingsDefinedPKM(startup, x) ?? GetBlank(x);
        else if (Extra.OfType<SAV3GCMemoryCard>().FirstOrDefault() is { } mc && SaveUtil.TryGetSaveFile(mc, out var mcSav))
            SAV = mcSav;
        else
            SAV = ReadSettingsAnyPKM(startup) ?? GetBlankSaveFile(startup.DefaultSaveVersion, SAV);
    }

    // step 3
    public void ReadTemplateIfNoEntity(string path)
    {
        if (Entity is not null)
            return;
        if (SAV is not { } sav)
            return;

        var pk = sav.LoadTemplate(path);
        var isBlank = pk.Data.SequenceEqual(sav.BlankPKM.Data);
        if (isBlank)
            EntityTemplates.TemplateFields(pk, sav);

        Entity = pk;
    }

    private static SaveFile? ReadSettingsDefinedPKM(IStartupSettings startup, PKM pk) => startup.AutoLoadSaveOnStartup switch
    {
        SaveFileLoadSetting.RecentBackup => SaveFinder.DetectSaveFiles(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token).FirstOrDefault(z => z.IsCompatiblePKM(pk)),
        SaveFileLoadSetting.LastLoaded => GetMostRecentlyLoaded(startup.RecentlyLoaded).FirstOrDefault(z => z.IsCompatiblePKM(pk)),
        _ => null,
    };

    private static SaveFile? ReadSettingsAnyPKM(IStartupSettings startup) => startup.AutoLoadSaveOnStartup switch
    {
        SaveFileLoadSetting.RecentBackup => SaveFinder.DetectSaveFiles(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token).FirstOrDefault(),
        SaveFileLoadSetting.LastLoaded => GetMostRecentlyLoaded(startup.RecentlyLoaded).FirstOrDefault(),
        _ => null,
    };

    #region Utility
    private static SaveFile GetBlank(PKM pk)
    {
        var ctx = pk.Context;
        var version = ctx.GetSingleGameVersion();
        if (pk is { Format: 1, Japanese: true })
            version = GameVersion.BU;

        return BlankSaveFile.Get(version, pk.OriginalTrainerName, (LanguageID)pk.Language);
    }

    private static SaveFile GetBlankSaveFile(GameVersion version, SaveFile? current)
    {
        var lang = BlankSaveFile.GetSafeLanguage(current);
        var tr = BlankSaveFile.GetSafeTrainerName(current, lang);
        var sav = BlankSaveFile.Get(version, tr, lang);
        if (sav.Version == GameVersion.Invalid) // will fail to load
        {
            var max = GameInfo.Sources.VersionDataSource.MaxBy(z => z.Value)!;
            var maxVer = (GameVersion)max.Value;
            sav = BlankSaveFile.Get(maxVer, tr, lang);
        }
        return sav;
    }

    private static IEnumerable<SaveFile> GetMostRecentlyLoaded(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (!File.Exists(path))
                continue;

            if (SaveUtil.TryGetSaveFile(path, out var sav))
                yield return sav;
        }
    }
    #endregion
}
