using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PB8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPB8 : HomeOptional1, IGameDataSide
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PB8;
    private const int SIZE = HomeCrypto.SIZE_1GAME_PB8;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPB8() : base(SIZE) { }
    public GameDataPB8(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPB8 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    public ushort Move1 { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x04..]); set => WriteUInt16LittleEndian(Data[0x04..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x06..]); set => WriteUInt16LittleEndian(Data[0x06..], value); }

    public int Move1_PP { get => Data[0x08]; set => Data[0x08] = (byte)value; }
    public int Move2_PP { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    public int Move3_PP { get => Data[0x0A]; set => Data[0x0A] = (byte)value; }
    public int Move4_PP { get => Data[0x0B]; set => Data[0x0B] = (byte)value; }
    public int Move1_PPUps { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
    public int Move2_PPUps { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
    public int Move3_PPUps { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public int Move4_PPUps { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], value); }

    private Span<byte> RecordFlag => Data.Slice(0x18, 14);
    public bool GetMoveRecordFlag(int index) => FlagUtil.GetFlag(RecordFlag, index >> 3, index & 7);
    public void SetMoveRecordFlag(int index, bool value) => FlagUtil.SetFlag(RecordFlag, index >> 3, index & 7, value);
    public bool GetMoveRecordFlagAny() => RecordFlag.IndexOfAnyExcept<byte>(0) >= 0;
    public void ClearMoveRecordFlags() => RecordFlag.Clear();

    public int Ball { get => Data[0x26]; set => Data[0x26] = (byte)value; }
    public int Egg_Location { get => ReadUInt16LittleEndian(Data[0x27..]); set => WriteUInt16LittleEndian(Data[0x27..], (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data[0x29..]); set => WriteUInt16LittleEndian(Data[0x29..], (ushort)value); }

    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.BDSP.GetFormEntry(species, form);

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
        if (pkh.DataPK9 is { } g9)
            return Create(g9);
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
