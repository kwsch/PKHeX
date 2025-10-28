using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public readonly ref struct PokeDexEntry9Paldea
{
    public const int SIZE = 0x18;

    private readonly Span<byte> Data;
    // ReSharper disable once ConvertToPrimaryConstructor
    public PokeDexEntry9Paldea(Span<byte> data) => Data = data;
    public void Clear() => Data.Clear();

    /*  Structure: 0x18 bytes
        0x0 u32 0 = not shown, 1 = shown, don't have, 2 = seen, 3 = obtained
        0x4 u32 forms obtained (flags)
        0x8 u16 flags for genders seen, can be on mons you don't have
            01 = male seen
            02 = female seen
            04 = genderless seen
        0x0A u16 language obtained
        0x0C u8 shiny obtained (bool)
        0x0D u8 New
        0x0E u8 - unused alignment
        0x0F u8 - unused alignment
        0x10 u16? display form
        0x14 u8 display gender (0 = male, 1 = female, 2 = genderless)
        0x15 u8 display shiny (bool)
        0x16 u8 has gender difference (bool)
        0x17 u8 - unused alignment
     */

    public uint GetState() => ReadUInt32LittleEndian(Data);
    private uint FlagsFormSeen     { get => ReadUInt32LittleEndian(Data[0x04..]); set => WriteUInt32LittleEndian(Data[0x04..], value); }
    private ushort FlagsGenderSeen     { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    private ushort FlagsLanguage   { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }

    public bool GetSeenIsShiny() => Data[0x0C] != 0;
    public bool GetDisplayIsNew() => Data[0x0D] != 0;
    public byte GetUnused0E() => Data[0x0E];
    public byte GetUnused0F() => Data[0x0F];
    private uint DisplayFormIndex { get => ReadUInt32LittleEndian(Data[0x10..]); set => WriteUInt32LittleEndian(Data[0x10..], value); }
    private byte DisplayGender { get => Data[0x14]; set => Data[0x14] = value; }
    public bool GetDisplayIsShiny() => Data[0x15] != 0;
    public bool GetDisplayGenderIsDifferent() => Data[0x16] != 0;
    public byte GetUnused17() => Data[0x17];

    public void SetState(uint value) => WriteUInt32LittleEndian(Data, value);
    public void SetSeenIsShiny(bool value = true) => Data[0x0C] = value ? (byte)1 : (byte)0;
    public void SetDisplayIsNew(bool value = true) => Data[0x0D] = value ? (byte)1 : (byte)0;
    public void SetUnused0E(byte value) => Data[0x0E] = value;
    public void SetUnused0F(byte value) => Data[0x0F] = value;
    public void SetDisplayIsShiny(bool value = true) => Data[0x15] = value ? (byte)1 : (byte)0;
    public void SetDisplayGenderIsDifferent(bool value = true) => Data[0x16] = value ? (byte)1 : (byte)0;
    public void SetUnused17(byte value) => Data[0x17] = value;

    public bool IsUnknown => GetState() == 0;
    public bool IsKnown => GetState() != 0;
    public bool IsSeen => GetState() >= 2;
    public bool IsCaught => GetState() >= 3;

    public void SetCaught(bool value) => SetState(value ? 3u : 2u);
    public void SetSeen(bool value)
    {
        var newValue = !value ? 1 : Math.Min(GetState(), 2);
        SetState(newValue);
    }

    public bool GetIsGenderSeen(byte gender) => (FlagsGenderSeen & (1u << gender)) != 0;

    public void SetIsGenderSeen(byte gender, bool value)
    {
        if (value)
            FlagsGenderSeen |= (ushort)(1u << gender);
        else
            FlagsGenderSeen &= (ushort)~(1u << gender);
    }

    public bool GetIsFormSeen(byte form) => (FlagsFormSeen & (1u << form)) != 0;
    public void SetIsFormSeen(byte form, bool value)
    {
        if (value)
            FlagsFormSeen |= 1u << form;
        else
            FlagsFormSeen &= ~(1u << form);
    }

    public uint GetDisplayForm() => DisplayFormIndex;
    public void SetDisplayForm(uint form) => DisplayFormIndex = form;

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

    public uint GetDisplayGender() => DisplayGender switch
    {
        0 => 0,
        1 => 1,
        _ => 2,
    };

    public void SetDisplayGender(int value)
    {
        DisplayGender = value switch
        {
            0 => 0,
            1 => 1,
            _ => 2,
        };
    }

    public void ClearCaught()
    {
        SetCaught(false);
        SetSeenIsShiny(false);
        SetDisplayForm(0);
        SetDisplayGender(0);
        SetDisplayIsShiny(false);
        SetDisplayGenderIsDifferent(false);
        FlagsLanguage = 0;
    }
}
