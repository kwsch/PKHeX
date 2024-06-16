using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> format for <see cref="GameVersion.Pt"/>
/// </summary>
public sealed class SAV4Pt : SAV4Sinnoh
{
    public SAV4Pt() : base(GeneralSize, StorageSize)
    {
        Initialize();
        Mystery = new MysteryBlock4Pt(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4Pt.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    public SAV4Pt(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize)
    {
        Initialize();
        Mystery = new MysteryBlock4Pt(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4Pt.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    public override Zukan4 Dex { get; }
    public override MysteryBlock4Pt Mystery { get; }
    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4Pt((byte[])Data.Clone()) : new SAV4Pt();
    public override GameVersion Version { get => GameVersion.Pt; set { } }
    public override PersonalTable4 Personal => PersonalTable.Pt;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_Pt;
    public override int MaxItemID => Legal.MaxItemID_4_Pt;

    public const int GeneralSize = 0xCF2C;
    private const int StorageSize = 0x121E4; // Start 0xCF2C, +4 starts box data

    protected override BlockInfo4[] ExtraBlocks =>
    [
        new BlockInfo4(0, 0x20000, 0x2AC0), // Hall of Fame
        new BlockInfo4(1, 0x23000, 0x0BB0), // Battle Hall
        new BlockInfo4(2, 0x24000, 0x1D60), // Battle Video (My Video)
        new BlockInfo4(3, 0x26000, 0x1D60), // Battle Video (Other Videos 1)
        new BlockInfo4(4, 0x28000, 0x1D60), // Battle Video (Other Videos 2)
        new BlockInfo4(5, 0x2A000, 0x1D60), // Battle Video (Other Videos 3)
    ];

    private void Initialize() => GetSAVOffsets();

    protected override int EventWork => 0xDAC;
    protected override int EventFlag => 0xFEC;
    private const int OffsetMystery = 0xB4C0;
    protected override int DaycareOffset => 0x1654;
    public override BattleFrontierFacility4 MaxFacility => BattleFrontierFacility4.Arcade;

    private const int OFS_ToughWord = 0xCEB4;
    private const int OFS_VillaFurniture = 0x111F;

    private const int PokeDex = 0x1328;

    private void GetSAVOffsets()
    {
        AdventureInfo = 0;
        Trainer1 = 0x68;
        Party = 0xA0;
        Extra = 0x2820;
        FashionCase = 0x4E38;
        OFS_Record = 0x61B0;
        OFS_Chatter = 0x64EC;
        Geonet = 0xA4C4;
        WondercardFlags = 0xB4C0;

        OFS_HONEY = 0x7F38;

        OFS_UG_Stats = 0x3CB4;
        OFS_UG_Items = 0x4538;

        PoketchStart = 0x1160;

        OFS_PoffinCase = 0x52E8;
        Seal = 0x6494;

        Box = 4;
    }

    #region Storage
    private const int OFS_Wallpaper = 0x121C6;

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
        if ((uint)box > 18)
            return 0;
        int value = Storage[GetBoxWallpaperOffset(box)];
        return AdjustWallpaper(value, -0x08);
    }

    public override void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= 18)
            return;
        value = AdjustWallpaper(value, 0x08);
        Storage[GetBoxWallpaperOffset(box)] = (byte)value;
    }

    public bool GetWallpaperUnlocked(Wallpaper4Pt wallpaperId)
    {
        if (wallpaperId < Wallpaper4Pt.Distortion)
            return true;

        var unlockableWallpaperIdx = wallpaperId - Wallpaper4Pt.Distortion;
        return FlagUtil.GetFlag(Storage, OFS_Wallpaper + (unlockableWallpaperIdx >> 3), unlockableWallpaperIdx & 7);
    }

    public void SetWallpaperUnlocked(Wallpaper4Pt wallpaperId, bool value)
    {
        if (wallpaperId < Wallpaper4Pt.Distortion)
            return; // Always unlocked

        var unlockableWallpaperIdx = wallpaperId - Wallpaper4Pt.Distortion;
        FlagUtil.SetFlag(Storage, OFS_Wallpaper + (unlockableWallpaperIdx >> 3), unlockableWallpaperIdx & 7, value);
        State.Edited = true;
    }
    #endregion

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage4Pt.Instance;
            InventoryPouch[] pouch =
            [
                new InventoryPouch4(InventoryType.Items, info, 999, 0x630),
                new InventoryPouch4(InventoryType.KeyItems, info, 1, 0x8C4),
                new InventoryPouch4(InventoryType.TMHMs, info, 99, 0x98C),
                new InventoryPouch4(InventoryType.MailItems, info, 999, 0xB1C),
                new InventoryPouch4(InventoryType.Medicine, info, 999, 0xB4C),
                new InventoryPouch4(InventoryType.Berries, info, 999, 0xBEC),
                new InventoryPouch4(InventoryType.Balls, info, 999, 0xCEC),
                new InventoryPouch4(InventoryType.BattleItems, info, 999, 0xD28),
            ];
            return pouch.LoadAll(General);
        }
        set => value.SaveAll(General);
    }

    public override int M { get => ReadUInt16LittleEndian(General[0x1280..]); set => WriteUInt16LittleEndian(General[0x1280..], (ushort)value); }
    public override int X { get => ReadUInt16LittleEndian(General[0x1288..]); set => WriteUInt16LittleEndian(General[0x1288..], (ushort)(X2 = value)); }
    public override int Y { get => ReadUInt16LittleEndian(General[0x128C..]); set => WriteUInt16LittleEndian(General[0x128C..], (ushort)(Y2 = value)); }

    public override Span<byte> RivalTrash
    {
        get => RivalSpan;
        set { if (value.Length == MaxStringLengthTrainer * 2) value.CopyTo(RivalSpan); }
    }

    private Span<byte> RivalSpan => General.Slice(0x27E8, MaxStringLengthTrainer * 2);

    public override int X2 { get => ReadUInt16LittleEndian(General[0x287E..]); set => WriteUInt16LittleEndian(General[0x287E..], (ushort)value); }
    public override int Y2 { get => ReadUInt16LittleEndian(General[0x2882..]); set => WriteUInt16LittleEndian(General[0x2882..], (ushort)value); }
    public override int Z  { get => ReadUInt16LittleEndian(General[0x2886..]); set => WriteUInt16LittleEndian(General[0x2886..], (ushort)value); }

    public override uint SafariSeed { get => ReadUInt32LittleEndian(General[0x5660..]); set => WriteUInt32LittleEndian(General[0x5660..], value); }
    public override uint SwarmSeed { get => ReadUInt32LittleEndian(General[0x5664..]); set => WriteUInt32LittleEndian(General[0x5664..], value); }
    public override uint SwarmMaxCountModulo => 22;
    public override int BP { get => ReadUInt16LittleEndian(General[0x7234..]); set => WriteUInt16LittleEndian(General[0x7234..], (ushort)value); }

    protected override ReadOnlySpan<ushort> TreeSpecies =>
    [
        000, 000, 000, 000, 000, 000,
        415, 265, 412, 420, 190, 190,
        412, 420, 415, 190, 190, 214,
        446, 446, 446, 446, 446, 446,
    ];

    public Roamer4 RoamerMesprit   => GetRoamer(0);
    public Roamer4 RoamerCresselia => GetRoamer(1);
    public Roamer4 RoamerUnused    => GetRoamer(2); // Darkrai
    public Roamer4 RoamerMoltres   => GetRoamer(3);
    public Roamer4 RoamerZapdos    => GetRoamer(4);
    public Roamer4 RoamerArticuno  => GetRoamer(5);

    private Roamer4 GetRoamer(int index)
    {
        const int size = Roamer4.SIZE;
        var ofs = 0x7FF4 + (index * size);
        var mem = GeneralBuffer.Slice(ofs, size);
        return new Roamer4(mem);
    }

    public bool GetToughWordUnlocked(ToughWord4 word)
    {
        if (word > ToughWord4.REMSleep)
            throw new ArgumentOutOfRangeException(nameof(word), word, null);
        return GetFlag(OFS_ToughWord + ((byte)word >> 3), (byte)word & 7);
    }

    public void SetToughWordUnlocked(ToughWord4 word, bool value)
    {
        if (word > ToughWord4.REMSleep)
            throw new ArgumentOutOfRangeException(nameof(word), word, null);
        SetFlag(OFS_ToughWord + ((byte)word >> 3), (byte)word & 7, value);
        State.Edited = true;
    }

    public bool GetVillaFurniturePurchased(VillaFurniture4 index)
    {
        if (index > VillaFurniture4.Chandelier)
            throw new ArgumentOutOfRangeException(nameof(index));
        return GetFlag(OFS_VillaFurniture + ((byte)index >> 3), (byte)index & 7);
    }

    public void SetVillaFurniturePurchased(VillaFurniture4 index, bool value = true)
    {
        if (index > VillaFurniture4.Chandelier)
            throw new ArgumentOutOfRangeException(nameof(index));
        SetFlag(OFS_VillaFurniture + ((byte)index >> 3), (byte)index & 7, value);
        State.Edited = true;
    }
}
