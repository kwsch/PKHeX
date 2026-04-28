using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data for <see cref="PA9"/> data transferred into HOME.
/// </summary>
public sealed class GameDataPA9 : HomeOptional1, IGameDataSide<PA9>, IScaledSize3, IGameDataSplitAbility, IAlpha
{
    private const HomeGameDataFormat ExpectFormat = HomeGameDataFormat.PA9;
    private const int SIZE = HomeCrypto.SIZE_4GAME_PA9;
    protected override HomeGameDataFormat Format => ExpectFormat;

    public GameDataPA9() : base(SIZE) { }
    public GameDataPA9(Memory<byte> data) : base(data) => EnsureSize(SIZE);
    public GameDataPA9 Clone() => new(ToArray());
    public int WriteTo(Span<byte> result) => WriteWithHeader(result);

    #region Structure

    public byte Scale { get => Data[0x00]; set => Data[0x00] = value; }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x01..]); set => WriteUInt16LittleEndian(Data[0x01..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x03..]); set => WriteUInt16LittleEndian(Data[0x03..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x05..]); set => WriteUInt16LittleEndian(Data[0x05..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x07..]); set => WriteUInt16LittleEndian(Data[0x07..], value); }
    public bool IsAlpha { get => Data[0x9] != 0; set => Data[0x9] = value ? (byte)1 : (byte)0; }

    // Second set of flags is stored in the second block of PA9, devs declared it matching block order, not sequential bitflag order.
    private const int PlusStartB = 0xA;
    internal const int PlusCountB = PA9.PlusCount1;
    private const int PlusLengthB = PlusCountB / 8; // 12
    public Span<byte> PlusFlagsB => Data.Slice(PlusStartB, PlusLengthB);

    // First set of flags is stored in the third block of PA9, devs declared it matching block order, not sequential bitflag order.
    private const int PlusStartC = PlusStartB + PlusLengthB;
    internal const int PlusCountC = PA9.PlusCount0;
    private const int PlusLengthC = PlusCountC / 8; // 33
    public Span<byte> PlusFlagsC => Data.Slice(PlusStartC, PlusLengthC);

    public byte Ball { get => Data[0x37]; set => Data[0x37] = value; }
    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x38..]); set => WriteUInt16LittleEndian(Data[0x38..], value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data[0x3A..]); set => WriteUInt16LittleEndian(Data[0x3A..], value); }
    public byte Obedience_Level { get => Data[0x3C]; set => Data[0x3C] = value; }
    public ushort Ability { get => ReadUInt16LittleEndian(Data[0x3D..]); set => WriteUInt16LittleEndian(Data[0x3D..], value); }
    public byte AbilityNumber { get => Data[0x3F]; set => Data[0x3F] = value; }

    // not sure how best to handle the dropping of these
    public ushort RelearnMove1 { get => 0; set { } }
    public ushort RelearnMove2 { get => 0; set { } }
    public ushort RelearnMove3 { get => 0; set { } }
    public ushort RelearnMove4 { get => 0; set { } }

    #endregion

    #region Plus Moves

    public bool GetMovePlusFlag(int index)
    {
        if ((uint)index >= PlusCountC)
            return GetMovePlusFlag1(index - PlusCountC);
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, PlusStartC + ofs, index & 7);
    }

    private bool GetMovePlusFlag1(int index)
    {
        if ((uint)index >= PlusCountB)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, PlusStartB + ofs, index & 7);
    }

    public void SetMovePlusFlag(int index, bool value = true)
    {
        if ((uint)index >= PlusCountC)
        {
            SetMovePlusFlag1(value, index - PlusCountC);
            return;
        }
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, PlusStartC + ofs, index & 7, value);
    }

    private void SetMovePlusFlag1(bool value, int index)
    {
        if ((uint)index >= PlusCountB)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, PlusStartB + ofs, index & 7, value);
    }

    public bool GetMovePlusFlagAny() => GetMovePlusFlagAny0() || GetMovePlusFlagAny1();
    private bool GetMovePlusFlagAny0() => PlusFlagsC.ContainsAnyExcept<byte>(0);
    private bool GetMovePlusFlagAny1() => PlusFlagsB.ContainsAnyExcept<byte>(0);

    public void ClearMovePlusFlags()
    {
        ClearMovePlusFlags0();
        ClearMovePlusFlags1();
    }

    private void ClearMovePlusFlags0() => PlusFlagsC.Clear();
    private void ClearMovePlusFlags1() => PlusFlagsB.Clear();
    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.ZA.GetFormEntry(species, form);

    public void CopyTo(PA9 pk, PKH pkh)
    {
        this.CopyTo(pk);
        pk.Scale = Scale;
        pk.IsAlpha = IsAlpha;
        if (IsAlpha)
            pk.Scale = pk.HeightScalar = pk.WeightScalar = 255;

        PlusFlagsC.CopyTo(pk.PlusFlags0);
        PlusFlagsB.CopyTo(pk.PlusFlags1);
        pk.ObedienceLevel = Obedience_Level;
        pk.Ability = Ability;
        pk.AbilityNumber = AbilityNumber;
    }

    public void CopyFrom(PA9 pk, PKH pkh)
    {
        this.CopyFrom(pk);
        pkh.HeightScalar = Scale = pk.Scale; // Overwrite Height
        IsAlpha = pk.IsAlpha;
        pk.PlusFlags0.CopyTo(PlusFlagsC);
        pk.PlusFlags1.CopyTo(PlusFlagsB);
        Obedience_Level = pk.ObedienceLevel;
        Ability = (ushort)pk.Ability;
        AbilityNumber = (byte)pk.AbilityNumber;
    }

    public PA9 ConvertToPKM(PKH pkh)
    {
        var pk = new PA9();
        pkh.CopyTo(pk);
        CopyTo(pk, pkh);
        pk.Move1_PP = pk.Move2_PP = pk.Move3_PP = pk.Move4_PP = 0; // Match HOME's behavior of zero PP.

        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    #endregion

    /// <summary> Reconstructive logic to best apply suggested values. </summary>
    public static GameDataPA9? TryCreate(PKH pkh)
    {
        if (!PersonalTable.ZA.IsPresentInGame(pkh.Species, pkh.Form))
            return null;

        var result = CreateInternal(pkh);
        if (result is null)
            return null;

        result.PopulateFromCore(pkh);
        return result;
    }

    private static GameDataPA9? CreateInternal(PKH pkh)
    {
        var side = GetNearestNeighbor(pkh);
        if (side is null)
            return null;

        var result = new GameDataPA9();
        result.InitializeFrom(side, pkh);
        return result;
    }

    private static IGameDataSide? GetNearestNeighbor(PKH pkh)
        => pkh.DataPK9 ?? pkh.DataPK8 ?? pkh.DataPB8 ?? pkh.DataPB7 ?? pkh.DataPA8 as IGameDataSide;

    public void InitializeFrom(IGameDataSide side, PKH pkh)
    {
        Ball = side.Ball;
        MetLocation = side.MetLocation != Locations.Default8bNone ? side.MetLocation : (ushort)0;
        EggLocation = side.EggLocation != Locations.Default8bNone ? side.EggLocation : (ushort)0;

        if (side is IScaledSize3 s3)
            Scale = s3.Scale;
        else
            Scale = pkh.HeightScalar;
        if (side is IGameDataSplitAbility a)
            AbilityNumber = a.AbilityNumber;
        else
            AbilityNumber = 1;

        PopulateFromCore(pkh);
    }

    private void PopulateFromCore(PKH pkh)
    {
        Obedience_Level = pkh.MetLevel;

        var pi = PersonalTable.ZA.GetFormEntry(pkh.Species, pkh.Form);
        var index = AbilityNumber >> 1;
        if (index >= pi.AbilityCount)
            index = 0;
        Ability = (ushort)pi.GetAbilityAtIndex(index);

        var level = Experience.GetLevel(pkh.EXP, pi.EXPGrowth);
        this.ResetMoves(pkh.Species, pkh.Form, level, LearnSource9ZA.Instance, EntityContext.Gen9a);
    }
}
