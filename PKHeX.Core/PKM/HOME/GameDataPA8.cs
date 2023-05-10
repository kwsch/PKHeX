using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PA8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPA8 : HomeOptional1, IGameDataSide, IScaledSizeAbsolute
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PA8;
    private const int SIZE = HomeCrypto.SIZE_1GAME_PA8;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPA8() : base(SIZE) { }
    public GameDataPA8(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPA8 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    public bool IsAlpha { get => Data[0x00] != 0; set => Data[0x00] = (byte)(value ? 1 : 0); }
    public bool IsNoble { get => Data[0x01] != 0; set => Data[0x01] = (byte)(value ? 1 : 0); }
    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public byte HeightScalarCopy { get => Data[0x04]; set => Data[0x04] = value; }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x05..]); set => WriteUInt16LittleEndian(Data[0x05..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x07..]); set => WriteUInt16LittleEndian(Data[0x07..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x09..]); set => WriteUInt16LittleEndian(Data[0x09..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x0B..]); set => WriteUInt16LittleEndian(Data[0x0B..], value); }

    public int Move1_PP { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
    public int Move2_PP { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public int Move3_PP { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
    public int Move4_PP { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x11..]); set => WriteUInt16LittleEndian(Data[0x11..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x13..]); set => WriteUInt16LittleEndian(Data[0x13..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x15..]); set => WriteUInt16LittleEndian(Data[0x15..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x17..]); set => WriteUInt16LittleEndian(Data[0x17..], value); }
    public byte GV_HP  { get => Data[0x19]; set => Data[0x19] = value; }
    public byte GV_ATK { get => Data[0x1A]; set => Data[0x1A] = value; }
    public byte GV_DEF { get => Data[0x1B]; set => Data[0x1B] = value; }
    public byte GV_SPE { get => Data[0x1C]; set => Data[0x1C] = value; }
    public byte GV_SPA { get => Data[0x1D]; set => Data[0x1D] = value; }
    public byte GV_SPD { get => Data[0x1E]; set => Data[0x1E] = value; }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data[0x1F..]); set => WriteSingleLittleEndian(Data[0x1F..], value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data[0x23..]); set => WriteSingleLittleEndian(Data[0x23..], value); }
    public int Ball { get => Data[0x27]; set => Data[0x27] = (byte)value; }

    private Span<byte> PurchasedRecord => Data.Slice(0x28, 8);
    public bool GetPurchasedRecordFlag(int index) => FlagUtil.GetFlag(PurchasedRecord, index >> 3, index & 7);
    public void SetPurchasedRecordFlag(int index, bool value) => FlagUtil.SetFlag(PurchasedRecord, index >> 3, index & 7, value);
    public bool GetPurchasedRecordFlagAny() => PurchasedRecord.IndexOfAnyExcept<byte>(0) >= 0;
    public int GetPurchasedCount() => System.Numerics.BitOperations.PopCount(ReadUInt64LittleEndian(PurchasedRecord));

    private Span<byte> MasteredRecord => Data.Slice(0x30, 8);
    public bool GetMasteredRecordFlag(int index) => FlagUtil.GetFlag(MasteredRecord, index >> 3, index & 7);
    public void SetMasteredRecordFlag(int index, bool value) => FlagUtil.SetFlag(MasteredRecord, index >> 3, index & 7, value);
    public bool GetMasteredRecordFlagAny() => MasteredRecord.IndexOfAnyExcept<byte>(0) >= 0;

    public int Egg_Location { get => ReadUInt16LittleEndian(Data[0x38..]); set => WriteUInt16LittleEndian(Data[0x38..], (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data[0x3A..]); set => WriteUInt16LittleEndian(Data[0x3A..], (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.LA.GetFormEntry(species, form);
    public int Move1_PPUps { get => 0; set { } }
    public int Move2_PPUps { get => 0; set { } }
    public int Move3_PPUps { get => 0; set { } }
    public int Move4_PPUps { get => 0; set { } }

    #endregion

    #region Conversion

    public void CopyTo(PA8 pk)
    {
        ((IGameDataSide)this).CopyTo(pk);
        pk.IsAlpha = IsAlpha;
        pk.IsNoble = IsNoble;
        pk.AlphaMove = AlphaMove;
        pk.Scale = HeightScalarCopy;
        pk.HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.WeightAbsolute = pk.CalcWeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.GV_HP = GV_HP;
        pk.GV_ATK = GV_ATK;
        pk.GV_DEF = GV_DEF;
        pk.GV_SPE = GV_SPE;
        pk.GV_SPA = GV_SPA;
        pk.GV_SPD = GV_SPD;
        Data.Slice(0x28, 8 + 8).CopyTo(pk.Data.AsSpan(0x155)); // Copy both bitflag regions
    }

    public PKM ConvertToPKM(PKH pkh) => ConvertToPA8(pkh);

    public PA8 ConvertToPA8(PKH pkh)
    {
        var pk = new PA8();
        pkh.CopyTo(pk);
        CopyTo(pk);
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPA8? TryCreate(PKH pkh)
    {
        if (pkh.DataPB7 is { } x)
            return GameDataPB7.Create<GameDataPA8>(x);
        if (pkh.DataPB8 is { } b)
            return GameDataPB8.Create<GameDataPA8>(b);
        if (pkh.DataPK9 is { } g9)
            return GameDataPK9.Create<GameDataPA8>(g9);
        if (pkh.DataPK8 is { } c)
            return new GameDataPA8 { Ball = c.Met_Location == Locations.HOME_SWLA ? (int)Core.Ball.LAPoke : c.Ball, Met_Location = c.Met_Location, Egg_Location = c.Egg_Location };
        return null;
    }
}
