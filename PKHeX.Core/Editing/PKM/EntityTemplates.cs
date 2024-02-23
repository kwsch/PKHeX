namespace PKHeX.Core;

/// <summary>
/// Logic for filling in template data for <see cref="PKM"/> objects.
/// </summary>
public static class EntityTemplates
{
    /// <summary>
    /// Applies junk data to a <see cref="SaveFile.BlankPKM"/>, which is preferable to a completely empty entity.
    /// </summary>
    /// <param name="pk">Blank data</param>
    /// <param name="tr">Trainer info to apply</param>
    public static void TemplateFields(PKM pk, ITrainerInfo tr)
    {
        pk.Move1 = (int)Move.Pound;
        pk.HealPP();
        pk.Ball = 4;
        if (pk.Format >= 4)
            pk.MetDate = EncounterDate.GetDate(pk.Context.GetConsole());

        pk.Version = GetTemplateVersion(tr);
        pk.Species = GetTemplateSpecies(pk, tr);
        pk.Language = GetTemplateLanguage(tr);
        pk.Gender = pk.GetSaneGender();

        pk.ClearNickname();

        pk.OriginalTrainerName = tr.OT;
        pk.OriginalTrainerGender = tr.Gender;
        pk.ID32 = tr.ID32;
        if (tr is IRegionOrigin o && pk is IRegionOrigin gt)
        {
            gt.ConsoleRegion = o.ConsoleRegion;
            gt.Country = o.Country;
            gt.Region = o.Region;
        }

        ApplyTrashBytes(pk, tr);
        pk.RefreshChecksum();
    }

    private static GameVersion GetTemplateVersion(ITrainerInfo tr)
    {
        var version = tr.Version;
        if (version.IsValidSavedVersion())
            return version;
        version = version.GetSingleVersion();
        if (version.IsValidSavedVersion())
            return version;
        return default; // 0
    }

    private static void ApplyTrashBytes(PKM pk, ITrainerInfo tr)
    {
        // Copy OT trash bytes for sensitive games (Gen1/2)
        if (pk is not GBPKM pk12)
            return;
        switch (tr)
        {
            case SAV1 s1:
                s1.OriginalTrainerTrash.CopyTo(pk12.OriginalTrainerTrash);
                break;
            case SAV2 s2:
                s2.OriginalTrainerTrash.CopyTo(pk12.OriginalTrainerTrash);
                break;
        }
    }

    private static ushort GetTemplateSpecies(PKM pk, ITrainerInfo tr)
    {
        ushort species = tr is IGameValueLimit s ? s.MaxSpeciesID : pk.Version.GetMaxSpeciesID();
        if (species == 0)
            species = pk.MaxSpeciesID;
        return species;
    }

    private static int GetTemplateLanguage(ITrainerInfo tr)
    {
        var lang = tr.Language;
        if (lang <= 0)
            lang = (int)LanguageID.English;
        return lang;
    }
}
