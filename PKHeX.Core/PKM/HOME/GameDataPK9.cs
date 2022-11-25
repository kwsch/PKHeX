using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PK8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPK9 : HomeOptional1, IGameDataSide
{
    private const int SIZE = HomeCrypto.SIZE_1GAME_PK9;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PK9;

    public GameDataPK9() : base(Format, SIZE) { }
    public GameDataPK9(byte[] data, int offset = 0) : base(Format, SIZE, data, offset) { }
    public GameDataPK8 Clone() => new(ToArray(SIZE));
    public int CopyTo(Span<byte> result) => CopyTo(result, SIZE);

    #region Structure

    public ushort Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x04), value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x06), value); }

    public int Move1_PP    { get => Data[Offset + 0x08]; set => Data[Offset + 0x08] = (byte)value; }
    public int Move2_PP    { get => Data[Offset + 0x09]; set => Data[Offset + 0x09] = (byte)value; }
    public int Move3_PP    { get => Data[Offset + 0x0A]; set => Data[Offset + 0x0A] = (byte)value; }
    public int Move4_PP    { get => Data[Offset + 0x0B]; set => Data[Offset + 0x0B] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x0C]; set => Data[Offset + 0x0C] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x10), value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x16), value); }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x18), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x1A), (ushort)value); }

    public int Ball { get => Data[Offset + 0x1C]; set => Data[Offset + 0x1C] = (byte)value; }
    public byte Scale { get => Data[Offset + 0x1D]; set => Data[Offset + 0x1D] = value; }
    public MoveType TeraTypeOriginal { get => (MoveType)Data[Offset + 0x1E]; set => Data[Offset + 0x1E] = (byte)value; }
    public MoveType TeraTypeModified { get => (MoveType)Data[Offset + 0x1F]; set => Data[Offset + 0x1F] = (byte)value; }

    private const int RecordStart = 0x20;
    private const int RecordCount = PK9.COUNT_RECORD; // Up to 200 TM flags, but not all are used.
    private const int RecordLength = RecordCount / 8;
    private Span<byte> RecordFlags => Data.AsSpan(Offset + RecordStart, RecordLength);

    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > RecordCount) // 0x19 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, RecordStart + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value = true)
    {
        if ((uint)index > RecordCount) // 0x19 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, RecordStart + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, RecordStart, RecordLength, static z => z != 0) >= 0;

    public void ClearMoveRecordFlags() => Data.AsSpan(RecordStart, RecordLength).Clear();

    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.SV.GetFormEntry(species, form);

    public void CopyTo(PK9 pk)
    {
        ((IGameDataSide)this).CopyTo(pk);
        pk.Scale = Scale;
        pk.TeraTypeOriginal = TeraTypeOriginal;
        pk.TeraTypeOverride = TeraTypeModified;
        RecordFlags.CopyTo(pk.RecordFlags);
    }

    public PKM ConvertToPKM(PKH pkh) => ConvertToPK9(pkh);

    public PK9 ConvertToPK9(PKH pkh)
    {
        var pk = new PK9();
        pkh.CopyTo(pk);
        CopyTo(pk);
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPK9? TryCreate(PKH pkh)
    {
        var orig = pkh.LatestGameData;
        if (orig is GameDataPK9 pk9)
            return pk9;

        if (!PersonalTable.SV.IsPresentInGame(pkh.Species, pkh.Form))
            return null;

        var result = new GameDataPK9();
        orig.CopyTo(result);
        return result;
    }
}
