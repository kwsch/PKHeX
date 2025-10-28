using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public readonly ref struct PokeDexEntry9Kitakami
{
    public const int SIZE = 0x20;

    private readonly Span<byte> Data;
    // ReSharper disable once ConvertToPrimaryConstructor
    public PokeDexEntry9Kitakami(Span<byte> data) => Data = data;

    /*  Structure: 0x20 bytes
        0x00 u32 bitflags for forms obtained
        0x04 u32 bitflags for forms seen
        0x08 u32 bitflags for forms heard of (book visible but empty for adjacent mons)

        0x0C u32 bitflags for previous forms seen, new if different from 0x4 and copies 0x8
        0x10 u16 language bitflags

        0x12 u8 flags for genders seen, can be on mons you don't have
             01 = male seen
             02 = female seen
             04 = genderless seen
        0x13 u8 bitflags for shiny seen, default always has bit 1
             01 = regular model available
             02 = shiny model available (seen)

        0x14 u8 form displayed (Paldea dex)
        0x15 u8 gender displayed (0 = male, 1 = female, 2 = genderless) (Paldea dex)
        0x16 u8 bool for shiny displayed (Paldea dex)
        0x17 alignment

        0x18 u8 form displayed (Kitakami dex)
        0x19 u8 gender displayed (0 = male, 1 = female, 2 = genderless) (Kitakami dex)
        0x1A u8 bool for shiny displayed (Kitakami dex)
        0x1B alignment

        0x1C u8 form displayed (Blueberry dex)
        0x1D u8 gender displayed (0 = male, 1 = female, 2 = genderless) (Blueberry dex)
        0x1E u8 bool for shiny displayed (Blueberry dex)
        0x1F alignment
     */

    private uint FlagsFormObtained { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    private uint FlagsFormSeen { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    private uint FlagsFormHeard { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    private uint FlagsFormChecked { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }

    private ushort FlagsLanguage { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ref byte FlagsGenderSeen => ref Data[0x12];
    public ref byte FlagsShinySeen => ref Data[0x13]; // bit0 = regular, bit1 = shiny

    public ref byte DisplayedPaldeaForm => ref Data[0x14];
    public ref byte DisplayedPaldeaGender => ref Data[0x15];
    public ref byte DisplayedPaldeaShiny => ref Data[0x16];

    public ref byte DisplayedKitakamiForm => ref Data[0x18];
    public ref byte DisplayedKitakamiGender => ref Data[0x19];
    public ref byte DisplayedKitakamiShiny => ref Data[0x1A];

    public ref byte DisplayedBlueberryForm => ref Data[0x1C];
    public ref byte DisplayedBlueberryGender => ref Data[0x1D];
    public ref byte DisplayedBlueberryShiny => ref Data[0x1E];

    public bool IsSeen => FlagsFormSeen != 0;
    public bool IsCaught => FlagsFormObtained != 0;
    public bool IsKnown => FlagsFormHeard != 0;

    private static bool GetFlag(ref byte flags, int bit) => (flags & (1 << bit)) != 0;

    private static void SetFlag(ref byte flags, int bit, bool value)
    {
        if (value)
            flags |= (byte)(1 << bit);
        else
            flags &= (byte)~(1 << bit);
    }

    public bool GetObtainedForm(byte form) => (FlagsFormObtained & (1u << form)) != 0;

    public void SetObtainedForm(byte form, bool value)
    {
        if (value)
            FlagsFormObtained |= 1u << form;
        else
            FlagsFormObtained &= ~(1u << form);
    }

    public bool GetSeenForm(byte form) => (FlagsFormSeen & (1u << form)) != 0;

    public void SetSeenForm(byte form, bool value)
    {
        if (value)
            FlagsFormSeen |= 1u << form;
        else
            FlagsFormSeen &= ~(1u << form);
    }

    public bool GetHeardForm(byte form) => (FlagsFormHeard & (1u << form)) != 0;

    public void SetHeardForm(byte form, bool value)
    {
        if (value)
            FlagsFormHeard |= 1u << form;
        else
            FlagsFormHeard &= ~(1u << form);
    }

    public bool GetCheckedForm(byte form) => (FlagsFormChecked & (1u << form)) != 0;

    public void SetCheckedForm(byte form, bool value)
    {
        if (value)
            FlagsFormChecked |= 1u << form;
        else
            FlagsFormChecked &= ~(1u << form);
    }

    public bool GetIsAnyFormNotViewedShowNew() => FlagsFormChecked != FlagsFormHeard;

    public static int GetDexLangFlag(int lang) => lang switch
    {
        > 10 or 6 or <= 0 => 0, // invalid language
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

    public bool GetIsGenderSeen(byte gender) => GetFlag(ref FlagsGenderSeen, gender);
    public void SetIsGenderSeen(byte gender, bool value) => SetFlag(ref FlagsGenderSeen, gender, value);

    public bool GetIsModelSeen(bool shiny) => GetFlag(ref FlagsShinySeen, shiny ? 1 : 0);
    public void SetIsModelSeen(bool shiny, bool value) => SetFlag(ref FlagsShinySeen, shiny ? 1 : 0, value);

    public void Clear()
    {
        Data.Clear();
        this.FlagsShinySeen = 1;
    }

    public void SetAllLanguageFlags(ushort value = 0b11_1111_1111) => FlagsLanguage = value; // 10 languages

    public void ClearCaught()
    {
        FlagsFormObtained = 0;
        FlagsLanguage = 0;
        ClearLocalPaldea();
        ClearLocalKitakami();
        ClearLocalBlueberry();
    }

    private void ClearLocalPaldea() => SetLocalPaldea(0, 0, 0);
    private void ClearLocalKitakami() => SetLocalKitakami(0, 0, 0);
    private void ClearLocalBlueberry() => SetLocalBlueberry(0, 0, 0);

    public void RegisterFormFlags(byte form, bool value = true, bool hasChecked = false)
    {
        SetObtainedForm(form, value);
        SetSeenForm(form, value);
        SetHeardForm(form, value);
        SetCheckedForm(form, value && hasChecked);
    }

    public void SetLocalStates(PersonalInfo9SV pi, byte form, byte gender, bool shiny)
    {
        if (this.DisplayedPaldeaForm == 0 && pi.DexPaldea != 0)
            SetLocalPaldea(form, gender, shiny ? (byte)1 : (byte)0);
        if (this.DisplayedKitakamiForm == 0 && pi.DexKitakami != 0)
            SetLocalKitakami(form, gender, shiny ? (byte)1 : (byte)0);
        if (this.DisplayedBlueberryForm == 0 && pi.DexBlueberry != 0)
            SetLocalBlueberry(form, gender, shiny ? (byte)1 : (byte)0);
    }

    public void SetLocalPaldea(byte form, byte gender, byte sv)
    {
        this.DisplayedPaldeaForm = form;
        this.DisplayedPaldeaGender = gender;
        this.DisplayedPaldeaShiny = sv;
    }

    public void SetLocalKitakami(byte form, byte gender, byte sv)
    {
        this.DisplayedKitakamiForm = form;
        this.DisplayedKitakamiGender = gender;
        this.DisplayedKitakamiShiny = sv;
    }

    public void SetLocalBlueberry(byte form, byte gender, byte sv)
    {
        this.DisplayedBlueberryForm = form;
        this.DisplayedBlueberryGender = gender;
        this.DisplayedBlueberryShiny = sv;
    }

    public void ClearSeen(byte form)
    {
        SetSeenForm(form, false);
        if (FlagsFormSeen == 0)
        {
            // Shouldn't have anything if no forms are seen
            Clear();
            return;
        }
        SetObtainedForm(form, false);
        SetHeardForm(form, false);
        SetCheckedForm(form, false);
        if (DisplayedPaldeaForm == form)
            ClearLocalPaldea();
        if (DisplayedKitakamiForm == form)
            ClearLocalKitakami();
        if (DisplayedBlueberryForm == form)
            ClearLocalBlueberry();
    }
}
