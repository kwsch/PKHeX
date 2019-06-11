using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public static class SaveExtensions
    {
        /// <summary>
        /// Checks if the <see cref="PKM"/> is compatible with the input <see cref="SaveFile"/>, and makes any necessary modifications to force compatibility.
        /// </summary>
        /// <remarks>Should only be used when forcing a backwards conversion to sanitize the PKM fields to the target format.
        /// If the PKM is compatible, some properties may be forced to sanitized values.</remarks>
        /// <param name="SAV">Save File target that the PKM will be injected.</param>
        /// <param name="pk">PKM input that is to be injected into the Save File.</param>
        /// <returns>Indication whether or not the PKM is compatible.</returns>
        public static bool IsPKMCompatibleWithModifications(this SaveFile SAV, PKM pk) => PKMConverter.IsPKMCompatibleWithModifications(pk, SAV);

        /// <summary>
        /// Sets the details of a path to a <see cref="SaveFile"/> object.
        /// </summary>
        /// <param name="sav">Save File to set path details to.</param>
        /// <param name="path">Full Path of the file</param>
        public static void SetFileInfo(this SaveFile sav, string path)
        {
            if (!sav.Exportable) // Blank save file
            {
                sav.FileFolder = sav.FilePath = string.Empty;
                sav.FileName = "Blank Save File";
                return;
            }

            sav.FilePath = path;
            sav.FileFolder = Path.GetDirectoryName(path);
            sav.FileName = string.Empty;
            var bakName = Util.CleanFileName(sav.BAKName);
            sav.FileName = Path.GetFileName(path);
            if (sav.FileName?.EndsWith(bakName) == true)
                sav.FileName = sav.FileName.Substring(0, sav.FileName.Length - bakName.Length);
        }

        /// <summary>
        /// Gets suggested export options for the savefile.
        /// </summary>
        /// <param name="sav"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static ExportFlags GetSuggestedFlags(this SaveFile sav, string ext)
        {
            var flags = ExportFlags.None;
            if (ext == ".dsv")
                flags |= ExportFlags.IncludeFooter;
            if (ext == ".gci" || (sav is IGCSaveFile gc && !gc.IsMemoryCardSave))
                flags |= ExportFlags.IncludeHeader;
            return flags;
        }

        /// <summary>
        /// Checks a <see cref="PKM"/> file for compatibility to the <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="SAV"><see cref="SaveFile"/> that is being checked.</param>
        /// <param name="pkm"><see cref="PKM"/> that is being tested for compatibility.</param>
        /// <returns></returns>
        public static IReadOnlyList<string> IsPKMCompatible(this SaveFile SAV, PKM pkm)
        {
            IBasicStrings strings = GameInfo.Strings;
            return SAV.GetSaveFileErrata(pkm, strings);
        }

        private static IReadOnlyList<string> GetSaveFileErrata(this SaveFile SAV, PKM pkm, IBasicStrings strings)
        {
            var errata = new List<string>();
            if (SAV.Generation > 1)
            {
                ushort held = (ushort)pkm.HeldItem;
                var itemstr = GameInfo.Strings.GetItemStrings(pkm.Format, (GameVersion) pkm.Version);
                if (held > itemstr.Count)
                    errata.Add($"{MsgIndexItemRange} {held}");
                else if (held > SAV.MaxItemID)
                    errata.Add($"{MsgIndexItemGame} {itemstr[held]}");
                else if (!pkm.CanHoldItem(SAV.HeldItems))
                    errata.Add($"{MsgIndexItemHeld} {itemstr[held]}");
            }

            if (pkm.Species > strings.Species.Count)
                errata.Add($"{MsgIndexSpeciesRange} {pkm.Species}");
            else if (SAV.MaxSpeciesID < pkm.Species)
                errata.Add($"{MsgIndexSpeciesGame} {strings.Species[pkm.Species]}");

            if (!SAV.Personal[pkm.Species].IsFormeWithinRange(pkm.AltForm) && !FormConverter.IsValidOutOfBoundsForme(pkm.Species, pkm.AltForm, pkm.GenNumber))
                errata.Add(string.Format(LegalityCheckStrings.LFormInvalidRange, Math.Max(0, SAV.Personal[pkm.Species].FormeCount - 1), pkm.AltForm));

            if (pkm.Moves.Any(m => m > strings.Move.Count))
                errata.Add($"{MsgIndexMoveRange} {string.Join(", ", pkm.Moves.Where(m => m > strings.Move.Count).Select(m => m.ToString()))}");
            else if (pkm.Moves.Any(m => m > SAV.MaxMoveID))
                errata.Add($"{MsgIndexMoveGame} {string.Join(", ", pkm.Moves.Where(m => m > SAV.MaxMoveID).Select(m => strings.Move[m]))}");

            if (pkm.Ability > strings.Ability.Count)
                errata.Add($"{MsgIndexAbilityRange} {pkm.Ability}");
            else if (pkm.Ability > SAV.MaxAbilityID)
                errata.Add($"{MsgIndexAbilityGame} {strings.Ability[pkm.Ability]}");

            return errata;
        }

        /// <summary>
        /// Imports compatible <see cref="PKM"/> data to the <see cref="SAV"/>, starting at the provided box.
        /// </summary>
        /// <param name="SAV">Save File that will receive the <see cref="compat"/> data.</param>
        /// <param name="compat">Compatible <see cref="PKM"/> data that can be set to the <see cref="SAV"/> without conversion.</param>
        /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
        /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
        /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
        /// <returns>Count of injected <see cref="PKM"/>.</returns>
        public static int ImportPKMs(this SaveFile SAV, IEnumerable<PKM> compat, bool overwrite = false, int boxStart = 0, PKMImportSetting noSetb = PKMImportSetting.UseDefault)
        {
            int startCount = boxStart * SAV.BoxSlotCount;
            int maxCount = SAV.SlotCount;
            int index = startCount;
            int nonOverwriteImport = 0;

            foreach (var pk in compat)
            {
                if (overwrite)
                {
                    while (SAV.IsSlotOverwriteProtected(index))
                        ++index;

                    SAV.SetBoxSlotAtIndex(pk, index, noSetb);
                }
                else
                {
                    index = SAV.NextOpenBoxSlot(index-1);
                    if (index < 0) // Boxes full!
                        break;

                    SAV.SetBoxSlotAtIndex(pk, index, noSetb);
                    nonOverwriteImport++;
                }

                if (++index == maxCount) // Boxes full!
                    break;
            }
            return (overwrite) ? index - startCount : nonOverwriteImport; // actual imported count
        }

        public static IEnumerable<PKM> GetCompatible(this SaveFile SAV, IEnumerable<PKM> pks)
        {
            var savtype = SAV.PKMType;
            foreach (var temp in pks)
            {
                var pk = PKMConverter.ConvertToType(temp, savtype, out string c);
                if (pk == null)
                {
                    Debug.WriteLine(c);
                    continue;
                }

                if (PKMConverter.IsIncompatibleGB(pk.Format, SAV.Japanese, pk.Japanese))
                {
                    c = PKMConverter.GetIncompatibleGBMessage(pk, SAV.Japanese);
                    Debug.WriteLine(c);
                    continue;
                }

                var compat = SAV.IsPKMCompatible(pk);
                if (compat.Count > 0)
                    continue;

                yield return pk;
            }
        }

        /// <summary>
        /// Gets a compatible <see cref="PKM"/> for editing with a new <see cref="SaveFile"/>.
        /// </summary>
        /// <param name="sav">SaveFile to receive the compatible <see cref="pk"/></param>
        /// <param name="pk">Current Pokémon being edited</param>
        /// <returns>Current Pokémon, assuming conversion is possible. If conversion is not possible, a blank <see cref="PKM"/> will be obtained from the <see cref="sav"/>.</returns>
        public static PKM GetCompatiblePKM(this SaveFile sav, PKM pk = null)
        {
            if (pk == null)
                return sav.BlankPKM;
            if (pk.Format < 3 && sav.Generation < 7)
            {
                // gen1-2 compatibility check
                if (pk.Japanese != sav.Japanese)
                    return sav.BlankPKM;
                if (sav is SAV2 s2 && s2.Korean != pk.Korean)
                    return sav.BlankPKM;
            }
            return PKMConverter.ConvertToType(pk, sav.PKMType, out _) ?? sav.BlankPKM;
        }

        /// <summary>
        /// Gets a blank file for the save file. If the template path exists, a template load will be attempted.
        /// </summary>
        /// <param name="sav">Save File to fetch a template for</param>
        /// <param name="templatePath">Path to look for a template in</param>
        /// <returns>Template if it exists, or a blank <see cref="PKM"/> from the <see cref="sav"/></returns>
        public static PKM LoadTemplate(this SaveFile sav, string templatePath = null)
        {
            var blank = sav.BlankPKM;
            if (!Directory.Exists(templatePath))
                return blank;

            var di = new DirectoryInfo(templatePath);
            string path = Path.Combine(templatePath, $"{di.Name}.{blank.Extension}");

            if (!File.Exists(path) || !PKX.IsPKM(new FileInfo(path).Length))
                return blank;

            var pk = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(path), prefer: blank.Format);
            return PKMConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out path) ?? blank; // no sneaky plz; reuse string
        }
    }
}
