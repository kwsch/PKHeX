using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORAS"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6AO : SAV6, ISaveBlock6AO
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

        public override PersonalTable Personal => PersonalTable.AO;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItem_AO;
        public SaveBlockAccessor6AO Blocks { get; }
        protected override SaveFile CloneInternal() => new SAV6AO((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_6_AO;
        public override int MaxItemID => Legal.MaxItemID_6_AO;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;

        private void Initialize()
        {
            GTS = 0x18200; // GtsData
            Fused = 0x16A00; // UnionPokemon

            PCLayout = 0x04400;
            BattleBoxOffset = 0x04A00;
            PSS = 0x05000;
            Party = 0x14200;
            EventConst = 0x14A00;
            PokeDex = 0x15000;
            HoF = 0x19E00;
            DaycareOffset = 0x1BC00;
            BerryField = 0x1C400;
            WondercardFlags = 0x1CC00;
            Contest = 0x23600;
            SecretBase = 0x23A00;
            EonTicket = 0x319B8;
            Box = 0x33000;
            JPEG = 0x67C00;

            EventFlag = EventConst + 0x2FC;
            WondercardData = WondercardFlags + 0x100;
            Daycare2 = DaycareOffset + 0x1F0;
        }

        public int EonTicket { get; private set; }
        public int Contest { get; private set; }
        private int Daycare2 { get; set; }
        public int SecretBase { get; private set; }
        public int GTS { get; private set; }
        public int Fused { get; private set; }

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
        public SubEventLog6 SUBE => Blocks.SUBE;
        public ConfigSave6 Config => Blocks.Config;
        public Encount6 Encount => Blocks.Encount;

        public Misc6AO Misc => Blocks.Misc;
        public Zukan6AO Zukan => Blocks.Zukan;
        #endregion

        public override GameVersion Version => Game switch
        {
            (int) GameVersion.AS => GameVersion.AS,
            (int) GameVersion.OR => GameVersion.OR,
            _ => GameVersion.Invalid
        };

        public override bool GetCaught(int species) => Blocks.Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Blocks.Zukan.GetSeen(species);
        public override void SetSeen(int species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
        public override void SetCaught(int species, bool caught) => Blocks.Zukan.SetCaught(species, caught);
        protected override void SetDex(PKM pkm) => Blocks.Zukan.SetDex(pkm);

        public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
        public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
        public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
        public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }

        public override int MultiplayerSpriteID
        {
            get => Blocks.Status.MultiplayerSpriteID_1;
            set => Blocks.Status.MultiplayerSpriteID_1 = Blocks.Status.MultiplayerSpriteID_2 = value;
        }

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => true;

        public override int GetDaycareSlotOffset(int loc, int slot)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            return ofs + 8 + (slot * (SIZE_STORED + 8));
        }

        public override uint? GetDaycareEXP(int loc, int slot)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            return BitConverter.ToUInt32(Data, ofs + ((SIZE_STORED + 8) * slot) + 4);
        }

        public override bool? IsDaycareOccupied(int loc, int slot)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            return Data[ofs + ((SIZE_STORED + 8) * slot)] == 1;
        }

        public override string GetDaycareRNGSeed(int loc)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            var data = Data.Skip(ofs + 0x1E8).Take(DaycareSeedSize / 2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        public override bool? IsDaycareHasEgg(int loc)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            return Data[ofs + 0x1E0] == 1;
        }

        public override void SetDaycareEXP(int loc, int slot, uint EXP)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            BitConverter.GetBytes(EXP).CopyTo(Data, ofs + ((SIZE_STORED + 8) * slot) + 4);
        }

        public override void SetDaycareOccupied(int loc, int slot, bool occupied)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            Data[ofs + ((SIZE_STORED + 8) * slot)] = occupied ? 1 : 0;
        }

        public override void SetDaycareRNGSeed(int loc, string seed)
        {
            if (loc != 0)
                return;
            if (DaycareOffset < 0)
                return;
            if (seed.Length > DaycareSeedSize)
                return;

            Util.GetBytesFromHexString(seed).CopyTo(Data, DaycareOffset + 0x1E8);
        }

        public override void SetDaycareHasEgg(int loc, bool hasEgg)
        {
            int ofs = loc == 0 ? DaycareOffset : Daycare2;
            Data[ofs + 0x1E0] = hasEgg ? 1 : 0;
        }

        public override string JPEGTitle => HasJPPEGData ? string.Empty : StringConverter.GetString6(Data, JPEG, 0x1A);
        public override byte[] GetJPEGData() => HasJPPEGData ? Array.Empty<byte>() : GetData(JPEG + 0x54, 0xE004);
        private bool HasJPPEGData => Data[JPEG + 0x54] == 0xFF;

        protected override bool[] MysteryGiftReceivedFlags { get => Blocks.MysteryGift.GetReceivedFlags(); set => Blocks.MysteryGift.SetReceivedFlags(value); }
        protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.MysteryGift.GetGifts(); set => Blocks.MysteryGift.SetGifts(value); }

        public override int CurrentBox { get => Blocks.BoxLayout.CurrentBox; set => Blocks.BoxLayout.CurrentBox = value; }
        protected override int GetBoxWallpaperOffset(int box) => Blocks.BoxLayout.GetBoxWallpaperOffset(box);
        public override int BoxesUnlocked { get => Blocks.BoxLayout.BoxesUnlocked; set => Blocks.BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => Blocks.BoxLayout.BoxFlags; set => Blocks.BoxLayout.BoxFlags = value; }

        public bool BattleBoxLocked
        {
            get => Blocks.BattleBox.Locked;
            set => Blocks.BattleBox.Locked = value;
        }
    }
}
