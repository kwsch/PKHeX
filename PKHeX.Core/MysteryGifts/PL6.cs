using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokemon Link Data Storage
/// </summary>
/// <remarks>
/// This Template object is very similar to the <see cref="PCD"/> structure in that it stores more data than just the gift.
/// This template object is only present in Generation 6 save files.
/// </remarks>
public sealed class PL6(Memory<byte> Raw)
{
    public const int Size = 0xA47;
    public const string Filter = "Pokémon Link Data|*.pl6|All Files (*.*)|*.*";

    public PL6() : this(new byte[Size]) { }

    public Span<byte> Data => Raw.Span;

    /// <summary>
    /// Pokémon Link Flag
    /// </summary>
    public byte Flags
    {
        get => Data[0x00];
        set => Data[0x00] = value;
    }

    public bool Enabled { get => (Flags & 0x80) != 0; set => Flags = value ? (byte)(1 << 7) : (byte)0; }

    private Span<byte> Source => Data.Slice(0x01, 110);

    /// <summary>
    /// Name of data source
    /// </summary>
    public string Origin { get => StringConverter6.GetString(Source); set => StringConverter6.SetString(Source, value, 54, 0, StringConverterOption.ClearZero); }

    // Pokemon transfer flags?
    public uint Flags1 { get => ReadUInt32LittleEndian(Data[0x099..]); set => WriteUInt32LittleEndian(Data[0x099..], value); }
    public uint Flags2 { get => ReadUInt32LittleEndian(Data[0x141..]); set => WriteUInt32LittleEndian(Data[0x141..], value); }
    public uint Flags3 { get => ReadUInt32LittleEndian(Data[0x1E9..]); set => WriteUInt32LittleEndian(Data[0x1E9..], value); }
    public uint Flags4 { get => ReadUInt32LittleEndian(Data[0x291..]); set => WriteUInt32LittleEndian(Data[0x291..], value); }
    public uint Flags5 { get => ReadUInt32LittleEndian(Data[0x339..]); set => WriteUInt32LittleEndian(Data[0x339..], value); }
    public uint Flags6 { get => ReadUInt32LittleEndian(Data[0x3E1..]); set => WriteUInt32LittleEndian(Data[0x3E1..], value); }

    // Pokémon
    public LinkEntity6 Entity1 { get => new(Raw.Slice(0x09D, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x09D..]); }
    public LinkEntity6 Entity2 { get => new(Raw.Slice(0x145, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x145..]); }
    public LinkEntity6 Entity3 { get => new(Raw.Slice(0x1ED, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x1ED..]); }
    public LinkEntity6 Entity4 { get => new(Raw.Slice(0x295, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x295..]); }
    public LinkEntity6 Entity5 { get => new(Raw.Slice(0x33D, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x33D..]); }
    public LinkEntity6 Entity6 { get => new(Raw.Slice(0x3E5, LinkEntity6.Size)); set => value.Data.CopyTo(Data[0x3E5..]); }

    // Item Properties
    public ushort Item1     { get => ReadUInt16LittleEndian(Data[0x489..]); set => WriteUInt16LittleEndian(Data[0x489..], value); }
    public ushort Quantity1 { get => ReadUInt16LittleEndian(Data[0x48B..]); set => WriteUInt16LittleEndian(Data[0x48B..], value); }
    public ushort Item2     { get => ReadUInt16LittleEndian(Data[0x48D..]); set => WriteUInt16LittleEndian(Data[0x48D..], value); }
    public ushort Quantity2 { get => ReadUInt16LittleEndian(Data[0x48F..]); set => WriteUInt16LittleEndian(Data[0x48F..], value); }
    public ushort Item3     { get => ReadUInt16LittleEndian(Data[0x491..]); set => WriteUInt16LittleEndian(Data[0x491..], value); }
    public ushort Quantity3 { get => ReadUInt16LittleEndian(Data[0x493..]); set => WriteUInt16LittleEndian(Data[0x493..], value); }
    public ushort Item4     { get => ReadUInt16LittleEndian(Data[0x495..]); set => WriteUInt16LittleEndian(Data[0x495..], value); }
    public ushort Quantity4 { get => ReadUInt16LittleEndian(Data[0x497..]); set => WriteUInt16LittleEndian(Data[0x497..], value); }
    public ushort Item5     { get => ReadUInt16LittleEndian(Data[0x499..]); set => WriteUInt16LittleEndian(Data[0x499..], value); }
    public ushort Quantity5 { get => ReadUInt16LittleEndian(Data[0x49B..]); set => WriteUInt16LittleEndian(Data[0x49B..], value); }
    public ushort Item6     { get => ReadUInt16LittleEndian(Data[0x49D..]); set => WriteUInt16LittleEndian(Data[0x49D..], value); }
    public ushort Quantity6 { get => ReadUInt16LittleEndian(Data[0x49F..]); set => WriteUInt16LittleEndian(Data[0x49F..], value); }

    public ushort BattlePoints { get => ReadUInt16LittleEndian(Data[0x4A1..]); set => WriteUInt16LittleEndian(Data[0x4A1..], value); }
    public ushort Pokemiles { get => ReadUInt16LittleEndian(Data[0x4A3..]); set => WriteUInt16LittleEndian(Data[0x4A3..], value); }
}

/// <summary>
/// Pokémon Link Gift Template
/// </summary>
/// <remarks>
/// This Template object is very similar to the <see cref="WC6"/> structure and similar objects, in that the structure offsets are ordered the same.
/// This template object is only present in Generation 6 save files.
/// </remarks>
public sealed class LinkEntity6(Memory<byte> Raw) : IRibbonSetEvent3, IRibbonSetEvent4, IEncounterInfo, IMoveset, IRelearn,
    IContestStats, IMemoryOT, ITrainerID32
{
    internal const int Size = 0xA0;

    public Span<byte> Data => Raw.Span;

    public LinkEntity6() : this(new byte[Size]) { }

    public TrainerIDFormat TrainerIDDisplayFormat => TrainerIDFormat.SixteenBit;

    public uint ID32 { get => ReadUInt32LittleEndian(Data[..]); set => WriteUInt32LittleEndian(Data[..], value); }
    public ushort TID16 { get => ReadUInt16LittleEndian(Data[..]); set => WriteUInt16LittleEndian(Data[..], value); }
    public ushort SID16 { get => ReadUInt16LittleEndian(Data[0x02..]); set => WriteUInt16LittleEndian(Data[0x02..], value); }
    public byte OriginGame { get => Data[0x04]; set => Data[0x04] = value; }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data[0x08..]); set => WriteUInt32LittleEndian(Data[0x08..], value); }
    public byte Ball { get => Data[0xE]; set => Data[0xE] = value; }
    public int HeldItem { get => ReadUInt16LittleEndian(Data[0x10..]); set => WriteUInt16LittleEndian(Data[0x10..], (ushort)value); }
    public ushort Move1 { get => ReadUInt16LittleEndian(Data[0x12..]); set => WriteUInt16LittleEndian(Data[0x12..], value); }
    public ushort Move2 { get => ReadUInt16LittleEndian(Data[0x14..]); set => WriteUInt16LittleEndian(Data[0x14..], value); }
    public ushort Move3 { get => ReadUInt16LittleEndian(Data[0x16..]); set => WriteUInt16LittleEndian(Data[0x16..], value); }
    public ushort Move4 { get => ReadUInt16LittleEndian(Data[0x18..]); set => WriteUInt16LittleEndian(Data[0x18..], value); }
    public ushort Species { get => ReadUInt16LittleEndian(Data[0x1A..]); set => WriteUInt16LittleEndian(Data[0x1A..], value); }
    public byte Form { get => Data[0x1C]; set => Data[0x1C] = value; }
    public int Language { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }

    public Span<byte> NicknameTrash => Data.Slice(0x1E, 0x1A);

    public string Nickname
    {
        get => StringConverter6.GetString(NicknameTrash);
        set => StringConverter6.SetString(NicknameTrash, value, 12, Language, StringConverterOption.ClearZero);
    }

    public Nature Nature { get => (Nature)Data[0x38]; set => Data[0x38] = (byte)value; }
    public byte Gender { get => Data[0x39]; set => Data[0x39] = value; }
    public int AbilityType { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
    public int PIDType { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
    public ushort EggLocation { get => ReadUInt16LittleEndian(Data[0x3C..]); set => WriteUInt16LittleEndian(Data[0x3C..], value); }
    public ushort Location { get => ReadUInt16LittleEndian(Data[0x3E..]); set => WriteUInt16LittleEndian(Data[0x3E..], value); }
    public byte MetLevel  { get => Data[0x40]; set => Data[0x40] = value; }

    public byte ContestCool { get => Data[0x41]; set => Data[0x41] = value; }
    public byte ContestBeauty { get => Data[0x42]; set => Data[0x42] = value; }
    public byte ContestCute { get => Data[0x43]; set => Data[0x43] = value; }
    public byte ContestSmart { get => Data[0x44]; set => Data[0x44] = value; }
    public byte ContestTough { get => Data[0x45]; set => Data[0x45] = value; }
    public byte ContestSheen { get => Data[0x46]; set => Data[0x46] = value; }

    public int IV_HP { get => Data[0x47]; set => Data[0x47] = (byte)value; }
    public int IV_ATK { get => Data[0x48]; set => Data[0x48] = (byte)value; }
    public int IV_DEF { get => Data[0x49]; set => Data[0x49] = (byte)value; }
    public int IV_SPE { get => Data[0x4A]; set => Data[0x4A] = (byte)value; }
    public int IV_SPA { get => Data[0x4B]; set => Data[0x4B] = (byte)value; }
    public int IV_SPD { get => Data[0x4C]; set => Data[0x4C] = (byte)value; }

    public byte OTGender { get => Data[0x4D]; set => Data[0x4D] = value; }

    public Span<byte> OriginalTrainerTrash => Data.Slice(0x4E, 0x1A);

    public string OT
    {
        get => StringConverter6.GetString(OriginalTrainerTrash);
        set => StringConverter6.SetString(OriginalTrainerTrash, value, 12, Language, StringConverterOption.ClearZero);
    }

    public int Level { get => Data[0x68]; set => Data[0x68] = (byte)value; }
    public bool IsEgg { get => Data[0x69] == 1; set => Data[0x69] = value ? (byte)1 : (byte)0; }
    public uint PID { get => ReadUInt32LittleEndian(Data[0x6C..]); set => WriteUInt32LittleEndian(Data[0x6C..], value); }
    public ushort RelearnMove1 { get => ReadUInt16LittleEndian(Data[0x70..]); set => WriteUInt16LittleEndian(Data[0x70..], value); }
    public ushort RelearnMove2 { get => ReadUInt16LittleEndian(Data[0x72..]); set => WriteUInt16LittleEndian(Data[0x72..], value); }
    public ushort RelearnMove3 { get => ReadUInt16LittleEndian(Data[0x74..]); set => WriteUInt16LittleEndian(Data[0x74..], value); }
    public ushort RelearnMove4 { get => ReadUInt16LittleEndian(Data[0x76..]); set => WriteUInt16LittleEndian(Data[0x76..], value); }
    public byte OriginalTrainerMemoryIntensity { get => Data[0x78]; set => Data[0x78] = value; }
    public byte OriginalTrainerMemory { get => Data[0x79]; set => Data[0x79] = value; }
    public ushort OriginalTrainerMemoryVariable { get => ReadUInt16LittleEndian(Data[0x7A..]); set => WriteUInt16LittleEndian(Data[0x7A..], value); }
    public byte OriginalTrainerMemoryFeeling { get => Data[0x7C]; set => Data[0x7C] = value; }

    private byte RIB0 { get => Data[0x0C]; set => Data[0x0C] = value; }
    private byte RIB1 { get => Data[0x0D]; set => Data[0x0D] = value; }

    public bool RibbonChampionBattle   { get => (RIB0 & (1 << 0)) == 1 << 0; set => RIB0 = (byte)((RIB0 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonChampionRegional { get => (RIB0 & (1 << 1)) == 1 << 1; set => RIB0 = (byte)((RIB0 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonChampionNational { get => (RIB0 & (1 << 2)) == 1 << 2; set => RIB0 = (byte)((RIB0 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonCountry          { get => (RIB0 & (1 << 3)) == 1 << 3; set => RIB0 = (byte)((RIB0 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonNational         { get => (RIB0 & (1 << 4)) == 1 << 4; set => RIB0 = (byte)((RIB0 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonEarth            { get => (RIB0 & (1 << 5)) == 1 << 5; set => RIB0 = (byte)((RIB0 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonWorld            { get => (RIB0 & (1 << 6)) == 1 << 6; set => RIB0 = (byte)((RIB0 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RibbonEvent            { get => (RIB0 & (1 << 7)) == 1 << 7; set => RIB0 = (byte)((RIB0 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }
    public bool RibbonChampionWorld    { get => (RIB1 & (1 << 0)) == 1 << 0; set => RIB1 = (byte)((RIB1 & ~(1 << 0)) | (value ? 1 << 0 : 0)); }
    public bool RibbonBirthday         { get => (RIB1 & (1 << 1)) == 1 << 1; set => RIB1 = (byte)((RIB1 & ~(1 << 1)) | (value ? 1 << 1 : 0)); }
    public bool RibbonSpecial          { get => (RIB1 & (1 << 2)) == 1 << 2; set => RIB1 = (byte)((RIB1 & ~(1 << 2)) | (value ? 1 << 2 : 0)); }
    public bool RibbonSouvenir         { get => (RIB1 & (1 << 3)) == 1 << 3; set => RIB1 = (byte)((RIB1 & ~(1 << 3)) | (value ? 1 << 3 : 0)); }
    public bool RibbonWishing          { get => (RIB1 & (1 << 4)) == 1 << 4; set => RIB1 = (byte)((RIB1 & ~(1 << 4)) | (value ? 1 << 4 : 0)); }
    public bool RibbonClassic          { get => (RIB1 & (1 << 5)) == 1 << 5; set => RIB1 = (byte)((RIB1 & ~(1 << 5)) | (value ? 1 << 5 : 0)); }
    public bool RibbonPremier          { get => (RIB1 & (1 << 6)) == 1 << 6; set => RIB1 = (byte)((RIB1 & ~(1 << 6)) | (value ? 1 << 6 : 0)); }
    public bool RIB1_7                 { get => (RIB1 & (1 << 7)) == 1 << 7; set => RIB1 = (byte)((RIB1 & ~(1 << 7)) | (value ? 1 << 7 : 0)); }

    public byte LevelMin => MetLevel;
    public byte LevelMax => MetLevel;

    public Moveset Moves
    {
        get => new(Move1, Move2, Move3, Move4);
        set
        {
            Move1 = value.Move1;
            Move2 = value.Move2;
            Move3 = value.Move3;
            Move4 = value.Move4;
        }
    }

    public Moveset Relearn
    {
        get => new(RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4);
        set
        {
            RelearnMove1 = value.Move1;
            RelearnMove2 = value.Move2;
            RelearnMove3 = value.Move3;
            RelearnMove4 = value.Move4;
        }
    }

    public byte Generation => 6;
    public bool IsShiny => false;
    public GameVersion Version => GameVersion.Gen6;
    public EntityContext Context => EntityContext.Gen6;
    public AbilityPermission Ability => (AbilityPermission)AbilityType;
    public Ball FixedBall => (Ball)Ball;
    public Shiny Shiny => Shiny.Never;

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var wc6 = new WC6();
        Data.CopyTo(wc6.Data.AsSpan(0x68));
        return wc6.ConvertToPKM(tr, criteria);
    }
}
