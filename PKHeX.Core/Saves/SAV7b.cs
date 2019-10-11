using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 <see cref="SaveFile"/> object for <see cref="GameVersion.GG"/> games.
    /// </summary>
    public sealed class SAV7b : SAV_BEEF
    {
        protected override string BAKText => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Filter => "savedata|*.bin";
        public override string Extension => ".bin";
        public override string[] PKMExtensions => PKM.Extensions.Where(f => f[1] == 'b' && f[f.Length - 1] == '7').ToArray();

        public override Type PKMType => typeof(PB7);
        public override PKM BlankPKM => new PB7();
        public override int SIZE_STORED => SIZE_PARTY;
        protected override int SIZE_PARTY => 260;

        public override SaveFile Clone() => new SAV7b((byte[])Data.Clone());

        public SAV7b() : base(SaveUtil.SIZE_G7GG, BlockInfoGG, 0xB8800)
        {
            Initialize();
            ClearBoxes();
        }

        public SAV7b(byte[] data) : base(data, BlockInfoGG, 0xB8800)
        {
            Initialize();
        }

        private void Initialize()
        {
            Personal = PersonalTable.GG;

            Box = GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            Party = GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            EventFlag = GetBlockOffset(BelugaBlockIndex.EventWork);
            PokeDex = GetBlockOffset(BelugaBlockIndex.Zukan);
            Zukan = new Zukan7b(this, PokeDex, 0x550);
            Config = new ConfigSave7b(this);
            Items = new MyItem7b(this);
            Storage = new PokeListHeader(this);
            Status = new MyStatus7b(this);
            Played = new PlayTime7b(this);
            Misc = new Misc7b(this);
            EventWork = new EventWork7b(this);
            GiftRecords = new WB7Records(this);

            WondercardData = GiftRecords.Offset;

            HeldItems = Legal.HeldItems_GG;
        }

        // Save Block accessors
        public MyItem Items { get; private set; }
        public Misc7b Misc { get; private set; }
        public Zukan7b Zukan { get; private set; }
        public MyStatus7b Status { get; private set; }
        public PlayTime7b Played { get; private set; }
        public ConfigSave7b Config { get; private set; }
        public EventWork7b EventWork { get; private set; }
        public PokeListHeader Storage { get; private set; }
        public WB7Records GiftRecords { get; private set; }

        public override InventoryPouch[] Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Feature Overrides
        public override int Generation => 7;
        public override int MaxMoveID => Legal.MaxMoveID_7b;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_7b;
        public override int MaxItemID => Legal.MaxItemID_7b;
        public override int MaxBallID => Legal.MaxBallID_7b;
        public override int MaxGameID => Legal.MaxGameID_7b;
        public override int MaxAbilityID => Legal.MaxAbilityID_7b;

        public override int MaxIV => 31;
        public override int MaxEV => 252;
        public override int OTLength => 12;
        public override int NickLength => 12;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventFlagMax => 4160; // 0xDC0 (true max may be up to 0x7F less. 23A8 starts u64 hashvals)
        protected override int EventConstMax => 1000;

        public override bool HasParty => false; // handled via team slots
        public override bool HasEvents => true; // advanced!

        // BoxSlotCount => 1000 -- why couldn't this be a multiple of 30...
        public override int BoxSlotCount => 25;
        public override int BoxCount => 40; // 1000/25

        public BlockInfo GetBlock(BelugaBlockIndex index) => Blocks[(int)index];
        public int GetBlockOffset(BelugaBlockIndex index) => GetBlock(index).Offset;

        private const int boGG = 0xB8800 - 0x200; // nowhere near 1MB (savedata.bin size)

        private static readonly BlockInfo[] BlockInfoGG =
        {
            new BlockInfo7b(boGG, 00, 0x00000, 0x00D90),
            new BlockInfo7b(boGG, 01, 0x00E00, 0x00200),
            new BlockInfo7b(boGG, 02, 0x01000, 0x00168),
            new BlockInfo7b(boGG, 03, 0x01200, 0x01800),
            new BlockInfo7b(boGG, 04, 0x02A00, 0x020E8),
            new BlockInfo7b(boGG, 05, 0x04C00, 0x00930),
            new BlockInfo7b(boGG, 06, 0x05600, 0x00004),
            new BlockInfo7b(boGG, 07, 0x05800, 0x00130),
            new BlockInfo7b(boGG, 08, 0x05A00, 0x00012),
            new BlockInfo7b(boGG, 09, 0x05C00, 0x3F7A0),
            new BlockInfo7b(boGG, 10, 0x45400, 0x00008),
            new BlockInfo7b(boGG, 11, 0x45600, 0x00E90),
            new BlockInfo7b(boGG, 12, 0x46600, 0x010A4),
            new BlockInfo7b(boGG, 13, 0x47800, 0x000F0),
            new BlockInfo7b(boGG, 14, 0x47A00, 0x06010),
            new BlockInfo7b(boGG, 15, 0x4DC00, 0x00200),
            new BlockInfo7b(boGG, 16, 0x4DE00, 0x00098),
            new BlockInfo7b(boGG, 17, 0x4E000, 0x00068),
            new BlockInfo7b(boGG, 18, 0x4E200, 0x69780),
            new BlockInfo7b(boGG, 19, 0xB7A00, 0x000B0),
            new BlockInfo7b(boGG, 20, 0xB7C00, 0x00940),
        };

        public bool FixPreWrite() => Storage.CompressStorage();

        protected override void SetPKM(PKM pkm)
        {
            var pk = (PB7)pkm;
            // Apply to this Save File
            int CT = pk.CurrentHandler;
            var Date = DateTime.Now;
            pk.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk.Moves.Contains(216)) // Return
                    pk.CurrentFriendship = pk.OppositeFriendship;
                else if (pk.Moves.Contains(218)) // Frustration
                    pk.CurrentFriendship = pk.OppositeFriendship;
            }
            pk.RefreshChecksum();
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        protected override PKM GetPKM(byte[] data) => new PB7(data);
        protected override byte[] DecryptPKM(byte[] data) => PKX.DecryptArray6(data);
        public override int GetBoxOffset(int box) => Box + (box * BoxSlotCount * SIZE_STORED);
        protected override IList<int>[] SlotPointers => new[] { Storage.PokeListInfo };

        public override int GetPartyOffset(int slot) => Storage.GetPartyOffset(slot);
        public override int PartyCount { get => Storage.PartyCount; protected set => Storage.PartyCount = value; }
        protected override void SetPartyValues(PKM pkm, bool isParty) => base.SetPartyValues(pkm, true);

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            var val = StorageSlotFlag.None;
            if (Storage.PokeListInfo[6] == index)
                val |= StorageSlotFlag.Starter;
            int position = Array.IndexOf(Storage.PokeListInfo, index);
            if ((uint) position < 6)
                val |= (StorageSlotFlag)((int)StorageSlotFlag.Party1 << position);
            return val;
        }

        public override string GetBoxName(int box) => $"Box {box + 1}";
        public override void SetBoxName(int box, string value) { }

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7b(value, maxLength, Language, PadToSize, PadWith);
        }

        public override GameVersion Version
        {
            get
            {
                return Game switch
                {
                    (int)GameVersion.GP => GameVersion.GP,
                    (int)GameVersion.GE => GameVersion.GE,
                    _ => GameVersion.Invalid
                };
            }
        }

        // Player Information
        public override int TID { get => Status.TID; set => Status.TID = value; }
        public override int SID { get => Status.SID; set => Status.SID = value; }
        public override int Game { get => Status.Game; set => Status.Game = value; }
        public override int Gender { get => Status.Gender; set => Status.Gender = value; }
        public override int Language { get => Status.Language; set => Config.Language = Status.Language = value; } // stored in multiple places
        public override string OT { get => Status.OT; set => Status.OT = value; }
        public override uint Money { get => Misc.Money; set => Misc.Money = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }

        /// <summary>
        /// Gets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <returns>Flag is Set (true) or not Set (false)</returns>
        public override bool GetEventFlag(int flagNumber) => EventWork.GetFlag(flagNumber);

        /// <summary>
        /// Sets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <param name="value">Event Flag status to set</param>
        /// <remarks>Flag is Set (true) or not Set (false)</remarks>
        public override void SetEventFlag(int flagNumber, bool value) => EventWork.SetFlag(flagNumber, value);

        protected override bool[] MysteryGiftReceivedFlags { get => GiftRecords.Flags; set => GiftRecords.Flags = value; }
        protected override DataMysteryGift[] MysteryGiftCards { get => GiftRecords.Records; set => GiftRecords.Records = (WR7[])value; }

        public override int GameSyncIDSize => MyStatus7b.GameSyncIDSize; // 64 bits
        public override string GameSyncID { get => Status.GameSyncID; set => Status.GameSyncID = value; }
    }
}
