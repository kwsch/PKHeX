using System;

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

        if (IsMegaFormXY(species, SAV.SaveRevision) || IsMegaFormZA(species, SAV.SaveRevision))
            entry.SetIsSeenMega(1, true);
        else if (species is (int)Species.Magearna or (int)Species.Meowstic)
            entry.SetIsSeenMega(1, true);
        else if (species == (int)Species.Tatsugiri)
            entry.SetIsSeenMega(form < 3 ? form : (byte)Math.Clamp(form - 3, 0, 3), true);
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
    /// Fetches extra alternate form flags for all forms it can shift between, for regular entities.
    /// </summary>
    public static bool GetFormExtraFlags(ushort species, out uint value)
    {
        value = species switch
        {
            (int)Species.Furfrou => 1, // base form
            (int)Species.Aegislash => 3, // both forms

            // Returning in DLC
            (int)Species.Rotom => 0x3F, // all 5+1 forms
            (int)Species.Meloetta => 3, // both forms
            (int)Species.Genesect => 0x1F, // all 4+1 forms
            (int)Species.Mimikyu => 3, // both forms
            (int)Species.Morpeko => 3, // both forms
            (int)Species.Hoopa => 3, // both forms

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
    public bool GetFormExtraFlagsShinySeen(ushort species, byte form, out uint value)
    {
        value = species switch
        {
            (int)Species.Furfrou => 1, // base form
            (int)Species.Aegislash => 3, // both forms

            (int)Species.Spewpa or (int)Species.Scatterbug => 0xF_FFFF, // all 20 forms of Vivillon. Not for Vivillon!

            // Returning in DLC
            (int)Species.Rotom => 0x3F, // all 5+1 forms
            (int)Species.Meloetta => 3, // both forms
            (int)Species.Genesect => 0x1F, // all 4+1 forms
            (int)Species.Mimikyu => 3, // both forms
            (int)Species.Morpeko => 3, // both forms
            (int)Species.Hoopa => 3, // both forms

            (int)Species.Tatsugiri => form switch
            {
                0 => 8,
                1 => 16,
                2 => 32,
                _ => 0,
            }, // Mega

            (int)Species.Magearna => form switch
            {
                1 => 8u,
                0 => 4u,
                _ => 0u,
            }, // Mega

            // Mega Forms
            (int)Species.Zygarde => 0b11_1111, // all forms (normally sets 31, but Mega adds another flag)
            (int)Species.Greninja when form != 2 => 0b1011, // Mega and Ash/Normal forms (normally sets 3, but Mega adds another flag)
            (int)Species.Floette when form == 5 => 0b10_0000, // Mega (not form 1)
            _ => GetFallbackBits(species),
        };
        return value != 0;
    }

    private uint GetFallbackBits(ushort species)
    {
        if (SAV.SaveRevision is not 0) // DLC released
            return GetFallbackBitsDLC(species);
        return GetFallbackBitsBase(species);
    }

    private static uint GetFallbackBitsDLC(ushort species) => species switch
    {
        (int)Species.Absol or (int)Species.Lucario or (int)Species.Garchomp
                               => 0b_0110, // Mega + Z Mega
        (int)Species.Meowstic  => 0b_0100, // Mega only (M,F,Mega)
        (int)Species.Raichu    => 0b_1100, // X and Y forms (Base,Alolan,X,Y)

        // What about others? Slowbro?

        _ => GetFallbackBitsBase(species),
    };

    private static uint GetFallbackBitsBase(ushort species)
    {
        if (IsMegaFormXY(species, 0))
            return 0b_0110; // Two mega forms (Base,Mega X,Mega Y)
        if (FormInfo.HasMegaForm(species))
            return 0b_0010; // One mega form (Base,Mega)
        return 0;
    }

    public static bool IsMegaFormXY(ushort species, int revision) => species switch
    {
        (int)Species.Raichu when revision is not 0 => true,
        (int)Species.Charizard => true,
        (int)Species.Mewtwo => true,
        _ => false,
    };

    public static bool IsMegaFormZA(ushort species, int revision) => revision is not 0 && species switch
    {
        (int)Species.Absol => true,
        (int)Species.Lucario => true,
        (int)Species.Garchomp => true,
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

            if (IsMegaFormXY(species, SAV.SaveRevision) || IsMegaFormZA(species, SAV.SaveRevision))
                entry.SetIsSeenMega(1, value);
            else if (species is (int)Species.Magearna or (int)Species.Meowstic)
                entry.SetIsSeenMega(1, value);
            else if (species == (int)Species.Tatsugiri)
                entry.SetIsSeenMega(form < 3 ? form : (byte)Math.Clamp(form - 3, 0, 3), true);

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
