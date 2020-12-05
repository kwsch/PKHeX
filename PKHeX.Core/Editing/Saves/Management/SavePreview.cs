using System.Collections.Generic;
using System.IO;

namespace PKHeX.Core
{
    /// <summary>
    /// Bindable Summary of a <see cref="SaveFile"/> for use in a sortable table.
    /// </summary>
    public sealed class SavePreview
    {
        public readonly SaveFile Save;
        public readonly string FilePath;

        public SavePreview(SaveFile sav, string parent, string path)
        {
            Save = sav;
            Folder = parent;
            FilePath = path;
        }

        public SavePreview(SaveFile sav, List<INamedFolderPath> paths)
        {
            var meta = sav.Metadata;
            var dir = meta.FileFolder;
            const string notFound = "???";
            var parent = dir == null ? notFound : paths.Find(z => dir.StartsWith(z.Path))?.DisplayText ?? notFound;

            Save = sav;
            Folder = parent;
            FilePath = meta.FilePath ?? notFound;
        }

        public string OT => Save.OT;
        public int G => Save.Generation;
        public GameVersion Game => Save.Version;

        public string Played => Save.PlayTimeString.PadLeft(9, '0');
        public string FileTime => File.GetLastWriteTimeUtc(FilePath).ToString("yyyy.MM.dd:hh:mm:ss");

        public string TID => Save.Generation >= 7 ? Save.TrainerID7.ToString("000000") : Save.TID.ToString("00000");
        public string SID => Save.Generation >= 7 ? Save.TrainerSID7.ToString("0000") : Save.SID.ToString("00000");

        // ReSharper disable once MemberCanBePrivate.Local
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string Folder { get; }

        public string Name => Path.GetFileName(FilePath);
    }
}
