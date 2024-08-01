using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core;

/// <summary>
/// Extension methods for <see cref="SaveFile"/> syntax sugar.
/// </summary>
public static class SaveExtensions
{
    /// <summary>
    /// Evaluates a <see cref="PKM"/> file for compatibility to the <see cref="SaveFile"/>.
    /// </summary>
    /// <param name="sav"><see cref="SaveFile"/> that is being checked.</param>
    /// <param name="pk"><see cref="PKM"/> that is being tested for compatibility.</param>
    public static IReadOnlyList<string> EvaluateCompatibility(this SaveFile sav, PKM pk)
    {
        return sav.GetSaveFileErrata(pk, GameInfo.Strings);
    }

    /// <summary>
    /// Checks a <see cref="PKM"/> file for compatibility to the <see cref="SaveFile"/>.
    /// </summary>
    /// <param name="sav"><see cref="SaveFile"/> that is being checked.</param>
    /// <param name="pk"><see cref="PKM"/> that is being tested for compatibility.</param>
    public static bool IsCompatiblePKM(this SaveFile sav, PKM pk)
    {
        if (sav.PKMType != pk.GetType())
            return false;

        if (sav is ILangDeviantSave il && !EntityConverter.IsCompatibleGB(pk, il.Japanese, pk.Japanese))
            return false;

        return true;
    }

    private static List<string> GetSaveFileErrata(this SaveFile sav, PKM pk, IBasicStrings strings)
    {
        var errata = new List<string>();
        ushort held = (ushort)pk.HeldItem;
        if (sav.Generation > 1 && held != 0)
        {
            string? msg = null;
            if (held > sav.MaxItemID)
                msg = MsgIndexItemGame;
            else if (!pk.CanHoldItem(sav.HeldItems))
                msg = MsgIndexItemHeld;
            if (msg != null)
            {
                var itemstr = GameInfo.Strings.GetItemStrings(pk.Context, pk.Version);
                errata.Add($"{msg} {(held >= itemstr.Length ? held.ToString() : itemstr[held])}");
            }
        }

        if (pk.Species >= strings.Species.Count)
            errata.Add($"{MsgIndexSpeciesRange} {pk.Species}");
        else if (sav.MaxSpeciesID < pk.Species)
            errata.Add($"{MsgIndexSpeciesGame} {strings.Species[pk.Species]}");

        if (!sav.Personal[pk.Species].IsFormWithinRange(pk.Form) && !FormInfo.IsValidOutOfBoundsForm(pk.Species, pk.Form, pk.Generation))
            errata.Add(string.Format(LegalityCheckStrings.LFormInvalidRange, Math.Max(0, sav.Personal[pk.Species].FormCount - 1), pk.Form));

        var movestr = strings.Move;
        for (int i = 0; i < 4; i++)
        {
            var move = pk.GetMove(i);
            if ((uint)move >= movestr.Count)
                errata.Add($"{MsgIndexMoveRange} {move}");
            else if (move > sav.MaxMoveID)
                errata.Add($"{MsgIndexMoveGame} {movestr[move]}");
        }

        if (pk.Ability > strings.Ability.Count)
            errata.Add($"{MsgIndexAbilityRange} {pk.Ability}");
        else if (pk.Ability > sav.MaxAbilityID)
            errata.Add($"{MsgIndexAbilityGame} {strings.Ability[pk.Ability]}");

        return errata;
    }

    /// <summary>
    /// Imports compatible <see cref="PKM"/> data to the <see cref="sav"/>, starting at the provided box.
    /// </summary>
    /// <param name="sav">Save File that will receive the <see cref="compat"/> data.</param>
    /// <param name="compat">Compatible <see cref="PKM"/> data that can be set to the <see cref="sav"/> without conversion.</param>
    /// <param name="overwrite">Overwrite existing full slots. If true, will only overwrite empty slots.</param>
    /// <param name="boxStart">First box to start loading to. All prior boxes are not modified.</param>
    /// <param name="noSetb">Bypass option to not modify <see cref="PKM"/> properties when setting to Save File.</param>
    /// <returns>Count of injected <see cref="PKM"/>.</returns>
    public static int ImportPKMs(this SaveFile sav, IEnumerable<PKM> compat, bool overwrite = false, int boxStart = 0, PKMImportSetting noSetb = PKMImportSetting.UseDefault)
    {
        int startCount = boxStart * sav.BoxSlotCount;
        int maxCount = sav.SlotCount;
        int index = startCount;
        int nonOverwriteImport = 0;

        foreach (var pk in compat)
        {
            if (overwrite)
            {
                while (sav.IsBoxSlotOverwriteProtected(index))
                    ++index;

                // The above will return false if out of range. We need to double-check.
                if (index >= maxCount) // Boxes full!
                    break;

                sav.SetBoxSlotAtIndex(pk, index, noSetb);
            }
            else
            {
                index = sav.NextOpenBoxSlot(index-1);
                if (index < 0) // Boxes full!
                    break;

                sav.SetBoxSlotAtIndex(pk, index, noSetb);
                nonOverwriteImport++;
            }

            if (++index == maxCount) // Boxes full!
                break;
        }
        return overwrite ? index - startCount : nonOverwriteImport; // actual imported count
    }

    public static IEnumerable<PKM> GetCompatible(this SaveFile sav, IEnumerable<PKM> pks)
    {
        var savtype = sav.PKMType;

        foreach (var temp in pks)
        {
            var pk = EntityConverter.ConvertToType(temp, savtype, out var c);
            if (pk == null)
            {
                Debug.WriteLine(c.GetDisplayString(temp, savtype));
                continue;
            }

            if (sav is ILangDeviantSave il && !EntityConverter.IsCompatibleGB(temp, il.Japanese, pk.Japanese))
            {
                var str = EntityConverterResult.IncompatibleLanguageGB.GetIncompatibleGBMessage(pk, il.Japanese);
                Debug.WriteLine(str);
                continue;
            }

            var compat = sav.EvaluateCompatibility(pk);
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
    public static PKM GetCompatiblePKM(this SaveFile sav, PKM pk)
    {
        if (pk.Format >= 3 || sav.Generation >= 7)
            return EntityConverter.ConvertToType(pk, sav.PKMType, out _) ?? sav.BlankPKM;
        // Gen1/2 compatibility check
        if (pk.Japanese != ((ILangDeviantSave)sav).Japanese)
            return sav.BlankPKM;
        if (sav is SAV2 s2 && s2.Korean != pk.Korean)
            return sav.BlankPKM;
        return EntityConverter.ConvertToType(pk, sav.PKMType, out _) ?? sav.BlankPKM;
    }

    /// <summary>
    /// Gets a blank file for the save file. Adapts it to the save file.
    /// </summary>
    /// <param name="sav">Save File to fetch a template for</param>
    /// <returns>Template if it exists, or a blank <see cref="PKM"/> from the <see cref="sav"/></returns>
    private static PKM LoadTemplateInternal(this SaveFile sav)
    {
        var pk = sav.BlankPKM;
        EntityTemplates.TemplateFields(pk, sav);
        return pk;
    }

    /// <summary>
    /// Gets a blank file for the save file. If the template path exists, a template load will be attempted.
    /// </summary>
    /// <param name="sav">Save File to fetch a template for</param>
    /// <param name="templatePath">Path to look for a template in</param>
    /// <returns>Template if it exists, or a blank <see cref="PKM"/> from the <see cref="sav"/></returns>
    public static PKM LoadTemplate(this SaveFile sav, string? templatePath = null)
    {
        if (!Directory.Exists(templatePath))
            return LoadTemplateInternal(sav);

        var di = new DirectoryInfo(templatePath);
        string path = Path.Combine(templatePath, $"{di.Name}.{sav.PKMType.Name.ToLowerInvariant()}");

        if (!File.Exists(path))
            return LoadTemplateInternal(sav);
        var fi = new FileInfo(path);
        if (!EntityDetection.IsSizePlausible(fi.Length))
            return LoadTemplateInternal(sav);

        var data = File.ReadAllBytes(path);
        var prefer = EntityFileExtension.GetContextFromExtension(fi.Extension, sav.Context);
        var pk = EntityFormat.GetFromBytes(data, prefer);
        if (pk?.Species is not > 0)
            return LoadTemplateInternal(sav);

        return EntityConverter.ConvertToType(pk, sav.BlankPKM.GetType(), out _) ?? LoadTemplateInternal(sav);
    }
}
