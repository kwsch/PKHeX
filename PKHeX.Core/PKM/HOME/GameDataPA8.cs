using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PA8"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPA8 : HomeOptional1, IGameDataSide<PA8>, IScaledSizeAbsolute, IScaledSize3, IGameDataSplitAbility, IPokerusStatus
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PA8;
    private const int SIZE = HomeCrypto.SIZE_2GAME_PA8;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPA8() : base(SIZE) { }
    public GameDataPA8(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPA8 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    public bool IsAlpha { get => Data[0x00] != 0; set => Data[0x00] = (byte)(value ? 1 : 0); }
    public bool IsNoble { get => Data[0x01] != 0; set => Data[0x01] = (byte)(value ? 1 : 0); }
    public ushort AlphaMove { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public byte Scale { get => Data[0x04]; set => Data[0x04] = value; }

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
    public byte Ball { get => Data[0x27]; set => Data[0x27] = value; }

    private Span<byte> PurchasedRecord => Data.Slice(0x28, 8);
    public bool GetPurchasedRecordFlag(int index) => FlagUtil.GetFlag(PurchasedRecord, index >> 3, index & 7);
    public void SetPurchasedRecordFlag(int index, bool value) => FlagUtil.SetFlag(PurchasedRecord, index >> 3, index & 7, value);
    public bool GetPurchasedRecordFlagAny() => PurchasedRecord.ContainsAnyExcept<byte>(0);
    public int GetPurchasedCount() => System.Numerics.BitOperations.PopCount(ReadUInt64LittleEndian(PurchasedRecord));

    private Span<byte> MasteredRecord => Data.Slice(0x30, 8);
    public bool GetMasteredRecordFlag(int index) => FlagUtil.GetFlag(MasteredRecord, index >> 3, index & 7);
    public void SetMasteredRecordFlag(int index, bool value) => FlagUtil.SetFlag(MasteredRecord, index >> 3, index & 7, value);
    public bool GetMasteredRecordFlagAny() => MasteredRecord.ContainsAnyExcept<byte>(0);

    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x38..]); set => WriteUInt16LittleEndian(Data[0x38..], value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data[0x3A..]); set => WriteUInt16LittleEndian(Data[0x3A..], value); }

    public byte PokerusState { get => Data[0x3C]; set => Data[0x3C] = value; }
    public ushort Ability { get => ReadUInt16LittleEndian(Data[0x3D..]); set => WriteUInt16LittleEndian(Data[0x3D..], value); }
    public byte AbilityNumber { get => Data[0x3F]; set => Data[0x3F] = value; }

    // Not stored.
    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.LA.GetFormEntry(species, form);
    public int Move1_PPUps { get => 0; set { } }
    public int Move2_PPUps { get => 0; set { } }
    public int Move3_PPUps { get => 0; set { } }
    public int Move4_PPUps { get => 0; set { } }

    #endregion

    #region Conversion

    public void CopyTo(PA8 pk, PKH pkh)
    {
        this.CopyTo(pk);
        pk.IsAlpha = IsAlpha;
        pk.IsNoble = IsNoble;
        pk.AlphaMove = AlphaMove;
        pk.Scale = Scale;
        pk.HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.WeightAbsolute = pk.CalcWeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.GV_HP = GV_HP;
        pk.GV_ATK = GV_ATK;
        pk.GV_DEF = GV_DEF;
        pk.GV_SPE = GV_SPE;
        pk.GV_SPA = GV_SPA;
        pk.GV_SPD = GV_SPD;
        PurchasedRecord.CopyTo(pk.PurchasedRecord);
        MasteredRecord.CopyTo(pk.MasteredRecord);
        pk.PokerusState = PokerusState;
        pk.AbilityNumber = AbilityNumber;
        pk.Ability = Ability;
    }

    public void CopyFrom(PA8 pk, PKH pkh)
    {
        this.CopyFrom(pk);
        IsAlpha = pk.IsAlpha;
        IsNoble = pk.IsNoble;
        AlphaMove = pk.AlphaMove;
        pkh.HeightScalar = Scale = pk.Scale; // Overwrite Height
        HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        WeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        GV_HP = pk.GV_HP;
        GV_ATK = pk.GV_ATK;
        GV_DEF = pk.GV_DEF;
        GV_SPE = pk.GV_SPE;
        GV_SPA = pk.GV_SPA;
        GV_SPD = pk.GV_SPD;
        pk.PurchasedRecord.CopyTo(PurchasedRecord);
        pk.MasteredRecord.CopyTo(MasteredRecord);
        PokerusState = pk.PokerusState;
        AbilityNumber = (byte)pk.AbilityNumber;
        Ability = (ushort)pk.Ability;

        // Special: Add the Mark
        if (pk.IsAlpha)
            pkh.Core.RibbonMarkAlpha = true;
    }

    public PA8 ConvertToPKM(PKH pkh)
    {
        var pk = new PA8();
        pkh.CopyTo(pk);
        CopyTo(pk, pkh);

        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPA8? TryCreate(PKH pkh)
    {
        if (!PersonalTable.LA.IsPresentInGame(pkh.Species, pkh.Form))
            return null;

        var result = CreateInternal(pkh);
        if (result == null)
            return null;

        result.PopulateFromCore(pkh);
        return result;
    }

    private static GameDataPA8? CreateInternal(PKH pkh)
    {
        var side = GetNearestNeighbor(pkh);
        if (side == null)
            return null;

        var result = new GameDataPA8();
        result.InitializeFrom(side, pkh);
        return result;
    }

    private static IGameDataSide? GetNearestNeighbor(PKH pkh) => pkh.DataPK9 as IGameDataSide
                                                              ?? pkh.DataPB8 as IGameDataSide
                                                              ?? pkh.DataPK8 as IGameDataSide
                                                              ?? pkh.DataPB7;

    public void InitializeFrom(IGameDataSide side, PKH pkh)
    {
        Ball = GetLegendBall(side.Ball, pkh.LA);
        MetLocation = side.MetLocation != Locations.Default8bNone ? side.MetLocation : (ushort)0;
        EggLocation = side.EggLocation != Locations.Default8bNone ? side.EggLocation : (ushort)0;

        if (side is IScaledSize3 s3)
            Scale = s3.Scale;
        else
            Scale = pkh.HeightScalar;
        if (side is IPokerusStatus p)
            PokerusState = p.PokerusState;
        if (side is IGameDataSplitAbility a)
            AbilityNumber = a.AbilityNumber;
        else
            AbilityNumber = 1;

        PopulateFromCore(pkh);
    }

    private static byte GetLegendBall(byte ball, bool wasLA)
    {
        if (!wasLA)
            return ball;
        if (((Ball)ball).IsLegendBall())
            return ball;
        return (byte)Core.Ball.LAPoke;
    }

    private void PopulateFromCore(PKH pkh)
    {
        var pi = PersonalTable.LA.GetFormEntry(pkh.Species, pkh.Form);
        HeightAbsolute = PA8.GetHeightAbsolute(pi, pkh.HeightScalar);
        WeightAbsolute = PA8.GetWeightAbsolute(pi, pkh.HeightScalar, pkh.WeightScalar);
        Ability = (ushort)pi.GetAbilityAtIndex(AbilityNumber >> 1);

        var level = Experience.GetLevel(pkh.EXP, pi.EXPGrowth);
        this.ResetMoves(pkh.Species, pkh.Form, level, LearnSource8LA.Instance, EntityContext.Gen8a);
    }
}
