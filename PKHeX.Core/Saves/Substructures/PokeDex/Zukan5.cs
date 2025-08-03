using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Zukan5BW(Memory<byte> dex) : Zukan5(dex)
{
    protected override int FormLen => 9;
    public override (byte Index, byte Count) GetFormIndex(ushort species) => GetFormIndexBW(species);

    public static (byte Index, byte Count) GetFormIndexBW(ushort species) => species switch
    {
        201 => (000, 28), // 28 Unown
        386 => (028, 4), // 4 Deoxys
        492 => (032, 2), // 2 Shaymin
        487 => (034, 2), // 2 Giratina
        479 => (036, 6), // 6 Rotom
        422 => (042, 2), // 2 Shellos
        423 => (044, 2), // 2 Gastrodon
        412 => (046, 3), // 3 Burmy
        413 => (049, 3), // 3 Wormadam
        351 => (052, 4), // 4 Castform
        421 => (056, 2), // 2 Cherrim
        585 => (058, 4), // 4 Deerling
        586 => (062, 4), // 4 Sawsbuck
        648 => (066, 2), // 2 Meloetta
        555 => (068, 2), // 2 Darmanitan
        550 => (070, 2), // 2 Basculin
        _ => default,
    };
}

public sealed class Zukan5B2W2(Memory<byte> dex) : Zukan5(dex)
{
    protected override int FormLen => 11; // form flags size is + 11 from B/W with new forms (Therians) and Kyurem forms
    public override (byte Index, byte Count) GetFormIndex(ushort species) => GetFormIndexB2W2(species);

    public static (byte Index, byte Count) GetFormIndexB2W2(ushort species) => species switch
    {
        646 => (072, 3), // 3 Kyurem
        647 => (075, 2), // 2 Keldeo
        642 => (077, 2), // 2 Thundurus
        641 => (079, 2), // 2 Tornadus
        645 => (081, 2), // 2 Landorus
        _ => Zukan5BW.GetFormIndexBW(species), // B2/W2 is just appended to BW forms
    };
}

/// <summary>
/// Pokédex structure used for Generation 5 games.
/// </summary>
public abstract class Zukan5(Memory<byte> raw)
{
    public Span<byte> Data => raw.Span;
    private const int BitSeenSize = 0x54;
    private const int DexLangIDCount = 7;

    public uint Magic { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint Packed { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }

    public bool IsNationalDexUnlocked { get => (Packed & 1) == 1; set => Packed = (Packed & ~1u) | (value ? 1u : 0u); } // bit 0
    public uint Flags { get => (Packed >> 1) & 0x3FF; set => Packed = (Packed & ~0x7FEu) | ((value & 0x3FF) << 1); } // bit 1-10
    public bool IsNationalDexMode { get => (Packed & 0x800u) != 0; set => Packed = (Packed & ~0x800u) | (value ? 0x800u : 0u); } // bit 11
    public ushort InitialSpecies { get => (ushort)((Packed >> 12) & 0x3FF); set => Packed = (Packed & ~0x3FF000u) | ((uint)value << 12); } // bit 12-21
    // remaining 10 bits are unused

    private const int OFS_CAUGHT = 0x8;
    private const int OFS_SEEN = OFS_CAUGHT + BitSeenSize; // 0x5C
    private const int OFS_DISP = OFS_SEEN + (BitSeenSize * 4); // 0x1AC
    private const int FormDex = OFS_DISP + (BitSeenSize * 4); // 0x2FC
    protected abstract int FormLen { get; } // byte count

    private int OFS_LANG => FormDex + (FormLen * 4); // 0x2FC + (0xB * 4) = 0x328 for B2/W2, 0x2FC + (0x9 * 4) = 0x320 for BW
    private int OFS_SPINDA => OFS_LANG + 431; // ((DexLangIDCount * Legal.MaxSpeciesID_4) / 8), rounded up to 432 bytes

    public uint Spinda
    {
        get => ReadUInt32LittleEndian(Data[OFS_SPINDA..]);
        set => WriteUInt32LittleEndian(Data[OFS_SPINDA..], value);
    }

    private bool GetFlag(int ofs, int bitIndex) => FlagUtil.GetFlag(Data, ofs + (bitIndex >> 3), bitIndex);
    private void SetFlag(int ofs, int bitIndex, bool value = true) => FlagUtil.SetFlag(Data, ofs + (bitIndex >> 3), bitIndex, value);
    private static int GetRegion(byte gender, bool isShiny) => (isShiny ? 2 : 0) | (gender & 1);
    public abstract (byte Index, byte Count) GetFormIndex(ushort species);

    public bool GetSeen(ushort species, byte gender, bool isShiny)
    {
        var region = GetRegion(gender, isShiny);
        return GetSeen(species, region);
    }

    public bool GetSeen(ushort species, int region)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_SEEN + (region * BitSeenSize), bit);
    }

    public void SetSeen(ushort species, byte gender, bool isShiny, bool value = true)
    {
        var region = GetRegion(gender, isShiny);
        SetSeen(species, region, value);
    }

    public void SetSeen(ushort species, int region, bool value = true)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return;
        int bit = species - 1;
        SetFlag(OFS_SEEN + (region * BitSeenSize), bit, value);
    }

    public bool GetSeen(ushort species)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return false;
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
        {
            if (GetFlag(OFS_SEEN + (i * BitSeenSize), bit))
                return true; // already seen in any region
        }
        return false;
    }

    public void ClearSeen(ushort species)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return;
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
            SetFlag(OFS_SEEN + (i * BitSeenSize), bit, false);
    }

    public bool GetCaught(ushort species)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_CAUGHT, bit);
    }

    public void SetCaught(ushort species, bool value = true)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return;
        int bit = species - 1;
        SetFlag(OFS_CAUGHT, bit, value);
    }

    public bool GetDisplayed(ushort species, byte gender, bool isShiny)
    {
        var region = GetRegion(gender, isShiny);
        return GetDisplayed(species, region);
    }

    public bool GetDisplayed(ushort species, int region)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_DISP + (region * BitSeenSize), bit);
    }

    public bool GetDisplayedAny(ushort species)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return false;
        // Check Displayed Status for base form
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
        {
            if (GetFlag(OFS_DISP + (i * BitSeenSize), bit))
                return true;
        }
        return false;
    }

    public void SetDisplayed(ushort species, byte gender, bool isShiny, bool value = true)
    {
        var region = GetRegion(gender, isShiny);
        SetDisplayed(species, region, value);
    }

    public void SetDisplayed(ushort species, int region, bool value = true)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return;
        int bit = species - 1;
        SetFlag(OFS_DISP + (region * BitSeenSize), bit, value);
    }

    public void ClearDisplayed(ushort species)
    {
        if (species is 0 or > Legal.MaxSpeciesID_5)
            return;
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
            SetFlag(OFS_DISP + (i * BitSeenSize), bit, false);
    }

    public bool GetLanguageFlag(ushort species, LanguageID language)
    {
        int langIndex = GetLanguageIndex(language);
        if (langIndex < 0)
            return false; // invalid language index
        return GetLanguageFlag(species, langIndex);
    }

    public bool GetLanguageFlag(ushort species, int langIndex)
    {
        if (species is 0 or > Legal.MaxSpeciesID_4) // no Gen5
            return false;
        int bit = species - 1;
        int lbit = (bit * DexLangIDCount) + langIndex;
        return GetFlag(OFS_LANG, lbit);
    }

    public void SetLanguageFlag(ushort species, LanguageID langIndex, bool isLanguageSet = true)
    {
        int index = GetLanguageIndex(langIndex);
        if (index < 0)
            return; // invalid language index
        SetLanguageFlag(species, index, isLanguageSet);
    }

    public void SetLanguageFlag(ushort species, int index, bool isLanguageSet)
    {
        if (species is 0 or > Legal.MaxSpeciesID_4) // no Gen5
            return;
        int bit = species - 1;
        int lbit = (bit * DexLangIDCount) + index;
        SetFlag(OFS_LANG, lbit, isLanguageSet);
    }

    public void SetAllLanguage(ushort species, bool isLanguageSet = true)
    {
        if (species is 0 or > Legal.MaxSpeciesID_4) // no Gen5
            return;
        int bit = species - 1;
        for (int i = 0; i < DexLangIDCount; i++)
        {
            int lbit = (bit * DexLangIDCount) + i;
            SetFlag(OFS_LANG, lbit, isLanguageSet);
        }
    }

    public bool GetFormSeen(int formIndex, bool isShiny)
    {
        int region = isShiny ? 1 : 0; // 0 = non-shiny, 1 = shiny
        return GetFormFlag(formIndex, region);
    }

    public bool GetFormFlag(int formIndex, int region)
    {
        if (formIndex < 0 || formIndex >= FormLen * 8)
            return false; // invalid form index
        return GetFlag(FormDex + (region * FormLen), formIndex);
    }

    public void SetFormSeen(int formIndex, bool isShiny, bool value = true)
    {
        int region = isShiny ? 1 : 0; // 0 = non-shiny, 1 = shiny
        SetFormFlag(formIndex, region, value);
    }

    public void SetFormFlag(int formIndex, int region, bool value)
    {
        if (formIndex < 0 || formIndex >= FormLen * 8)
            return;
        SetFlag(FormDex + (region * FormLen), formIndex, value);
    }

    public bool GetFormDisplayed(int formIndex0, int formCount)
    {
        if (formIndex0 < 0 || formCount <= 1 || formIndex0 + formCount >= FormLen * 8)
            return false; // invalid form index or count
        for (int i = 0; i < formCount; i++)
        {
            int formIndex = formIndex0 + i;
            if (GetFlag(FormDex + (2 * FormLen), formIndex)) // Nonshiny
                return true; // already set
            if (GetFlag(FormDex + (2 * FormLen), formIndex)) // Shiny
                return true; // already set
        }
        return false;
    }

    public void SetFormDisplayed(int formIndex, bool isShiny, bool value = true)
    {
        if (formIndex < 0 || formIndex >= FormLen * 8)
            return; // invalid form index
        int region = isShiny ? 1 : 0; // 0 = non-shiny, 1 = shiny
        SetFlag(FormDex + (2 * FormLen) + (region * FormLen), formIndex, value);
    }

    public void SetDex(PKM pk)
    {
        if (pk.Species is 0 or > Legal.MaxSpeciesID_5)
            return;
        if (pk.IsEgg) // do not add
            return;

        var species = pk.Species;
        if (species == (int)Species.Spinda && !GetSeen(species))
            Spinda = pk.EncryptionConstant;

        var gender = pk.Gender;
        var isShiny = pk.IsShiny;
        SetSeen(species, gender, isShiny);
        SetCaught(species);
        if (!GetDisplayedAny(species))
            SetDisplayed(species, gender, isShiny);

        if (species <= Legal.MaxSpeciesID_4)
            SetLanguageFlag(species, (LanguageID)pk.Language);

        var (formIndex, count) = GetFormIndex(species);
        if (count == 0)
            return; // no forms

        // Set the Form Seen Flag
        SetFormSeen(formIndex, isShiny);
        if (GetFormDisplayed(formIndex, count))
            return; // already displayed
        var form = pk.Form;
        if (form >= count)
            return; // invalid form index
        // Set the Form Displayed Flag
        var index = formIndex + form;
        SetFormDisplayed(index, isShiny);
    }

    /// <summary>
    /// Gets the bit index for the language.
    /// </summary>
    /// <param name="language">Entry language ID.</param>
    private static int GetLanguageIndex(LanguageID language)
    {
        var index = (int)language - 1; // LanguageID starts at 1
        if (index > 5)
            index--; // 0-6 language values
        if ((uint)index > 5)
            return -1; // Invalid language index
        return index;
    }

    public void GiveAll(ushort species, bool state, bool shinyToo, LanguageID language, bool allLanguages)
    {
        if (!state)
        {
            ClearSeen(species);
            return;
        }

        var pi = PersonalTable.BW[species]; // only need for Gender info
        CompleteSeen(species, shinyToo, pi);
        CompleteObtained(species, language, allLanguages);
    }

    public void CompleteObtained(ushort species, LanguageID language, bool allLanguages)
    {
        SetCaught(species);
        if (allLanguages)
            SetAllLanguage(species);
        else
            SetLanguageFlag(species, language);
    }

    public void CompleteSeen<T>(ushort species, bool shinyToo, T pi) where T : IGenderDetail
    {
        if (!pi.OnlyFemale)
        {
            SetSeen(species, 0); // non-shiny
            if (shinyToo)
                SetSeen(species, 2); // shiny
        }
        if (pi is { OnlyMale: false, Genderless: false })
        {
            SetSeen(species, 1); // non-shiny
            if (shinyToo)
                SetSeen(species, 3); // shiny
        }
        if (!GetDisplayedAny(species))
            SetDisplayed(species, pi.OnlyFemale ? 1 : 0); // 0 = non-shiny, 1 = shiny

        CompleteForms(species, shinyToo);
    }

    public void CompleteForms(ushort species, bool shinyToo, bool firstFormOnly = false)
    {
        var (index, count) = GetFormIndex(species);
        if (count == 0)
            return; // no forms
        if (firstFormOnly)
            count = 1;
        for (int i = 0; i < count; i++)
        {
            int formIndex = index + i;
            SetFormSeen(formIndex, false); // non-shiny
            if (shinyToo)
                SetFormSeen(formIndex, true); // shiny
        }
        if (!GetFormDisplayed(index, count))
            SetFormDisplayed(index, false); // non-shiny
    }

    public void SeenNone()
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_5; species++)
            ClearSeen(species);
    }

    public void SeenAll(bool shinyToo)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_5; species++)
        {
            var pi = PersonalTable.BW[species]; // only need for Gender info
            CompleteSeen(species, shinyToo, pi);
        }
    }

    public void CaughtAll(LanguageID language, bool allLanguages)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_5; species++)
            CompleteObtained(species, language, allLanguages);
    }

    public void CaughtNone()
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_5; species++)
        {
            SetCaught(species, false);
            SetAllLanguage(species, false);
        }
    }

    public void ClearFormSeen() => Data.Slice(FormDex, FormLen * 4).Clear();

    public void SetFormsSeen1(bool shinyToo) => SetFormsSeen(shinyToo, true);

    public void SetFormsSeen(bool shinyToo, bool firstFormOnly = false)
    {
        for (ushort species = 1; species <= Legal.MaxSpeciesID_5; species++)
            CompleteForms(species, shinyToo, firstFormOnly);
    }
}
