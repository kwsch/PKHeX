using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

// TODO HOME ZA -- Simply copied from SV, needs to be updated to ZA when official support is added.

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

    public int Move1_PP    { get => Data[0x09]; set => Data[0x09] = (byte)value; }
    public int Move2_PP    { get => Data[0x0A]; set => Data[0x0A] = (byte)value; }
    public int Move3_PP    { get => Data[0x0B]; set => Data[0x0B] = (byte)value; }
    public int Move4_PP    { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
    public int Move1_PPUps { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
    public int Move2_PPUps { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
    public int Move3_PPUps { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
    public int Move4_PPUps { get => Data[0x10]; set => Data[0x10] = (byte)value; }

    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x11..]); set => WriteUInt16LittleEndian(Data[0x11..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x13..]); set => WriteUInt16LittleEndian(Data[0x13..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x15..]); set => WriteUInt16LittleEndian(Data[0x15..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x17..]); set => WriteUInt16LittleEndian(Data[0x17..], value); }
    public bool IsAlpha { get => Data[0x19] != 0; set => Data[0x19] = value ? (byte)1 : (byte)0; }
    // 0x1A Padding (???) TODO HOME ZA; this is adapted from SV's structure, replacing the 2 byte properties for Tera Type.
    public byte Ball { get => Data[0x1B]; set => Data[0x1B] = value; }
    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x1C..]); set => WriteUInt16LittleEndian(Data[0x1C..], value); }
    public ushort MetLocation { get => ReadUInt16LittleEndian(Data[0x1E..]); set => WriteUInt16LittleEndian(Data[0x1E..], value); }

    private const int RecordStartBase = 0x20;
    internal const int COUNT_RECORD_BASE = PA9.COUNT_RECORD_BASE; // Up to 200 TM flags, but not all are used.
    private const int RecordLengthBase = COUNT_RECORD_BASE / 8; // 0x19 bytes, 8 bits
    public Span<byte> RecordFlagsBase => Data.Slice(RecordStartBase, RecordLengthBase);

    // Rev2 Additions
    public byte Obedience_Level { get => Data[0x39]; set => Data[0x39] = value; }
    public ushort Ability { get => ReadUInt16LittleEndian(Data[0x3A..]); set => WriteUInt16LittleEndian(Data[0x3A..], value); }
    public byte AbilityNumber { get => Data[0x3C]; set => Data[0x3C] = value; }

    // Rev3 Additions
    private const int RecordStartDLC = 0x3D;
    internal const int COUNT_RECORD_DLC = PA9.COUNT_RECORD_DLC; // 13 additional bytes allocated for DLC1/2 TM Flags
    private const int RecordLengthDLC = COUNT_RECORD_DLC / 8;
    public Span<byte> RecordFlagsDLC => Data.Slice(RecordStartDLC, RecordLengthDLC);

    // Rev4 Additions (ZA)
    private const int PlusStart0 = 0x4A;
    internal const int PlusCount0 = PA9.PlusCount0;
    private const int PlusLength0 = PlusCount0 / 8;
    public Span<byte> PlusFlags0 => Data.Slice(PlusStart0, PlusLength0);

    private const int PlusStart1 = PlusStart0 + PlusLength0; // 0x6B
    internal const int PlusCount1 = PA9.PlusCount1;
    private const int PlusLength1 = PlusCount1 / 8;
    public Span<byte> PlusFlags1 => Data.Slice(PlusStart1, PlusLength1);

    #endregion

    #region TM Flag Methods
    public bool GetMoveRecordFlag(int index)
    {
        if ((uint)index >= COUNT_RECORD_BASE)
            return GetMoveRecordFlagDLC(index - COUNT_RECORD_BASE);
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, RecordStartBase + ofs, index & 7);
    }

    private bool GetMoveRecordFlagDLC(int index)
    {
        if ((uint)index >= COUNT_RECORD_DLC)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, RecordStartDLC + ofs, index & 7);
    }

    public void SetMoveRecordFlag(int index, bool value = true)
    {
        if ((uint)index >= COUNT_RECORD_BASE)
        {
            SetMoveRecordFlagDLC(value, index - COUNT_RECORD_BASE);
            return;
        }
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, RecordStartBase + ofs, index & 7, value);
    }

    private void SetMoveRecordFlagDLC(bool value, int index)
    {
        if ((uint)index >= COUNT_RECORD_DLC)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, RecordStartDLC + ofs, index & 7, value);
    }

    public bool GetMoveRecordFlagAny() => GetMoveRecordFlagAnyBase() || GetMoveRecordFlagAnyDLC();
    private bool GetMoveRecordFlagAnyBase() => RecordFlagsBase.ContainsAnyExcept<byte>(0);
    private bool GetMoveRecordFlagAnyDLC() => RecordFlagsDLC.ContainsAnyExcept<byte>(0);

    public void ClearMoveRecordFlags()
    {
        ClearMoveRecordFlagsBase();
        ClearMoveRecordFlagsDLC();
    }

    private void ClearMoveRecordFlagsBase() => RecordFlagsBase.Clear();
    private void ClearMoveRecordFlagsDLC() => RecordFlagsDLC.Clear();
    #endregion

    #region Plus Moves

    public bool GetMovePlusFlag(int index)
    {
        if ((uint)index >= PlusCount0)
            return GetMovePlusFlag1(index - PlusCount0);
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, PlusStart0 + ofs, index & 7);
    }

    private bool GetMovePlusFlag1(int index)
    {
        if ((uint)index >= PlusCount1)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        return FlagUtil.GetFlag(Data, PlusStart1 + ofs, index & 7);
    }

    public void SetMovePlusFlag(int index, bool value = true)
    {
        if ((uint)index >= PlusCount0)
        {
            SetMovePlusFlag1(value, index - PlusCount0);
            return;
        }
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, PlusStart0 + ofs, index & 7, value);
    }

    private void SetMovePlusFlag1(bool value, int index)
    {
        if ((uint)index >= PlusCount1)
            throw new ArgumentOutOfRangeException(nameof(index));
        int ofs = index >> 3;
        FlagUtil.SetFlag(Data, PlusStart1 + ofs, index & 7, value);
    }

    public bool GetMovePlusFlagAny() => GetMovePlusFlagAny0() || GetMovePlusFlagAny1();
    private bool GetMovePlusFlagAny0() => PlusFlags0.ContainsAnyExcept<byte>(0);
    private bool GetMovePlusFlagAny1() => PlusFlags1.ContainsAnyExcept<byte>(0);

    public void ClearMovePlusFlags()
    {
        ClearMovePlusFlags0();
        ClearMovePlusFlags1();
    }

    private void ClearMovePlusFlags0() => PlusFlags0.Clear();
    private void ClearMovePlusFlags1() => PlusFlags1.Clear();
    #endregion

    #region Conversion

    public PersonalInfo GetPersonalInfo(ushort species, byte form) => PersonalTable.ZA.GetFormEntry(species, form);

    public void CopyTo(PA9 pk, PKH pkh)
    {
        this.CopyTo(pk);
        pk.Scale = Scale;
        PlusFlags0.CopyTo(pk.PlusFlags0);
        PlusFlags1.CopyTo(pk.PlusFlags1);
        RecordFlagsBase.CopyTo(pk.RecordFlagsBase);
        RecordFlagsDLC.CopyTo(pk.RecordFlagsDLC);
        pk.ObedienceLevel = Obedience_Level;
        pk.Ability = Ability;
        pk.AbilityNumber = AbilityNumber;
    }

    public void CopyFrom(PA9 pk, PKH pkh)
    {
        this.CopyFrom(pk);
        pkh.HeightScalar = Scale = pk.Scale; // Overwrite Height
        pk.PlusFlags0.CopyTo(PlusFlags0);
        pk.PlusFlags1.CopyTo(PlusFlags1);
        pk.RecordFlagsBase.CopyTo(RecordFlagsBase);
        pk.RecordFlagsDLC.CopyTo(RecordFlagsDLC);
        Obedience_Level = pk.ObedienceLevel;
        Ability = (ushort)pk.Ability;
        AbilityNumber = (byte)pk.AbilityNumber;
    }

    public PA9 ConvertToPKM(PKH pkh)
    {
        var pk = new PA9();
        pkh.CopyTo(pk);
        CopyTo(pk, pkh);

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

    private static IGameDataSide? GetNearestNeighbor(PKH pkh) => pkh.DataPK9 as IGameDataSide
                                                              ?? pkh.DataPK8 as IGameDataSide
                                                              ?? pkh.DataPB8 as IGameDataSide
                                                              ?? pkh.DataPB7 as IGameDataSide
                                                              ?? pkh.DataPA8;

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
