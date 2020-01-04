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
        protected override string BAKText => $"{OT} ({Version}) - {Blocks.Played.LastSavedTime}";
        public override string Filter => "savedata|*.bin";
        public override string Extension => ".bin";
        public override string[] PKMExtensions => PKM.Extensions.Where(f => f[1] == 'b' && f[f.Length - 1] == '7').ToArray();

        public override Type PKMType => typeof(PB7);
        public override PKM BlankPKM => new PB7();
        public override int SIZE_STORED => SIZE_PARTY;
        protected override int SIZE_PARTY => 260;
        public override byte[] GetDataForBox(PKM pkm) => pkm.EncryptedPartyData;

        public override PersonalTable Personal => PersonalTable.GG;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_GG;

        public override SaveFile Clone() => new SAV7b((byte[])Data.Clone());

        public SaveBlockAccessor7b Blocks { get; }
        public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;

        public SAV7b() : base(SaveUtil.SIZE_G7GG, 0xB8800)
        {
            Blocks = new SaveBlockAccessor7b(this);
            Initialize();
            ClearBoxes();
        }

        public SAV7b(byte[] data) : base(data, 0xB8800)
        {
            Blocks = new SaveBlockAccessor7b(this);
            Initialize();
        }

        private void Initialize()
        {
            Box = Blocks.GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            Party = Blocks.GetBlockOffset(BelugaBlockIndex.PokeListPokemon);
            EventFlag = Blocks.GetBlockOffset(BelugaBlockIndex.EventWork);
            PokeDex = Blocks.GetBlockOffset(BelugaBlockIndex.Zukan);

            WondercardData = Blocks.GiftRecords.Offset;
        }

        // Save Block accessors

        public override InventoryPouch[] Inventory { get => Blocks.Items.Inventory; set => Blocks.Items.Inventory = value; }

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

        public bool FixPreWrite() => Blocks.Storage.CompressStorage();

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

        protected override void SetDex(PKM pkm) => Blocks.Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Blocks.Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Blocks.Zukan.GetSeen(species);

        protected override PKM GetPKM(byte[] data) => new PB7(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);
        public override int GetBoxOffset(int box) => Box + (box * BoxSlotCount * SIZE_STORED);
        protected override IList<int>[] SlotPointers => new[] { Blocks.Storage.PokeListInfo };

        public override int GetPartyOffset(int slot) => Blocks.Storage.GetPartyOffset(slot);
        public override int PartyCount { get => Blocks.Storage.PartyCount; protected set => Blocks.Storage.PartyCount = value; }
        protected override void SetPartyValues(PKM pkm, bool isParty) => base.SetPartyValues(pkm, true);

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            var val = StorageSlotFlag.None;
            if (Blocks.Storage.PokeListInfo[6] == index)
                val |= StorageSlotFlag.Starter;
            int position = Array.IndexOf(Blocks.Storage.PokeListInfo, index);
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
        public override int TID { get => Blocks.Status.TID; set => Blocks.Status.TID = value; }
        public override int SID { get => Blocks.Status.SID; set => Blocks.Status.SID = value; }
        public override int Game { get => Blocks.Status.Game; set => Blocks.Status.Game = value; }
        public override int Gender { get => Blocks.Status.Gender; set => Blocks.Status.Gender = value; }
        public override int Language { get => Blocks.Status.Language; set => Blocks.Config.Language = Blocks.Status.Language = value; } // stored in multiple places
        public override string OT { get => Blocks.Status.OT; set => Blocks.Status.OT = value; }
        public override uint Money { get => Blocks.Misc.Money; set => Blocks.Misc.Money = value; }

        public override int PlayedHours { get => Blocks.Played.PlayedHours; set => Blocks.Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Blocks.Played.PlayedMinutes; set => Blocks.Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Blocks.Played.PlayedSeconds; set => Blocks.Played.PlayedSeconds = value; }

        /// <summary>
        /// Gets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <returns>Flag is Set (true) or not Set (false)</returns>
        public override bool GetEventFlag(int flagNumber) => Blocks.EventWork.GetFlag(flagNumber);

        /// <summary>
        /// Sets the <see cref="bool"/> status of a desired Event Flag
        /// </summary>
        /// <param name="flagNumber">Event Flag to check</param>
        /// <param name="value">Event Flag status to set</param>
        /// <remarks>Flag is Set (true) or not Set (false)</remarks>
        public override void SetEventFlag(int flagNumber, bool value) => Blocks.EventWork.SetFlag(flagNumber, value);

        protected override bool[] MysteryGiftReceivedFlags { get => Blocks.GiftRecords.Flags; set => Blocks.GiftRecords.Flags = value; }
        protected override DataMysteryGift[] MysteryGiftCards { get => Blocks.GiftRecords.Records; set => Blocks.GiftRecords.Records = (WR7[])value; }

        public override int GameSyncIDSize => MyStatus7b.GameSyncIDSize; // 64 bits
        public override string GameSyncID { get => Blocks.Status.GameSyncID; set => Blocks.Status.GameSyncID = value; }
    }
}
