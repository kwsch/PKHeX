using System;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for Generation 6 games.
/// </summary>
public abstract class Zukan6(Memory<byte> raw)
{
    // General structure: u32 magic, upgrade flags, 9*bitflags, form flags, language flags, u32 spinda

    /* 9 BitRegions with 0x60*8 bits
     * Region 0: Caught flags - has been captured/obtained via gift/trade/evolution.
     * Seen flags: Only should set the genders that have been seen. Genderless acts as male.
     * Region 1: Seen Male/Genderless
     * Region 2: Seen Female
     * Region 3: Seen Male/Genderless Shiny
     * Region 4: Seen Female Shiny
     * Displayed flags: only one should be set for a given species. Usually is the one first seen, or selected by the player.
     * Region 5: Displayed Male/Genderless
     * Region 6: Displayed Female
     * Region 7: Displayed Male/Genderless Shiny
     * Region 8: Displayed Female Shiny
     * Next, 4*{bytes} for form flags
     * Next, bitflag region for languages obtained
     * Lastly, the Spinda spot pattern, which is the first seen Spinda's encryption constant.
     */

    public Span<byte> Data => raw.Span;
    private const int BitSeenSize = 0x60;
    private const int DexLangIDCount = 7;

    public uint Magic { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint Packed { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }

    public bool IsNationalDexUnlocked { get => (Packed & 1) == 1; set => Packed = (Packed & ~1u) | (value ? 1u : 0u); } // bit 0
    public bool IsNationalDexMode { get => (Packed & 2) == 2; set => Packed = (Packed & ~2u) | (value ? 2u : 0u); } // bit 1
    // flags??
    public ushort InitialSpecies { get => (ushort)((Packed >> 6) & 0x3FF); set => Packed = (Packed & ~0xFFC0u) | ((uint)value << 6); } // bit 6-15
    // remaining 16 bits are unused

    private const ushort MaxSpecies = Legal.MaxSpeciesID_6;
    private const ushort MaxSpeciesLanguage = Legal.MaxSpeciesID_6;

    private const int OFS_CAUGHT = 0x8;
    private const int OFS_SEEN = OFS_CAUGHT + BitSeenSize; // 0x68
    private const int OFS_DISP = OFS_SEEN + (BitSeenSize * 4); // 0x1E8
    private const int FormDex = OFS_DISP + (BitSeenSize * 4); // 0x368
    protected abstract int FormLen { get; } // byte count

    private int OFS_LANG => FormDex + (FormLen * 4); // 0x368 + (0x18 * 4) = 0x3C8 for X/Y, 0x368 + (0x26 * 4) = 0x400 for OR/AS
    private int OFS_SPINDA => OFS_LANG + 640; // ((DexLangIDCount * Legal.MaxSpeciesID_6) / 8) = 630, rounded up to nearest multiple of 16 = 640

    public uint Spinda // 0x680 for OR/AS, 0x648 for X/Y
    {
        get => ReadUInt32LittleEndian(Data[OFS_SPINDA..]);
        set => WriteUInt32LittleEndian(Data[OFS_SPINDA..], value);
    }

    protected bool GetFlag(int ofs, int bitIndex) => FlagUtil.GetFlag(Data, ofs + (bitIndex >> 3), bitIndex);
    protected void SetFlag(int ofs, int bitIndex, bool value = true) => FlagUtil.SetFlag(Data, ofs + (bitIndex >> 3), bitIndex, value);
    private static int GetRegion(byte gender, bool isShiny) => (isShiny ? 2 : 0) | (gender & 1);
    public abstract (ushort Index, byte Count) GetFormIndex(ushort species);

    public bool GetSeen(ushort species, byte gender, bool isShiny)
    {
        var region = GetRegion(gender, isShiny);
        return GetSeen(species, region);
    }

    public bool GetSeen(ushort species, int region)
    {
        if (species is 0 or > MaxSpecies)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_SEEN + (region * BitSeenSize), bit);
    }

    public void SetSeen(ushort species, byte gender, bool isShiny, bool value = true)
    {
        var region = GetRegion(gender, isShiny);
        SetSeen(species, region, value);
    }

    public virtual void SetSeen(ushort species, int region, bool value = true)
    {
        if (species is 0 or > MaxSpecies)
            return;
        int bit = species - 1;
        SetFlag(OFS_SEEN + (region * BitSeenSize), bit, value);
    }

    public bool GetSeen(ushort species)
    {
        if (species is 0 or > MaxSpecies)
            return false;
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
        {
            if (GetFlag(OFS_SEEN + (i * BitSeenSize), bit))
                return true; // already seen in any region
        }
        return false;
    }

    public virtual void ClearSeen(ushort species)
    {
        if (species is 0 or > MaxSpecies)
            return;
        int bit = species - 1;
        for (int i = 0; i < 4; i++)
            SetFlag(OFS_SEEN + (i * BitSeenSize), bit, false);
    }

    public bool GetCaught(ushort species)
    {
        if (species is 0 or > MaxSpecies)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_CAUGHT, bit);
    }

    public abstract void SetCaughtFlag(ushort species, GameVersion origin);

    public void SetCaught(ushort species, bool value = true)
    {
        if (species is 0 or > MaxSpecies)
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
        if (species is 0 or > MaxSpecies)
            return false;
        int bit = species - 1;
        return GetFlag(OFS_DISP + (region * BitSeenSize), bit);
    }

    public bool GetDisplayedAny(ushort species)
    {
        if (species is 0 or > MaxSpecies)
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
        if (species is 0 or > MaxSpecies)
            return;
        int bit = species - 1;
        SetFlag(OFS_DISP + (region * BitSeenSize), bit, value);
    }

    public void ClearDisplayed(ushort species)
    {
        if (species is 0 or > MaxSpecies)
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
        if (species is 0 or > MaxSpeciesLanguage) // no Gen5
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
        if (species is 0 or > MaxSpeciesLanguage) // no Gen5
            return;
        int bit = species - 1;
        int lbit = (bit * DexLangIDCount) + index;
        SetFlag(OFS_LANG, lbit, isLanguageSet);
    }

    public void SetAllLanguage(ushort species, bool isLanguageSet = true)
    {
        if (species is 0 or > MaxSpeciesLanguage) // no Gen5
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
        if (pk.Species is 0 or > MaxSpecies)
            return;
        if (pk.IsEgg) // do not add
            return;

        var species = pk.Species;
        if (species == (int)Species.Spinda && !GetSeen(species))
            Spinda = pk.EncryptionConstant;

        var gender = pk.Gender;
        var isShiny = pk.IsShiny;
        SetSeen(species, gender, isShiny);
        SetCaughtFlag(species, pk.Version);
        SetDexOther(pk);
        if (!GetDisplayedAny(species))
            SetDisplayed(species, gender, isShiny);

        if (species <= MaxSpeciesLanguage)
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

    protected virtual void SetDexOther(PKM pk) { }

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

        var pi = PersonalTable.AO[species]; // only need for Gender info
        CompleteSeen(species, shinyToo, pi);
        CompleteObtained(species, language, allLanguages);
    }

    public virtual void CompleteObtained(ushort species, LanguageID language, bool allLanguages)
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
        for (ushort species = 1; species <= MaxSpecies; species++)
            ClearSeen(species);
    }

    public void SeenAll(bool shinyToo)
    {
        for (ushort species = 1; species <= MaxSpecies; species++)
        {
            var pi = PersonalTable.BW[species]; // only need for Gender info
            CompleteSeen(species, shinyToo, pi);
        }
    }

    public void CaughtAll(LanguageID language, bool allLanguages)
    {
        for (ushort species = 1; species <= MaxSpecies; species++)
            CompleteObtained(species, language, allLanguages);
    }

    public void CaughtNone()
    {
        for (ushort species = 1; species <= MaxSpecies; species++)
        {
            SetCaught(species, false);
            SetAllLanguage(species, false);
        }
    }

    public void ClearFormSeen() => Data.Slice(FormDex, FormLen * 4).Clear();

    public void SetFormsSeen1(bool shinyToo) => SetFormsSeen(shinyToo, true);

    public void SetFormsSeen(bool shinyToo, bool firstFormOnly = false)
    {
        for (ushort species = 1; species <= MaxSpecies; species++)
            CompleteForms(species, shinyToo, firstFormOnly);
    }
}

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.XY"/>.
/// </summary>
public sealed class Zukan6XY : Zukan6
{
    protected override int FormLen => 0x18;
    public override (ushort Index, byte Count) GetFormIndex(ushort species) => GetFormIndexXY(species);
    public Zukan6XY(Memory<byte> dex) : base(dex) { }

    // Spinda at 0x648
    // Obtained a {species} from Gen3/4/5 flags at 0x64C, 0th index is species=1
    // 0x54 bytes of flags

    // total structure size: 0x6A0

    public override void SetCaughtFlag(ushort species, GameVersion origin)
    {
        // Species: 1-649 for X/Y, and not for OR/AS; Set the Foreign Owned Flag
        if (origin < GameVersion.X && (uint)(species - 1) < (int)Species.Genesect)
            SetForeignFlag(species);
        else
            SetCaught(species);
    }

    public override void CompleteObtained(ushort species, LanguageID language, bool allLanguages)
    {
        base.CompleteObtained(species, language, allLanguages);
        if ((uint)(species - 1) < (int)Species.Genesect)
            SetForeignFlag(species);
    }

    public bool GetForeignFlag(ushort species)
    {
        int bit = species - 1;
        Debug.Assert((uint)bit < (int)Species.Genesect);
        return GetFlag(0x64C, bit);
    }

    public void SetForeignFlag(ushort species, bool value = true)
    {
        int bit = species - 1;
        Debug.Assert((uint)bit < (int)Species.Genesect);
        SetFlag(0x64C, bit, value);
    }

    public static (byte Index, byte Count) GetFormIndexXY(ushort species) => species switch
    {
        666 => (083, 20), // 20 Vivillion
        669 => (103, 5), // 5 Flabébé
        670 => (108, 6), // 6 Floette
        671 => (114, 5), // 5 Florges
        710 => (119, 4), // 4 Pumpkaboo
        711 => (123, 4), // 4 Gourgeist
        681 => (127, 2), // 2 Aegislash
        716 => (129, 2), // 2 Xerneas
        003 => (131, 2), // 2 Venusaur
        006 => (133, 3), // 3 Charizard
        009 => (136, 2), // 2 Blastoise
        065 => (138, 2), // 2 Alakazam
        094 => (140, 2), // 2 Gengar
        115 => (142, 2), // 2 Kangaskhan
        127 => (144, 2), // 2 Pinsir
        130 => (146, 2), // 2 Gyarados
        142 => (148, 2), // 2 Aerodactyl
        150 => (150, 3), // 3 Mewtwo
        181 => (153, 2), // 2 Ampharos
        212 => (155, 2), // 2 Scizor
        214 => (157, 2), // 2 Heracros
        229 => (159, 2), // 2 Houndoom
        248 => (161, 2), // 2 Tyranitar
        257 => (163, 2), // 2 Blaziken
        282 => (165, 2), // 2 Gardevoir
        303 => (167, 2), // 2 Mawile
        306 => (169, 2), // 2 Aggron
        308 => (171, 2), // 2 Medicham
        310 => (173, 2), // 2 Manetric
        354 => (175, 2), // 2 Banette
        359 => (177, 2), // 2 Absol
        380 => (179, 2), // 2 Latias
        381 => (181, 2), // 2 Latios
        445 => (183, 2), // 2 Garchomp
        448 => (185, 2), // 2 Lucario
        460 => (187, 2), // 2 Abomasnow
        _ => Zukan5B2W2.GetFormIndexB2W2(species),
    };

    public override void ClearSeen(ushort species)
    {
        base.ClearSeen(species);
        // Clear foreign flag as well
        if ((uint)(species - 1) < (int)Species.Genesect)
            SetForeignFlag(species, false);
    }

    public void SetSeen(ushort species, bool seen)
    {
        var pi = PersonalTable.XY[species];
        var gender = pi.OnlyFemale ? (byte)1 : (byte)0;
        SetSeen(species, gender, isShiny: false, value: seen);
    }

    public void Reset() => Data[4..].Clear(); // Clear all flags, except magic
}

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.ORAS"/>.
/// </summary>
public sealed class Zukan6AO : Zukan6
{
    protected override int FormLen => 0x26;
    public override (ushort Index, byte Count) GetFormIndex(ushort species) => GetFormIndexAO(species);

    public Zukan6AO(Memory<byte> dex) : base(dex) { }

    public override void SetCaughtFlag(ushort species, GameVersion origin) => SetCaught(species); // no foreign flag in OR/AS, unlike X/Y

    // Spinda at 0x680, counts at 0x684
    private const int CountEncounter = Legal.MaxSpeciesID_6 + 1; // 722, 0x5A4 bytes

    private static int GetOffsetCountSeen(ushort species)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, CountEncounter);
        return 0x684 + (species * 2);
    }
    private static int GetOffsetCountObtained(ushort species)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(species, CountEncounter);
        return 0xC28 + (species * 2);
    }

    // 0x11CC total length.

    public ushort GetCountSeen(ushort species) => ReadUInt16LittleEndian(Data[GetOffsetCountSeen(species)..]);
    public void SetCountSeen(ushort species, ushort value) => WriteUInt16LittleEndian(Data[GetOffsetCountSeen(species)..], value);

    // Turns out the game never actually calls these, but we can still display them to the user.
    public ushort GetCountObtained(ushort species) => ReadUInt16LittleEndian(Data[GetOffsetCountObtained(species)..]);
    public void SetCountObtained(ushort species, ushort value) => WriteUInt16LittleEndian(Data[GetOffsetCountObtained(species)..], value);

    public static (ushort Index, byte Count) GetFormIndexAO(ushort species) => species switch
    {
        025 => (189, 7), // 7 Pikachu
        720 => (196, 2), // 2 Hoopa
        015 => (198, 2), // 2 Beedrill
        018 => (200, 2), // 2 Pidgeot
        080 => (202, 2), // 2 Slowbro
        208 => (204, 2), // 2 Steelix
        254 => (206, 2), // 2 Sceptile
        260 => (208, 2), // 2 Swampert
        302 => (210, 2), // 2 Sableye
        319 => (212, 2), // 2 Sharpedo
        323 => (214, 2), // 2 Camerupt
        334 => (216, 2), // 2 Altaria
        362 => (218, 2), // 2 Glalie
        373 => (220, 2), // 2 Salamence
        376 => (222, 2), // 2 Metagross
        384 => (224, 2), // 2 Rayquaza
        428 => (226, 2), // 2 Lopunny
        475 => (228, 2), // 2 Gallade
        531 => (230, 2), // 2 Audino
        719 => (232, 2), // 2 Diancie
        382 => (234, 2), // 2 Kyogre
        383 => (236, 2), // 2 Groudon
        493 => (238, 18), // 18 Arceus
        649 => (256, 5), // 5 Genesect
        676 => (261, 1), // 10 Furfrou
        _ => Zukan6XY.GetFormIndexXY(species),
    };

    public void SetSeen(ushort species, bool seen)
    {
        var pi = PersonalTable.AO[species];
        var gender = pi.OnlyFemale ? (byte)1 : (byte)0;
        SetSeen(species, gender, isShiny: false, value: seen);
    }

    public override void SetSeen(ushort species, int region, bool value = true)
    {
        base.SetSeen(species, region, value);
        if (value && GetCountSeen(species) == 0)
            SetCountSeen(species, 1);
    }

    public void AddSeen(ushort species)
    {
        var current = GetCountSeen(species);
        if (current != ushort.MaxValue)
            SetCountSeen(species, (ushort)(current + 1));
    }

    public override void ClearSeen(ushort species)
    {
        base.ClearSeen(species);
        SetCountSeen(species, 0); // reset seen count
        SetCountObtained(species, 0); // reset obtained count
    }

    protected override void SetDexOther(PKM pk) => AddSeen(pk.Species);
}
