using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.XY"/>.
/// </summary>
/// <inheritdoc cref="SAV6" />
public sealed class SAV6XY : SAV6, ISaveBlock6XY, IMultiplayerSprite, IBoxDetailName, IBoxDetailWallpaper, IDaycareStorage, IDaycareEggState, IDaycareExperience, IDaycareRandomState<ulong>
{
    public SAV6XY(byte[] data) : base(data, SaveBlockAccessor6XY.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6XY(this);
        Initialize();
    }

    public SAV6XY() : base(SaveUtil.SIZE_G6XY, SaveBlockAccessor6XY.BlockMetadataOffset)
    {
        Blocks = new SaveBlockAccessor6XY(this);
        Initialize();
        ClearBoxes();
    }

    public override PersonalTable6XY Personal => PersonalTable.XY;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_XY;
    public SaveBlockAccessor6XY Blocks { get; }
    protected override SAV6XY CloneInternal() => new((byte[])Data.Clone());
    public override ushort MaxMoveID => Legal.MaxMoveID_6_XY;
    public override int MaxItemID => Legal.MaxItemID_6_XY;
    public override int MaxAbilityID => Legal.MaxAbilityID_6_XY;

    protected override int EventWork => 0x14A00;
    protected override int EventFlag => EventWork + 0x2F0;

    public override bool HasPokeDex => true;
    public override bool HasWondercards => true;

    private const int DaycareOffset = 0x1B200;
    private void Initialize()
    {
        // Enable Features
        Party = 0x14200;
        BattleBoxOffset = 0x04A00;
        PSS = 0x05000;
        HoF = 0x19400;

        BerryField = 0x1B800;
        WondercardFlags = 0x1BC00;
        Box = 0x22600;
        JPEG = 0x57200;

        // Extra Viewable Slots
        Fused = 0x16000;
        GTS = 0x17800;
    }

    public int GTS { get; private set; } = int.MinValue;
    public int Fused { get; private set; } = int.MinValue;

    #region Blocks
    public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
    public override MyItem Items => Blocks.Items;
    public override ItemInfo6 ItemInfo => Blocks.ItemInfo;
    public override GameTime6 GameTime => Blocks.GameTime;
    public override Situation6 Situation => Blocks.Situation;
    public override PlayTime6 Played => Blocks.Played;
    public override MyStatus6 Status => Blocks.Status;
    public override RecordBlock6 Records => Blocks.Records;
    public Puff6 Puff => Blocks.Puff;
    public OPower6 OPower => Blocks.OPower;
    public LinkBlock6 Link => Blocks.Link;
    public BoxLayout6 BoxLayout => Blocks.BoxLayout;
    public BattleBox6 BattleBox => Blocks.BattleBox;
    public MysteryBlock6 MysteryGift => Blocks.MysteryGift;
    public SuperTrainBlock SuperTrain => Blocks.SuperTrain;
    public MaisonBlock Maison => Blocks.Maison;
    public Zukan6XY Zukan => Blocks.Zukan;
    public Misc6XY Misc => Blocks.Misc;
    public Fashion6XY Fashion => Blocks.Fashion;
    public SubEventLog6 SUBE => Blocks.SUBE;
    public ConfigSave6 Config => Blocks.Config;
    public Encount6 Encount => Blocks.Encount;
    #endregion

    protected override void SetDex(PKM pk) => Blocks.Zukan.SetDex(pk);

    // Daycare - delegate from block
    public int DaycareSlotCount => Blocks.Daycare.DaycareSlotCount;
    public Memory<byte> GetDaycareSlot(int index) => Blocks.Daycare.GetDaycareSlot(index);
    public bool IsDaycareOccupied(int index) => Blocks.Daycare.IsDaycareOccupied(index);
    public void SetDaycareOccupied(int index, bool occupied) => Blocks.Daycare.SetDaycareOccupied(index, occupied);
    public uint GetDaycareEXP(int index) => Blocks.Daycare.GetDaycareEXP(index);
    public void SetDaycareEXP(int index, uint exp) => Blocks.Daycare.SetDaycareEXP(index, exp);

    public bool IsEggAvailable
    {
        get => Blocks.Daycare.IsEggAvailable;
        set => Blocks.Daycare.IsEggAvailable = value;
    }

    ulong IDaycareRandomState<ulong>.Seed
    {
        get => Blocks.Daycare.Seed;
        set => Blocks.Daycare.Seed = value;
    }

    public override string JPEGTitle => !HasJPPEGData ? string.Empty : StringConverter6.GetString(Data.AsSpan(JPEG, 0x1A));
    public override byte[] GetJPEGData() => !HasJPPEGData ? [] : Data.AsSpan(JPEG + 0x54, 0xE004).ToArray();
    private bool HasJPPEGData => Data[JPEG + 0x54] == 0xFF;

    public void UnlockAllFriendSafariSlots()
    {
        // Unlock + reveal all safari slots if friend data is present
        const int start = 0x1E7FF;
        const int size = 0x15;
        for (int i = 1; i < 101; i++)
        {
            int ofs = start + (i * size);
            if (Data[ofs] != 0) // no friend data == 0x00
                Data[ofs] = 0x3D;
        }

        State.Edited = true;
    }

    public override bool IsVersionValid() => Version is GameVersion.X or GameVersion.Y;

    protected override bool[] MysteryGiftReceivedFlags { get => Blocks.MysteryGift.GetReceivedFlags(); set => Blocks.MysteryGift.SetReceivedFlags(value); }
    protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.MysteryGift.GetGifts(); set => Blocks.MysteryGift.SetGifts(value); }

    public override bool GetCaught(ushort species) => Blocks.Zukan.GetCaught(species);
    public override bool GetSeen(ushort species) => Blocks.Zukan.GetSeen(species);
    public override void SetSeen(ushort species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
    public override void SetCaught(ushort species, bool caught) => Blocks.Zukan.SetCaught(species, caught);

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

    public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
    public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
    public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
    public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }

    public int MultiplayerSpriteID
    {
        get => Blocks.Status.MultiplayerSpriteID_1;
        set => Blocks.Status.MultiplayerSpriteID_1 = Blocks.Status.MultiplayerSpriteID_2 = value;
    }
}
