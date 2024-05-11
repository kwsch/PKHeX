using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORAS"/>.
/// </summary>
/// <inheritdoc cref="SAV6" />
public sealed class SAV6AO : SAV6, ISaveBlock6AO, IMultiplayerSprite, IBoxDetailName, IBoxDetailWallpaper, IMysteryGiftStorageProvider, IDaycareMulti
{
    public SAV6AO(byte[] data) : base(data, SaveBlockAccessor6AO.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AO(this);
        Initialize();
    }

    public SAV6AO() : base(SaveUtil.SIZE_G6ORAS, SaveBlockAccessor6AO.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6AO(this);
        Initialize();
        ClearBoxes();
    }

    public override PersonalTable6AO Personal => PersonalTable.AO;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_AO;
    public SaveBlockAccessor6AO Blocks { get; }
    protected override SAV6AO CloneInternal() => new((byte[])Data.Clone());
    public override ushort MaxMoveID => Legal.MaxMoveID_6_AO;
    public override int MaxItemID => Legal.MaxItemID_6_AO;
    public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;

    public override bool HasPokeDex => true;

    private void Initialize()
    {
        PSS = 0x05000;
        Party = 0x14200;
        HoF = 0x19E00;
        Box = 0x33000;
        JPEG = 0x67C00;
    }

    #region Blocks
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem6AO Items => Blocks.Items;
    public override ItemInfo6 ItemInfo => Blocks.ItemInfo;
    public override GameTime6 GameTime => Blocks.GameTime;
    public override Situation6 Situation => Blocks.Situation;
    public override PlayTime6 Played => Blocks.Played;
    public override MyStatus6 Status => Blocks.Status;
    public override RecordBlock6AO Records => Blocks.Records;
    public override EventWork6 EventWork => Blocks.EventWork;
    public UnionPokemon6 Fused => Blocks.Fused;
    public GTS6 GTS => Blocks.GTS;
    public Puff6 Puff => Blocks.Puff;
    public OPower6 OPower => Blocks.OPower;
    public LinkBlock6 Link => Blocks.Link;
    public BoxLayout6 BoxLayout => Blocks.BoxLayout;
    public BattleBox6 BattleBox => Blocks.BattleBox;
    public MysteryBlock6 MysteryGift => Blocks.MysteryGift;
    public SuperTrainBlock SuperTrain => Blocks.SuperTrain;
    public MaisonBlock Maison => Blocks.Maison;
    public SubEventLog6AO SUBE => Blocks.SUBE;
    public ConfigSave6 Config => Blocks.Config;
    public Encount6 Encount => Blocks.Encount;
    public Misc6AO Misc => Blocks.Misc;
    public Zukan6AO Zukan => Blocks.Zukan;
    public SecretBase6Block SecretBase => Blocks.SecretBase;
    public BerryField6AO BerryField => Blocks.BerryField;
    public Contest6 Contest => Blocks.Contest;
    public HallOfFame6 HallOfFame => Blocks.HallOfFame;

    MyItem ISaveBlock6Core.Items => Items;
    SubEventLog6 ISaveBlock6Main.SUBE => SUBE;
    RecordBlock6 ISaveBlock6Core.Records => Records;
    IMysteryGiftStorage IMysteryGiftStorageProvider.MysteryGiftStorage => MysteryGift;
    #endregion

    public override bool IsVersionValid() => Version is GameVersion.AS or GameVersion.OR;

    public override bool GetCaught(ushort species) => Blocks.Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Blocks.Zukan.GetSeen(species);
    public override void SetSeen(ushort species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
    public override void SetCaught(ushort species, bool caught) => Blocks.Zukan.SetCaught(species, caught);
    protected override void SetDex(PKM pk) => Blocks.Zukan.SetDex(pk);

    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
    public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
    public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
    public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }

    public int MultiplayerSpriteID
    {
        get => Blocks.Status.MultiplayerSpriteID_1;
        set => Blocks.Status.MultiplayerSpriteID_1 = Blocks.Status.MultiplayerSpriteID_2 = value;
    }

    // Daycare

    public override string JPEGTitle => !HasJPEGData ? string.Empty : StringConverter6.GetString(Data.AsSpan(JPEG, 0x1A));
    public override byte[] GetJPEGData() => !HasJPEGData ? [] : Data.AsSpan(JPEG + 0x54, 0xE004).ToArray();
    private bool HasJPEGData => Data[JPEG + 0x54] == 0xFF;

    public override int CurrentBox { get => Blocks.BoxLayout.CurrentBox; set => Blocks.BoxLayout.CurrentBox = value; }
    public override int BoxesUnlocked { get => Blocks.BoxLayout.BoxesUnlocked; set => Blocks.BoxLayout.BoxesUnlocked = value; }
    public override byte[] BoxFlags { get => Blocks.BoxLayout.BoxFlags; set => Blocks.BoxLayout.BoxFlags = value; }
    public int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
    public void SetBoxWallpaper(int box, int wallpaper) => BoxLayout.SetBoxWallpaper(box, wallpaper);
    public string GetBoxName(int box) => BoxLayout.GetBoxName(box);
    public void SetBoxName(int box, ReadOnlySpan<char> name) => BoxLayout.SetBoxName(box, name);

    public bool BattleBoxLocked
    {
        get => Blocks.BattleBox.Locked;
        set => Blocks.BattleBox.Locked = value;
    }

    public int DaycareCount => 2;
    public IDaycareStorage this[int index] => Blocks.Daycare[index];
}
