using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

// TODO -- NOTHING HERE IS CORRECT

/// <summary>
/// Side game data for Champions data linked to HOME (assumedly, not FR/LG).
/// </summary>
public sealed class GameDataPC9 : HomeOptional1, IScaledSize3, IGameDataSide
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PC9;
    private const int SIZE = HomeCrypto.SIZE_4GAME_PC9;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPC9() : base(SIZE) { }
    public GameDataPC9(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPC9 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure
    public byte Scale { get => Data[0x00]; set => Data[0x00] = value; }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x01..]); set => WriteUInt16LittleEndian(Data[0x01..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x03..]); set => WriteUInt16LittleEndian(Data[0x03..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x05..]); set => WriteUInt16LittleEndian(Data[0x05..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x07..]); set => WriteUInt16LittleEndian(Data[0x07..], value); }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x11..]); set => WriteUInt16LittleEndian(Data[0x11..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x13..]); set => WriteUInt16LittleEndian(Data[0x13..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x15..]); set => WriteUInt16LittleEndian(Data[0x15..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x17..]); set => WriteUInt16LittleEndian(Data[0x17..], value); }
    public byte Ball { get => Data[0x1B]; set => Data[0x1B] = value; }
    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }


    #endregion


    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.ZA.GetFormEntry(species, form);

    public PKM ConvertToPKM(PKH pkh) => throw new NotSupportedException("No conversion routine.");

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPC9? TryCreate(PKH pkh)
    {
        if (!PersonalTable.ZA.IsPresentInGame(pkh.Species, pkh.Form))
            return null;

        var result = CreateInternal(pkh);
        if (result is null)
            return null;

        result.PopulateFromCore(pkh);
        return result;
    }

    private static GameDataPC9? CreateInternal(PKH pkh)
    {
        var side = GetNearestNeighbor(pkh);
        if (side is null)
            return null;

        var result = new GameDataPC9();
        result.InitializeFrom(side, pkh);
        return result;
    }

    private static IGameDataSide? GetNearestNeighbor(PKH pkh)
        => pkh.DataPA9 ?? pkh.DataPK9 ?? pkh.DataPK8 ?? pkh.DataPB8 ?? pkh.DataPB7 ?? pkh.DataPA8 as IGameDataSide;

    public void InitializeFrom(IGameDataSide side, PKH pkh)
    {
        PopulateFromCore(pkh);
    }

    private void PopulateFromCore(PKH pkh)
    {
    }
}
