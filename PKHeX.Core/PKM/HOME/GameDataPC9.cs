using System;
using System.Text;
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
    public bool IsTransferred { get => Data[0x00] != 0; set => Data[0x00] = value ? (byte)1 : (byte)0; }
    public ulong Timestamp { get => ReadUInt64LittleEndian(Data[0x01..]); set => WriteUInt64LittleEndian(Data[0x01..], value); }
    public Span<byte> TransferID { get => Data.Slice(0x09, 0x10); set => value.CopyTo(Data.Slice(0x09, 0x10)); }
    public string TransferText { get => Encoding.UTF8.GetString(TransferID); set => Encoding.UTF8.GetBytes(value, TransferID); } // todo, this is just a guess

    #endregion


    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.ZA.GetFormEntry(species, form);

    public PKM ConvertToPKM(PKH pkh) => throw new NotSupportedException("No conversion routine.");

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPC9? TryCreate(PKH pkh) => CreateInternal();

    private static GameDataPC9? CreateInternal()
    {
        var result = new GameDataPC9()
        {
            IsTransferred = true,
            Timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            // TODO HOME CP -- transfer ID?
        };
        return result;
    }
}
