using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public readonly ref struct PokeDexEntry9a
{
    public const int SIZE = 0x84;

    private readonly Span<byte> Data;
    // ReSharper disable once ConvertToPrimaryConstructor
    public PokeDexEntry9a(Span<byte> data) => Data = data;
    public void Clear() => Data.Clear();

    /*  
       Structure: 0x84 bytes
       0x00 u32 [form-flags] Captured
       0x04 u32 [form-flags] Seen
       0x08 u16 language obtained
       0x0A b8  IsNew
       0x0B u8  flags for genders seen, can be on mons you don't have
            01: male seen
            02: female seen
            04: genderless seen
       0x0C u32 [form-flags] Shiny Seen
       0x10 b8  Seen Mega
       0x11 b8  Seen Alpha

       0x12-0x59: ??? all zeroes

       0x5A u8 DisplayForm
       0x5B u8 DisplayGender
       0x5C b8 DisplayShiny

       0x5D-0x83: ??? all zeroes, last 2 bytes junk (alloc?)
     */

    private uint FlagsFormCaught { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    private uint FlagsFormSeen { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    private ushort FlagsLanguage { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    private ref byte FlagsIsNew => ref Data[0x0A];
    private ref byte FlagsGenderSeen => ref Data[0x0B];
    private uint FlagsShinySeen { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }
    private ref byte FlagIsMega => ref Data[0x10];
    private ref byte FlagIsAlpha => ref Data[0x11];

    //

    public byte DisplayForm { get => Data[0x5A]; set => Data[0x5A] = value; }
    public DisplayGender9a DisplayGender { get => (DisplayGender9a)Data[0x5B]; set => Data[0x5B] = (byte)value; }
    private byte DisplayShiny { get => Data[0x5C]; set => Data[0x5C] = value; }

    //

    #region Interaction
    public bool IsSeen => FlagsFormSeen != 0;
    public bool IsCaught => FlagsFormCaught != 0;
    public bool GetIsFormCaught(byte form) => (FlagsFormCaught & (1u << form)) != 0;
    public void SetIsFormCaught(byte form, bool value)
    {
        if (value)
            FlagsFormCaught |= 1u << form;
        else
            FlagsFormCaught &= ~(1u << form);
    }

    public bool GetIsFormSeen(byte form) => (FlagsFormSeen & (1u << form)) != 0;
    public void SetIsFormSeen(byte form, bool value)
    {
        if (value)
            FlagsFormSeen |= 1u << form;
        else
            FlagsFormSeen &= ~(1u << form);
    }

    public void SetIsFormsSeen(uint formsOr) => FlagsFormSeen |= formsOr;
    public void SetIsFormsCaught(uint formsOr) => FlagsFormCaught |= formsOr;
    public void SetIsShinySeen(uint formsOr) => FlagsShinySeen |= formsOr;

    public static int GetDexLangFlag(int lang) => (uint)lang switch
    {
        > (int)MaxLanguageID or 6 or <= 0 => 0, // invalid language
        // skip over langID 0 (unused) => [0-8]
        // skip over langID 6 (unused)
        >= 7 => lang - 2,
        _ => lang - 1,
    };

    private static int GetLanguageBitMask(int langIndex) => 1 << GetDexLangFlag(langIndex);

    public bool GetLanguageFlag(int langIndex) => (FlagsLanguage & GetLanguageBitMask(langIndex)) != 0;

    public void SetLanguageFlag(int langIndex, bool value)
    {
        var mask = GetLanguageBitMask(langIndex);
        if (value)
            FlagsLanguage |= (ushort)mask;
        else
            FlagsLanguage &= (ushort)~mask;
    }

    public bool GetDisplayIsNew() => FlagsIsNew != 0;
    public void SetDisplayIsNew(bool value) => FlagsIsNew = value ? (byte)1 : (byte)0;

    public bool GetIsGenderSeen(byte gender) => (FlagsGenderSeen & (1u << gender)) != 0;
    public void SetIsGenderSeen(byte gender, bool value)
    {
        gender = Math.Clamp(gender, (byte)0, (byte)2); // M/F/Genderless
        var flag = 1u << gender;
        if (value)
            FlagsGenderSeen |= (byte)flag;
        else
            FlagsGenderSeen &= (byte)~flag;
    }

    public bool GetIsShinySeen(byte form) => (FlagsShinySeen & (1u << form)) != 0;
    public void SetIsShinySeen(byte form, bool value)
    {
        if (value)
            FlagsShinySeen |= 1u << form;
        else
            FlagsShinySeen &= ~(1u << form);
    }

    public bool GetIsSeenMega(byte megaIndex) => ((FlagIsMega >> megaIndex) & 1) != 0;
    public bool GetIsSeenAlpha() => FlagIsAlpha != 0;

    public void SetIsSeenMega(byte megaIndex, bool value)
    {
        if (value)
            FlagIsMega |= (byte)(1u << megaIndex);
        else
            FlagIsMega &= (byte)~(1u << megaIndex);
    }

    public void SetIsSeenAlpha(bool value) => FlagIsAlpha = value ? (byte)1 : (byte)0;

    public DisplayGender9a GetDisplayGender(Gender gender, ushort species, byte form)
    {
        if (gender == Gender.Genderless)
            return DisplayGender9a.Genderless; // Genderless

        var pi = PersonalTable.ZA[species, form];
        if (!pi.IsDualGender)
        {
            if (pi.OnlyMale)
                return DisplayGender9a.Male;
            if (pi.OnlyFemale)
                return DisplayGender9a.Female;
            return DisplayGender9a.Genderless;
        }

        // Gender differences?
        if (BiGender.Contains(species) || BiGenderDLC.Contains(species))
            return gender == Gender.Male ? DisplayGender9a.Male : DisplayGender9a.Female;
        return DisplayGender9a.GenderedNoDifference;
    }

    /// <summary>
    /// Species with gender differences in the dex.
    /// </summary>
    /// <remarks>Unique models to display</remarks>
    private static ReadOnlySpan<ushort> BiGender =>
    [
        003, 025, 026, 064, 065, 123, 129, 130, 133,
        154, 208, 212, 214, 229,
        252, 253, 254, 255, 256, 257, 258, 259, 260, 307, 308, 315, 322, 323,
        407, 443, 444, 445, 449, 450, 459, 460,
        485, // Heatran
        668, // Pyroar
    ];

    private static ReadOnlySpan<ushort> BiGenderDLC =>
    [
        (int)Species.Zubat,
        (int)Species.Golbat, // no Crobat

        (int)Species.Torchic,
        (int)Species.Combusken,
        (int)Species.Blaziken,

        (int)Species.Starly,
        (int)Species.Staravia,
        (int)Species.Staraptor,
    ];

    public void SetDisplayGender(Gender gender, ushort species, byte form)
    {
        DisplayGender = GetDisplayGender(gender, species, form);
    }

    #endregion

    public void ClearDisplay()
    {
        DisplayGender = 0;
        DisplayForm = 0;
        DisplayShiny = 0;
    }

    public void ClearCaught()
    {
        FlagsFormCaught = 0;
        FlagsLanguage = 0;
        ClearDisplay();
    }

    public bool GetDisplayIsShiny() => DisplayShiny != 0;
    public void SetDisplayIsShiny(bool state) => DisplayShiny = state ? (byte)1 : (byte)0;

    private const LanguageID MaxLanguageID = LanguageID.SpanishL;
    private const ushort LanguageMask = 0x3FF; // 10 bits set, ^ with unused (6) skipped.
    public void SetLanguageFlagAll(bool value) => FlagsLanguage = value ? LanguageMask : (ushort)0;
}

public enum DisplayGender9a : byte
{
    Genderless = 0,
    Male = 1,
    Female = 2,
    GenderedNoDifference = 3,
}
