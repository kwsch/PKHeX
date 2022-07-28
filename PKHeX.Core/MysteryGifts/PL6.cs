using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokemon Link Data Storage
/// </summary>
/// <remarks>
/// This Template object is very similar to the <see cref="PCD"/> structure in that it stores more data than just the gift.
/// This template object is only present in Generation 6 save files.
/// </remarks>
public sealed class PL6
{
    public const int Size = 0xA47;
    public const string Filter = "Pokémon Link Data|*.pl6|All Files (*.*)|*.*";

    public readonly byte[] Data;

    public PL6() => Data = new byte[Size];
    public PL6(byte[] data) => Data = data;

    /// <summary>
    /// Pokémon Link Flag
    /// </summary>
    public byte Flags
    {
        get => Data[0x00];
        set => Data[0x00] = value;
    }

    public bool Enabled { get => (Flags & 0x80) != 0; set => Flags = value ? (byte)(1 << 7) : (byte)0; }

    /// <summary>
    /// Name of data source
    /// </summary>
    public string Origin { get => StringConverter6.GetString(Data.AsSpan(0x01, 110)); set => StringConverter6.SetString(Data.AsSpan(0x01, 110), value.AsSpan(), 54, StringConverterOption.ClearZero); }

    // Pokemon transfer flags?
    public uint Flags_1 { get => ReadUInt32LittleEndian(Data.AsSpan(0x099)); set => WriteUInt32LittleEndian(Data.AsSpan(0x099), value); }
    public uint Flags_2 { get => ReadUInt32LittleEndian(Data.AsSpan(0x141)); set => WriteUInt32LittleEndian(Data.AsSpan(0x141), value); }
    public uint Flags_3 { get => ReadUInt32LittleEndian(Data.AsSpan(0x1E9)); set => WriteUInt32LittleEndian(Data.AsSpan(0x1E9), value); }
    public uint Flags_4 { get => ReadUInt32LittleEndian(Data.AsSpan(0x291)); set => WriteUInt32LittleEndian(Data.AsSpan(0x291), value); }
    public uint Flags_5 { get => ReadUInt32LittleEndian(Data.AsSpan(0x339)); set => WriteUInt32LittleEndian(Data.AsSpan(0x339), value); }
    public uint Flags_6 { get => ReadUInt32LittleEndian(Data.AsSpan(0x3E1)); set => WriteUInt32LittleEndian(Data.AsSpan(0x3E1), value); }

    // Pokémon
    public PL6_PKM Poke_1 { get => new(Data.Slice(0x09D, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x09D); }
    public PL6_PKM Poke_2 { get => new(Data.Slice(0x145, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x145); }
    public PL6_PKM Poke_3 { get => new(Data.Slice(0x1ED, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x1ED); }
    public PL6_PKM Poke_4 { get => new(Data.Slice(0x295, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x295); }
    public PL6_PKM Poke_5 { get => new(Data.Slice(0x33D, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x33D); }
    public PL6_PKM Poke_6 { get => new(Data.Slice(0x3E5, PL6_PKM.Size)); set => value.Data.CopyTo(Data, 0x3E5); }

    // Item Properties
    public int Item_1     { get => ReadUInt16LittleEndian(Data.AsSpan(0x489)); set => WriteUInt16LittleEndian(Data.AsSpan(0x489), (ushort)value); }
    public int Quantity_1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x48B)); set => WriteUInt16LittleEndian(Data.AsSpan(0x48B), (ushort)value); }
    public int Item_2     { get => ReadUInt16LittleEndian(Data.AsSpan(0x48D)); set => WriteUInt16LittleEndian(Data.AsSpan(0x48D), (ushort)value); }
    public int Quantity_2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x48F)); set => WriteUInt16LittleEndian(Data.AsSpan(0x48F), (ushort)value); }
    public int Item_3     { get => ReadUInt16LittleEndian(Data.AsSpan(0x491)); set => WriteUInt16LittleEndian(Data.AsSpan(0x491), (ushort)value); }
    public int Quantity_3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x493)); set => WriteUInt16LittleEndian(Data.AsSpan(0x493), (ushort)value); }
    public int Item_4     { get => ReadUInt16LittleEndian(Data.AsSpan(0x495)); set => WriteUInt16LittleEndian(Data.AsSpan(0x495), (ushort)value); }
    public int Quantity_4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x497)); set => WriteUInt16LittleEndian(Data.AsSpan(0x497), (ushort)value); }
    public int Item_5     { get => ReadUInt16LittleEndian(Data.AsSpan(0x499)); set => WriteUInt16LittleEndian(Data.AsSpan(0x499), (ushort)value); }
    public int Quantity_5 { get => ReadUInt16LittleEndian(Data.AsSpan(0x49B)); set => WriteUInt16LittleEndian(Data.AsSpan(0x49B), (ushort)value); }
    public int Item_6     { get => ReadUInt16LittleEndian(Data.AsSpan(0x49D)); set => WriteUInt16LittleEndian(Data.AsSpan(0x49D), (ushort)value); }
    public int Quantity_6 { get => ReadUInt16LittleEndian(Data.AsSpan(0x49F)); set => WriteUInt16LittleEndian(Data.AsSpan(0x49F), (ushort)value); }

    public int BattlePoints { get => ReadUInt16LittleEndian(Data.AsSpan(0x4A1)); set => WriteUInt16LittleEndian(Data.AsSpan(0x4A1), (ushort)value); }
    public int Pokemiles { get => ReadUInt16LittleEndian(Data.AsSpan(0x4A3)); set => WriteUInt16LittleEndian(Data.AsSpan(0x4A3), (ushort)value); }
}

/// <summary>
/// Pokemon Link Gift Template
/// </summary>
/// <remarks>
/// This Template object is very similar to the <see cref="WC6"/> structure and similar objects, in that the structure offsets are ordered the same.
/// This template object is only present in Generation 6 save files.
/// </remarks>
public sealed class PL6_PKM : IRibbonSetEvent3, IRibbonSetEvent4, IEncounterInfo
{
    internal const int Size = 0xA0;

    public readonly byte[] Data;

    public PL6_PKM() : this(new byte[Size]) { }
    public PL6_PKM(byte[] data) => Data = data;

    public int TID { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), (ushort)value); }
    public int SID { get => ReadUInt16LittleEndian(Data.AsSpan(0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(0x02), (ushort)value); }
    public int OriginGame { get => Data[0x04]; set => Data[0x04] = (byte)value; }
    public uint EncryptionConstant { get => ReadUInt32LittleEndian(Data.AsSpan(0x08)); set => WriteUInt32LittleEndian(Data.AsSpan(0x08), value); }
    public int Ball { get => Data[0xE]; set => Data[0xE] = (byte)value; }
    public int HeldItem { get => ReadUInt16LittleEndian(Data.AsSpan(0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(0x10), (ushort)value); }
    public int Move1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12), (ushort)value); }
    public int Move2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(0x14), (ushort)value); }
    public int Move3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x16)); set => WriteUInt16LittleEndian(Data.AsSpan(0x16), (ushort)value); }
    public int Move4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x18)); set => WriteUInt16LittleEndian(Data.AsSpan(0x18), (ushort)value); }
    public int Species { get => ReadUInt16LittleEndian(Data.AsSpan(0x1A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x1A), (ushort)value); }
    public int Form { get => Data[0x1C]; set => Data[0x1C] = (byte)value; }
    public int Language { get => Data[0x1D]; set => Data[0x1D] = (byte)value; }

    public string Nickname
    {
        get => StringConverter6.GetString(Data.AsSpan(0x1E, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0x1E, 0x1A), value.AsSpan(), 12, StringConverterOption.ClearZero);
    }

    public int Nature { get => Data[0x38]; set => Data[0x38] = (byte)value; }
    public int Gender { get => Data[0x39]; set => Data[0x39] = (byte)value; }
    public int AbilityType { get => Data[0x3A]; set => Data[0x3A] = (byte)value; }
    public int PIDType { get => Data[0x3B]; set => Data[0x3B] = (byte)value; }
    public int EggLocation { get => ReadUInt16LittleEndian(Data.AsSpan(0x3C)); set => WriteUInt16LittleEndian(Data.AsSpan(0x3C), (ushort)value); }
    public int MetLocation  { get => ReadUInt16LittleEndian(Data.AsSpan(0x3E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x3E), (ushort)value); }
    public byte MetLevel  { get => Data[0x40]; set => Data[0x40] = value; }

    public int CNT_Cool { get => Data[0x41]; set => Data[0x41] = (byte)value; }
    public int CNT_Beauty { get => Data[0x42]; set => Data[0x42] = (byte)value; }
    public int CNT_Cute { get => Data[0x43]; set => Data[0x43] = (byte)value; }
    public int CNT_Smart { get => Data[0x44]; set => Data[0x44] = (byte)value; }
    public int CNT_Tough { get => Data[0x45]; set => Data[0x45] = (byte)value; }
    public int CNT_Sheen { get => Data[0x46]; set => Data[0x46] = (byte)value; }

    public int IV_HP { get => Data[0x47]; set => Data[0x47] = (byte)value; }
    public int IV_ATK { get => Data[0x48]; set => Data[0x48] = (byte)value; }
    public int IV_DEF { get => Data[0x49]; set => Data[0x49] = (byte)value; }
    public int IV_SPE { get => Data[0x4A]; set => Data[0x4A] = (byte)value; }
    public int IV_SPA { get => Data[0x4B]; set => Data[0x4B] = (byte)value; }
    public int IV_SPD { get => Data[0x4C]; set => Data[0x4C] = (byte)value; }

    public int OTGender { get => Data[0x4D]; set => Data[0x4D] = (byte)value; }

    public string OT
    {
        get => StringConverter6.GetString(Data.AsSpan(0x4E, 0x1A));
        set => StringConverter6.SetString(Data.AsSpan(0x4E, 0x1A), value.AsSpan(), 12, StringConverterOption.ClearZero);
    }

    public int Level { get => Data[0x68]; set => Data[0x68] = (byte)value; }
    public bool IsEgg { get => Data[0x69] == 1; set => Data[0x69] = value ? (byte)1 : (byte)0; }
    public uint PID { get => ReadUInt32LittleEndian(Data.AsSpan(0x6C)); set => WriteUInt32LittleEndian(Data.AsSpan(0x6C), value); }
    public int RelearnMove1 { get => ReadUInt16LittleEndian(Data.AsSpan(0x70)); set => WriteUInt16LittleEndian(Data.AsSpan(0x70), (ushort)value); }
    public int RelearnMove2 { get => ReadUInt16LittleEndian(Data.AsSpan(0x72)); set => WriteUInt16LittleEndian(Data.AsSpan(0x72), (ushort)value); }
    public int RelearnMove3 { get => ReadUInt16LittleEndian(Data.AsSpan(0x74)); set => WriteUInt16LittleEndian(Data.AsSpan(0x74), (ushort)value); }
    public int RelearnMove4 { get => ReadUInt16LittleEndian(Data.AsSpan(0x76)); set => WriteUInt16LittleEndian(Data.AsSpan(0x76), (ushort)value); }
    public int OT_Intensity { get => Data[0x78]; set => Data[0x78] = (byte)value; }
    public int OT_Memory { get => Data[0x79]; set => Data[0x79] = (byte)value; }
    public int OT_TextVar { get => ReadUInt16LittleEndian(Data.AsSpan(0x7A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x7A), (ushort)value); }
    public int OT_Feeling { get => Data[0x7C]; set => Data[0x7C] = (byte)value; }

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

    public IReadOnlyList<int> Moves
    {
        get => new[] { Move1, Move2, Move3, Move4 };
        set
        {
            if (value.Count > 0) Move1 = value[0];
            if (value.Count > 1) Move2 = value[1];
            if (value.Count > 2) Move3 = value[2];
            if (value.Count > 3) Move4 = value[3];
        }
    }

    public IReadOnlyList<int> RelearnMoves
    {
        get => new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 };
        set
        {
            if (value.Count > 0) RelearnMove1 = value[0];
            if (value.Count > 1) RelearnMove2 = value[1];
            if (value.Count > 2) RelearnMove3 = value[2];
            if (value.Count > 3) RelearnMove4 = value[3];
        }
    }

    public int Generation => 6;
    public bool IsShiny => false;
    public bool EggEncounter => false;
    public GameVersion Version => GameVersion.Gen6;
    public EntityContext Context => EntityContext.Gen6;

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var wc6 = new WC6();
        Data.CopyTo(wc6.Data, 0x68);
        return wc6.ConvertToPKM(tr, criteria);
    }
}
