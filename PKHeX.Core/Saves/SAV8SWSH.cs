using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public sealed class SAV8SWSH : SAV8, ISaveBlock8SWSH, ITrainerStatRecord, ISaveFileRevision, ISCBlockArray
    {
        public SAV8SWSH(byte[] data) : base(data)
        {
            Data = Array.Empty<byte>();
            AllBlocks = SwishCrypto.Decrypt(data);
            Blocks = new SaveBlockAccessor8SWSH(this);
            SaveRevision = Zukan.GetRevision();
            Initialize();
        }

        private SAV8SWSH(IReadOnlyList<SCBlock> blocks) : base(Array.Empty<byte>())
        {
            Data = Array.Empty<byte>();
            AllBlocks = blocks;
            Blocks = new SaveBlockAccessor8SWSH(this);
            SaveRevision = Zukan.GetRevision();
            Initialize();
        }

        public SAV8SWSH()
        {
            AllBlocks = Meta8.GetBlankDataSWSH();
            Blocks = new SaveBlockAccessor8SWSH(this);
            SaveRevision = Zukan.GetRevision();
            Initialize();
            ClearBoxes();
        }

        public override void CopyChangesFrom(SaveFile sav)
        {
            // Absorb changes from all blocks
            var z = (SAV8SWSH)sav;
            var mine = AllBlocks;
            var newB = z.AllBlocks;
            for (int i = 0; i < mine.Count; i++)
                mine[i].CopyFrom(newB[i]);
            State.Edited = true;
        }

        public int SaveRevision { get; }

        public string SaveRevisionString => SaveRevision switch
        {
            0 => "-Base", // Vanilla
            1 => "-IoA", // DLC 1: Isle of Armor
            2 => "-CT", // DLC 2: Crown Tundra
            _ => throw new ArgumentOutOfRangeException(nameof(SaveRevision)),
        };

        public IReadOnlyList<SCBlock> AllBlocks { get; }
        public override bool ChecksumsValid => true;
        public override string ChecksumInfo => string.Empty;
        protected override void SetChecksums() { } // None!
        protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

        public override PersonalTable Personal => PersonalTable.SWSH;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SWSH;

        #region Blocks
        public SCBlockAccessor Accessor => Blocks;
        public SaveBlockAccessor8SWSH Blocks { get; }
        public override Box8 BoxInfo => Blocks.BoxInfo;
        public override Party8 PartyInfo => Blocks.PartyInfo;
        public override MyItem Items => Blocks.Items;
        public override MyStatus8 MyStatus => Blocks.MyStatus;
        public override Misc8 Misc => Blocks.Misc;
        public override Zukan8 Zukan => Blocks.Zukan;
        public override BoxLayout8 BoxLayout => Blocks.BoxLayout;
        public override PlayTime8 Played => Blocks.Played;
        public override Fused8 Fused => Blocks.Fused;
        public override Daycare8 Daycare => Blocks.Daycare;
        public override Record8 Records => Blocks.Records;
        public override TrainerCard8 TrainerCard => Blocks.TrainerCard;
        public override RaidSpawnList8 Raid => Blocks.Raid;
        public override RaidSpawnList8 RaidArmor => Blocks.RaidArmor;
        public override RaidSpawnList8 RaidCrown => Blocks.RaidCrown;
        public override TitleScreen8 TitleScreen => Blocks.TitleScreen;
        public override TeamIndexes8 TeamIndexes => Blocks.TeamIndexes;

        public T GetValue<T>(uint key) where T : struct
        {
            if (!State.Exportable)
                return default;
            var value = Blocks.GetBlockValue(key);
            if (value is T v)
                return v;
            throw new ArgumentException($"Incorrect type request! Expected {typeof(T).Name}, received {value.GetType().Name}", nameof(T));
        }

        public void SetValue<T>(uint key, T value) where T : struct
        {
            if (!State.Exportable)
                return;
            Blocks.SetBlockValue(key, value);
        }

        #endregion
        protected override SaveFile CloneInternal()
        {
            var blockCopy = new SCBlock[AllBlocks.Count];
            for (int i = 0; i < AllBlocks.Count; i++)
                blockCopy[i] = AllBlocks[i].Clone();
            return new SAV8SWSH(blockCopy);
        }

        private int m_spec, m_item, m_move, m_abil;
        public override int MaxMoveID => m_move;
        public override int MaxSpeciesID => m_spec;
        public override int MaxItemID => m_item;
        public override int MaxBallID => Legal.MaxBallID_8;
        public override int MaxGameID => Legal.MaxGameID_8;
        public override int MaxAbilityID => m_abil;

        private void Initialize()
        {
            Box = 0;
            Party = 0;
            PokeDex = 0;
            TeamIndexes.LoadBattleTeams();

            int rev = SaveRevision;
            if (rev == 0)
            {
                m_move = Legal.MaxMoveID_8_O0;
                m_spec = Legal.MaxSpeciesID_8_O0;
                m_item = Legal.MaxItemID_8_O0;
                m_abil = Legal.MaxAbilityID_8_O0;
            }
            else if (rev == 1)
            {
                m_move = Legal.MaxMoveID_8_R1;
                m_spec = Legal.MaxSpeciesID_8_R1;
                m_item = Legal.MaxItemID_8_R1;
                m_abil = Legal.MaxAbilityID_8_R1;
            }
            else
            {
                m_move = Legal.MaxMoveID_8_R2;
                m_spec = Legal.MaxSpeciesID_8_R2;
                m_item = Legal.MaxItemID_8_R2;
                m_abil = Legal.MaxAbilityID_8_R2;
            }
        }

        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
        public int RecordCount => Record8.RecordCount;

        public override StorageSlotFlag GetSlotFlags(int index)
        {
            int team = Array.IndexOf(TeamIndexes.TeamSlots, index);
            if (team < 0)
                return StorageSlotFlag.None;

            team /= 6;
            var result = (StorageSlotFlag)((int)StorageSlotFlag.BattleTeam1 << team);
            if (TeamIndexes.GetIsTeamLocked(team))
                result |= StorageSlotFlag.Locked;
            return result;
        }

        public override int CurrentBox { get => BoxLayout.CurrentBox; set => BoxLayout.CurrentBox = value; }
        public override int BoxesUnlocked { get => (byte)Blocks.GetBlockValue(SaveBlockAccessor8SWSH.KBoxesUnlocked); set => Blocks.SetBlockValue(SaveBlockAccessor8SWSH.KBoxesUnlocked, (byte)value); }

        public override byte[] BoxFlags
        {
            get => new [] {Convert.ToByte(Blocks.GetBlock(SaveBlockAccessor8SWSH.KSecretBoxUnlocked).Type - 1)};
            set
            {
                if (value.Length != 1)
                    return;
                var block = Blocks.GetBlock(SaveBlockAccessor8SWSH.KSecretBoxUnlocked);
                block.ChangeBooleanType((SCTypeCode)(value[0] & 1) + 1);
            }
        }

        public override bool HasBoxWallpapers => true;
        public override bool HasNamableBoxes => true;

        public override int GetBoxWallpaper(int box)
        {
            if ((uint)box >= BoxCount)
                return box;
            var b = Blocks.GetBlock(SaveBlockAccessor8SWSH.KBoxWallpapers);
            return b.Data[box];
        }

        public override void SetBoxWallpaper(int box, int value)
        {
            if ((uint)box >= BoxCount)
                return;
            var b = Blocks.GetBlock(SaveBlockAccessor8SWSH.KBoxWallpapers);
            b.Data[box] = (byte)value;
        }

        public void UnlockAllDiglett()
        {
            if (SaveRevision == 0)
                return; // no blocks

            (int Zone, int Max)[] zones =
            {
                (0201, 16), // Fields of Honor
                (0202, 18), // Soothing Wetlands
                (0203, 6), // Forest of Focus
                (0204, 7), // Challenge Beach
                (0205, 5), // Brawlers' Cave
                (0206, 6), // Challenge Road
                (0207, 5), // Courageous Cavern
                (0208, 5), // Loop Lagoon
                (0209, 13), // Training Lowlands
                (0210, 1), // Warm-Up Tunnel
                (0211, 8), // Potbottom Desert
                (0221, 9), // Workout Sea
                (0222, 5), // Stepping-Stone Sea
                (0223, 3), // Insular Sea
                (0224, 1), // Honeycalm Sea
                (0231, 9), // Honeycalm Island
            };
            var s = Blocks;
            static uint Hash(string str) => (uint)FnvHash.HashFnv1a_64(str);
            foreach (var (zone, max) in zones)
            {
                var baseName = $"z_wr{zone:0000}_F_DHIGUDA";
                s.GetBlock(Hash(baseName)).ChangeBooleanType(SCTypeCode.Bool2);
                for (int i = 0; i <= max; i++)
                {
                    var otherName = $"{baseName}_{i}";
                    s.GetBlock(Hash(otherName)).ChangeBooleanType(SCTypeCode.Bool2);
                }

                var countName = $"WK_EV_R1_DHIG_WR{zone:0000}";
                var value = max + 2;
                if (zone == 0223) // trio
                    value++;
                s.GetBlock(Hash(countName)).SetValue((uint)value);
            }

            const string TRIO = "z_wr0223_F_TRIO";
            s.GetBlock(Hash(TRIO)).ChangeBooleanType(SCTypeCode.Bool2);

            const string unreported = "WK_EV_R1_DHIGUDA_ADD";
            const string totalCount = "WK_EV_R1_DHIGUDA_COUNT";
            const string progressCt = "WK_EV_R1_DHIGUDA_PROGRESS";
            s.GetBlock(Hash(unreported)).SetValue((uint)0); // none unreported
            s.GetBlock(Hash(totalCount)).SetValue((uint)150); // all obtained count
            s.GetBlock(Hash(progressCt)).SetValue((uint)11); // all obtained progress value
        }
    }
}
