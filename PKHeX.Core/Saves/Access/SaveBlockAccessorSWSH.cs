using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SaveBlockAccessorSWSH : ISaveBlockAccessor<SCBlock>, ISaveBlock8Main
    {
        public IReadOnlyList<SCBlock> BlockInfo { get; }
        public Box8 BoxInfo { get; }
        public Party8 PartyInfo { get; }
        public MyItem Items { get; }
        public MyStatus8 MyStatus { get; }
        public Misc8 Misc { get; }
        public Zukan8 Zukan { get; }
        public BoxLayout8 BoxLayout { get; }
        public PlayTime8 Played { get; }
        public Fused8 Fused { get; }
        public Daycare8 Daycare { get; }
        public Record8 Records { get; }
        public TrainerCard8 TrainerCard{ get; }

        public SaveBlockAccessorSWSH(SAV8SWSH sav)
        {
            BlockInfo = sav.AllBlocks;
            BoxInfo = new Box8(sav, GetBlock(IBox));
            PartyInfo = new Party8(sav, GetBlock(IParty));
            Items = new MyItem8(sav, GetBlock(IItem));
            Zukan = new Zukan8(sav, GetBlock(IZukan));
            MyStatus = new MyStatus8(sav, GetBlock(IMyStatus));
            Misc = new Misc8(sav, GetBlock(IMisc));
            BoxLayout = new BoxLayout8(sav, GetBlock(IBoxLayout));
            TrainerCard = new TrainerCard8(sav, GetBlock(ITrainerCard));
            Played = new PlayTime8(sav, GetBlock(IPlayTime));
            Fused = new Fused8(sav, GetBlock(IFused));
            Daycare = new Daycare8(sav, GetBlock(IDaycare));
            Records = new Record8(sav, GetBlock(IRecord), Core.Records.MaxType_SWSH);
        }

        private const int IBox = 143; // Box Data
        private const int IMysteryGift = 186; // Mystery Gift Data
        private const int IItem = 191; // Items
        // Coordinates? 253
        private const int IBoxLayout = 275; // Box Names
        private const int IMisc = 288; // Money
        private const int IParty = 428; // Party Data
        private const int IDaycare = 465; // Daycare slots (2 daycares)
        private const int IRecord = 544;
        private const int IZukan = 699; // PokeDex
        private const int ITrainerCard = 1259; // Trainer Card
        private const int IPlayTime = 1302; // Time Played
        private const int IRepel = 1469;
        private const int IFused = 1789; // Fused PKM (*3)
        private const int IFashionUnlock = 1989; // Fashion unlock bool array (owned for (each apparel type) * 0x80, then another array for "new")
        private const int IMyStatus = 2275; // Trainer Details

        public SCBlock GetBlock(int index) => BlockInfo[index];
    }
}