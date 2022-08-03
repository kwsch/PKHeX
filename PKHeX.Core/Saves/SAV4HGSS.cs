using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> format for <see cref="GameVersion.HGSS"/>
/// </summary>
public sealed class SAV4HGSS : SAV4
{
    public SAV4HGSS() : base(GeneralSize, StorageSize)
    {
        Initialize();
        Dex = new Zukan4(this, PokeDex);
    }

    public SAV4HGSS(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize + GeneralGap)
    {
        Initialize();
        Dex = new Zukan4(this, PokeDex);
    }

    public override Zukan4 Dex { get; }
    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4HGSS((byte[])Data.Clone()) : new SAV4HGSS();

    public override IPersonalTable Personal => PersonalTable.HGSS;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_HGSS;
    public override int MaxItemID => Legal.MaxItemID_4_HGSS;
    private const int GeneralSize = 0xF628;
    private const int StorageSize = 0x12310; // Start 0xF700, +0 starts box data
    private const int GeneralGap = 0xD8;
    protected override int StorageStart => 0xF700; // unused section right after GeneralSize, alignment?
    protected override int FooterSize => 0x10;

    private void Initialize()
    {
        Version = GameVersion.HGSS;
        GetSAVOffsets();
    }

    protected override int EventWork => 0xDE4;
    protected override int EventFlag => 0x10C4;

    private void GetSAVOffsets()
    {
        AdventureInfo = 0;
        Trainer1 = 0x64;
        Party = 0x98;
        PokeDex = 0x12B8;
        WondercardFlags = 0x9D3C;
        WondercardData = 0x9E3C;

        DaycareOffset = 0x15FC;
        Seal = 0x4E20;

        Box = 0;
    }

    private Span<byte> LockCapsuleSpan => General.AsSpan(0xB064, PCD.Size);

    public PCD LockCapsuleSlot
    {
        get => new(LockCapsuleSpan.ToArray());
        set => value.Data.CopyTo(LockCapsuleSpan);
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
    protected override int GetBoxWallpaperOffset(int box) => BOX_WP + box;

    // 8 bytes current box (align 32) & (stored count?)
    public override int CurrentBox
    {
        get => Storage[BOX_END];
        set => Storage[BOX_END] = (byte)value;
    }

    public override byte[] BoxFlags
    {
        get => new[] { Storage[BOX_FLAGS] };
        set => Storage[BOX_FLAGS] = value[0];
    }

    public int Counter
    {
        get => ReadInt32LittleEndian(Storage.AsSpan(BOX_END + 4));
        set => WriteInt32LittleEndian(Storage.AsSpan(BOX_END + 4), value);
    }

    public override string GetBoxName(int box) => GetString(Storage.AsSpan(GetBoxNameOffset(box), BOX_NAME_LEN));

    public override void SetBoxName(int box, string value)
    {
        const int maxlen = 8;
        var span = Storage.AsSpan(GetBoxNameOffset(box), BOX_NAME_LEN);
        SetString(span, value.AsSpan(), maxlen, StringConverterOption.ClearZero);
    }

    private static int AdjustWallpaper(int value, int shift)
    {
        // Pt's  Special Wallpapers 1-8 are shifted by +0x8
        // HG/SS Special Wallpapers 1-8 (Primo Phrases) are shifted by +0x10
        if (value >= 0x10) // special
            return value + shift;
        return value;
    }

    public override int GetBoxWallpaper(int box)
    {
        int offset = GetBoxWallpaperOffset(box);
        int value = Storage[offset];
        return AdjustWallpaper(value, -0x10);
    }

    public override void SetBoxWallpaper(int box, int value)
    {
        value = AdjustWallpaper(value, 0x10);
        Storage[GetBoxWallpaperOffset(box)] = (byte)value;
    }
    #endregion

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            {
                new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_HGSS, 999, 0x644), // 0x644-0x8D7 (0x8CB)
                new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_HGSS, 1, 0x8D8), // 0x8D8-0x99F (0x979)
                new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_HGSS, 99, 0x9A0), // 0x9A0-0xB33 (0xB2F)
                new InventoryPouch4(InventoryType.MailItems, Legal.Pouch_Mail_HGSS, 999, 0xB34), // 0xB34-0xB63 (0xB63)
                new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_HGSS, 999, 0xB64), // 0xB64-0xC03 (0xBFB)
                new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berries_HGSS, 999, 0xC04), // 0xC04-0xD03
                new InventoryPouch4(InventoryType.Balls, Legal.Pouch_Ball_HGSS, 999, 0xD04), // 0xD04-0xD63
                new InventoryPouch4(InventoryType.BattleItems, Legal.Pouch_Battle_HGSS, 999, 0xD64), // 0xD64-0xD97
            };
            return pouch.LoadAll(General);
        }
        set => value.SaveAll(General);
    }

    public override int M { get => ReadUInt16LittleEndian(General.AsSpan(0x1234)); set => WriteUInt16LittleEndian(General.AsSpan(0x1234), (ushort)value); }
    public override int X { get => ReadUInt16LittleEndian(General.AsSpan(0x123C)); set => WriteUInt16LittleEndian(General.AsSpan(0x123C), (ushort)(X2 = value)); }
    public override int Y { get => ReadUInt16LittleEndian(General.AsSpan(0x1240)); set => WriteUInt16LittleEndian(General.AsSpan(0x1240), (ushort)(Y2 = value)); }

    public override Span<byte> Rival_Trash
    {
        get => General.AsSpan(0x22D4, OTLength * 2);
        set { if (value.Length == OTLength * 2) value.CopyTo(General.AsSpan(0x22D4)); }
    }

    public override int X2 { get => ReadUInt16LittleEndian(General.AsSpan(0x236E)); set => WriteUInt16LittleEndian(General.AsSpan(0x236E), (ushort)value); }
    public override int Y2 { get => ReadUInt16LittleEndian(General.AsSpan(0x2372)); set => WriteUInt16LittleEndian(General.AsSpan(0x2372), (ushort)value); }
    public override int Z  { get => ReadUInt16LittleEndian(General.AsSpan(0x2376)); set => WriteUInt16LittleEndian(General.AsSpan(0x2376), (ushort)value); }

    public int Badges16
    {
        get => General[Trainer1 + 0x1F];
        set => General[Trainer1 + 0x1F] = (byte)value;
    }

    private const int OFS_GearRolodex = 0xC0EC;
    private const byte GearMaxCallers = (byte)(PokegearNumber.Ernest + 1);

    public PokegearNumber GetCallerAtIndex(int index) => (PokegearNumber)General[OFS_GearRolodex + index];
    public void SetCallerAtIndex(int index, PokegearNumber caller) => General[OFS_GearRolodex + index] = (byte)caller;

    public PokegearNumber[] GetPokeGearRoloDex()
    {
        var arr = General.AsSpan(OFS_GearRolodex, GearMaxCallers);
        return MemoryMarshal.Cast<byte, PokegearNumber>(arr).ToArray();
    }

    public void SetPokeGearRoloDex(ReadOnlySpan<PokegearNumber> value)
    {
        if (value.Length > GearMaxCallers)
            throw new ArgumentOutOfRangeException(nameof(value));
        MemoryMarshal.Cast<PokegearNumber, byte>(value).CopyTo(General.AsSpan(OFS_GearRolodex, GearMaxCallers));
    }

    public void PokeGearUnlockAllCallers()
    {
        for (int i = 0; i < GearMaxCallers; i++)
            SetCallerAtIndex(i, (PokegearNumber)i);
    }

    public void PokeGearClearAllCallers(int start = 0)
    {
        for (int i = start; i < GearMaxCallers; i++)
            SetCallerAtIndex(i, PokegearNumber.None);
    }

    public void PokeGearUnlockAllCallersNoTrainers()
    {
        var nonTrainers = new[]
        {
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
        };
        for (int i = 0; i < nonTrainers.Length; i++)
            SetCallerAtIndex(i, nonTrainers[i]);

        // clear remaining callers
        PokeGearClearAllCallers(nonTrainers.Length);
    }

    // Apricorn Pouch
    public int GetApricornCount(int index) => General[0xE558 + index];
    public void SetApricornCount(int index, int count) => General[0xE558 + index] = (byte)count;

    // Pokewalker
    public const int WalkerPair = 0xE5E0;
    private const int OFS_WALKER = 0xE704;

    public uint PokewalkerSteps { get => ReadUInt32LittleEndian(General.AsSpan(OFS_WALKER)); set => WriteUInt32LittleEndian(General.AsSpan(OFS_WALKER), value); }
    public uint PokewalkerWatts { get => ReadUInt32LittleEndian(General.AsSpan(OFS_WALKER + 0x4)); set => WriteUInt32LittleEndian(General.AsSpan(OFS_WALKER + 4), value); }

    public bool[] GetPokewalkerCoursesUnlocked() => ArrayUtil.GitBitFlagArray(General.AsSpan(OFS_WALKER + 0x8), 32);
    public void SetPokewalkerCoursesUnlocked(ReadOnlySpan<bool> value) => ArrayUtil.SetBitFlagArray(General.AsSpan(OFS_WALKER + 0x8), value);

    public void PokewalkerCoursesSetAll(uint value = 0x07FF_FFFFu) => WriteUInt32LittleEndian(General.AsSpan(OFS_WALKER + 0x8), value);

    public override uint SwarmSeed { get => ReadUInt32LittleEndian(General.AsSpan(0x68A8)); set => WriteUInt32LittleEndian(General.AsSpan(0x68A8), value); }
    public override uint SwarmMaxCountModulo => 20;

    public Roamer4 RoamerRaikou => new(General, 0x68B4);
    public Roamer4 RoamerEntei  => new(General, 0x68C8);
    public Roamer4 RoamerLatias => new(General, 0x68DC);
    public Roamer4 RoamerLatios => new(General, 0x68F0);
}
