using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for Champions data linked to HOME.
/// </summary>
public sealed class GameDataPC9 : HomeOptional1
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PC9;
    private const int SIZE = HomeCrypto.SIZE_4GAME_PC9;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPC9() : base(SIZE) { }
    public GameDataPC9(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPC9 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    /// <summary> Indicates if the data is currently living in Champions, and is thus locked out from interaction until returned. </summary>
    public bool IsTransferred { get => Data[0x00] != 0; set => Data[0x00] = value ? (byte)1 : (byte)0; }

    /// <summary> Time of last deposit into Champions. time_t (64-bit) in seconds since Unix epoch. </summary>
    public ulong Timestamp { get => ReadUInt64LittleEndian(Data[0x01..]); set => WriteUInt64LittleEndian(Data[0x01..], value); }

    /// <summary> Probably a GUID, since Champions uses Unity (C#) and this is 16 bytes long. Probably used by Champions to fetch the Champions' specific data while in that game. </summary>
    public Span<byte> TagSpan => Data.Slice(0x09, 0x10);
    public Guid Tag => new(TagSpan);

    #endregion
}
