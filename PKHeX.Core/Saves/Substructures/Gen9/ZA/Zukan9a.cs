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

        entry.SetIsGenderSeen(pk.Gender, true);
        entry.SetIsFormSeen(form, true);
        entry.SetIsFormCaught(form, true);
        if (FormInfo.IsMegaForm(species, form))
            entry.SetIsSeenMega(0, true);
        if (IsMegaFormXY(species))
            entry.SetIsSeenMega(1, true);
        entry.SetLanguageFlag(pk.Language, true);

        var isShiny = pk.IsShiny;
        if (isShiny)
        {
            entry.SetIsShinySeen(form, true);
            if (GetFormExtraFlagsShinySeen(species, form, out var value))
                entry.SetIsShinySeen(value);
        }
        if (!isRegistered)
        {
            entry.DisplayForm = form;
            entry.SetDisplayGender((Gender)pk.Gender, species, form);
            entry.SetDisplayIsShiny(isShiny);

            if (GetFormExtraFlags(species, out var value))
            {
                entry.SetIsFormsSeen(value);
                entry.SetIsFormsCaught(value);
            }

            entry.SetDisplayIsNew(true);
        }
        if (pk is IAlphaReadOnly { IsAlpha: true })
            entry.SetIsSeenAlpha(true);
    }

    /// <summary>
    /// Fetches extra alternate form flags for all forms it can shift between, for Shiny entities.
    /// </summary>
    public static bool GetFormExtraFlags(ushort species, out uint value)
    {
        value = species switch
        {
            (int)Species.Furfrou => 1, // base form
            (int)Species.Aegislash => 3, // both forms

            // Returning in DLC
            (int)Species.Mimikyu => 3, // both forms
            (int)Species.Morpeko => 3, // both forms

            // Not present in game, but left in code from prior games.
            //(int)Species.Minior => 0, // handled separately (sets both core and meteor forms for the color) (((1u << 7) | 1u) << (form % 7))
            //(int)Species.Cramorant => 7, // all 3 forms
            //(int)Species.Eiscue => 3, // both forms
            //(int)Species.Palafin => 3, // both forms

            (int)Species.Zygarde => 0b01_0000, // Complete
            _ => 0u
        };
        return value != 0;
    }

    /// <summary>
    /// Fetches extra alternate form flags for all forms it can shift between, for Shiny entities.
    /// </summary>
    public static bool GetFormExtraFlagsShinySeen(ushort species, byte form, out uint value)
    {
        value = species switch
        {
            (int)Species.Furfrou => 1, // base form
            (int)Species.Aegislash => 3, // both forms

            (int)Species.Spewpa or (int)Species.Scatterbug => 0xF_FFFF, // all 20 forms of Vivillon. Not for Vivillon!

            // Returning in DLC
            (int)Species.Mimikyu => 3, // both forms
            (int)Species.Morpeko => 3, // both forms

            // Mega Forms
            (int)Species.Zygarde => 0b11_1111, // all forms (normally sets 31, but Mega adds another flag)
            (int)Species.Greninja when form != 2 => 0b1011, // Mega and Ash/Normal forms (normally sets 3, but Mega adds another flag)
            (int)Species.Floette when form == 5 => 0b10_0000, // Mega (not form 1)
            // New Megas (Absol, Lucario, Meowstic, Magearna, Tatsugiri) will need handling in DLC updates.
            _ when IsMegaFormXY(species) => 0b_0011, // Two mega forms
            _ when FormInfo.HasMegaForm(species) => 0b_0001, // One mega form
            _ => 0,
        };
        return value != 0;
    }

    public static bool IsMegaFormXY(ushort species) => species switch
    {
        (int)Species.Charizard => true,
        (int)Species.Mewtwo => true,
        _ => false,
    };

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

            entry.SetIsSeenAlpha(value);
            if (FormInfo.IsMegaForm(species, form))
                entry.SetIsSeenMega(0, value);
            if (IsMegaFormXY(species))
                entry.SetIsSeenMega(1, value);

            if (value)
            {
                if (GetFormExtraFlags(species, out var seen))
                    entry.SetIsFormsSeen(seen);
                if (shinyToo && GetFormExtraFlagsShinySeen(species, form, out var flags))
                    entry.SetIsShinySeen(flags);
            }
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

            if (GetFormExtraFlags(species, out var caught))
                entry.SetIsFormsCaught(caught);
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
