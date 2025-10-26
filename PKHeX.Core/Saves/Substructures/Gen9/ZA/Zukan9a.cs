namespace PKHeX.Core;

public sealed class Zukan9a(SAV9ZA sav, SCBlock block) : ZukanBase<SAV9ZA>(sav, block.Raw)
{
    public override bool GetSeen(ushort species) => GetEntry(species).IsSeen;

    public override bool GetCaught(ushort species) => GetEntry(species).IsCaught;

    public PokeDexEntry9a GetEntry(ushort species)
    {
        ushort index = species > SAV.MaxSpeciesID ? (ushort)0 : species;
        index = SpeciesConverter.GetInternal9(index);

        var offset = index * PokeDexEntry9a.SIZE;
        var slice = Data.Slice(offset, PokeDexEntry9a.SIZE);
        return new PokeDexEntry9a(slice);
    }

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
        Register(pk, species, form);
    }

    private void Register(PKM pk, ushort species, byte form)
    {
        var entry = GetEntry(species);
        var isRegistered = entry.IsCaught;

        entry.SetIsFormSeen(form, true);
        entry.SetIsFormCaught(form, true);
        if (FormInfo.IsMegaForm(species, form))
            entry.SetIsSeenMega(true);
        entry.SetLanguageFlag(pk.Language, true);

        var isShiny = pk.IsShiny;
        if (isShiny)
        {
            entry.SetIsShinySeen(form, true);
        }
        if (!isRegistered)
        {
            entry.DisplayForm = form;
            entry.SetDisplayGender((Gender)pk.Gender, species, form);
            entry.SetDisplayIsShiny(isShiny);

            entry.SetDisplayIsNew(true);
        }
        if (pk is IAlphaReadOnly { IsAlpha: true })
            entry.SetIsSeenAlpha(true);
    }

    public override void SeenNone()
    {
        Data.Clear();
    }

    public override void CaughtNone()
    {
        for (ushort species = 1; species <= SAV.MaxSpeciesID; species++)
            SetAllCaught(species, false);
    }

    public override void SeenAll(bool shinyToo = false)
    {
        SetAllSeen(true, shinyToo);
    }

    private void SeenAll(ushort species, PersonalInfo9ZA pi, bool value = true, bool shinyToo = false)
    {
        var formCount = pi.FormCount;
        var entry = GetEntry(species);
        for (byte form = 0; form < formCount; form++)
        {
            var other = SAV.Personal[species, form];
            if (!other.IsPresentInGame)
                continue;
            entry.SetIsFormSeen(form, value);
            if (shinyToo)
                entry.SetIsShinySeen(form, value);
        }

        // Set other seen flags
        if (pi.Genderless)
        {
            entry.SetIsGenderSeen(2, value);
        }
        else
        {
            if (!pi.OnlyFemale)
                entry.SetIsGenderSeen(0, value);
            if (!pi.OnlyMale)
                entry.SetIsGenderSeen(1, value);
        }
    }

    public override void CompleteDex(bool shinyToo = false)
    {
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            if (!SAV.Personal.IsSpeciesInGame(species))
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
            SetAllCaught(species, true, shinyToo);
        }
    }

    private void SetAllCaught(ushort species, bool value = true, bool shinyToo = false)
    {
        var entry = GetEntry(species);
        var formCount = SAV.Personal[species].FormCount;
        byte firstForm = 0xFF;
        for (byte form = 0; form < formCount; form++)
        {
            var other = SAV.Personal[species, form];
            if (!other.IsPresentInGame)
                continue;
            if (firstForm == 0xFF)
                firstForm = form;

            entry.SetIsFormCaught(form, value);
            entry.SetIsFormSeen(form, value);
            if (shinyToo)
                entry.SetIsShinySeen(form, value);
        }

        // Set language
        entry.SetLanguageFlagAll(value);

        if (!value)
        {
            entry.ClearDisplay();
        }
        else
        {
            // Set display data
            if (firstForm == 0xFF)
                firstForm = 0; // Failsafe, should not happen
            var pi = SAV.Personal[species, firstForm];
            entry.SetDisplayGender((Gender)pi.RandomGender(), species, firstForm);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        var pt = SAV.Personal;
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            var pi = pt[species];
            SeenAll(species, pi, value, shinyToo);
        }
    }

    private void SetAllSeen(ushort species, bool value = true, bool shinyToo = false)
    {
        var pi = SAV.Personal[species];
        SeenAll(species, pi, value, shinyToo);
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetAllSeen(species, true, shinyToo);
        SetAllCaught(species, true, shinyToo);
    }

    public override void ClearDexEntryAll(ushort species)
    {
        var entry = GetEntry(species);
        entry.Clear();
    }

    #endregion
}
