using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic object that assembles parameters used for starting up an editing environment.
    /// </summary>
    public sealed class StartupArguments
    {
        public PKM? Entity { get; private set; }
        public SaveFile? SAV { get; private set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public Exception? Error { get; }
        // ReSharper disable once CollectionNeverQueried.Global
        public readonly List<object> Extra = new();

        /// <summary>
        /// Step 1: Reads in command line arguments.
        /// </summary>
        public void ReadArguments(IEnumerable<string> args)
        {
            foreach (string path in args)
            {
                var other = FileUtil.GetSupportedFile(path, SAV);
                if (other is SaveFile s)
                {
                    s.Metadata.SetExtraInfo(path);
                    SAV = s;
                }
                else if (other is PKM pkm)
                {
                    Entity = pkm;
                }
                else if (other is not null)
                {
                    Extra.Add(other);
                }
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
            else
                SAV = ReadSettingsAnyPKM(startup) ?? GetBlankSaveFile(startup.DefaultSaveVersion, SAV);
        }

        // step 3
        public void ReadTemplateIfNoEntity(string path)
        {
            if (Entity is not null)
                return;

            var sav = SAV;
            if (sav is null)
                throw new NullReferenceException(nameof(sav));

            var pk = sav.LoadTemplate(path);
            var isBlank = pk.Data.SequenceEqual(sav.BlankPKM.Data);
            if (isBlank)
                EntityTemplates.TemplateFields(pk, sav);

            Entity = pk;
        }

        private static SaveFile? ReadSettingsDefinedPKM(IStartupSettings startup, PKM pkm)
        {
            var opt = startup.AutoLoadSaveOnStartup;
            if (opt is AutoLoadSetting.LastLoaded)
                return GetMostRecentlyLoaded(startup.RecentlyLoaded).FirstOrDefault(z => z.IsCompatiblePKM(pkm));
            if (opt is AutoLoadSetting.RecentBackup)
                return SaveFinder.DetectSaveFiles().FirstOrDefault(z => z.IsCompatiblePKM(pkm));
            return null;
        }

        private static SaveFile? ReadSettingsAnyPKM(IStartupSettings startup) => startup.AutoLoadSaveOnStartup switch
        {
            AutoLoadSetting.RecentBackup => SaveFinder.DetectSaveFiles().FirstOrDefault(),
            AutoLoadSetting.LastLoaded => GetMostRecentlyLoaded(startup.RecentlyLoaded).FirstOrDefault(),
            _ => null,
        };

        #region Utility
        private static SaveFile GetBlank(PKM pk)
        {
            var ver = (GameVersion)pk.Version;
            if (ver.GetGeneration() != pk.Format)
                ver = GameUtil.GetVersion(pk.Format);

            return SaveUtil.GetBlankSAV(ver, pk.OT_Name, (LanguageID)pk.Language);
        }

        private static SaveFile GetBlankSaveFile(GameVersion version, SaveFile? current)
        {
            var lang = SaveUtil.GetSafeLanguage(current);
            var tr = SaveUtil.GetSafeTrainerName(current, lang);
            var sav = SaveUtil.GetBlankSAV(version, tr, lang);
            if (sav.Version == GameVersion.Invalid) // will fail to load
                sav = SaveUtil.GetBlankSAV((GameVersion)GameInfo.VersionDataSource.Max(z => z.Value), tr, lang);
            return sav;
        }

        private static IEnumerable<SaveFile> GetMostRecentlyLoaded(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                if (!File.Exists(path))
                    continue;

                var sav = SaveUtil.GetVariantSAV(path);
                if (sav is null)
                    continue;

                yield return sav;
            }
        }
        #endregion
    }
}
