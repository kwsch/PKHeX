using System;
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
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxFolders">Option to save in child folders with the Box Name as the folder name.</param>
        /// <returns></returns>
        public static bool DumpBoxes(this SaveFile SAV, string path, out string result, bool boxFolders = false)
        {
            var boxdata = SAV.BoxData;
            if (boxdata == null)
            { result = MsgSaveBoxExportInvalid; return false; }

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

            result = string.Format(MsgSaveBoxExportPathCount, ctr) + Environment.NewLine + path;
            return true;
        }

        /// <summary>
        /// Dumps the <see cref="SaveFile.BoxData"/> to a folder with individual decrypted files.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being dumped from.</param>
        /// <param name="path">Folder to store <see cref="PKM"/> files.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="currentBox">Box contents to be dumped.</param>
        /// <returns></returns>
        public static bool DumpBox(this SaveFile SAV, string path, out string result, int currentBox)
        {
            var boxdata = SAV.BoxData;
            if (boxdata == null)
            { result = MsgSaveBoxExportInvalid; return false; }

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

            result = string.Format(MsgSaveBoxExportPathCount, ctr) + Environment.NewLine + path;
            return true;
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="path">Folder to load <see cref="PKM"/> files from. Files are only loaded from the top directory.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <param name="all">Enumerate all files even in sub-folders.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, string path, out string result, int boxStart = 0, bool boxClear = false, bool? noSetb = null, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            { result = MsgSaveBoxExportPathInvalid; return false; }

            var opt = all ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var filepaths = Directory.EnumerateFiles(path, "*.*", opt);
            return SAV.LoadBoxes(filepaths, out result, boxStart, boxClear, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="filepaths">Files to load <see cref="PKM"/> files from.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<string> filepaths, out string result, int boxStart = 0, bool boxClear = false, bool? noSetb = null)
        {
            int generation = SAV.Generation;
            var pks = GetPKMsFromPaths(filepaths, generation);
            return SAV.LoadBoxes(pks, out result, boxStart, boxClear, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="gifts">Gifts to load <see cref="PKM"/> files from.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<MysteryGift> gifts, out string result, int boxStart = 0, bool boxClear = false, bool? noSetb = null)
        {
            var pks = gifts.Select(z => z.ConvertToPKM(SAV));
            return SAV.LoadBoxes(pks, out result, boxStart, boxClear, noSetb);
        }

        /// <summary>
        /// Loads a folder of files to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> to load folder to.</param>
        /// <param name="pks">Unconverted <see cref="PKM"/> objects to load.</param>
        /// <param name="result">Result message from the method.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="boxClear">Instruction to clear boxes after the starting box.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>True if any files are imported.</returns>
        public static bool LoadBoxes(this SaveFile SAV, IEnumerable<PKM> pks, out string result, int boxStart = 0, bool boxClear = false, bool? noSetb = null)
        {
            if (!SAV.HasBox)
            { result = MsgSaveBoxFailNone; return false; }

            var compat = SAV.GetCompatible(pks);
            if (boxClear)
                SAV.ClearBoxes(boxStart);

            int ctr = SAV.ImportPKMs(compat, boxStart, noSetb);
            if (ctr <= 0)
            {
                result = MsgSaveBoxImportNoFiles;
                return false;
            }

            result = string.Format(MsgSaveBoxImportSuccess, ctr);
            return true;
        }

        private static IEnumerable<PKM> GetPKMsFromPaths(IEnumerable<string> filepaths, int generation)
        {
            return filepaths
                .Where(file => PKX.IsPKM(new FileInfo(file).Length))
                .Select(File.ReadAllBytes)
                .Select(data => PKMConverter.GetPKMfromBytes(data, prefer: generation))
                .Where(temp => temp != null);
        }
    }
}
