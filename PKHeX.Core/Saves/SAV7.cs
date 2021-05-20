using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 <see cref="SaveFile"/> object.
    /// </summary>
    public abstract class SAV7 : SAV_BEEF, ITrainerStatRecord, ISaveBlock7Main, IRegionOrigin, IGameSync
    {
        // Save Data Attributes
        protected internal override string ShortSummary => $"{OT} ({Version}) - {Played.LastSavedTime}";
        public override string Extension => string.Empty;

        public override IReadOnlyList<string> PKMExtensions => PKM.Extensions.Where(f =>
        {
            int gen = f.Last() - 0x30;
            return gen <= 7 && f[1] != 'b'; // ignore PB7
        }).ToArray();

        protected SAV7(byte[] data, int biOffset) : base(data, biOffset)
        {
        }

        protected SAV7(int size, int biOffset) : base(size, biOffset)
        {
        }

        protected void ReloadBattleTeams()
        {
            var demo = this is SAV7SM && new ReadOnlySpan<byte>(Data, BoxLayout.Offset, 0x4C4).IsRangeEmpty(); // up to Battle Box values
            if (demo || !State.Exportable)
            {
                BoxLayout.ClearBattleTeams();
            }
            else // Valid slot locking info present
            {
                BoxLayout.LoadBattleTeams();
            }
        }

        #region Blocks
        public abstract MyItem Items { get; }
        public abstract MysteryBlock7 MysteryGift { get; }
        public abstract PokeFinder7 PokeFinder { get; }
        public abstract JoinFesta7 Festa { get; }
        public abstract Daycare7 Daycare { get; }
        public abstract RecordBlock6 Records { get; }
        public abstract PlayTime6 Played { get; }
        public abstract MyStatus7 MyStatus { get; }
        public abstract FieldMoveModelSave7 Overworld { get; }
        public abstract Situation7 Situation { get; }
        public abstract ConfigSave7 Config { get; }
        public abstract GameTime7 GameTime { get; }
        public abstract Misc7 Misc { get; }
        public abstract Zukan7 Zukan { get; }
        public abstract BoxLayout7 BoxLayout { get; }
        public abstract BattleTree7 BattleTree { get; }
        public abstract ResortSave7 ResortSave { get; }
        public abstract FieldMenu7 FieldMenu { get; }
        public abstract FashionBlock7 Fashion { get; }
        public abstract HallOfFame7 Fame { get; }
        #endregion

        // Configuration
        protected override int SIZE_STORED => PokeCrypto.SIZE_6STORED;
        protected override int SIZE_PARTY => PokeCrypto.SIZE_6PARTY;
        public override PKM BlankPKM => new PK7();
        public override Type PKMType => typeof(PK7);

        public override int BoxCount => 32;
        public override int MaxEV => 252;
        public override int Generation => 7;
        protected override int GiftCountMax => 48;
        protected override int GiftFlagMax => 0x100 * 8;
        protected override int EventConstMax => 1000;
        public override int OTLength => 12;
        public override int NickLength => 12;

        public override int MaxBallID => Legal.MaxBallID_7; // 26
        public override int MaxGameID => Legal.MaxGameID_7;
        protected override PKM GetPKM(byte[] data) => new PK7(data);
        protected override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray6(data);

        // Feature Overrides

        // Blocks & Offsets
        private const int MemeCryptoBlock = 36;

        protected void ClearMemeCrypto()
        {
            new byte[0x80].CopyTo(Data, AllBlocks[MemeCryptoBlock].Offset + 0x100);
        }

        protected override byte[] GetFinalData()
        {
            BoxLayout.SaveBattleTeams();
            SetChecksums();
            var result = MemeCrypto.Resign7(Data);
            Debug.Assert(result != Data);
            return result;
        }

        public override GameVersion Version => Game switch
        {
            (int)GameVersion.SN => GameVersion.SN,
            (int)GameVersion.MN => GameVersion.MN,
            (int)GameVersion.US => GameVersion.US,
            (int)GameVersion.UM => GameVersion.UM,
            _ => GameVersion.Invalid
        };

        public override string GetString(byte[] data, int offset, int length) => StringConverter.GetString7(data, offset, length);

        public override byte[] SetString(string value, int maxLength, int PadToSize = 0, ushort PadWith = 0)
        {
            if (PadToSize == 0)
                PadToSize = maxLength + 1;
            return StringConverter.SetString7(value, maxLength, Language, PadToSize, PadWith);
        }

        // Player Information
        public override int TID { get => MyStatus.TID; set => MyStatus.TID = value; }
        public override int SID { get => MyStatus.SID; set => MyStatus.SID = value; }
        public override int Game { get => MyStatus.Game; set => MyStatus.Game = value; }
        public override int Gender { get => MyStatus.Gender; set => MyStatus.Gender = value; }
        public int GameSyncIDSize => MyStatus7.GameSyncIDSize; // 64 bits
        public string GameSyncID { get => MyStatus.GameSyncID; set => MyStatus.GameSyncID = value; }
        public int Region { get => MyStatus.SubRegion; set => MyStatus.SubRegion = value; }
        public int Country { get => MyStatus.Country; set => MyStatus.Country = value; }
        public int ConsoleRegion { get => MyStatus.ConsoleRegion; set => MyStatus.ConsoleRegion = value; }
        public override int Language { get => MyStatus.Language; set => MyStatus.Language = value; }
        public override string OT { get => MyStatus.OT; set => MyStatus.OT = value; }
        public override int MultiplayerSpriteID { get => MyStatus.MultiplayerSpriteID; set => MyStatus.MultiplayerSpriteID = value; }
        public override uint Money { get => Misc.Money; set => Misc.Money = value; }

        public override int PlayedHours { get => Played.PlayedHours; set => Played.PlayedHours = value; }
        public override int PlayedMinutes { get => Played.PlayedMinutes; set => Played.PlayedMinutes = value; }
        public override int PlayedSeconds { get => Played.PlayedSeconds; set => Played.PlayedSeconds = value; }
        public override uint SecondsToStart { get => GameTime.SecondsToStart; set => GameTime.SecondsToStart = value; }
        public override uint SecondsToFame { get => GameTime.SecondsToFame; set => GameTime.SecondsToFame = value; }

        // Stat Records
        public int RecordCount => 200;
        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);

        // Inventory
        public override IReadOnlyList<InventoryPouch> Inventory { get => Items.Inventory; set => Items.Inventory = value; }

        // Storage
        public override int GetPartyOffset(int slot) => Party + (SIZE_PARTY * slot);
        public override int GetBoxOffset(int box) => Box + (SIZE_STORED * box * 30);
        protected override int GetBoxWallpaperOffset(int box) => BoxLayout.GetBoxWallpaperOffset(box);
        public override int GetBoxWallpaper(int box) => BoxLayout.GetBoxWallpaper(box);
        public override void SetBoxWallpaper(int box, int value) => BoxLayout.SetBoxWallpaper(box, value);
        public override string GetBoxName(int box) => BoxLayout[box];
        public override void SetBoxName(int box, string value) => BoxLayout[box] = value;
        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
        public override int BoxesUnlocked { get => BoxLayout.BoxesUnlocked; set => BoxLayout.BoxesUnlocked = value; }
        public override byte[] BoxFlags { get => BoxLayout.BoxFlags; set => BoxLayout.BoxFlags = value; }

        protected override void SetPKM(PKM pkm, bool isParty = false)
        {
            PK7 pk7 = (PK7)pkm;
            // Apply to this Save File
            int CT = pk7.CurrentHandler;
            DateTime Date = DateTime.Now;
            pk7.Trade(this, Date.Day, Date.Month, Date.Year);
            if (CT != pk7.CurrentHandler) // Logic updated Friendship
            {
                // Copy over the Friendship Value only under certain circumstances
                if (pk7.HasMove(216)) // Return
                    pk7.CurrentFriendship = pk7.OppositeFriendship;
                else if (pk7.HasMove(218)) // Frustration
                    pkm.CurrentFriendship = pk7.OppositeFriendship;
            }

            pk7.FormArgumentElapsed = pk7.FormArgumentMaximum = 0;
            pk7.FormArgumentRemain = (byte)GetFormArgument(pkm);

            pkm.RefreshChecksum();
            AddCountAcquired(pkm);
        }

        private void AddCountAcquired(PKM pkm)
        {
            Records.AddRecord(pkm.WasEgg ? 008 : 006); // egg, capture
            if (pkm.CurrentHandler == 1)
                Records.AddRecord(011); // trade
            if (!pkm.WasEgg)
            {
                Records.AddRecord(004); // wild encounters
                Records.AddRecord(042); // balls used
            }
        }

        private static uint GetFormArgument(PKM pkm)
        {
            if (pkm.Form == 0)
                return 0;
            // Gen7 allows forms to be stored in the box with the current duration & form
            // Just cap out the form duration anyways
            return pkm.Species switch
            {
                (int)Species.Furfrou => 5u, // Furfrou
                (int)Species.Hoopa => 3u, // Hoopa
                _ => 0u
            };
        }

        protected override void SetDex(PKM pkm) => Zukan.SetDex(pkm);
        public override bool GetCaught(int species) => Zukan.GetCaught(species);
        public override bool GetSeen(int species) => Zukan.GetSeen(species);

        public override int PartyCount
        {
            get => Data[Party + (6 * SIZE_PARTY)];
            protected set => Data[Party + (6 * SIZE_PARTY)] = (byte)value;
        }

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            int team = Array.IndexOf(TeamSlots, index);
            if (team < 0)
                return StorageSlotFlag.None;

            team /= 6;
            var val = (StorageSlotFlag)((int)StorageSlotFlag.BattleTeam1 << team);
            if (BoxLayout.GetIsTeamLocked(team))
                val |= StorageSlotFlag.Locked;
            return val;
        }

        private int FusedCount => this is SAV7USUM ? 3 : 1;

        public int GetFusedSlotOffset(int slot)
        {
            if ((uint)slot >= FusedCount)
                return -1;
            return AllBlocks[08].Offset + (PokeCrypto.SIZE_6PARTY * slot); // 0x104*slot
        }

        public override int DaycareSeedSize => Daycare7.DaycareSeedSize; // 128 bits
        public override int GetDaycareSlotOffset(int loc, int slot) => Daycare.GetDaycareSlotOffset(slot);
        public override bool? IsDaycareOccupied(int loc, int slot) => Daycare.GetIsOccupied(slot);
        public override string GetDaycareRNGSeed(int loc) => Daycare.RNGSeed;
        public override bool? IsDaycareHasEgg(int loc) => Daycare.HasEgg;
        public override void SetDaycareOccupied(int loc, int slot, bool occupied) => Daycare.SetOccupied(slot, occupied);
        public override void SetDaycareRNGSeed(int loc, string seed) => Daycare.RNGSeed = seed;
        public override void SetDaycareHasEgg(int loc, bool hasEgg) => Daycare.HasEgg = hasEgg;

        protected override bool[] MysteryGiftReceivedFlags { get => MysteryGift.MysteryGiftReceivedFlags; set => MysteryGift.MysteryGiftReceivedFlags = value; }
        protected override DataMysteryGift[] MysteryGiftCards { get => MysteryGift.MysteryGiftCards; set => MysteryGift.MysteryGiftCards = value; }
    }
}
