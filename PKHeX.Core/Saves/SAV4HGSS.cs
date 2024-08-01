using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> format for <see cref="GameVersion.HGSS"/>
/// </summary>
public sealed class SAV4HGSS : SAV4, IBoxDetailName, IBoxDetailWallpaper
{
    public SAV4HGSS() : base(GeneralSize, StorageSize)
    {
        Initialize();
        Mystery = new MysteryBlock4HGSS(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4HGSS.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    public SAV4HGSS(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize + GeneralGap)
    {
        Initialize();
        Mystery = new MysteryBlock4HGSS(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4HGSS.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    public override Zukan4 Dex { get; }
    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4HGSS((byte[])Data.Clone()) : new SAV4HGSS();

    public override GameVersion Version { get => GameVersion.HGSS; set { } }
    public override PersonalTable4 Personal => PersonalTable.HGSS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_HGSS;
    public override int MaxItemID => Legal.MaxItemID_4_HGSS;
    public const int GeneralSize = 0xF628;
    private const int StorageSize = 0x12310; // Start 0xF700, +0 starts box data
    private const int GeneralGap = 0xD8;
    private const int PokeDex = 0x12B8;
    protected override int FooterSize => 0x10;

    protected override BlockInfo4[] ExtraBlocks =>
    [
        new BlockInfo4(0, 0x23000, 0x2AC0), // Hall of Fame
        new BlockInfo4(1, 0x26000, 0x0BB0), // Battle Hall
        new BlockInfo4(2, 0x27000, 0x1D60), // Battle Video (My Video)
        new BlockInfo4(3, 0x29000, 0x1D60), // Battle Video (Other Videos 1)
        new BlockInfo4(4, 0x2B000, 0x1D60), // Battle Video (Other Videos 2)
        new BlockInfo4(5, 0x2D000, 0x1D60), // Battle Video (Other Videos 3)
    ];

    private void Initialize()
    {
        Version = GameVersion.HGSS;
        GetSAVOffsets();
    }

    private const int OffsetMystery = 0x9D3C; // Flags and Gifts
    protected override int EventWork => 0xDE4;
    protected override int EventFlag => 0x10C4;
    protected override int DaycareOffset => 0x15FC;
    public override MysteryBlock4HGSS Mystery { get; }
    public override BattleFrontierFacility4 MaxFacility => BattleFrontierFacility4.Arcade;

    private void GetSAVOffsets()
    {
        AdventureInfo = 0;
        Trainer1 = 0x64;
        Party = 0x98;
        Extra = 0x230C;
        FashionCase = 0x3F64;
        OFS_Record = 0x4B3C;
        OFS_Chatter = 0x4E74;
        Geonet = 0x8D44;
        WondercardFlags = 0x9D3C;
        Seal = 0x4E20;

        Box = 0;
    }

    private Span<byte> LockCapsuleSpan => General.Slice(0xB064, PCD.Size);

    public PCD LockCapsuleSlot
    {
        get => new(LockCapsuleSpan.ToArray());
        set => value.Data.CopyTo(LockCapsuleSpan);
    }

    public MapUnlockState4 MapUnlockState
    {
        get => (MapUnlockState4)((General[0xBAE7] >> 3) & 3);
        set => General[0xBAE7] = (byte)((General[0xBAE7] & 0xE7) | ((byte)value << 3));
    }

    #region Storage
    // box{pk4[30}[18]
    // u32 currentBox
    // u32 counter
    // g4str[18] boxNames
    // byte[18] boxWallpapers
    // -- each box is chunked, padded to nearest 0x100 (resulting in 0x10 trailing zeroes)
    // -- The final 0x16 bytes in the Storage block are unused (padding to nearest 0x100).
    private const int BOX_COUNT = 18;
    private const int BOX_SLOTS = 30;
    private const int BOX_NAME_LEN = 40; // 20 characters

    private const int BOX_DATA_LEN = (BOX_SLOTS * PokeCrypto.SIZE_4STORED) + 0x10; // 0xFF0, each box chunk is padded to nearest 0x100
    private const int BOX_END = BOX_COUNT * BOX_DATA_LEN; // 18 * 0x1000
    private const int BOX_NAME = 0x12008; // after current & counter
    private const int BOX_WP = BOX_NAME + (BOX_COUNT * BOX_NAME_LEN); // 0x122D8;
    private const int BOX_FLAGS = 18 + BOX_WP; // 0x122EA;

    public override int GetBoxOffset(int box) => box * 0x1000;
    private static int GetBoxNameOffset(int box) => BOX_NAME + (box * BOX_NAME_LEN);
    private static int GetBoxWallpaperOffset(int box) => BOX_WP + box;

    // 8 bytes current box (align 32) & (stored count?)
    public override int CurrentBox
    {
        get => Storage[BOX_END];
        set => Storage[BOX_END] = (byte)value;
    }

    public override byte[] BoxFlags
    {
        get => [ Storage[BOX_FLAGS] ];
        set => Storage[BOX_FLAGS] = value[0];
    }

    public int Counter
    {
        get => ReadInt32LittleEndian(Storage[(BOX_END + 4)..]);
        set => WriteInt32LittleEndian(Storage[(BOX_END + 4)..], value);
    }

    private Span<byte> GetBoxNameSpan(int box) => Storage.Slice(GetBoxNameOffset(box), BOX_NAME_LEN);
    public string GetBoxName(int box) => GetString(GetBoxNameSpan(box));

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        const int maxlen = 8;
        var span = GetBoxNameSpan(box);
        SetString(span, value, maxlen, StringConverterOption.ClearZero);
    }

    private static int AdjustWallpaper(int value, int shift)
    {
        // Pt's  Special Wallpapers 1-8 are shifted by +0x8
        // HG/SS Special Wallpapers 1-8 (Primo Phrases) are shifted by +0x10
        if (value >= 0x10) // special
            return value + shift;
        return value;
    }

    public int GetBoxWallpaper(int box)
    {
        int offset = GetBoxWallpaperOffset(box);
        int value = Storage[offset];
        return AdjustWallpaper(value, -0x10);
    }

    public void SetBoxWallpaper(int box, int value)
    {
        value = AdjustWallpaper(value, 0x10);
        Storage[GetBoxWallpaperOffset(box)] = (byte)value;
    }
    #endregion

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage4HGSS.Instance;
            InventoryPouch[] pouch =
            [
                new InventoryPouch4(InventoryType.Items, info, 999, 0x644), // 0x644-0x8D7 (0x8CB)
                new InventoryPouch4(InventoryType.KeyItems, info, 1, 0x8D8), // 0x8D8-0x99F (0x979)
                new InventoryPouch4(InventoryType.TMHMs, info, 99, 0x9A0), // 0x9A0-0xB33 (0xB2F)
                new InventoryPouch4(InventoryType.MailItems, info, 999, 0xB34), // 0xB34-0xB63 (0xB63)
                new InventoryPouch4(InventoryType.Medicine, info, 999, 0xB64), // 0xB64-0xC03 (0xBFB)
                new InventoryPouch4(InventoryType.Berries, info, 999, 0xC04), // 0xC04-0xD03
                new InventoryPouch4(InventoryType.Balls, info, 999, 0xD04), // 0xD04-0xD63
                new InventoryPouch4(InventoryType.BattleItems, info, 999, 0xD64), // 0xD64-0xD97
            ];
            return pouch.LoadAll(General);
        }
        set => value.SaveAll(General);
    }

    public override int M { get => ReadUInt16LittleEndian(General[0x1234..]); set => WriteUInt16LittleEndian(General[0x1234..], (ushort)value); }
    public override int X { get => ReadUInt16LittleEndian(General[0x123C..]); set => WriteUInt16LittleEndian(General[0x123C..], (ushort)(X2 = value)); }
    public override int Y { get => ReadUInt16LittleEndian(General[0x1240..]); set => WriteUInt16LittleEndian(General[0x1240..], (ushort)(Y2 = value)); }

    public override Span<byte> RivalTrash
    {
        get => RivalSpan;
        set { if (value.Length == MaxStringLengthTrainer * 2) value.CopyTo(RivalSpan); }
    }

    private Span<byte> RivalSpan => General.Slice(0x22D4, MaxStringLengthTrainer * 2);

    public override int X2 { get => ReadUInt16LittleEndian(General[0x236E..]); set => WriteUInt16LittleEndian(General[0x236E..], (ushort)value); }
    public override int Y2 { get => ReadUInt16LittleEndian(General[0x2372..]); set => WriteUInt16LittleEndian(General[0x2372..], (ushort)value); }
    public override int Z  { get => ReadUInt16LittleEndian(General[0x2376..]); set => WriteUInt16LittleEndian(General[0x2376..], (ushort)value); }

    public int Badges16
    {
        get => General[Trainer1 + 0x1F];
        set => General[Trainer1 + 0x1F] = (byte)value;
    }

    private const int OFS_GearRolodex = 0xC0EC;
    private const byte GearMaxCallers = (byte)(PokegearNumber.Ernest + 1);

    public PokegearNumber GetCallerAtIndex(int index) => (PokegearNumber)General[OFS_GearRolodex + index];
    public void SetCallerAtIndex(int index, PokegearNumber caller) => General[OFS_GearRolodex + index] = (byte)caller;

    public Span<PokegearNumber> GetPokeGearRoloDex()
    {
        var arr = General.Slice(OFS_GearRolodex, GearMaxCallers);
        return MemoryMarshal.Cast<byte, PokegearNumber>(arr);
    }

    public void SetPokeGearRoloDex(ReadOnlySpan<PokegearNumber> value)
    {
        if (value.Length > GearMaxCallers)
            throw new ArgumentOutOfRangeException(nameof(value));
        MemoryMarshal.AsBytes(value).CopyTo(General.Slice(OFS_GearRolodex, GearMaxCallers));
    }

    public void PokeGearUnlockAllCallers()
    {
        for (int i = 0; i < GearMaxCallers; i++)
            SetCallerAtIndex(i, (PokegearNumber)i);
    }

    public void PokeGearClearAllCallers(int start = 0)
    {
        var dex = GetPokeGearRoloDex();
        dex[start..].Fill(PokegearNumber.None);
    }

    private static ReadOnlySpan<PokegearNumber> NotTrainers =>
    [
        PokegearNumber.Mother,
        PokegearNumber.Professor_Elm,
        PokegearNumber.Professor_Oak,
        PokegearNumber.Ethan,
        PokegearNumber.Lyra,
        PokegearNumber.Kurt,
        PokegearNumber.Daycare_Man,
        PokegearNumber.Daycare_Lady,
        PokegearNumber.Bill,
        PokegearNumber.Bike_Shop,
        PokegearNumber.Baoba,
    ];

    public void PokeGearUnlockAllCallersNoTrainers()
    {
        var dex = GetPokeGearRoloDex();
        NotTrainers.CopyTo(dex);

        // clear remaining callers
        PokeGearClearAllCallers(NotTrainers.Length);
    }

    // Apricorn Pouch
    public int GetApricornCount(int index) => General[0xE558 + index];
    public void SetApricornCount(int index, int count) => General[0xE558 + index] = (byte)count;

    // Pokewalker
    public const int WalkerPair = 0xE5E0;
    private const int OFS_WALKER = 0xE704;

    public uint PokewalkerSteps { get => ReadUInt32LittleEndian(General[OFS_WALKER..]); set => WriteUInt32LittleEndian(General[OFS_WALKER..], value); }
    public uint PokewalkerWatts { get => ReadUInt32LittleEndian(General[(OFS_WALKER + 0x4)..]); set => WriteUInt32LittleEndian(General[(OFS_WALKER + 4)..], value); }

    public const int PokewalkerCourseFlagCount = 32;
    public void GetPokewalkerCoursesUnlocked(Span<bool> value)
    {
        if (value.Length != PokewalkerCourseFlagCount)
            throw new ArgumentOutOfRangeException(nameof(value));
        FlagUtil.GetBitFlagArray(General[(OFS_WALKER + 0x8)..], value);
    }

    public void SetPokewalkerCoursesUnlocked(ReadOnlySpan<bool> value)
    {
        if (value.Length != PokewalkerCourseFlagCount)
            throw new ArgumentOutOfRangeException(nameof(value));
        FlagUtil.SetBitFlagArray(General[(OFS_WALKER + 0x8)..], value);
    }

    public void PokewalkerCoursesUnlockAll() => PokewalkerCoursesSetAll(0x07FF_FFFFu); // 31 used
    public void PokewalkerCoursesUnlockNone() => PokewalkerCoursesSetAll(0);
    public void PokewalkerCoursesSetAll(uint bitFlags) => WriteUInt32LittleEndian(General[(OFS_WALKER + 0x8)..], bitFlags);

    // Swarm
    public override uint SwarmSeed { get => ReadUInt32LittleEndian(General[0x68A8..]); set => WriteUInt32LittleEndian(General[0x68A8..], value); }
    public override uint SwarmMaxCountModulo => 20;
    public override int BP { get => ReadUInt16LittleEndian(General[0x5BB8..]); set => WriteUInt16LittleEndian(General[0x5BB8..], (ushort)value); }

    // Roamers
    public Roamer4 RoamerRaikou => GetRoamer(0);
    public Roamer4 RoamerEntei  => GetRoamer(1);
    public Roamer4 RoamerLatias => GetRoamer(2);
    public Roamer4 RoamerLatios => GetRoamer(3);

    private Roamer4 GetRoamer(int index)
    {
        const int size = Roamer4.SIZE;
        var ofs = 0x68B4 + (index * size);
        var mem = GeneralBuffer.Slice(ofs, size);
        return new Roamer4(mem);
    }

    // Pokeathlon
    public uint PokeathlonPoints { get => ReadUInt32LittleEndian(General[0xE548..]); set => WriteUInt32LittleEndian(General[0xE548..], value); }
}

public enum MapUnlockState4 : byte
{
    Johto = 0,
    JohtoPlus = 1,
    JohtoKanto = 2,
    Invalid = 3,
}
