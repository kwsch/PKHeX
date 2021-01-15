using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.XY"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6XY : SAV6, ISaveBlock6XY
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

        public override PersonalTable Personal => PersonalTable.XY;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItem_XY;
        public SaveBlockAccessor6XY Blocks { get; }
        protected override SaveFile CloneInternal() => new SAV6XY((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_6_XY;
        public override int MaxItemID => Legal.MaxItemID_6_XY;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_XY;

        private void Initialize()
        {
            // Enable Features
            Party = 0x14200;
            PCLayout = 0x4400;
            BattleBoxOffset = 0x04A00;
            PSS = 0x05000;
            EventConst = 0x14A00;
            PokeDex = 0x15000;
            HoF = 0x19400;
            DaycareOffset = 0x1B200;
            BerryField = 0x1B800;
            WondercardFlags = 0x1BC00;
            Box = 0x22600;
            JPEG = 0x57200;

            EventFlag = EventConst + 0x2FC;
            WondercardData = WondercardFlags + 0x100;

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

        protected override void SetDex(PKM pkm) => Blocks.Zukan.SetDex(pkm);

        // Daycare
        public override int DaycareSeedSize => 16;
        public override bool HasTwoDaycares => false;
        public override bool? IsDaycareOccupied(int loc, int slot) => Data[DaycareOffset + 0 + ((SIZE_STORED + 8) * slot)] == 1;
        public override uint? GetDaycareEXP(int loc, int slot) => BitConverter.ToUInt32(Data, DaycareOffset + 4 + ((SIZE_STORED + 8) * slot));

        public override int GetDaycareSlotOffset(int loc, int slot) => DaycareOffset + 8 + (slot * (SIZE_STORED + 8));
        public override bool? IsDaycareHasEgg(int loc) => Data[DaycareOffset + 0x1E0] == 1;
        public override void SetDaycareHasEgg(int loc, bool hasEgg) => Data[DaycareOffset + 0x1E0] = hasEgg ? 1 : 0;
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) => Data[DaycareOffset + ((SIZE_STORED + 8) * slot)] = occupied ? 1 : 0;
        public override void SetDaycareEXP(int loc, int slot, uint EXP) => BitConverter.GetBytes(EXP).CopyTo(Data, DaycareOffset + 4 + ((SIZE_STORED + 8) * slot));

        public override string GetDaycareRNGSeed(int loc)
        {
            int ofs = DaycareOffset;
            var data = Data.Skip(ofs + 0x1E8).Take(DaycareSeedSize / 2).Reverse().ToArray();
            return BitConverter.ToString(data).Replace("-", string.Empty);
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

        public override string JPEGTitle => HasJPPEGData ? string.Empty : StringConverter.GetString6(Data, JPEG, 0x1A);
        public override byte[] GetJPEGData() => HasJPPEGData ? Array.Empty<byte>() : GetData(JPEG + 0x54, 0xE004);
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

        public override GameVersion Version => Game switch
        {
            (int) GameVersion.X => GameVersion.X,
            (int) GameVersion.Y => GameVersion.Y,
            _ => GameVersion.Invalid
        };

        protected override bool[] MysteryGiftReceivedFlags { get => Blocks.MysteryGift.GetReceivedFlags(); set => Blocks.MysteryGift.SetReceivedFlags(value); }
        protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.MysteryGift.GetGifts(); set => Blocks.MysteryGift.SetGifts(value); }

        public override bool GetCaught(int species) => Blocks.Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Blocks.Zukan.GetSeen(species);
        public override void SetSeen(int species, bool seen) => Blocks.Zukan.SetSeen(species, seen);
        public override void SetCaught(int species, bool caught) => Blocks.Zukan.SetCaught(species, caught);

        public override int CurrentBox { get => Blocks.BoxLayout.CurrentBox; set => Blocks.BoxLayout.CurrentBox = value; }
        protected override int GetBoxWallpaperOffset(int box) => Blocks.BoxLayout.GetBoxWallpaperOffset(box);
        public override int BoxesUnlocked { get => Blocks.BoxLayout.BoxesUnlocked; set => Blocks.BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => Blocks.BoxLayout.BoxFlags; set => Blocks.BoxLayout.BoxFlags = value; }

        public bool BattleBoxLocked
        {
            get => Blocks.BattleBox.Locked;
            set => Blocks.BattleBox.Locked = value;
        }

        public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }
        public override int Vivillon { get => Blocks.Misc.Vivillon; set => Blocks.Misc.Vivillon = value; }
        public override int Badges { get => Blocks.Misc.Badges; set => Blocks.Misc.Badges = value; }
        public override int BP { get => Blocks.Misc.BP; set => Blocks.Misc.BP = value; }
    }
}
