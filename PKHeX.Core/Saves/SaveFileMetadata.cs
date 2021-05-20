using System;
using System.IO;

namespace PKHeX.Core
{
    /// <summary>
    /// Tracks information about where the <see cref="SAV"/> originated from, and provides logic for saving to a file.
    /// </summary>
    public class SaveFileMetadata
    {
        private readonly SaveFile SAV;

        /// <summary>
        /// Full path where the <see cref="SAV"/> originated from.
        /// </summary>
        public string? FilePath { get; private set; }

        /// <summary>
        /// File Name of the <see cref="SAV"/>.
        /// </summary>
        /// <remarks>This is not always the original file name. We try to strip out the Backup name markings to get the original filename.</remarks>
        public string? FileName { get; private set; }

        /// <summary>
        /// Directory in which the <see cref="SAV"/> was saved in.
        /// </summary>
        public string? FileFolder { get; private set; }

        private byte[] Footer = Array.Empty<byte>(); // .dsv
        private byte[] Header = Array.Empty<byte>(); // .gci

        private string BAKSuffix => $" [{SAV.ShortSummary}].bak";

        /// <summary>
        /// Simple summary of the save file, to help differentiate it from other save files with the same filename.
        /// </summary>
        public string BAKName => FileName + BAKSuffix;

        public SaveFileMetadata(SaveFile sav) => SAV = sav;

        public bool HasHeader => Header.Length != 0;
        public bool HasFooter => Footer.Length != 0;

        /// <summary>
        /// File Dialog filter to help save the file.
        /// </summary>
        public string Filter => $"{SAV.GetType().Name}|{GetSuggestedExtension()}|All Files|*.*";

        /// <summary>
        /// Writes the input <see cref="data"/> and appends the <see cref="Header"/> and <see cref="Footer"/> if requested.
        /// </summary>
        /// <param name="data">Finalized save file data (with fixed checksums) to be written to a file</param>
        /// <param name="flags">Toggle flags </param>
        /// <returns>Final save file data.</returns>
        public byte[] Finalize(byte[] data, ExportFlags flags)
        {
            if (Footer.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeFooter))
                return ArrayUtil.ConcatAll(data, Footer);
            if (Header.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeHeader))
                return ArrayUtil.ConcatAll(Header, data);
            return data;
        }

        /// <summary>
        /// Sets the details of any trimmed header and footer arrays to a <see cref="SaveFile"/> object.
        /// </summary>
        public void SetExtraInfo(byte[] header, byte[] footer)
        {
            Header = header;
            Footer = footer;
        }

        /// <summary>
        /// Sets the details of a path to a <see cref="SaveFile"/> object.
        /// </summary>
        /// <param name="path">Full Path of the file</param>
        public void SetExtraInfo(string path)
        {
            var sav = SAV;
            if (!sav.State.Exportable || string.IsNullOrWhiteSpace(path)) // Blank save file
            {
                sav.Metadata.SetAsBlank();
                return;
            }

            SetAsLoadedFile(path);
        }

        private void SetAsLoadedFile(string path)
        {
            FilePath = path;
            FileFolder = Path.GetDirectoryName(path);
            FileName = GetFileName(path, BAKSuffix);
        }

        private static string GetFileName(string path, string bak)
        {
            var bakName = Util.CleanFileName(bak);
            var fn = Path.GetFileName(path);
            return fn.EndsWith(bakName) ? fn[..^bakName.Length] : fn;
        }

        private void SetAsBlank()
        {
            FileFolder = FilePath = string.Empty;
            FileName = "Blank Save File";
        }

        /// <summary>
        /// Gets the suggested file extension when writing to a saved file.
        /// </summary>
        public string GetSuggestedExtension()
        {
            var sav = SAV;
            var fn = sav.Metadata.FileName;
            if (fn != null)
                return Path.GetExtension(fn);

            if ((sav.Generation is 4 or 5) && sav.Metadata.HasFooter)
                return ".dsv";
            return sav.Extension;
        }

        /// <summary>
        /// Gets suggested export options for the save file.
        /// </summary>
        /// <param name="ext">Selected export extension</param>
        public ExportFlags GetSuggestedFlags(string? ext = null)
        {
            var flags = ExportFlags.None;
            if (ext == ".dsv")
                flags |= ExportFlags.IncludeFooter;
            if (ext == ".gci" || SAV is IGCSaveFile {IsMemoryCardSave: false})
                flags |= ExportFlags.IncludeHeader;
            return flags;
        }
    }
}
