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
        Dex = new Zukan4(this, PokeDex);
    }

    public SAV4Pt(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize)
    {
        Initialize();
        Dex = new Zukan4(this, PokeDex);
    }

    public override Zukan4 Dex { get; }
    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4Pt((byte[])Data.Clone()) : new SAV4Pt();
    public override IPersonalTable Personal => PersonalTable.Pt;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_Pt;
    public override int MaxItemID => Legal.MaxItemID_4_Pt;

    private const int GeneralSize = 0xCF2C;
    private const int StorageSize = 0x121E4; // Start 0xCF2C, +4 starts box data
    protected override int StorageStart => GeneralSize;

    private void Initialize()
    {
        Version = GameVersion.Pt;
        GetSAVOffsets();
    }

    protected override int EventWork => 0xDAC;
    protected override int EventFlag => 0xFEC;

    private void GetSAVOffsets()
    {
        AdventureInfo = 0;
        Trainer1 = 0x68;
        Party = 0xA0;
        PokeDex = 0x1328;
        WondercardFlags = 0xB4C0;
        WondercardData = 0xB5C0;

        DaycareOffset = 0x1654;
        OFS_HONEY = 0x7F38;

        OFS_UG_Stats = 0x3CB4;
        OFS_UG_Items = 0x4538;

        PoketchStart = 0x1160;

        OFS_PoffinCase = 0x52E8;
        Seal = 0x6494;

        Box = 4;
    }

    #region Storage
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
    #endregion

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            {
                new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_Pt, 999, 0x630),
                new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_Pt, 1, 0x8C4),
                new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_Pt, 99, 0x98C),
                new InventoryPouch4(InventoryType.MailItems, Legal.Pouch_Mail_Pt, 999, 0xB1C),
                new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_Pt, 999, 0xB4C),
                new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berries_Pt, 999, 0xBEC),
                new InventoryPouch4(InventoryType.Balls, Legal.Pouch_Ball_Pt, 999, 0xCEC),
                new InventoryPouch4(InventoryType.BattleItems, Legal.Pouch_Battle_Pt, 999, 0xD28),
            };
            return pouch.LoadAll(General);
        }
        set => value.SaveAll(General);
    }

    public override int M { get => ReadUInt16LittleEndian(General.AsSpan(0x1280)); set => WriteUInt16LittleEndian(General.AsSpan(0x1280), (ushort)value); }
    public override int X { get => ReadUInt16LittleEndian(General.AsSpan(0x1288)); set => WriteUInt16LittleEndian(General.AsSpan(0x1288), (ushort)(X2 = value)); }
    public override int Y { get => ReadUInt16LittleEndian(General.AsSpan(0x128C)); set => WriteUInt16LittleEndian(General.AsSpan(0x128C), (ushort)(Y2 = value)); }

    public override Span<byte> Rival_Trash
    {
        get => General.AsSpan(0x27E8, OTLength * 2);
        set { if (value.Length == OTLength * 2) value.CopyTo(General.AsSpan(0x27E8)); }
    }

    public override int X2 { get => ReadUInt16LittleEndian(General.AsSpan(0x287E)); set => WriteUInt16LittleEndian(General.AsSpan(0x287E), (ushort)value); }
    public override int Y2 { get => ReadUInt16LittleEndian(General.AsSpan(0x2882)); set => WriteUInt16LittleEndian(General.AsSpan(0x2882), (ushort)value); }
    public override int Z { get => ReadUInt16LittleEndian(General.AsSpan(0x2886)); set => WriteUInt16LittleEndian(General.AsSpan(0x2886), (ushort)value); }

    public override uint SafariSeed { get => ReadUInt32LittleEndian(General.AsSpan(0x5660)); set => WriteUInt32LittleEndian(General.AsSpan(0x5660), value); }
    public override uint SwarmSeed { get => ReadUInt32LittleEndian(General.AsSpan(0x5664)); set => WriteUInt32LittleEndian(General.AsSpan(0x5664), value); }
    public override uint SwarmMaxCountModulo => 22;

    public Roamer4 RoamerMesprit   => new(General, 0x7FF4);
    public Roamer4 RoamerCresselia => new(General, 0x8008);
    public Roamer4 RoamerUnused    => new(General, 0x801C); // Darkrai
    public Roamer4 RoamerArticuno  => new(General, 0x8030);
    public Roamer4 RoamerZapdos    => new(General, 0x8044);
    public Roamer4 RoamerMoltres   => new(General, 0x8058);
}
