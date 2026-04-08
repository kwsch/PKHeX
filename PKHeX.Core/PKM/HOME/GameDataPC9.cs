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

    /// <summary> Indicates if the data currently resides in Champions. </summary>
    public ChampionsTransferState State { get => (ChampionsTransferState)Data[0x00]; set => Data[0x00] = (byte)value; }

    /// <summary> Time of last sync with Champions (deposit/return). time_t (64-bit) in seconds since Unix epoch. </summary>
    public ulong Timestamp { get => ReadUInt64LittleEndian(Data[0x01..]); set => WriteUInt64LittleEndian(Data[0x01..], value); }

    /// <summary> Probably a GUID, since Champions uses Unity (C#) and this is 16 bytes long. Probably used by Champions to fetch the Champions' specific data while in that game. </summary>
    public Span<byte> TagSpan => Data.Slice(0x09, 0x10);
    public Guid Tag => new(TagSpan);

    #endregion
}

/// <summary>
/// Represents the transfer state of an entity with respect to the Pokémon Champions ecosystem.
/// </summary>
public enum ChampionsTransferState : byte
{
    /// <summary> Never transferred into Champions. Not really a valid state, as value should be one of the other options once initialized. </summary>
    None = 0,
    /// <summary> Indicates that the entity is currently deposited into Champions, and is thus locked out from interaction until returned. </summary>
    Transferred = 1,
    /// <summary> Indicates that the entity has been returned from Champions and is no longer locked out from interaction. </summary>
    Returned = 2,
}
