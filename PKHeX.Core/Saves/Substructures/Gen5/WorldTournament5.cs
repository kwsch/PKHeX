using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class WorldTournament5(Memory<byte> Raw)
{
    public const int SIZE = 0x1214;
    public const string Extension = "pwt";

    private Span<byte> Data => Raw.Span;
    public bool IsUninitialized => !Data.ContainsAnyExcept<byte>(0xFF, 0);

    private Span<byte> NameTrash => Data[0xA..0x54]; // 36 chars
    private Span<byte> DescriptionTrash => Data[0x54..0xEA]; // 74 chars
    private Span<byte> AboutTrash => Data[0xEA..0x1C8]; // 110 chars

    public const int NameLength = 36; // chars
    public const int DescriptionLength = 74; // chars
    public const int AboutLength = 110; // chars

    public string Name
    {
        get => StringConverter5.GetString(NameTrash);
        set => StringConverter5.SetString(NameTrash, value, NameLength, 0, StringConverterOption.ClearFF);
    }

    public string Description
    {
        get => StringConverter5.GetString(DescriptionTrash);
        set => StringConverter5.SetString(DescriptionTrash, value, DescriptionLength, 0, StringConverterOption.ClearFF);
    }

    public string About
    {
        get => StringConverter5.GetString(AboutTrash);
        set => StringConverter5.SetString(AboutTrash, value, AboutLength, 0, StringConverterOption.ClearFF);
    }

    public ushort Checksum
    {
        get => ReadUInt16LittleEndian(Data[^4..]);
        set => WriteUInt16LittleEndian(Data[^4..], value);
    }

    private ushort CalculateChecksum() => Checksums.CRC16_CCITT(Data[..^4]);
    public void RefreshChecksums() => Checksum = CalculateChecksum();
}
