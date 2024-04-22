using System;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.SV"/>.
/// </summary>>
public sealed class Zukan9Paldea(SAV9SV sav, SCBlock block) : ZukanBase<SAV9SV>(sav, block.Data)
{
    public PokeDexEntry9Paldea Get(ushort species)
    {
        if (species > SAV.MaxSpeciesID)
            throw new ArgumentOutOfRangeException(nameof(species), species, null);

        const int size = PokeDexEntry9Paldea.SIZE;
        var internalSpecies = SpeciesConverter.GetInternal9(species);
        var span = block.Data.AsSpan(internalSpecies * size, size);
        return new PokeDexEntry9Paldea(span);
    }

    public override bool GetSeen(ushort species) => Get(species).IsSeen;
    public override bool GetCaught(ushort species) => Get(species).IsCaught;
    public void SetCaught(ushort species, bool value = true) => Get(species).SetCaught(value);
    public bool GetIsLanguageIndexObtained(ushort species, int langIndex) => Get(species).GetLanguageFlag(langIndex);
    public void SetIsLanguageIndexObtained(ushort species, int langIndex, bool value = true) => Get(species).SetLanguageFlag(langIndex, value);

    public bool GetIsLanguageObtained(ushort species, int language)
    {
        int langIndex = PokeDexEntry9Paldea.GetDexLangFlag(language);
        if (langIndex < 0)
            return false;

        return GetIsLanguageIndexObtained(species, language);
    }

    public void SetIsLanguageObtained(ushort species, int language, bool value = true)
    {
        int langIndex = PokeDexEntry9Paldea.GetDexLangFlag(language);
        if (langIndex < 0)
            return;

        SetIsLanguageIndexObtained(species, language, value);
    }

    public uint GetFormDisplayed(ushort species) => Get(species).GetDisplayForm();
    public void SetFormDisplayed(ushort species, byte form = 0) => Get(species).SetDisplayForm(form);
    public uint GetGenderDisplayed(ushort species) => Get(species).GetDisplayGender();
    public void SetGenderDisplayed(ushort species, int value = 0) => Get(species).SetDisplayGender(value);

    public bool GetDisplayShiny(ushort species) => Get(species).GetDisplayIsShiny();
    public void SetDisplayShiny(ushort species, bool value = true) => Get(species).SetDisplayIsShiny(value);

    #region Inherited
    public override void SetDex(PKM pk)
    {
        if (pk.IsEgg) // do not add
            return;
        var species = pk.Species;
        var form = pk.Form;
        var pt = SAV.Personal;
        if (!pt.IsPresentInGame(species, form))
            return;

        // Don't register bad species-form data for DLC-less saves.
        var pi = pt.GetFormEntry(species, form);
        if (SAV.SaveRevision == 0 && pi.DexGroup > 1)
            return;
        if (SAV.SaveRevision == 1 && pi.DexGroup > 2)
            return;

        Register(pk, species, form);
    }

    public void UpdateAdjacent(ushort species)
    {
        var (group, index) = GetDexIndex(species);
        if (index == 0)
            return;

        MarkAsKnown1(group, index - 1);
        MarkAsKnown1(group, index + 1);
    }

    private void MarkAsKnown1(byte group, int index)
    {
        if (group == 1 && index <= 9) // Don't set adjacent for starters. Hide their evolutions!
            return;

        var species = GetSpecies(group, index);
        if (species == 0)
            return;
        var entry = Get(species);
        if (!entry.IsKnown)
            entry.SetState(1);
    }

    private ushort GetSpecies(byte group, int index)
    {
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            var (g, i) = GetDexIndex(species);
            if (g == group && i == index)
                return species;
        }
        return 0;
    }

    public (byte Group, ushort Index) GetDexIndex(ushort species)
    {
        var pt = SAV.Personal;
        // For each form including form 0, check the dex index.
        var pi = pt.GetFormEntry(species, 0);
        for (byte f = 0; f <= pi.FormCount; f++)
        {
            pi = pt.GetFormEntry(species, f);
            if (pi.DexPaldea != 0)
                return (1, pi.DexPaldea);
        }
        return default;
    }

    public override void SeenNone()
    {
        Array.Clear(block.Data, 0, block.Data.Length);
    }

    public override void CaughtNone()
    {
        for (ushort i = 0; i <= SAV.MaxSpeciesID; i++)
        {
            var entry = Get(i);
            entry.ClearCaught();
        }
    }

    public override void SeenAll(bool shinyToo = false)
    {
        SetAllSeen(true, shinyToo);
    }

    public void SeenAll(ushort species, byte formCount, bool value = true, bool shinyToo = false)
    {
        // Wipe existing gender flags.
        var entry = Get(species);
        entry.SetIsGenderSeen(0, false);
        entry.SetIsGenderSeen(1, false);
        if (value && !entry.IsSeen)
            entry.SetSeen(value);
        if (!value || shinyToo)
            entry.SetSeenIsShiny(value);

        var pt = SAV.Personal;
        for (byte form = 0; form < formCount; form++)
        {
            var pi = pt.GetFormEntry(species, form);
            bool seenForm = value && pi.IsPresentInGame;
            SetIsFormSeen(entry, pi, form, seenForm);
        }
    }

    private static void SetIsFormSeen(PokeDexEntry9Paldea entry, IGenderDetail pi, byte form, bool seenForm)
    {
        entry.SetIsFormSeen(form, seenForm);
        if (seenForm)
            SetIsFormSeenGender(entry, pi);
    }

    private static void SetIsFormSeenGender(PokeDexEntry9Paldea entry, IGenderDetail pi)
    {
        if (pi.IsDualGender)
        {
            entry.SetIsGenderSeen(0, true);
            entry.SetIsGenderSeen(1, true);
        }
        else
        {
            var gender = pi.FixedGender();
            entry.SetIsGenderSeen(gender, true);
        }
    }

    public override void CompleteDex(bool shinyToo = false)
    {
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            if (!SAV.Personal.IsSpeciesInGame(species))
                continue;
            if (GetDexIndex(species).Index == 0)
                continue;
            SetDexEntryAll(species, shinyToo);
        }
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        SeenAll(shinyToo);
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            if (!SAV.Personal.IsSpeciesInGame(species))
                continue;
            if (GetDexIndex(species).Index == 0)
                continue;
            SetAllCaught(species, true, shinyToo);
        }
    }

    public void SetAllCaught(ushort species, bool value = true, bool shinyToo = false)
    {
        SetCaught(species, value);
        for (int i = 1; i <= (int)LanguageID.ChineseT; i++)
            SetIsLanguageObtained(species, i, value);

        if (value)
        {
            var pi = SAV.Personal[species];
            if (shinyToo)
                SetDisplayShiny(species);

            SetGenderDisplayed(species, pi.RandomGender());
        }
        else
        {
            SetDisplayShiny(species, false);
            SetGenderDisplayed(species, 0);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        var pt = SAV.Personal;
        for (ushort species = 0; species < SAV.MaxSpeciesID; species++)
        {
            if (value && GetDexIndex(species).Index == 0)
                continue;
            var pi = pt[species];
            SeenAll(species, pi.FormCount, value, shinyToo);
        }
    }

    private void SetAllSeen(ushort species, bool value = true, bool shinyToo = false)
    {
        var pi = SAV.Personal[species];
        var fc = pi.FormCount;
        SeenAll(species, fc, value, shinyToo);
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetAllSeen(species, true, shinyToo);
        SetAllCaught(species, true, shinyToo);
    }

    public override void ClearDexEntryAll(ushort species) => Get(species).Clear();

    #endregion

    public void Register(PKM pk, ushort species, byte form)
    {
        var entry = Get(species);
        if (!entry.IsKnown)
            entry.SetDisplayIsNew();

        entry.SetCaught(true);
        entry.SetIsGenderSeen(pk.Gender, true);
        entry.SetIsFormSeen(form, true);
        entry.SetDisplayForm(form);
        entry.SetDisplayGender(pk.Gender);
        if (pk.IsShiny)
        {
            entry.SetDisplayIsShiny();
            entry.SetSeenIsShiny();
        }
        entry.SetLanguageFlag(pk.Language, true);
        if (SAV.Language != pk.Language)
            entry.SetLanguageFlag(SAV.Language, true);

        // Update adjacent entries if not seen.
        UpdateAdjacent(species);
    }
}
