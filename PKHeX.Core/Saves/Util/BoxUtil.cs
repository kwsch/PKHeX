using System.Collections.Generic;
using System.IO;
using System.Linq;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Contains extension methods for use with a <see cref="SaveFile"/>.
    /// </summary>
    public static class BoxUtil
    {
        /// <summary>
        /// Dumps a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being dumped from.</param>
        /// <param name="path">Folder to store <see cref="PKM"/> files.</param>
        /// <param name="boxFolders">Option to save in child folders with the Box Name as the folder name.</param>
        /// <returns>-1 if aborted, otherwise the amount of files dumped.</returns>
        public static int DumpBoxes(this SaveFile SAV, string path, bool boxFolders = false)
        {
            if (!SAV.HasBox)
                return -1;

            var boxdata = SAV.BoxData;
            int ctr = 0;
            foreach (PKM pk in boxdata)
            {
                if (pk.Species == 0 || !pk.Valid)
                    continue;

                ctr++;
                string fileName = Util.CleanFileName(pk.FileName);
                string boxfolder = string.Empty;
                if (boxFolders)
                {
                    boxfolder = SAV.GetBoxName(pk.Box - 1);
                    Directory.CreateDirectory(Path.Combine(path, boxfolder));
                }
                if (!File.Exists(Path.Combine(Path.Combine(path, boxfolder), fileName)))
                    File.WriteAllBytes(Path.Combine(Path.Combine(path, boxfolder), fileName), pk.DecryptedBoxData);
            }
            return ctr;
        }

        /// <summary>
        /// Dumps the <see cref="SaveFile.BoxData"/> to a folder with individual decrypted files.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being dumped from.</param>
        /// <param name="path">Folder to store <see cref="PKM"/> files.</param>
        /// <param name="currentBox">Box contents to be dumped.</param>
        /// <returns>-1 if aborted, otherwise the amount of files dumped.</returns>
        public static int DumpBox(this SaveFile SAV, string path, int currentBox)
        {
            if (!SAV.HasBox)
                return -1;

            var boxdata = SAV.BoxData;
            int ctr = 0;
            foreach (PKM pk in boxdata)
            {
                if (pk.Species == 0 || !pk.Valid || pk.Box - 1 != currentBox)
                    continue;

                ctr++;
                string fileName = Util.CleanFileName(pk.FileName);
                if (!File.Exists(Path.Combine(path, fileName)))
                    File.WriteAllBytes(Path.Combine(path, fileName), pk.DecryptedBoxData);
            }
            return ctr;
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="path">Folder to load <see cref="PKM"/> files from. Files are only loaded from the top directory.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <param name="all">Enumerate all files even in sub-folders.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, string path, out string result, int boxStart = 0, bool boxClear = false, bool overwrite = false, bool? noSetb = null, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            { result = MsgSaveBoxExportPathInvalid; return false; }

            var opt = all ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var filepaths = Directory.EnumerateFiles(path, "*.*", opt);
            return SAV.LoadBoxes(filepaths, out result, boxStart, boxClear, overwrite, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="filepaths">Files to load <see cref="PKM"/> files from.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<string> filepaths, out string result, int boxStart = 0, bool boxClear = false, bool overwrite = false, bool? noSetb = null)
        {
            var pks = GetPossiblePKMsFromPaths(SAV, filepaths);
            return SAV.LoadBoxes(pks, out result, boxStart, boxClear, overwrite, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="gifts">Gifts to load <see cref="PKM"/> files from.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<MysteryGift> gifts, out string result, int boxStart = 0, bool boxClear = false, bool overwrite = false, bool? noSetb = null)
        {
            var pks = gifts.Select(z => z.ConvertToPKM(SAV));
            return SAV.LoadBoxes(pks, out result, boxStart, boxClear, overwrite, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="pks">Unconverted <see cref="PKM"/> objects to load.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<PKM> pks, out string result, int boxStart = 0, bool boxClear = false, bool overwrite = false, bool? noSetb = null)
        {
            if (!SAV.HasBox)
            { result = MsgSaveBoxFailNone; return false; }

            var compat = SAV.GetCompatible(pks);
            if (boxClear)
                SAV.ClearBoxes(boxStart);

            int ctr = SAV.ImportPKMs(compat, overwrite, boxStart, noSetb);
            if (ctr <= 0)
            {
                result = MsgSaveBoxImportNoFiles;
                return false;
            }

            result = string.Format(MsgSaveBoxImportSuccess, ctr);
            return true;
        }

        public static IEnumerable<PKM> GetPKMsFromPaths(IEnumerable<string> filepaths, int generation)
        {
            return filepaths
                .Where(file => PKX.IsPKM(new FileInfo(file).Length))
                .Select(File.ReadAllBytes)
                .Select(data => PKMConverter.GetPKMfromBytes(data, prefer: generation))
                .Where(temp => temp != null);
        }

        private static IEnumerable<PKM> GetPossiblePKMsFromPaths(SaveFile sav, IEnumerable<string> filepaths)
        {
            foreach (var f in filepaths)
            {
                var obj = FileUtil.GetSupportedFile(f, sav);
                switch (obj)
                {
                    case PKM pk:
                        yield return pk;
                        break;
                    case MysteryGift g when g.IsPokémon:
                        yield return g.ConvertToPKM(sav);
                        break;
                    case GP1 g when g.Species != 0:
                        yield return g.ConvertToPB7(sav);
                        break;
                }
            }
        }
    }
}
