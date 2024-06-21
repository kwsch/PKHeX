using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public readonly ref struct HallFame6Entity
{
    public const int SIZE = 0x48;
    private readonly Span<byte> Data;
    private readonly int Language;

    // ReSharper disable once ConvertToPrimaryConstructor
    public HallFame6Entity(Span<byte> data, int language)
    {
        Data = data;
        Language = language;
    }

    public ushort Species { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort HeldItem { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x08..]); set => WriteUInt16LittleEndian(Data[0x08..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x0A..]); set => WriteUInt16LittleEndian(Data[0x0A..], value); }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[0x0C..]); set => WriteUInt32LittleEndian(Data[0x0C..], value); }

    public ushort TID16 { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    private uint Pack { get => ReadUInt32LittleEndian(Data[0x14..]); set => WriteUInt32LittleEndian(Data[0x14..], value); }

    public byte Form { get => (byte)(Pack & 0x1Fu); set => Pack = (Pack & ~0x1Fu) | (value & 0x1Fu); }
    public uint Gender { get => (Pack >> 05) & 0x03u; set => Pack = (Pack & ~(0x03u << 05)) | ((value & 0x03) << 05); }
    public uint Level { get => (Pack >> 07) & 0x7Fu; set => Pack = (Pack & ~(0x7Fu << 07)) | ((value & 0x7F) << 07); }
    private uint Shiny { get => (Pack >> 14) & 0x01u; set => Pack = (Pack & ~(0x01u << 14)) | ((value & 0x01) << 14); }
    private uint Nick { get => (Pack >> 15) & 0x01u; set => Pack = (Pack & ~(0x01u << 15)) | ((value & 0x01) << 15); }
    public uint OriginalTrainerGender { get => (Pack >> 16) & 0x01u; set => Pack = (Pack & ~(0x01u << 16)) | ((value & 0x01) << 16); }
    // remaining bits unused

    public bool IsNicknamed { get => Nick == 1; set => Nick = value ? 1u : 0u; }
    public bool IsShiny { get => Shiny == 1; set => Shiny = value ? 1u : 0u; }

    private Span<byte> NicknameTrash => Data.Slice(0x18, 24);
    private Span<byte> OriginalTrainerTrash => Data.Slice(0x30, 24);

    // Don't mimic in-game behavior of not clearing strings. First entry should always have clean trash.
    private const StringConverterOption Option = StringConverterOption.ClearZero;

    public void ClearTrash()
    {
        NicknameTrash.Clear();
        OriginalTrainerTrash.Clear();
    }

    public string Nickname
    {
        get => StringConverter6.GetString(NicknameTrash);
        set => StringConverter6.SetString(NicknameTrash, value, 12, Language, Option);
    }

    public string OriginalTrainerName
    {
        get => StringConverter6.GetString(OriginalTrainerTrash);
        set => StringConverter6.SetString(OriginalTrainerTrash, value, 12, Language, Option);
    }
}

public readonly ref struct HallFame6Index
{
    public const int SIZE = 0x4;
    private readonly Span<byte> Data;
    // ReSharper disable once ConvertToPrimaryConstructor
    public HallFame6Index(Span<byte> data) => Data = data;

    public uint Value { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }

    // Structure:
    // u32:14 - Clear Index (how many times Fame has been completed/cleared)
    // u32:08 - Year (since 2000)
    // u32:04 - Month
    // u32:05 - Day
    // u32:01 - Index has Data bool flag (0 = no data, 1 = has data)

    public uint ClearIndex { get => Value & 0x3FFFu; set => Value = (Value & ~0x3FFFu) | (value & 0x3FFFu); }
    public uint Year { get => (Value >> 14) & 0xFFu; set => Value = (Value & ~(0xFFu << 14)) | ((value & 0xFFu) << 14); }
    public uint Month { get => (Value >> 22) & 0xFu; set => Value = (Value & ~(0xFu << 22)) | ((value & 0xFu) << 22); }
    public uint Day { get => (Value >> 26) & 0x1Fu; set => Value = (Value & ~(0x1Fu << 26)) | ((value & 0x1Fu) << 26); }
    public bool HasData { get => (Value >> 31) == 1; set => Value = (Value & ~(0x1u << 31)) | ((value ? 1u : 0u) << 31); }
}
