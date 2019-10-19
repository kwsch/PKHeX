using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 <see cref="SaveFile"/> object for <see cref="GameVersion.SWSH"/> games.
    /// </summary>
    public sealed class SAV8SWSH : SAV8, ISaveBlock8SWSH
    {
        public SAV8SWSH(byte[] data) : base(data, SaveBlockAccessorSWSH.boGG)
        {
            Blocks = new SaveBlockAccessorSWSH(this);
            Initialize();
        }

        public SAV8SWSH() : base(SaveUtil.SIZE_G8SWSH, SaveBlockAccessorSWSH.boGG)
        {
            Blocks = new SaveBlockAccessorSWSH(this);
            Initialize();
        }

        public override PersonalTable Personal => PersonalTable.SWSH;
        public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_SWSH;

        #region Blocks
        public SaveBlockAccessorSWSH Blocks { get; }
        public override IReadOnlyList<BlockInfo> AllBlocks => Blocks.BlockInfo;
        public override MyItem Items => Blocks.Items;
        public override Record8 Records => Blocks.Records;
        public override PlayTime8 Played => Blocks.Played;
        public override MyStatus8 MyStatus => Blocks.MyStatus;
        public override ConfigSave8 Config => Blocks.Config;
        public override GameTime8 GameTime => Blocks.GameTime;
        public override Misc8 Misc => Blocks.Misc;
        public override Zukan8 Zukan => Blocks.Zukan;
        public override EventWork8 EventWork => Blocks.EventWork;
        public override BoxLayout8 BoxLayout => Blocks.BoxLayout;
        public override Situation8 Situation => Blocks.Situation;
        public override FieldMoveModelSave8 Overworld => Blocks.Overworld;

        #endregion
        public override SaveFile Clone() => new SAV8SWSH((byte[])Data.Clone());
        public override int MaxMoveID => Legal.MaxMoveID_8;
        public override int MaxSpeciesID => Legal.MaxSpeciesID_8;
        public override int MaxItemID => Legal.MaxItemID_8;
        public override int MaxBallID => Legal.MaxBallID_8;
        public override int MaxGameID => Legal.MaxGameID_8;
        public override int MaxAbilityID => Legal.MaxAbilityID_8;

        private void Initialize()
        {
        }
    }
}