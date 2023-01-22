using System;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.SV"/>.
/// </summary>>
public sealed class Zukan9 : ZukanBase<SAV9SV>
{
    private readonly SCBlock Paldea;

    public Zukan9(SAV9SV sav, SCBlock paldea) : base(sav, 0)
    {
        Paldea = paldea;
    }

    /// <summary>
    /// Checks how much DLC patches have been installed by detecting if DLC blocks are present.
    /// </summary>
    public int GetRevision() => 0; // No DLC1 data allocated
    private const int EntrySize = PokeDexEntry9SV.SIZE;

    public PokeDexEntry9SV Get(ushort species)
    {
        if (species > SAV.MaxSpeciesID)
            throw new ArgumentOutOfRangeException(nameof(species), species, null);
        var span = Paldea.Data.AsSpan(species * EntrySize);
        return new PokeDexEntry9SV(span);
    }

    public override bool GetSeen(ushort species) => Get(species).IsSeen;
    public override bool GetCaught(ushort species) => Get(species).IsCaught;
    public void SetCaught(ushort species, bool value = true) => Get(species).SetCaught(value);
    public bool GetIsLanguageIndexObtained(ushort species, int langIndex) => Get(species).GetLanguageFlag(langIndex);
    public void SetIsLanguageIndexObtained(ushort species, int langIndex, bool value = true) => Get(species).SetLanguageFlag(langIndex, value);

    public bool GetIsLanguageObtained(ushort species, int language)
    {
        int langIndex = PokeDexEntry9SV.GetDexLangFlag(language);
        if (langIndex < 0)
            return false;

        return GetIsLanguageIndexObtained(species, language);
    }

    public void SetIsLanguageObtained(ushort species, int language, bool value = true)
    {
        int langIndex = PokeDexEntry9SV.GetDexLangFlag(language);
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
        if (pi.DexIndex != 0)
            return (pi.DexGroup, pi.DexIndex);

        for (byte f = 1; f <= pi.FormCount; f++)
        {
            pi = pt.GetFormEntry(species, f);
            if (pi.DexIndex != 0)
                return (pi.DexGroup, pi.DexIndex);
        }
        return (0, 0);
    }

    public override void SeenNone()
    {
        Array.Clear(Paldea.Data, 0, Paldea.Data.Length);
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

    private void SeenAll(ushort species, byte formCount, bool value = true, bool shinyToo = false)
    {
        var pt = SAV.Personal;
        for (byte form = 0; form < formCount; form++)
        {
            var pi = pt.GetFormEntry(species, form);
            var seenSpecies = value;
            bool seenForm = seenSpecies && pi.IsPresentInGame;
            SeenAll(species, form, pi, shinyToo, seenSpecies, seenForm);
        }
    }

    private void SeenAll(ushort species, byte form, IGenderDetail pi, bool shinyToo, bool seenSpecies, bool seenForm)
    {
        var entry = Get(species);
        if (seenSpecies && !entry.IsSeen)
            entry.SetSeen(seenSpecies);

        if (!seenSpecies || (seenForm && pi.IsDualGender))
        {
            entry.SetIsGenderSeen(0, seenForm);
            entry.SetIsGenderSeen(1, seenForm);
        }
        else
        {
            var gender = pi.FixedGender();
            entry.SetIsGenderSeen(gender, seenForm);
        }
        entry.SetIsFormSeen(form, seenForm);

        if (!seenSpecies || shinyToo)
            entry.SetSeenIsShiny(seenSpecies);
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

    private void SetAllCaught(ushort species, bool value = true, bool shinyToo = false)
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
}
