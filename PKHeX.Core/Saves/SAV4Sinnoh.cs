using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Abstract <see cref="SaveFile"/> format for <see cref="GameVersion.DP"/> and <see cref="GameVersion.Pt"/>
    /// </summary>
    public abstract class SAV4Sinnoh : SAV4
    {
        protected override int StorageStart => GeneralSize;
        protected override int FooterSize => 0x14;
        protected SAV4Sinnoh() { }
        protected SAV4Sinnoh(byte[] data) : base(data) { }

        #region Storage
        // u32 currentBox
        // box{pk4[30}[18]
        // g4str[18] boxNames
        // byte[18] boxWallpapers
        private const int BOX_COUNT = 18;
        private const int BOX_SLOTS = 30;
        private const int BOX_NAME_LEN = 40; // 20 characters

        private const int BOX_DATA_LEN = (BOX_SLOTS * PokeCrypto.SIZE_4STORED); // 0xFF0, no padding between boxes (to nearest 0x100)
        private const int BOX_END = BOX_COUNT * BOX_DATA_LEN; // 18 * 0xFF0
        private const int BOX_NAME = 4 + BOX_END; // after box data
        private const int BOX_WP = BOX_NAME + (BOX_COUNT * BOX_NAME_LEN); // 0x121B4;
        private const int BOX_FLAGS = 18 + BOX_WP; // 0x121C6

        public override int GetBoxOffset(int box) => 4 + (box * BOX_DATA_LEN);
        private static int GetBoxNameOffset(int box) => BOX_NAME + (box * BOX_NAME_LEN);
        protected override int GetBoxWallpaperOffset(int box) => BOX_WP + box;

        public override int CurrentBox // (align 32)
        {
            get => Storage[0];
            set => Storage[0] = (byte)value;
        }

        public override byte[] BoxFlags
        {
            get => new[] { Storage[BOX_FLAGS] };
            set => Storage[BOX_FLAGS] = value[0];
        }

        public override string GetBoxName(int box) => GetString(Storage, GetBoxNameOffset(box), BOX_NAME_LEN);

        public override void SetBoxName(int box, string value)
        {
            const int maxlen = 8;
            if (value.Length > maxlen)
                value = value.Substring(0, maxlen); // Hard cap
            int offset = GetBoxNameOffset(box);
            var str = SetString(value, maxlen);
            SetData(Storage, str, offset);
        }
        #endregion

        #region Poketch
        public int PoketchStart { get; protected set; }
        private byte PoketchPacked { get => General[PoketchStart]; set => General[PoketchStart] = value; }

        public bool PoketchEnabled { get => (PoketchPacked & 1) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 1) : (PoketchPacked & ~1)); }
        public bool PoketchFlag1 { get => (PoketchPacked & 2) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 2) : (PoketchPacked & ~2)); }
        public bool PoketchFlag2 { get => (PoketchPacked & 4) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 4) : (PoketchPacked & ~4)); }

        public PoketchColor PoketchColor
        {
            get => (PoketchColor) ((PoketchPacked >> 3) & 7);
            set => PoketchPacked = (byte) ((PoketchPacked & 0xC7) | ((int) value << 3));
        }

        public bool PoketchFlag6 { get => (PoketchPacked & 0x40) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 0x40) : (PoketchPacked & ~0x40)); }
        public bool PoketchFlag7 { get => (PoketchPacked & 0x80) != 0; set => PoketchPacked = (byte)(value ? (PoketchPacked | 0x80) : (PoketchPacked & ~0x80)); }
        private byte Poketch1 { get => General[PoketchStart + 1]; set => General[PoketchStart + 1] = value; }
        public sbyte CurrentPoketchApp { get => (sbyte)General[PoketchStart + 2]; set => General[PoketchStart + 2] = (byte)Math.Min((sbyte)PoketchApp.Alarm_Clock, value); }

        public bool GetPoketchAppUnlocked(PoketchApp index)
        {
            if (index > PoketchApp.Alarm_Clock)
                throw new ArgumentException(nameof(index));
            return General[PoketchStart + 3 + (int) index] != 0;
        }

        public void SetPoketchAppUnlocked(PoketchApp index, bool value = true)
        {
            if (index > PoketchApp.Alarm_Clock)
                throw new ArgumentException(nameof(index));
            var b = value ? 1 : 0;
            General[PoketchStart + 3 + (int)index] = (byte)b;
        }

        // 8 bytes unk

        public uint PoketchStepCounter
        {
            get => BitConverter.ToUInt32(General, PoketchStart + 0x24);
            set => SetData(General, BitConverter.GetBytes(value), PoketchStart + 0x24);
        }

        // 2 bytes for alarm clock time setting

        public byte[] PoketchDotArtistData
        {
            get => General.Slice(PoketchStart + 0x2A, 120);
            set => SetData(General, value, PoketchStart + 0x2A);
        }

        // map marking stuff is at the end, unimportant

        #endregion

        #region Honey Trees
        protected int OFS_HONEY;
        protected const int HONEY_SIZE = 8;

        public HoneyTree GetHoneyTree(int index)
        {
            if ((uint)index > 21)
                throw new ArgumentException(nameof(index));
            return new HoneyTree(General.Slice(OFS_HONEY + (HONEY_SIZE * index), HONEY_SIZE));
        }

        public void SetHoneyTree(HoneyTree tree, int index)
        {
            if (index <= 21)
                SetData(General, tree.Data, OFS_HONEY + (HONEY_SIZE * index));
        }

        public int[] MunchlaxTrees
        {
            get
            {
                int A = (TID >> 8) % 21;
                int B = (TID & 0x00FF) % 21;
                int C = (SID >> 8) % 21;
                int D = (SID & 0x00FF) % 21;

                if (A == B) B = (B + 1) % 21;
                if (A == C) C = (C + 1) % 21;
                if (B == C) C = (C + 1) % 21;
                if (A == D) D = (D + 1) % 21;
                if (B == D) D = (D + 1) % 21;
                if (C == D) D = (D + 1) % 21;

                return new[] { A, B, C, D };
            }
        }
        #endregion

        public int OFS_PoffinCase { get; protected set; }

        //Underground Scores
        protected int OFS_UG_Stats;
        public uint UG_PlayersMet { get => BitConverter.ToUInt32(General, OFS_UG_Stats); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats); }
        public uint UG_Gifts { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0x4); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x4); }
        public uint UG_Spheres { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0xC); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0xC); }
        public uint UG_Fossils { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0x10); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x10); }
        public uint UG_TrapsAvoided { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0x18); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x18); }
        public uint UG_TrapsTriggered { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0x1C); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x1C); }
        public uint UG_Flags { get => BitConverter.ToUInt32(General, OFS_UG_Stats + 0x34); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x34); }
    }

    public enum PoketchColor
    {
        Green = 0,
        Yellow = 1,
        Orange = 2,
        Red = 3,
        Purple = 4,
        Blue = 5,
        Turquoise = 6,
        White = 7,
    }

    public enum PoketchApp
    {
        Digital_Watch,
        Calculator,
        Memo_Pad,
        Pedometer,
        Party,
        Friendship_Checker,
        Dowsing_Machine,
        Berry_Searcher,
        Daycare,
        History,
        Counter,
        Analog_Watch,
        Marking_Map,
        Link_Searcher,
        Coin_Toss,
        Move_Tester,
        Calendar,
        Dot_Artist,
        Roulette,
        Trainer_Counter,
        Kitchen_Timer,
        Color_Changer,
        Matchup_Checker,
        Stopwatch,
        Alarm_Clock,
    }
}