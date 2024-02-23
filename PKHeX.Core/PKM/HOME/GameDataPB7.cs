using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PB7"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPB7 : HomeOptional1, IGameDataSide<PB7>, IScaledSizeAbsolute, IMemoryOT, IGameDataSplitAbility
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PB7;
    private const int SIZE = HomeCrypto.SIZE_2GAME_PB7;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPB7() : base(SIZE) { }
    public GameDataPB7(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPB7 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    public byte AV_HP  { get => Data[0x00]; set => Data[0x00] = value; }
    public byte AV_ATK { get => Data[0x01]; set => Data[0x01] = value; }
    public byte AV_DEF { get => Data[0x02]; set => Data[0x02] = value; }
    public byte AV_SPE { get => Data[0x03]; set => Data[0x03] = value; }
    public byte AV_SPA { get => Data[0x04]; set => Data[0x04] = value; }
    public byte AV_SPD { get => Data[0x05]; set => Data[0x05] = value; }
    public byte ResortEventState { get => Data[0x06]; set => Data[0x06] = value; }

    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x07..]); set => WriteUInt16LittleEndian(Data[0x07..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x09..]); set => WriteUInt16LittleEndian(Data[0x09..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x0B..]); set => WriteUInt16LittleEndian(Data[0x0B..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x0D..]); set => WriteUInt16LittleEndian(Data[0x0D..], value); }

    public int Move1_PP { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
    public int Move2_PP { get => Data[0x10]; set => Data[0x10] = (byte)value; }
    public int Move3_PP { get => Data[0x11]; set => Data[0x11] = (byte)value; }
    public int Move4_PP { get => Data[0x12]; set => Data[0x12] = (byte)value; }
    public int Move1_PPUps { get => Data[0x13]; set => Data[0x13] = (byte)value; }
    public int Move2_PPUps { get => Data[0x14]; set => Data[0x14] = (byte)value; }
    public int Move3_PPUps { get => Data[0x15]; set => Data[0x15] = (byte)value; }
    public int Move4_PPUps { get => Data[0x16]; set => Data[0x16] = (byte)value; }

    public ushort RelearnMove1  { get => ReadUInt16LittleEndian(Data[0x17..]); set => WriteUInt16LittleEndian(Data[0x17..], value); }
    public ushort RelearnMove2  { get => ReadUInt16LittleEndian(Data[0x19..]); set => WriteUInt16LittleEndian(Data[0x19..], value); }
    public ushort RelearnMove3  { get => ReadUInt16LittleEndian(Data[0x1B..]); set => WriteUInt16LittleEndian(Data[0x1B..], value); }
    public ushort RelearnMove4  { get => ReadUInt16LittleEndian(Data[0x1D..]); set => WriteUInt16LittleEndian(Data[0x1D..], value); }
    public float HeightAbsolute { get => ReadSingleLittleEndian(Data[0x1F..]); set => WriteSingleLittleEndian(Data[0x1F..], value); }
    public float WeightAbsolute { get => ReadSingleLittleEndian(Data[0x23..]); set => WriteSingleLittleEndian(Data[0x23..], value); }

    public byte FieldEventFatigue1 { get => Data[0x27]; set => Data[0x27] = value; }
    public byte FieldEventFatigue2 { get => Data[0x28]; set => Data[0x28] = value; }
    public byte Fullness { get => Data[0x29]; set => Data[0x29] = value; }
    public byte Rank { get => Data[0x2A]; set => Data[0x2A] = value; }
    public int OriginalTrainerAffection { get => Data[0x2B]; set => Data[0x2B] = (byte)value; }
    public byte OriginalTrainerMemoryIntensity { get => Data[0x2C]; set => Data[0x2C] = value; }
    public byte OriginalTrainerMemory { get => Data[0x2D]; set => Data[0x2D] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data[0x2E..]); set => WriteUInt16LittleEndian(Data[0x2E..], value); }
    public byte OriginalTrainerMemoryFeeling { get => Data[0x30]; set => Data[0x30] = value; }
    public byte Enjoyment { get => Data[0x31]; set => Data[0x31] = value; }
    public uint GeoPadding { get => ReadUInt32LittleEndian(Data[0x32..]); set => WriteUInt32LittleEndian(Data[0x32..], value); }
    public byte Ball { get => Data[0x36]; set => Data[0x36] = value; }
    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x37..]); set => WriteUInt16LittleEndian(Data[0x37..], value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data[0x39..]); set => WriteUInt16LittleEndian(Data[0x39..], value); }

    public byte PokerusState { get => Data[0x3B]; set => Data[0x3B] = value; }
    public ushort Ability { get => ReadUInt16LittleEndian(Data[0x3C..]); set => WriteUInt16LittleEndian(Data[0x3C..], value); }
    public byte AbilityNumber { get => Data[0x3E]; set => Data[0x3E] = value; }

    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.GG.GetFormEntry(species, form);

    public void CopyTo(PB7 pk, PKH pkh)
    {
        this.CopyTo(pk);
        pk.AV_HP = AV_HP;
        pk.AV_ATK = AV_ATK;
        pk.AV_DEF = AV_DEF;
        pk.AV_SPE = AV_SPE;
        pk.AV_SPA = AV_SPA;
        pk.AV_SPD = AV_SPD;
        pk.ResortEventStatus = (ResortEventState)ResortEventState;
        pk.HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        pk.WeightAbsolute = pk.CalcWeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.

        // Some fields are unused as PB7, don't bother copying.
        pk.FieldEventFatigue1 = FieldEventFatigue1;
        pk.FieldEventFatigue2 = FieldEventFatigue2;
        pk.Fullness = Fullness;
        // pk.Rank = Rank;
        // pk.OriginalTrainerAffection
        // pk.OriginalTrainerMemoryIntensity
        // pk.OriginalTrainerMemory
        // pk.OriginalTrainerMemoryVariable
        // pk.OriginalTrainerMemoryFeeling
        pk.Enjoyment = Enjoyment;
        // pk.GeoPadding = GeoPadding;
        pk.AbilityNumber = AbilityNumber;
        pk.Ability = Ability;
    }

    public void CopyFrom(PB7 pk, PKH pkh)
    {
        this.CopyFrom(pk);
        AV_HP = pk.AV_HP;
        AV_ATK = pk.AV_ATK;
        AV_DEF = pk.AV_DEF;
        AV_SPE = pk.AV_SPE;
        AV_SPA = pk.AV_SPA;
        AV_SPD = pk.AV_SPD;
        ResortEventState = (byte)pk.ResortEventStatus;
        HeightAbsolute = pk.CalcHeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.
        WeightAbsolute = pk.CalcWeightAbsolute; // Ignore the stored value, be nice and recalculate for the user.

        // Some fields are unused as PB7, don't bother copying.
        FieldEventFatigue1 = pk.FieldEventFatigue1;
        FieldEventFatigue2 = pk.FieldEventFatigue2;
        Fullness = pk.Fullness;
        // Rank = pk.Rank;
        // OriginalTrainerAffection
        // OriginalTrainerMemoryIntensity
        // OriginalTrainerMemory
        // OriginalTrainerMemoryVariable
        // OriginalTrainerMemoryFeeling
        Enjoyment = pk.Enjoyment;
        // GeoPadding = pk.GeoPadding;
        AbilityNumber = (byte)pk.AbilityNumber;
        Ability = (ushort)pk.Ability;

        // All other side formats have HT Language. Just fake a value.
        if (pkh is { HandlingTrainerLanguage: 0, IsUntraded: false })
            pkh.HandlingTrainerLanguage = (byte)pk.Language;
    }

    public PB7 ConvertToPKM(PKH pkh)
    {
        var pk = new PB7();
        pkh.CopyTo(pk);
        CopyTo(pk, pkh);

        pk.ResetCalculatedValues();
        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPB7? TryCreate(PKH pkh)
    {
        if (!PersonalTable.GG.IsPresentInGame(pkh.Species, pkh.Form))
            return null;

        // There isn't an actual preference since this format cannot naturally backwards transfer.
        // Just pick out the first one.
        var result = CreateInternal(pkh);
        if (result == null)
            return null;

        result.PopulateFromCore(pkh);
        return result;
    }

    private static GameDataPB7? CreateInternal(PKH pkh)
    {
        var side = GetNearestNeighbor(pkh);
        if (side == null)
            return null;

        var ball = side.Ball;
        if (pkh.Version is GameVersion.GO)
            return new GameDataPB7 { Ball = ball, MetLocation = Locations.GO7 };
        if (pkh.Version is GameVersion.GP or GameVersion.GE)
            return new GameDataPB7 { Ball = ball, MetLocation = side.MetLocation };

        var result = new GameDataPB7();
        result.InitializeFrom(side, pkh);
        return result;
    }

    public void InitializeFrom(IGameDataSide side, PKH pkh)
    {
        MetLocation = side.MetLocation != Locations.Default8bNone ? side.MetLocation : (ushort)0;
        EggLocation = side.EggLocation != Locations.Default8bNone ? side.EggLocation : (ushort)0;

        if (side is IGameDataSplitAbility a)
            AbilityNumber = a.AbilityNumber;
        else
            AbilityNumber = 1;
    }

    private void PopulateFromCore(PKH pkh)
    {
        var pi = PersonalTable.GG.GetFormEntry(pkh.Species, pkh.Form);
        HeightAbsolute = PB7.GetHeightAbsolute(pi, pkh.HeightScalar);
        WeightAbsolute = PB7.GetWeightAbsolute(pi, pkh.HeightScalar, pkh.WeightScalar);
        Ability = (ushort)pi.GetAbilityAtIndex(AbilityNumber >> 1);
    }

    private static IGameDataSide? GetNearestNeighbor(PKH pkh) => pkh.DataPK9 as IGameDataSide
                                                              ?? pkh.DataPB8 as IGameDataSide
                                                              ?? pkh.DataPK8 as IGameDataSide
                                                              ?? pkh.DataPB7;

    public static T Create<T>(GameDataPB7 data) where T : IGameDataSide, new() => new()
    {
        Ball = data.Ball,
        MetLocation = data.MetLocation,
        EggLocation = data.EggLocation,
    };
}
