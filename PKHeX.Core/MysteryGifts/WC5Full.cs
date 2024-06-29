using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class WC5Full
{
    public const int Size = 0x2D0;
    public readonly byte[] Data;
    public readonly PGF Gift;

    private Span<byte> Metadata => Data.AsSpan(PGF.Size);

    public WC5Full(byte[] data)
    {
        Data = data;
        var wc5 = data.AsSpan(0, PGF.Size).ToArray();
        Gift = new PGF(wc5)
        {
            Date = EncounterDate.GetDateNDS(),
            RestrictVersion = RestrictVersion,
            RestrictLanguage = RestrictLanguage,
        };
    }

    public byte RestrictVersion { get => Metadata[2]; set => Metadata[2] = value; }
    // 1 byte unused, since ^ is u16

    // 506 bytes for distribution text
    public Span<byte> DistributionTextData => Metadata[4..^6];

    public string DistributionText
    {
        get => StringConverter5.GetString(DistributionTextData);
        set => StringConverter5.SetString(DistributionTextData, value, (DistributionTextData.Length / 2) - 1, 0, StringConverterOption.ClearFF);
    }

    public byte DownloadAnimation { get => Metadata[^6]; set => Metadata[^6] = value; }
    public byte RestrictLanguage { get => Metadata[^5]; set => Metadata[^5] = value; }

    // u16 unused
    public ushort Checksum { get => ReadUInt16LittleEndian(Metadata[^2..]); set => WriteUInt16LittleEndian(Metadata[^2..], value); }
}
