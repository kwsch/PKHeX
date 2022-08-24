using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PA8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPA8 : HomeOptional1, IGameDataSide, IScaledSizeAbsolute
{
    private const int SIZE = HomeCrypto.SIZE_1GAME_PA8;
    private const HomeGameDataFormat Format = HomeGameDataFormat.PA8;

    public GameDataPA8() : base(Format, SIZE) { }
    public GameDataPA8(byte[] data, int offset = 0) : base(Format, SIZE, data, offset) { }
    public GameDataPA8 Clone() => new(ToArray(SIZE));
    public int CopyTo(Span<byte> result) => CopyTo(result, SIZE);

    #region Structure

    public bool IsAlpha { get => Data[Offset + 0x00] != 0; set => Data[Offset + 0x00] = (byte)(value ? 1 : 0); }
    public bool IsNoble { get => Data[Offset + 0x01] != 0; set => Data[Offset + 0x01] = (byte)(value ? 1 : 0); }
    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), value); }
    public byte HeightScalarCopy { get => Data[Offset + 0x04]; set => Data[Offset + 0x04] = value; }

    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x05)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x05), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x07)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x07), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x09)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x09), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0B)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0B), (ushort)value); }

    public int Move1_PP { get => Data[Offset + 0x0D]; set => Data[Offset + 0x0D] = (byte)value; }
    public int Move2_PP { get => Data[Offset + 0x0E]; set => Data[Offset + 0x0E] = (byte)value; }
    public int Move3_PP { get => Data[Offset + 0x0F]; set => Data[Offset + 0x0F] = (byte)value; }
    public int Move4_PP { get => Data[Offset + 0x10]; set => Data[Offset + 0x10] = (byte)value; }
    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x11)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x11), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x13)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x13), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x15)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x15), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x17)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x17), (ushort)value); }
    public byte GV_HP  { get => Data[Offset + 0x19]; set => Data[Offset + 0x19] = value; }
    public byte GV_ATK { get => Data[Offset + 0x1A]; set => Data[Offset + 0x1A] = value; }
    public byte GV_DEF { get => Data[Offset + 0x1B]; set => Data[Offset + 0x1B] = value; }
    public byte GV_SPE { get => Data[Offset + 0x1C]; set => Data[Offset + 0x1C] = value; }
    public byte GV_SPA { get => Data[Offset + 0x1D]; set => Data[Offset + 0x1D] = value; }
    public byte GV_SPD { get => Data[Offset + 0x1E]; set => Data[Offset + 0x1E] = value; }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x1F)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x1F), value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x23)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x23), value); }
    public int Ball { get => Data[Offset + 0x27]; set => Data[Offset + 0x27] = (byte)value; }

    public bool GetPurchasedRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x28 + ofs, index & 7);
    }

    public void SetPurchasedRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x28 + ofs, index & 7, value);
    }

    public bool GetPurchasedRecordFlagAny() => Array.FindIndex(Data, Offset + 0x28, 8, static z => z != 0) >= 0;

    public int GetPurchasedCount()
    {
        var value = ReadUInt64LittleEndian(Data.AsSpan(0x155));
        ulong result = 0;
        for (int i = 0; i < 64; i++)
            result += ((value >> i) & 1);
        return (int)result;
    }

    public bool GetMasteredRecordFlag(int index)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, Offset + 0x30 + ofs, index & 7);
    }

    public void SetMasteredRecordFlag(int index, bool value)
    {
        if ((uint)index > 63) // 8 bytes, 8 bits
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, Offset + 0x30 + ofs, index & 7, value);
    }

    public bool GetMasteredRecordFlagAny() => Array.FindIndex(Data, Offset + 0x30, 8, static z => z != 0) >= 0;

    public int Egg_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x38)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x38), (ushort)value); }
    public int Met_Location { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x3A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x3A), (ushort)value); }

    // Not stored.
    public PersonalInfo GetPersonalInfo(int species, int form) => PersonalTable.LA.GetFormEntry(species, form);
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
        pk.HeightScalarCopy = HeightScalarCopy;
        pk.HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.WeightAbsolute = pk.CalcWeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.GV_HP = GV_HP;
        pk.GV_ATK = GV_ATK;
        pk.GV_DEF = GV_DEF;
        pk.GV_SPE = GV_SPE;
        pk.GV_SPA = GV_SPA;
        pk.GV_SPD = GV_SPD;
        Data.AsSpan(Offset + 0x28, 8 + 8).CopyTo(pk.Data.AsSpan(0x155)); // Copy both bitflag regions
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
        if (pkh.DataPK8 is { } c)
            return new GameDataPA8 { Ball = c.Met_Location == Locations.HOME_SWLA ? (int)Core.Ball.LAPoke : c.Ball, Met_Location = c.Met_Location, Egg_Location = c.Egg_Location };
        return null;
    }
}
