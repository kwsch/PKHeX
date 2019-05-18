namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 <see cref="SaveFile"/> object for <see cref="GameVersion.ORASDEMO"/>.
    /// </summary>
    /// <inheritdoc cref="SAV6" />
    public sealed class SAV6AODemo : SAV6
    {
        public SAV6AODemo(byte[] data) : base(data) => Initialize();
        public SAV6AODemo() : base(SaveUtil.SIZE_G6ORASDEMO) => Initialize();
        public override SaveFile Clone() => new SAV6AODemo((byte[])Data.Clone());

        private void Initialize()
        {
            /* 00: */ Bag              = 0x00000; // MyItem
            /* 01: */ // ItemInfo      = 0x00C00; // ItemInfo6
            /* 02: */ // GameTime      = 0x00E00; // GameTime
            /* 03: */ // Trainer1      = 0x01000; // Situation
            /* 04: */ //               = 0x01200; // [00004] RandomGroup (rand seeds)
            /* 05: */ PlayTime         = 0x01400; // PlayTime
            /* 06: */ //               = 0x01600; // [00024] temp variables (u32 id + 32 u8)
            /* 07: */ //               = 0x01800; // [02100] FieldMoveModelSave
            /* 08: */ Trainer2         = 0x03A00; // Misc
            /* 09: */ //               = 0x03C00; // MyStatus
            /* 10: */ Party            = 0x03E00; // PokePartySave
            /* 11: */ EventConst       = 0x04600; // EventWork
            /* 12: */ //               = 0x04C00; // [00004] Packed Menu Bits
            /* 13: */ //               = 0x04E00; // [00048] Repel Info, (Swarm?) and other overworld info (roamer)
            /* 14: */ SUBE             = 0x05000; // PokeDiarySave
            /* 15: */ // Record        = 0x05400; // Record

            Items = new MyItem6AO(     this, 0x00000);
            ItemInfo = new ItemInfo6(  this, 0x00C00);
            GameTime = new GameTime6(  this, 0x00E00);
            Situation = new Situation6(this, 0x01000);
            Played = new PlayTime6(    this, 0x01400);
            Status = new MyStatus6(    this, 0x03C00);
            Records = new Record6(this, 0x05400, Core.Records.MaxType_AO);
            EventFlag = EventConst + 0x2FC;

            HeldItems = Legal.HeldItem_AO;
            Personal = PersonalTable.AO;
        }

        public override int MaxMoveID => Legal.MaxMoveID_6_AO;
        public override int MaxItemID => Legal.MaxItemID_6_AO;
        public override int MaxAbilityID => Legal.MaxAbilityID_6_AO;

        public override GameVersion Version
        {
            get
            {
                switch (Game)
                {
                    case (int)GameVersion.AS: return GameVersion.AS;
                    case (int)GameVersion.OR: return GameVersion.OR;
                }
                return GameVersion.Invalid;
            }
        }
    }
}