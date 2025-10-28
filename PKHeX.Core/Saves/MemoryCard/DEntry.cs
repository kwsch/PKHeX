using System;
using System.Buffers.Binary;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// GameCube Directory Entry structure
/// </summary>
/// <remarks>0x40 "header" of a GCI file.</remarks>
public readonly ref struct DEntry(Span<byte> data)
{
    public const int SIZE = 0x40;
    private readonly Span<byte> Data = data;

    public Span<byte> GameCode => Data[..4];
    public Span<byte> MakerCode => Data[4..6];
    public ref byte Unused6 => ref Data[6];
    public ref byte BannerAndIconFlags => ref Data[7];
    public Span<byte> FileName => Data[8..0x28];

    // Seconds since Jan 1, 2000
    public uint ModificationTime { get => BinaryPrimitives.ReadUInt32BigEndian(Data[0x28..]); set => BinaryPrimitives.WriteUInt32BigEndian(Data[0x28..], value); }
    public uint ImageOffset { get => BinaryPrimitives.ReadUInt32BigEndian(Data[0x2C..]); set => BinaryPrimitives.WriteUInt32BigEndian(Data[0x2C..], value); }

    // 2 bits per icon
    public ushort IconFormat { get => BinaryPrimitives.ReadUInt16BigEndian(Data[0x30..]); set => BinaryPrimitives.WriteUInt16BigEndian(Data[0x30..], value); }

    public ushort AnimationSpeed { get => BinaryPrimitives.ReadUInt16BigEndian(Data[0x32..]); set => BinaryPrimitives.WriteUInt16BigEndian(Data[0x32..], value); }

    public ref byte FilePermissions => ref Data[0x34];
    public ref byte CopyCounter => ref Data[0x35];

    public ushort FirstBlock { get => BinaryPrimitives.ReadUInt16BigEndian(Data[0x36..]); set => BinaryPrimitives.WriteUInt16BigEndian(Data[0x36..], value); }
    public ushort BlockCount { get => BinaryPrimitives.ReadUInt16BigEndian(Data[0x38..]); set => BinaryPrimitives.WriteUInt16BigEndian(Data[0x38..], value); }

    public ushort Unused3A { get => BinaryPrimitives.ReadUInt16BigEndian(Data[0x3A..]); set => BinaryPrimitives.WriteUInt16BigEndian(Data[0x3A..], value); }
    public uint CommentsAddress { get => BinaryPrimitives.ReadUInt32BigEndian(Data[0x3C..]); set => BinaryPrimitives.WriteUInt32BigEndian(Data[0x3C..], value); }

    public bool IsEmpty => BinaryPrimitives.ReadUInt32BigEndian(GameCode) is 0 or uint.MaxValue; // FF is "Uninitialized", but check for 0 to be sure.
    public DateTime ModificationDate => Epoch.AddSeconds(ModificationTime);

    public void CopyTo(DEntry savDEntry) => Data[..SIZE].CopyTo(savDEntry.Data[..SIZE]);
    private static DateTime Epoch => new(2000, 1, 1);
    public void SetModificationTime(DateTime dt) => ModificationTime = (uint)dt.Subtract(Epoch).TotalSeconds;

    public int SaveDataOffset => FirstBlock * SAV3GCMemoryCard.BLOCK_SIZE;
    public int SaveDataLength => BlockCount * SAV3GCMemoryCard.BLOCK_SIZE;
    public bool IsStartInvalid => FirstBlock < 5; // 5 is the first block that can be used for data

    public string GetSaveName(Encoding encoding)
    {
        Span<char> result = stackalloc char[4 + 1 + 2 + 1 + 0x20];
        encoding.GetChars(GameCode, result); // 4 bytes
        result[4] = '-';
        encoding.GetChars(MakerCode, result[5..]); // 2 bytes
        result[7] = '-';
        encoding.GetChars(FileName, result[8..]); // 0x20 bytes (max)

        var zero = result.IndexOf('\0');
        if (zero >= 0)
            result = result[..zero];

        return result.ToString();
    }
}
