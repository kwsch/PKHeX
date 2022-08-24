using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PB8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPB8 : HomeOptional1, IGameDataSide
{
    private const int SIZE = HomeCrypto.SIZE_1GAME_PB8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PB8;

    public GameDataPB8() : base(Format, SIZE) { }
    public GameDataPB8(byte[] data, int offset = 0) : base(Format, SIZE, data, offset) { }
    public GameDataPB8 Clone() => new(ToArray(SIZE));
    public int CopyTo(Span<byte> result) => CopyTo(result, SIZE);

    #region Structure

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x04), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x06), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x08]; set => Data[Offset + 0x08] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x09]; set => Data[Offset + 0x09] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0A]; set => Data[Offset + 0x0A] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x0B]; set => Data[Offset + 0x0B] = (byte)value; }
    public int Move1_PPUps { get => Data[Offset + 0x0C]; set => Data[Offset + 0x0C] = (byte)value; }
    public int Move2_PPUps { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move3_PPUps { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move4_PPUps { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }

    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x10), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x16), (ushort)value); }
    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x18 + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value)
    {
        if ((uint)index > 112) // 14 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x18 + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => Array.FindIndex(Data, Offset + 0x18, 14, static z => z != 0) >= 0;

    public int Ball { get => Data[Offset + 0x26]; set => Data[Offset + 0x26] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x27)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x27), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x29)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x29), (ushort)value); }

    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.BDSP.GetFormEntry(species, form);

    public void CopyTo(PB8 pk)
    {
        ((IGameDataSide)this).CopyTo(pk);
        // Move Records are not settable in PB8; do not copy even if nonzero (illegal).
    }

    public PKM ConvertToPKM(PKH pkh) => ConvertToPB8(pkh);

    public PB8 ConvertToPB8(PKH pkh)
    {
        var pk = new PB8();
        pkh.CopyTo(pk);
        CopyTo(pk);
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPB8? TryCreate(PKH pkh)
    {
        if (pkh.DataPB7 is { } x)
            return Create(x);
        if (pkh.DataPK8 is { } b)
            return Create(b);
        if (pkh.DataPA8 is { } a)
            return Create(a);
        return null;
    }

    public static T Create<T>(GameDataPB8 data) where T : IGameDataSide, new() => new()
    {
        Ball = data.Ball,
        Met_Location = data.Met_Location == Locations.Default8bNone ? 0 : data.Met_Location,
        Egg_Location = data.Egg_Location == Locations.Default8bNone ? 0 : data.Egg_Location,
    };

    public static GameDataPB8 Create(IGameDataSide data) => new()
    {
        Ball = data.Ball,
        Met_Location = data.Met_Location == 0 ? Locations.Default8bNone : data.Met_Location,
        Egg_Location = data.Egg_Location == 0 ? Locations.Default8bNone : data.Egg_Location,
    };
}
