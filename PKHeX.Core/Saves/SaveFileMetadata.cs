using System;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    public class SaveFileMetadata
    {
        private readonly SaveFile SAV;

        public string? FileName;
        public string? FilePath;
        public string? FileFolder;
        private byte[] Footer = Array.Empty<byte>(); // .dsv
        private byte[] Header = Array.Empty<byte>(); // .gci

        public string BAKName => $"{FileName} [{SAV.ShortSummary}].bak";

        public SaveFileMetadata(SaveFile sav) => SAV = sav;

        public bool HasHeader => Header.Length != 0;
        public bool HasFooter => Footer.Length != 0;
        public string Filter => $"{SAV.GetType().Name}|{GetSuggestedExtension()}|All Files|*.*";

        public byte[] Finalize(byte[] data, ExportFlags flags)
        {
            if (Footer.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeFooter))
                return data.Concat(Footer).ToArray();
            if (Header.Length > 0 && flags.HasFlagFast(ExportFlags.IncludeHeader))
                return Header.Concat(data).ToArray();
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
            if (!sav.State.Exportable) // Blank save file
            {
                sav.Metadata.FileFolder = sav.Metadata.FilePath = string.Empty;
                sav.Metadata.FileName = "Blank Save File";
                return;
            }

            sav.Metadata.FilePath = path;
            sav.Metadata.FileFolder = Path.GetDirectoryName(path);
            sav.Metadata.FileName = string.Empty;
            var bakName = Util.CleanFileName(sav.Metadata.BAKName);
            sav.Metadata.FileName = Path.GetFileName(path);
            if (sav.Metadata.FileName?.EndsWith(bakName) == true)
                sav.Metadata.FileName = sav.Metadata.FileName.Substring(0, sav.Metadata.FileName.Length - bakName.Length);
        }

        public string GetSuggestedExtension()
        {
            var sav = SAV;
            var fn = sav.Metadata.FileName;
            if (fn != null)
                return Path.GetExtension(fn);

            if ((sav.Generation == 4 || sav.Generation == 5) && sav.Metadata.HasFooter)
                return ".dsv";
            return sav.Extension;
        }
    }
}