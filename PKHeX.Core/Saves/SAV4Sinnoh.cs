using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Abstract <see cref="SaveFile"/> format for <see cref="GameVersion.DP"/> and <see cref="GameVersion.Pt"/>
/// </summary>
public abstract class SAV4Sinnoh : SAV4, IBoxDetailName, IBoxDetailWallpaper
{
    protected override int FooterSize => 0x14;
    protected SAV4Sinnoh([ConstantExpected] int gSize, [ConstantExpected] int sSize) : base(gSize, sSize) { }
    protected SAV4Sinnoh(byte[] data, [ConstantExpected] int gSize, [ConstantExpected] int sSize, [ConstantExpected] int sStart) : base(data, gSize, sSize, sStart) { }

    #region Storage
    // u32 currentBox
    // box{pk4[30][18]}
    // g4str[18] boxNames
    // byte[18] boxWallpapers
    private const int BOX_COUNT = 18;
    private const int BOX_SLOTS = 30;
    private const int BOX_NAME_LEN = 40; // 20 characters

    private const int BOX_DATA_LEN = (BOX_SLOTS * PokeCrypto.SIZE_4STORED); // 0xFF0, no padding between boxes (to nearest 0x100)
    private const int BOX_END = BOX_COUNT * BOX_DATA_LEN; // 18 * 0xFF0
    private const int BOX_NAME = 4 + BOX_END; // after box data
    private const int BOX_WP = BOX_NAME + (BOX_COUNT * BOX_NAME_LEN); // 0x121B4;
    private const int BOX_FLAGS = 18 + BOX_WP; // 0x121C6

    public override int GetBoxOffset(int box) => 4 + (box * BOX_DATA_LEN);
    private static int GetBoxNameOffset(int box) => BOX_NAME + (box * BOX_NAME_LEN);
    protected static int GetBoxWallpaperOffset(int box) => BOX_WP + box;

    public override int CurrentBox // (align 32)
    {
        get => Storage[0];
        set => Storage[0] = (byte)value;
    }

    public override byte[] BoxFlags
    {
        get => [ Storage [BOX_FLAGS] ];
        set => Storage[BOX_FLAGS] = value[0];
    }

    private Span<byte> GetBoxNameSpan(int box) => Storage.Slice(GetBoxNameOffset(box), BOX_NAME_LEN);
    public string GetBoxName(int box) => GetString(GetBoxNameSpan(box));

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        const int maxlen = 8;
        var span = GetBoxNameSpan(box);
        SetString(span, value, maxlen, StringConverterOption.ClearZero);
    }
    public abstract int GetBoxWallpaper(int box);
    public abstract void SetBoxWallpaper(int box, int value);
    #endregion

    #region Poketch
    protected int PoketchStart { private get; set; }
    private byte PoketchPacked { get => General[PoketchStart]; set => General[PoketchStart] = value; }

    public bool PoketchEnabled { get => (PoketchPacked & 1) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 1) : (PoketchPacked & ~1)); }
    public bool PoketchFlag1 { get => (PoketchPacked & 2) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 2) : (PoketchPacked & ~2)); }
    public bool PoketchFlag2 { get => (PoketchPacked & 4) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 4) : (PoketchPacked & ~4)); }

    public PoketchColor PoketchColor
    {
        get => (PoketchColor) ((PoketchPacked >> 3) & 7);
        set => PoketchPacked = (byte) ((PoketchPacked & 0xC7) | ((int) value << 3));
    }

    public bool PoketchFlag6 { get => (PoketchPacked & 0x40) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 0x40) : (PoketchPacked & ~0x40)); }
    public bool PoketchFlag7 { get => (PoketchPacked & 0x80) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 0x80) : (PoketchPacked & ~0x80)); }
    public byte PoketchUnlockedCount { get => General[PoketchStart + 1]; set => General[PoketchStart + 1] = value; }
    public sbyte CurrentPoketchApp { get => (sbyte)General[PoketchStart + 2]; set => General[PoketchStart + 2] = (byte)Math.Min((sbyte)PoketchApp.Alarm_Clock, value); }

    public bool GetPoketchAppUnlocked(PoketchApp index)
    {
        if (index > PoketchApp.Alarm_Clock)
            throw new ArgumentOutOfRangeException(nameof(index));
        return General[PoketchStart + 3 + (int) index] != 0;
    }

    public void SetPoketchAppUnlocked(PoketchApp index, bool value = true)
    {
        if (index > PoketchApp.Alarm_Clock)
            throw new ArgumentOutOfRangeException(nameof(index));
        var b = value ? 1 : 0;
        General[PoketchStart + 3 + (int)index] = (byte)b;
    }

    // 8 bytes unk

    public uint PoketchStepCounter
    {
        get => ReadUInt32LittleEndian(General[(PoketchStart + 0x24)..]);
        set => WriteUInt32LittleEndian(General[(PoketchStart + 0x24)..], value);
    }

    // 2 bytes for alarm clock time setting

    public byte[] GetPoketchDotArtistData() => General.Slice(PoketchStart + 0x2A, 120).ToArray();

    public void SetPoketchDotArtistData(ReadOnlySpan<byte> value)
    {
        if (value.Length != 120)
            throw new ArgumentOutOfRangeException($"Expected {120} bytes.", nameof(value.Length));
        value.CopyTo(General[(PoketchStart + 0x2A)..]);
    }

    // map marking stuff is at the end, unimportant

    #endregion

    #region Honey Trees
    protected int OFS_HONEY;
    protected const int HONEY_SIZE = HoneyTreeValue.Size; // 8

    private Span<byte> GetHoneyTreeSpan(int index)
    {
        if ((uint)index > 21)
            throw new ArgumentOutOfRangeException(nameof(index));
        return General.Slice(OFS_HONEY + (HONEY_SIZE * index), HONEY_SIZE);
    }

    public HoneyTreeValue GetHoneyTree(int index) => new(GetHoneyTreeSpan(index).ToArray());
    public void SetHoneyTree(HoneyTreeValue tree, int index) => SetData(GetHoneyTreeSpan(index), tree.Data);

    #endregion

    public int OFS_PoffinCase { get; protected set; }

    #region Underground
    //Underground Scores
    protected int OFS_UG_Stats;
    public uint UG_PlayersMet     { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x00)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x00)..], value); }
    public uint UG_Gifts          { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x04)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x04)..], value); }
    public uint UG_Spheres        { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x0C)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x0C)..], value); }
    public uint UG_Fossils        { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x10)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x10)..], value); }
    public uint UG_TrapsAvoided   { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x18)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x18)..], value); }
    public uint UG_TrapsTriggered { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x1C)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x1C)..], value); }
    public uint UG_Flags          { get => ReadUInt32LittleEndian(General[(OFS_UG_Stats + 0x34)..]); set => WriteUInt32LittleEndian(General[(OFS_UG_Stats + 0x34)..], value); }

    //Underground Items
    protected int OFS_UG_Items;

    public const int UG_POUCH_SIZE = 0x28; // 40 for each of the inventory pouches

    public Span<byte> GetUGI_Traps() => General.Slice(OFS_UG_Items, UG_POUCH_SIZE);
    public Span<byte> GetUGI_Goods() => General.Slice(OFS_UG_Items + 0x28, UG_POUCH_SIZE);
    public Span<byte> GetUGI_Treasures() => General.Slice(OFS_UG_Items + 0x50, UG_POUCH_SIZE);

    /// <summary>
    /// First 40 are the sphere type, last 40 are the sphere sizes
    /// </summary>
    public Span<byte> GetUGI_Spheres() => General.Slice(OFS_UG_Items + 0x78, UG_POUCH_SIZE * 2);

    #endregion

    public abstract uint SafariSeed { get; set; }
    public uint GetSafariIndex(int slot) => (SafariSeed >> (slot * 5)) & 0x1F;
    public void SetSafariIndex(int slot, uint value) => SafariSeed = (SafariSeed & ~(0x1Fu << (slot * 5))) | (value << (slot * 5));

    protected abstract ReadOnlySpan<ushort> TreeSpecies { get; }
    private const int GroupEntryCount = 6;

    public ushort GetHoneyTreeSpecies(int group, int index) =>
        TreeSpecies.Slice(group * GroupEntryCount, GroupEntryCount)[index];
}
