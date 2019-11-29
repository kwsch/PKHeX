using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public sealed class SAV8SWSH : SAV8, ISaveBlock8SWSH, ITrainerStatRecord
    {
        public SAV8SWSH(byte[] data) : this(data, SwishCrypto.Decrypt(data))
        {
        }

        private SAV8SWSH(byte[] data, IReadOnlyList<SCBlock> blocks) : base(data)
        {
            Data = Array.Empty<byte>();
            AllBlocks = blocks;
            Blocks = new SaveBlockAccessorSWSH(this);
            Initialize();
        }

        public SAV8SWSH()
        {
            AllBlocks = Meta8.GetBlankDataSWSH();
            Blocks = new SaveBlockAccessorSWSH(this);
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
                mine[i].Data = newB[i].Data;
            Edited = true;
        }

        public IReadOnlyList<SCBlock> AllBlocks { get; }
        public override bool ChecksumsValid => true;
        public override string ChecksumInfo => string.Empty;
        protected override void SetChecksums() { } // None!
        protected override byte[] GetFinalData() => SwishCrypto.Encrypt(AllBlocks);

        public override PersonalTable Personal => PersonalTable.SWSH;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SWSH;

        #region Blocks
        public SaveBlockAccessorSWSH Blocks { get; }
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

        #endregion
        public override SaveFile Clone() => new SAV8SWSH(BAK, AllBlocks.Select(z => z.Clone()).ToArray());

        public override int MaxMoveID => Legal.MaxMoveID_8;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_8;
        public override int MaxItemID => Legal.MaxItemID_8;
        public override int MaxBallID => Legal.MaxBallID_8;
        public override int MaxGameID => Legal.MaxGameID_8;
        public override int MaxAbilityID => Legal.MaxAbilityID_8;

        private void Initialize()
        {
            Box = 0;
            Party = 0;
            PokeDex = 0;
        }

        protected override void SetPartyValues(PKM pkm, bool isParty)
        {
            base.SetPartyValues(pkm, isParty);
            ((PK8)pkm).FormArgument = GetFormArgument((PK8)pkm, isParty);
        }

        private static uint GetFormArgument(PK8 pkm, bool isParty)
        {
            if (pkm.Species == (int) Species.Alcremie)
                return pkm.FormArgument & 7;

            if (!isParty || pkm.AltForm == 0)
                return 0;

            // Neither species is available in SW/SH, but the game code still does this!
            return pkm.Species switch
            {
                (int)Species.Furfrou => 5u, // Furfrou
                (int)Species.Hoopa => 3u, // Hoopa
                _ => 0u
            };
        }

        public int GetRecord(int recordID) => Records.GetRecord(recordID);
        public void SetRecord(int recordID, int value) => Records.SetRecord(recordID, value);
        public int GetRecordMax(int recordID) => Records.GetRecordMax(recordID);
        public int GetRecordOffset(int recordID) => Records.GetRecordOffset(recordID);
        public int RecordCount => Record8.RecordCount;
    }
}