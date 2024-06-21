using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> format for <see cref="GameVersion.DP"/>
/// </summary>
public sealed class SAV4DP : SAV4Sinnoh
{
    public SAV4DP() : base(GeneralSize, StorageSize)
    {
        Initialize();
        Mystery = new MysteryBlock4DP(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4DP.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    public SAV4DP(byte[] data) : base(data, GeneralSize, StorageSize, GeneralSize)
    {
        Initialize();
        Mystery = new MysteryBlock4DP(this, GeneralBuffer.Slice(OffsetMystery, MysteryBlock4DP.Size));
        Dex = new Zukan4(this, GeneralBuffer[PokeDex..]);
    }

    private const int OffsetMystery = 0xA6D0;
    private const int PokeDex = 0x12DC;
    public override Zukan4 Dex { get; }
    public override MysteryBlock4DP Mystery { get; }

    protected override SAV4 CloneInternal4() => State.Exportable ? new SAV4DP((byte[])Data.Clone()) : new SAV4DP();
    public override GameVersion Version { get => GameVersion.DP; set { } }
    public override PersonalTable4 Personal => PersonalTable.DP;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_DP;
    public override int MaxItemID => Legal.MaxItemID_4_DP;

    public const int GeneralSize = 0xC100;
    private const int StorageSize = 0x121E0; // Start 0xC100, +4 starts box data

    protected override BlockInfo4[] ExtraBlocks =>
    [
        new BlockInfo4(0, 0x20000, 0x2AC0), // Hall of Fame
    ];

    private void Initialize()
    {
        Version = GameVersion.DP;
        GetSAVOffsets();
    }

    protected override int EventWork => 0xD9C;
    protected override int EventFlag => 0xFDC;
    protected override int DaycareOffset => 0x141C;
    public override BattleFrontierFacility4 MaxFacility => BattleFrontierFacility4.Tower;

    private void GetSAVOffsets()
    {
        AdventureInfo = 0;
        Trainer1 = 0x64;
        Party = 0x98;
        FashionCase = 0x4BA8;
        OFS_Record = 0x5F08;
        OFS_Chatter = 0x61CC;
        Geonet = 0x96D8;
        WondercardFlags = 0xA6D0;
        OFS_HONEY = 0x72E4;
        OFS_UG_Stats = 0x3A2C;
        OFS_UG_Items = 0x42B0;

        PoketchStart = 0x114C;
        Seal = 0x6178;

        OFS_PoffinCase = 0x5050;

        Box = 4;
    }

    #region Storage
    public override int GetBoxWallpaper(int box)
    {
        if ((uint)box >= 18)
            return 0;
        return Storage[GetBoxWallpaperOffset(box)];
    }

    public override void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= 18)
            return;
        Storage[GetBoxWallpaperOffset(box)] = (byte)value;
    }
    #endregion

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage4DP.Instance;
            InventoryPouch[] pouch =
            [
                new InventoryPouch4(InventoryType.Items, info, 999, 0x624),
                new InventoryPouch4(InventoryType.KeyItems, info, 1, 0x8B8),
                new InventoryPouch4(InventoryType.TMHMs, info, 99, 0x980),
                new InventoryPouch4(InventoryType.MailItems, info, 999, 0xB10),
                new InventoryPouch4(InventoryType.Medicine, info, 999, 0xB40),
                new InventoryPouch4(InventoryType.Berries, info, 999, 0xBE0),
                new InventoryPouch4(InventoryType.Balls, info, 999, 0xCE0),
                new InventoryPouch4(InventoryType.BattleItems, info, 999, 0xD1C),
            ];
            return pouch.LoadAll(General);
        }
        set => value.SaveAll(General);
    }

    public override int M { get => ReadUInt16LittleEndian(General[0x1238..]); set => WriteUInt16LittleEndian(General[0x1238..], (ushort)value); }
    public override int X { get => ReadUInt16LittleEndian(General[0x1240..]); set => WriteUInt16LittleEndian(General[0x1240..], (ushort)(X2 = value)); }
    public override int Y { get => ReadUInt16LittleEndian(General[0x1244..]); set => WriteUInt16LittleEndian(General[0x1244..], (ushort)(Y2 = value)); }

    public override Span<byte> RivalTrash
    {
        get => General.Slice(0x25A8, MaxStringLengthTrainer * 2);
        set { if (value.Length == MaxStringLengthTrainer * 2) value.CopyTo(General[0x25A8..]); }
    }

    public override int X2 { get => ReadUInt16LittleEndian(General[0x25FA..]); set => WriteUInt16LittleEndian(General[0x25FA..], (ushort)value); }
    public override int Y2 { get => ReadUInt16LittleEndian(General[0x25FE..]); set => WriteUInt16LittleEndian(General[0x25FE..], (ushort)value); }
    public override int Z { get => ReadUInt16LittleEndian(General[0x2602..]); set => WriteUInt16LittleEndian(General[0x2602..], (ushort)value); }

    public override uint SafariSeed { get => ReadUInt32LittleEndian(General[0x53C4..]); set => WriteUInt32LittleEndian(General[0x53C4..], value); }
    public override uint SwarmSeed { get => ReadUInt32LittleEndian(General[0x53C8..]); set => WriteUInt32LittleEndian(General[0x53C8..], value); }
    public override uint SwarmMaxCountModulo => 28;
    public override int BP { get => ReadUInt16LittleEndian(General[0x65F8..]); set => WriteUInt16LittleEndian(General[0x65F8..], (ushort)value); }

    protected override ReadOnlySpan<ushort> TreeSpecies =>
    [
        000, 000, 000, 000, 000, 000,
        265, 266, 415, 412, 420, 190,
        415, 412, 420, 190, 214, 265,
        446, 446, 446, 446, 446, 446,
    ];

    public Roamer4 RoamerMesprit   => GetRoamer(0);
    public Roamer4 RoamerCresselia => GetRoamer(1);
    public Roamer4 RoamerUnused    => GetRoamer(2); // Darkrai

    private Roamer4 GetRoamer(int index)
    {
        const int size = Roamer4.SIZE;
        var ofs = 0x73A0 + (index * size);
        var mem = GeneralBuffer.Slice(ofs, size);
        return new Roamer4(mem);
    }
}
