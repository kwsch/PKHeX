using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class SAV7SM : SAV7, ISaveBlock7SM
    {
        public SAV7SM(byte[] data) : base(data, SaveBlockAccessor7SM.BlockMetadataOffset)
        {
            Blocks = new SaveBlockAccessor7SM(this);
            Initialize();
            ClearMemeCrypto();
        }

        public SAV7SM() : base(SaveUtil.SIZE_G7SM, SaveBlockAccessor7SM.BlockMetadataOffset)
        {
            Blocks = new SaveBlockAccessor7SM(this);
            Initialize();
            ClearBoxes();
        }

        private void Initialize()
        {
            Party = Blocks.BlockInfo[04].Offset;
            EventConst = Blocks.BlockInfo[05].Offset;
            PokeDex = Blocks.BlockInfo[06].Offset;
            EventFlag = EventConst + (EventConstMax * 2); // After Event Const (u16)*n
            HoF = EventFlag + (EventFlagMax / 8); // After Event Flags (1b)*(1u8/8b)*n

            TeamSlots = Blocks.BoxLayout.TeamSlots;
            Box = Blocks.BlockInfo[14].Offset;
            WondercardData = Blocks.MysteryGift.Offset;
            DaycareOffset = Blocks.Daycare.Offset;

            ReloadBattleTeams();
        }

        public override PersonalTable Personal => PersonalTable.SM;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SM;
        protected override SaveFile CloneInternal() => new SAV7SM((byte[])Data.Clone());

        #region Blocks
        public SaveBlockAccessor7SM Blocks { get; }
        public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
        public override MyItem Items => Blocks.Items;
        public override MysteryBlock7 MysteryGift => Blocks.MysteryGift;
        public override PokeFinder7 PokeFinder => Blocks.PokeFinder;
        public override JoinFesta7 Festa => Blocks.Festa;
        public override Daycare7 Daycare => Blocks.Daycare;
        public override RecordBlock6 Records => Blocks.Records;
        public override PlayTime6 Played => Blocks.Played;
        public override MyStatus7 MyStatus => Blocks.MyStatus;
        public override FieldMoveModelSave7 Overworld => Blocks.Overworld;
        public override Situation7 Situation => Blocks.Situation;
        public override ConfigSave7 Config => Blocks.Config;
        public override GameTime7 GameTime => Blocks.GameTime;
        public override Misc7 Misc => Blocks.Misc;
        public override Zukan7 Zukan => Blocks.Zukan;
        public override BoxLayout7 BoxLayout => Blocks.BoxLayout;
        public override BattleTree7 BattleTree => Blocks.BattleTree;
        public override ResortSave7 ResortSave => Blocks.ResortSave;
        public override FieldMenu7 FieldMenu => Blocks.FieldMenu;
        public override FashionBlock7 Fashion => Blocks.Fashion;
        #endregion

        protected override int EventFlagMax => 3968;
        public override int MaxMoveID => Legal.MaxMoveID_7;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7;
        public override int MaxItemID => Legal.MaxItemID_7;
        public override int MaxAbilityID => Legal.MaxAbilityID_7;

        private const ulong MagearnaConst = 0xCBE05F18356504AC;

        public void UpdateMagearnaConstant()
        {
            var flag = GetEventFlag(3100);
            ulong value = flag ? MagearnaConst : 0ul;
            SetData(BitConverter.GetBytes(value), Blocks.BlockInfo[35].Offset + 0x168);
        }
    }
}