using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Contains extension methods for use with a <see cref="SaveFile"/>.
    /// </summary>
    public static class SAVUtil
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
            { result = "Invalid Box Data, unable to dump."; return false; }

            int ctr = 0;
            foreach (PKM pk in boxdata)
            {
                if (pk.Species == 0 || !pk.Valid)
                    continue;

                ctr++;
                string fileName = Util.CleanFileName(pk.FileName);
                string boxfolder = "";
                if (boxFolders)
                {
                    boxfolder = SAV.GetBoxName(pk.Box - 1);
                    Directory.CreateDirectory(Path.Combine(path, boxfolder));
                }
                if (!File.Exists(Path.Combine(Path.Combine(path, boxfolder), fileName)))
                    File.WriteAllBytes(Path.Combine(Path.Combine(path, boxfolder), fileName), pk.DecryptedBoxData);
            }

            result = $"Dumped Boxes ({ctr} pkm) to path:" + Environment.NewLine + path;
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
            { result = "Invalid Box Data, unable to dump."; return false; }

            int ctr = 0;
            foreach (PKM pk in boxdata)
            {
                if (pk.Species == 0 || !pk.Valid || (pk.Box - 1) != currentBox)
                    continue;

                ctr++;
                string fileName = Util.CleanFileName(pk.FileName);
                if (!File.Exists(Path.Combine(path, fileName)))
                    File.WriteAllBytes(Path.Combine(path, fileName), pk.DecryptedBoxData);
            }

            result = $"Dumped Box ({ctr} pkm) to path:" + Environment.NewLine + path;
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
            { result = "Invalid path specified."; return false; }

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
            { result = "Save file does not have boxes."; return false; }

            var compat = GetPKMForSaveFile(SAV, pks);
            if (boxClear)
                SAV.ClearBoxes(boxStart);

            int ctr = SAV.ImportPKMs(compat, boxStart, noSetb);
            if (ctr <= 0)
            {
                result = "No files loaded.";
                return false;
            }

            result = $"Loaded {ctr} files to boxes.";
            return true;
        }

        private static int ImportPKMs(this SaveFile SAV, IEnumerable<PKM> compat, int boxStart, bool? noSetb)
        {
            int startCount = boxStart * SAV.BoxSlotCount;
            int maxCount = SAV.BoxCount * SAV.BoxSlotCount;
            int i = startCount;
            int getbox() => i / SAV.BoxSlotCount;
            int getslot() => i % SAV.BoxSlotCount;

            foreach (var pk in compat)
            {
                int box = getbox();
                int slot = getslot();
                while (SAV.IsSlotLocked(box, slot))
                {
                    ++i;
                    box = getbox();
                    slot = getslot();
                }

                int offset = SAV.GetBoxOffset(box) + slot * SAV.SIZE_STORED;
                SAV.SetStoredSlot(pk, offset, noSetb);

                if (++i == maxCount) // Boxes full!
                    break;
            }
            i -= startCount; // actual imported count
            return i;
        }
        private static IEnumerable<PKM> GetPKMsFromPaths(IEnumerable<string> filepaths, int generation)
        {
            return filepaths
                .Where(file => PKX.IsPKM(new FileInfo(file).Length))
                .Select(File.ReadAllBytes)
                .Select(data => PKMConverter.GetPKMfromBytes(data, prefer: generation))
                .Where(temp => temp != null);
        }
        private static IEnumerable<PKM> GetPKMForSaveFile(this SaveFile SAV, IEnumerable<PKM> pks)
        {
            var savtype = SAV.PKMType;
            foreach (var temp in pks)
            {
                PKM pk = PKMConverter.ConvertToType(temp, savtype, out string c);

                if (pk == null)
                { Debug.WriteLine(c); continue; }

                var compat = SAV.IsPKMCompatible(pk);
                if (compat.Length > 0)
                    continue;

                yield return pk;
            }
        }

        /// <summary>
        /// Checks a <see cref="PKM"/> file for compatibility to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being checked.</param>
        /// <param name="pkm"><see cref="PKM"/> that is being tested for compatibility.</param>
        /// <returns></returns>
        public static string[] IsPKMCompatible(this SaveFile SAV, PKM pkm)
        {
            // Check if PKM properties are outside of the valid range
            List<string> errata = new List<string>();
            if (SAV.Generation > 1)
            {
                ushort held = (ushort)pkm.HeldItem;

                if (held > GameInfo.Strings.itemlist.Length)
                    errata.Add($"Item Index beyond range: {held}");
                else if (held > SAV.MaxItemID)
                    errata.Add($"Game can't obtain item: {GameInfo.Strings.itemlist[held]}");
                else if (!pkm.CanHoldItem(SAV.HeldItems))
                    errata.Add($"Game can't hold item: {GameInfo.Strings.itemlist[held]}");
            }

            if (pkm.Species > GameInfo.Strings.specieslist.Length)
                errata.Add($"Species Index beyond range: {pkm.Species}");
            else if (SAV.MaxSpeciesID < pkm.Species)
                errata.Add($"Game can't obtain species: {GameInfo.Strings.specieslist[pkm.Species]}");

            if (!SAV.Personal[pkm.Species].IsFormeWithinRange(pkm.AltForm) && !FormConverter.IsValidOutOfBoundsForme(pkm.Species, pkm.AltForm, pkm.GenNumber))
                errata.Add(string.Format(LegalityCheckStrings.V304, Math.Max(0, SAV.Personal[pkm.Species].FormeCount - 1), pkm.AltForm));

            if (pkm.Moves.Any(m => m > GameInfo.Strings.movelist.Length))
                errata.Add($"Item Index beyond range: {string.Join(", ", pkm.Moves.Where(m => m > GameInfo.Strings.movelist.Length).Select(m => m.ToString()))}");
            else if (pkm.Moves.Any(m => m > SAV.MaxMoveID))
                errata.Add($"Game can't have move: {string.Join(", ", pkm.Moves.Where(m => m > SAV.MaxMoveID).Select(m => GameInfo.Strings.movelist[m]))}");

            if (pkm.Ability > GameInfo.Strings.abilitylist.Length)
                errata.Add($"Ability Index beyond range: {pkm.Ability}");
            else if (pkm.Ability > SAV.MaxAbilityID)
                errata.Add($"Game can't have ability: {GameInfo.Strings.abilitylist[pkm.Ability]}");

            return errata.ToArray();
        }

        /// <summary>
        /// Removes the <see cref="PKM.HeldItem"/> for all <see cref="PKM"/> in the <see cref="SaveFile.BoxData"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being operated on.</param>
        /// <param name="item"><see cref="PKM.HeldItem"/> to set. If no argument is supplied, the held item will be removed.</param>
        public static void SetBoxDataAllHeldItems(this SaveFile SAV, int item = 0)
        {
            var boxdata = SAV.BoxData;
            foreach (PKM pk in boxdata)
            {
                pk.HeldItem = item;
                pk.RefreshChecksum();
            }
            SAV.BoxData = boxdata;
        }
    }
}
