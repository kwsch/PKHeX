using System;
using System.Collections.Generic;
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
        public static bool dumpBoxes(this SaveFile SAV, string path, out string result, bool boxFolders = false)
        {
            PKM[] boxdata = SAV.BoxData;
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
                    boxfolder = SAV.getBoxName(pk.Box);
                    Directory.CreateDirectory(Path.Combine(path, boxfolder));
                }
                if (!File.Exists(Path.Combine(Path.Combine(path, boxfolder), fileName)))
                    File.WriteAllBytes(Path.Combine(Path.Combine(path, boxfolder), fileName), pk.DecryptedBoxData);
            }

            result = $"Dumped Boxes ({ctr} pkm) to path:\n" + path;
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
        /// <returns></returns>
        public static bool loadBoxes(this SaveFile SAV, string path, out string result, int boxStart = 0, bool boxClear = false, bool? noSetb = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            { result = "Invalid path specified."; return false; }
            if (!SAV.HasBox)
            { result = "Save file does not have boxes."; return false; }
            
            if (boxClear)
                SAV.resetBoxes(boxStart);

            int startCount = boxStart*SAV.BoxSlotCount;
            int maxCount = SAV.BoxCount*SAV.BoxSlotCount;
            int ctr = startCount;
            int pastctr = 0;
            string[] filepaths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in filepaths)
            {
                if (!PKX.getIsPKM(new FileInfo(file).Length))
                    continue;

                // Check for format compatibility with save; if transfer is necessary => convert.
                string c; // format conversion comment
                byte[] data = File.ReadAllBytes(file);
                PKM temp = PKMConverter.getPKMfromBytes(data, prefer: SAV.Generation);
                PKM pk = PKMConverter.convertToFormat(temp, SAV.PKMType, out c);

                if (pk == null)
                { Console.WriteLine(c); continue; }

                if (SAV.IsPKMCompatible(pk).Length > 0)
                    continue;

                while (SAV.getIsSlotLocked(ctr/SAV.BoxSlotCount, ctr%SAV.BoxSlotCount))
                    ctr++;

                int offset = SAV.getBoxOffset(ctr/SAV.BoxSlotCount) + ctr%SAV.BoxSlotCount*SAV.SIZE_STORED;
                SAV.setStoredSlot(pk, offset, noSetb);
                if (pk.Format != temp.Format) // Transferred
                    pastctr++;
                if (++ctr == maxCount) // Boxes full!
                    break;
            }

            ctr -= startCount; // actual imported count
            if (ctr <= 0)
            { result = "No files loaded"; return false; }

            result = $"Loaded {ctr} files to boxes.";
            if (pastctr > 0)
                result += Environment.NewLine + $"Conversion successful for {pastctr} past generation files.";

            return true;
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
                errata.Add($"Species Index beyond range: {pkm.HeldItem}");
            else if (SAV.MaxSpeciesID < pkm.Species)
                errata.Add($"Game can't obtain species: {GameInfo.Strings.specieslist[pkm.Species]}");

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
    }
}
