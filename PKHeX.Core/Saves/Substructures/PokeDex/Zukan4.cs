using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used by <see cref="SAV4"/> games.
/// </summary>
public sealed class Zukan4(SAV4 sav, Memory<byte> raw) : ZukanBase<SAV4>(sav, raw)
{
    // General structure: u32 magic, 4*bitflags, u32 spinda, form flags, language flags, more form flags, upgrade flags

    /* 4 BitRegions with 0x40*8 bits
    * Region 0: Caught (Captured/Owned) flags
    * Region 1: Seen flags
    * Region 2: First Seen Gender
    * Region 3: Second Seen Gender
    * When setting a newly seen species (first time), we set the gender bit to both First and Second regions.
    * When setting an already-seen species, we set the Second region bit if the now-seen gender-bit is not equal to the first-seen bit.
    * 4 possible states: 00, 01, 10, 11
    * 00 - 1Seen: Male Only
    * 01 - 2Seen: Male First, Female Second
    * 10 - 2Seen: Female First, Male Second
    * 11 - 1Seen: Female Only
    * assuming the species is seen, (bit1 ^ bit2) + 1 = genders in dex
    */

    public const string GENDERLESS = "Genderless";
    public const string MALE = "Male";
    public const string FEMALE = "Female";
    private const int SIZE_REGION = 0x40;
    private const int COUNT_REGION = 4;
    private const int OFS_SPINDA = sizeof(uint) + (COUNT_REGION * SIZE_REGION);
    private const int OFS_FORM1 = OFS_SPINDA + sizeof(uint);

    private bool HGSS => SAV is SAV4HGSS;
    private bool DP => SAV is SAV4DP;

    public uint Magic { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    public override bool GetCaught(ushort species) => GetRegionFlag(0, species - 1);
    public override bool GetSeen(ushort species) => GetRegionFlag(1, species - 1);
    public int GetSeenGenderFirst(ushort species) => GetRegionFlag(2, species - 1) ? 1 : 0;
    public int GetSeenGenderSecond(ushort species) => GetRegionFlag(3, species - 1) ? 1 : 0;
    public bool GetSeenSingleGender(ushort species) => GetSeenGenderFirst(species) == GetSeenGenderSecond(species);

    private bool GetRegionFlag(int region, int index)
    {
        var ofs = 4 + (region * SIZE_REGION) + (index >> 3);
        return FlagUtil.GetFlag(Data, ofs, index);
    }

    public void SetCaught(ushort species, bool value = true) => SetRegionFlag(0, species - 1, value);
    public void SetSeen(ushort species, bool value = true) => SetRegionFlag(1, species - 1, value);
    public void SetSeenGenderFirst(ushort species, int value = 0) => SetRegionFlag(2, species - 1, value == 1);
    public void SetSeenGenderSecond(ushort species, int value = 0) => SetRegionFlag(3, species - 1, value == 1);

    private void SetRegionFlag(int region, int index, bool value)
    {
        var ofs = 4 + (region * SIZE_REGION) + (index >> 3);
        FlagUtil.SetFlag(Data, ofs, index, value);
    }

    public uint SpindaPID { get => ReadUInt32LittleEndian(Data[OFS_SPINDA..]); set => WriteUInt32LittleEndian(Data[OFS_SPINDA..], value); }

    public static string[] GetFormNames4Dex(ushort species)
    {
        string[] formNames = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, [], EntityContext.Gen4);
        if (species == (int)Species.Pichu)
            formNames = [MALE, FEMALE, formNames[1]]; // Spiky
        return formNames;
    }

    public const byte FORM_NONE = byte.MaxValue;

    public byte[] GetForms(ushort species)
    {
        const int brSize = 0x40;
        if (species == (int)Species.Deoxys)
        {
            var br1 = Data[0x4 + (1 * brSize) - 1];
            var br2 = Data[0x4 + (2 * brSize) - 1];
            uint val = (uint)(br1 | (br2 << 8));
            return GetDexFormValues(val, 4, 4);
        }

        const int FormOffset1 = 4 + (4 * brSize) + 4;
        switch (species)
        {
            case (int)Species.Shellos: // Shellos
                return GetDexFormValues(Data[FormOffset1 + 0], 1, 2);
            case (int)Species.Gastrodon: // Gastrodon
                return GetDexFormValues(Data[FormOffset1 + 1], 1, 2);
            case (int)Species.Burmy: // Burmy
                return GetDexFormValues(Data[FormOffset1 + 2], 2, 3);
            case (int)Species.Wormadam: // Wormadam
                return GetDexFormValues(Data[FormOffset1 + 3], 2, 3);
            case (int)Species.Unown: // Unown
                return Data.Slice(FormOffset1 + 4, 0x1C).ToArray();
        }
        if (DP)
            return [];

        int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
        int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
        return species switch
        {
            (int)Species.Rotom => GetDexFormValues(ReadUInt32LittleEndian(Data[FormOffset2..]), 3, 6),
            (int)Species.Shaymin => GetDexFormValues(Data[FormOffset2 + 4], 1, 2),
            (int)Species.Giratina => GetDexFormValues(Data[FormOffset2 + 5], 1, 2),
            (int)Species.Pichu when HGSS => GetDexFormValues(Data[FormOffset2 + 6], 2, 3),
            _ => [],
        };
    }

    public void SetForms(ushort species, ReadOnlySpan<byte> forms)
    {
        const int brSize = 0x40;
        switch (species)
        {
            case (int)Species.Deoxys: // Deoxys
                uint newval = SetDexFormValues(forms, 4, 4);
                Data[0x4 + (1 * brSize) - 1] = (byte)(newval & 0xFF);
                Data[0x4 + (2 * brSize) - 1] = (byte)((newval >> 8) & 0xFF);
                break;
        }

        const int FormOffset1 = OFS_FORM1;
        switch (species)
        {
            case (int)Species.Shellos: // Shellos
                Data[FormOffset1 + 0] = (byte)SetDexFormValues(forms, 1, 2);
                return;
            case (int)Species.Gastrodon: // Gastrodon
                Data[FormOffset1 + 1] = (byte)SetDexFormValues(forms, 1, 2);
                return;
            case (int)Species.Burmy: // Burmy
                Data[FormOffset1 + 2] = (byte)SetDexFormValues(forms, 2, 3);
                return;
            case (int)Species.Wormadam: // Wormadam
                Data[FormOffset1 + 3] = (byte)SetDexFormValues(forms, 2, 3);
                return;
            case (int)Species.Unown: // Unown
                var unown = Data.Slice(FormOffset1 + 4, 0x1C);
                forms.CopyTo(unown);
                if (forms.Length != unown.Length)
                    unown[forms.Length..].Fill(FORM_NONE);
                return;
        }

        if (DP)
            return;

        int PokeDexLanguageFlags = FormOffset1 + (HGSS ? 0x3C : 0x20);
        int FormOffset2 = PokeDexLanguageFlags + 0x1F4;
        switch (species)
        {
            case (int)Species.Rotom: // Rotom
                var value = SetDexFormValues(forms, 3, 6);
                WriteUInt32LittleEndian(Data[FormOffset2..], value);
                return;
            case (int)Species.Shaymin: // Shaymin
                Data[FormOffset2 + 4] = (byte)SetDexFormValues(forms, 1, 2);
                return;
            case (int)Species.Giratina: // Giratina
                Data[FormOffset2 + 5] = (byte)SetDexFormValues(forms, 1, 2);
                return;
            case (int)Species.Pichu when HGSS: // Pichu
                Data[FormOffset2 + 6] = (byte)SetDexFormValues(forms, 2, 3);
                return;
        }
    }

    private static byte[] GetDexFormValues(uint Value, int BitsPerForm, int readCt)
    {
        byte[] Forms = new byte[readCt];
        int n1 = 0xFF >> (8 - BitsPerForm);
        for (int i = 0; i < Forms.Length; i++)
        {
            int val = (int)(Value >> (i * BitsPerForm)) & n1;
            if (n1 == val && BitsPerForm > 1)
                Forms[i] = byte.MaxValue;
            else
                Forms[i] = (byte)val;
        }

        // (BitsPerForm > 1) was already handled, handle (BitsPerForm == 1)
        if (BitsPerForm == 1 && Forms[0] == Forms[1] && Forms[0] == 1)
            Forms[0] = Forms[1] = byte.MaxValue;

        return Forms;
    }

    private static uint SetDexFormValues(ReadOnlySpan<byte> Forms, int BitsPerForm, int readCt)
    {
        int n1 = 0xFF >> (8 - BitsPerForm);
        uint Value = 0xFFFFFFFF << (readCt * BitsPerForm);
        for (int i = 0; i < Forms.Length; i++)
        {
            int val = Forms[i];
            if (val == -1)
                val = n1;

            Value |= (uint)(val << (BitsPerForm * i));
            if (i >= readCt)
                throw new ArgumentOutOfRangeException(nameof(readCt), "Array count should be less than bitfield count");
        }
        return Value;
    }

    private static bool TryInsertForm(Span<byte> forms, byte form)
    {
        if (forms.IndexOf(form) >= 0)
            return false; // already in list

        // insert at first empty
        var index = forms.IndexOf(FORM_NONE);
        if (index < 0)
            return false; // no free slots?

        forms[index] = form;
        return true;
    }

    private const byte UnownEmpty = byte.MaxValue;

    public int GetUnownFormIndex(byte form)
    {
        const int ofs = OFS_FORM1 + 4;
        for (byte i = 0; i < 0x1C; i++)
        {
            byte val = Data[ofs + i];
            if (val == form)
                return i;
            if (val == FORM_NONE) // end of populated indexes
                return UnownEmpty;
        }
        return UnownEmpty;
    }

    public int GetUnownFormIndexNext(byte form)
    {
        const int ofs = OFS_FORM1 + 4;
        for (int i = 0; i < 0x1C; i++)
        {
            byte val = Data[ofs + i];
            if (val == form)
                return i;
            if (val == FORM_NONE)
                return i;
        }

        return UnownEmpty;
    }

    public void ClearUnownForms()
    {
        const int ofs = OFS_FORM1 + 4;
        for (int i = 0; i < 0x1C; i++)
            Data[ofs + i] = FORM_NONE;
    }

    public bool GetUnownForm(byte form) => GetUnownFormIndex(form) != UnownEmpty;

    public void AddUnownForm(byte form)
    {
        var index = GetUnownFormIndexNext(form);
        if (index == UnownEmpty)
            return;

        const int ofs = OFS_FORM1 + 4;
        Data[ofs + index] = form;
    }

    public override void SetDex(PKM pk)
    {
        var species = pk.Species;
        if (species is 0 or > Legal.MaxSpeciesID_4)
            return;
        if (pk.IsEgg) // do not add
            return;

        var gender = pk.Gender;
        var form = pk.Form;
        var language = pk.Language;
        SetDex(species, gender, form, language);
    }

    private void SetDex(ushort species, byte gender, byte form, int language)
    {
        SetCaught(species);
        SetSeenGender(species, gender);
        SetSeen(species);
        SetForms(species, form, gender);
        SetLanguage(species, language);
    }

    public void SetSeenGender(ushort species, byte gender)
    {
        if (!GetSeen(species))
            SetSeenGenderNewFlag(species, gender);
        else if (GetSeenSingleGender(species))
            SetSeenGenderSecond(species, gender);
    }

    public void SetSeenGenderNewFlag(ushort species, byte gender)
    {
        SetSeenGenderFirst(species, gender);
        SetSeenGenderSecond(species, gender);
    }

    public void SetSeenGenderNeither(ushort species)
    {
        SetSeenGenderFirst(species, 0);
        SetSeenGenderSecond(species, 0);
    }

    private void SetForms(ushort species, byte form, byte gender)
    {
        if (species == (int)Species.Unown) // Unown
        {
            AddUnownForm(form);
            return;
        }

        var forms = GetForms(species);
        if (forms.Length == 0)
            return;

        if (species == (int)Species.Pichu && HGSS) // Pichu (HGSS Only)
        {
            var formID = form == 1 ? (byte)2 : gender;
            if (TryInsertForm(forms, formID))
                SetForms(species, forms);
        }
        else
        {
            if (TryInsertForm(forms, form))
                SetForms(species, forms);
        }
    }

    public void SetLanguage(ushort species, int language, bool value = true)
    {
        int lang = GetGen4LanguageBitIndex(language);
        SetLanguageBitIndex(species, lang, value);
    }

    public bool GetLanguageBitIndex(ushort species, int lang)
    {
        int dpl = 1 + DPLangSpecies.IndexOf(species);
        if (DP && dpl <= 0)
            return false;

        int PokeDexLanguageFlags = OFS_FORM1 + (HGSS ? 0x3C : 0x20);
        var ofs = PokeDexLanguageFlags + (DP ? dpl : species);
        return FlagUtil.GetFlag(Data, ofs, lang & 7);
    }

    public void SetLanguageBitIndex(ushort species, int lang, bool value)
    {
        int dpl = 1 + DPLangSpecies.IndexOf(species);
        if (DP && dpl <= 0)
            return;

        int PokeDexLanguageFlags = OFS_FORM1 + (HGSS ? 0x3C : 0x20);
        var ofs = PokeDexLanguageFlags + (DP ? dpl : species);
        FlagUtil.SetFlag(Data, ofs, lang & 7, value);
    }

    public bool HasLanguage(ushort species) => GetSpeciesLanguageByteIndex(species) >= 0;

    private int GetSpeciesLanguageByteIndex(ushort species)
    {
        if (DP)
            return DPLangSpecies.IndexOf(species);
        return species;
    }

    private static ReadOnlySpan<ushort> DPLangSpecies => [023, 025, 054, 077, 120, 129, 202, 214, 215, 216, 228, 278, 287, 315];

    public static int GetGen4LanguageBitIndex(int lang) => --lang switch
    {
        3 => 4, // invert ITA/GER
        4 => 3, // invert ITA/GER
        > 5 => 0, // Japanese
        < 0 => 1, // English
        _ => lang,
    };

    [Flags]
    public enum SetDexArgs
    {
        None,
        SeenAll = 1 << 0,

        CaughtNone = 1 << 1,
        CaughtAll = 1 << 2,

        SetNoLanguages = 1 << 3,
        SetAllLanguages = 1 << 4,
        SetSingleLanguage = 1 << 5,

        SetAllForms = 1 << 6,

        Complete = SeenAll | CaughtAll | SetAllLanguages | SetAllForms,
    }

    public void ModifyAll(ushort species, SetDexArgs args, int lang = 0)
    {
        if (args == SetDexArgs.None)
        {
            ClearSeen(species);
            return;
        }
        if ((args & SetDexArgs.SeenAll) != 0)
            CompleteSeen(species);

        if ((args & SetDexArgs.CaughtNone) != 0)
        {
            SetCaught(species, false);
            ClearLanguages(species);
        }
        else if ((args & SetDexArgs.CaughtAll) != 0)
        {
            SetCaught(species);
        }

        if ((args & SetDexArgs.SetNoLanguages) != 0)
        {
            ClearLanguages(species);
        }
        if ((args & SetDexArgs.SetAllLanguages) != 0)
        {
            SetLanguages(species);
        }
        else if ((args & SetDexArgs.SetSingleLanguage) != 0)
        {
            SetLanguage(species, lang);
        }

        if ((args & SetDexArgs.SetAllForms) != 0)
        {
            CompleteForms(species);
        }
    }

    private void CompleteForms(ushort species)
    {
        var forms = GetFormNames4Dex(species);
        if (forms.Length <= 1)
            return;

        Span<byte> values = stackalloc byte[forms.Length];
        for (byte i = 1; i < values.Length; i++)
            values[i] = i;
        SetForms(species, values);
    }

    private void CompleteSeen(ushort species)
    {
        SetSeen(species);
        var pi = PersonalTable.HGSS[species];
        if (pi.IsDualGender)
        {
            SetSeenGenderFirst(species, 0);
            SetSeenGenderSecond(species, 1);
        }
        else
        {
            SetSeenGenderNewFlag(species, (byte)(pi.FixedGender() & 1));
        }
    }

    public void ClearSeen(ushort species)
    {
        SetCaught(species, false);
        SetSeen(species, false);
        SetSeenGenderNeither(species);

        SetForms(species, []);
        ClearLanguages(species);
    }

    private const int LangCount = 6;
    private void ClearLanguages(ushort species)
    {
        for (int i = 0; i < 8; i++)
            SetLanguageBitIndex(species, i, false);
    }

    private void SetLanguages(ushort species, bool value = true)
    {
        for (int i = 0; i < LangCount; i++)
            SetLanguageBitIndex(species, i, value);
    }

    // Bulk Manipulation
    public override void CompleteDex(bool shinyToo = false) => IterateAll(z => ModifyAll(z, SetDexArgs.Complete));
    public override void SeenNone() => IterateAll(ClearSeen);
    public override void CaughtNone() => IterateAll(z => SetCaught(z, false));
    public override void SeenAll(bool shinyToo = false) => IterateAll(CompleteSeen);

    public override void SetDexEntryAll(ushort species, bool shinyToo = false) => ModifyAll(species, SetDexArgs.Complete);
    public override void ClearDexEntryAll(ushort species) => ModifyAll(species, SetDexArgs.None);

    private static void IterateAll(Action<ushort> a)
    {
        for (ushort i = 1; i <= Legal.MaxSpeciesID_4; i++)
            a(i);
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        if (!value)
        {
            SeenNone();
            return;
        }
        IterateAll(CompleteSeen);
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        SeenAll();
        IterateAll(z => SetCaught(z));
    }
}
