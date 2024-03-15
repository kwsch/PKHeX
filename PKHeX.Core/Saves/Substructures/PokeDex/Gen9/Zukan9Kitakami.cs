using System;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.SV"/>.
/// </summary>>
public sealed class Zukan9Kitakami(SAV9SV sav, SCBlock Block) : ZukanBase<SAV9SV>(sav, default)
{
    public PokeDexEntry9Kitakami Get(ushort species)
    {
        if (species > SAV.MaxSpeciesID)
            throw new ArgumentOutOfRangeException(nameof(species), species, null);

        const int size = PokeDexEntry9Kitakami.SIZE;
        var internalSpecies = SpeciesConverter.GetInternal9(species);
        var span = Block.Data.AsSpan(internalSpecies * size, size);
        return new PokeDexEntry9Kitakami(span);
    }

    public override bool GetSeen(ushort species) => Get(species).IsSeen;
    public override bool GetCaught(ushort species) => Get(species).IsCaught;

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

    private void MarkAsKnown1(byte group, int index, byte form = 0)
    {
        if (group == 1 && index <= 9) // Don't set adjacent for starters. Hide their evolutions!
            return;

        var species = GetSpecies(group, index);
        if (species == 0)
            return;
        var entry = Get(species);
        if (!entry.IsKnown)
            entry.SetHeardForm(form, true);
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
            if (pi.DexKitakami != 0)
                return (2, pi.DexKitakami);
            if (pi.DexBlueberry != 0)
                return (3, pi.DexBlueberry);
        }
        return (0, 0);
    }

    public override void SeenNone()
    {
        Array.Clear(Block.Data, 0, Block.Data.Length);
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
        entry.FlagsGenderSeen = 0;
        entry.FlagsShinySeen = (byte)(value ? shinyToo ? 3 : 1 : 0);

        var pt = SAV.Personal;
        for (byte form = 0; form < formCount; form++)
        {
            var pi = pt.GetFormEntry(species, form);
            bool actual = value && pi.IsPresentInGame;
            SetSeen(entry, pi, form, actual, shinyToo && value);
        }
    }

    private static void SetSeen(PokeDexEntry9Kitakami entry, PersonalInfo9SV pi, byte form, bool seen, bool shinyToo)
    {
        if (!seen)
        {
            entry.ClearSeen(form);
            return;
        }

        entry.SetSeenForm(form, true);
        entry.SetHeardForm(form, true);

        if (pi.IsDualGender)
        {
            entry.FlagsGenderSeen = 3;
        }
        else
        {
            var displayGender = pi.FixedGender();
            entry.SetIsGenderSeen(displayGender, true);
        }

        if (shinyToo)
            entry.SetIsModelSeen(true, true);
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
        var entry = Get(species);
        if (value)
        {
            var pi = SAV.Personal[species];
            if (shinyToo)
                entry.SetIsModelSeen(true, true);

            for (byte form = 0; form < pi.FormCount; form++)
            {
                if (!SAV.Personal.IsPresentInGame(species, form))
                    continue;
                SetSeen(entry, pi, form, true, shinyToo);
                entry.SetObtainedForm(form, true);
                entry.SetLocalStates(pi, form, pi.RandomGender(), shinyToo);
            }
            entry.SetAllLanguageFlags();
        }
        else
        {
            entry.SetAllLanguageFlags(0);
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
        var pi = SAV.Personal.GetFormEntry(species, form);
        if (!pi.IsPresentInGame)
            return;

        var entry = Get(species);
        bool isShiny = pk.IsShiny;

        SetSeen(entry, pi, form, true, isShiny);
        entry.SetObtainedForm(form, true);
        entry.SetLocalStates(pi, form, pi.RandomGender(), isShiny);
        if (isShiny)
            entry.SetIsModelSeen(true, true);
        entry.SetLanguageFlag(pk.Language, true);
        if (SAV.Language != pk.Language)
            entry.SetLanguageFlag(SAV.Language, true);

        // Update adjacent entries if not seen.
        UpdateAdjacent(species);
    }
}
