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

        private const int BOX_DATA_LEN = (BOX_SLOTS * PKX.SIZE_4STORED); // 0xFF0, no padding between boxes (to nearest 0x100)
        private const int BOX_END = BOX_COUNT * BOX_DATA_LEN; // 18 * 0xFF0
        private const int BOX_NAME = 4 + BOX_END; // after box data
        private const int BOX_WP = BOX_NAME + (BOX_COUNT * BOX_NAME_LEN); // 0x121B4;
        private const int BOX_WP_END = 18 + BOX_WP; // 0x121C6

        public override int GetBoxOffset(int box) => 4 + (box * BOX_DATA_LEN);
        private static int GetBoxNameOffset(int box) => BOX_NAME + (box * BOX_NAME_LEN);
        protected override int GetBoxWallpaperOffset(int box) => BOX_WP + box;

        public override int CurrentBox // (align 32)
        {
            get => Storage[0];
            set => Storage[0] = (byte)value;
        }

        public int Unknown
        {
            get => BitConverter.ToInt32(Storage, BOX_WP_END);
            set => SetData(Storage, BitConverter.GetBytes(value), BOX_WP_END);
        }

        public override string GetBoxName(int box) => GetString(Storage, GetBoxNameOffset(box), BOX_NAME_LEN);

        public override void SetBoxName(int box, string value)
        {
            const int maxlen = BOX_NAME_LEN / 2;
            if (value.Length > maxlen)
                value = value.Substring(0, maxlen); // Hard cap
            int offset = GetBoxNameOffset(box);
            var str = SetString(value, maxlen);
            SetData(Storage, str, offset);
        }
        #endregion

        public int CurrentPoketchApp { get => (sbyte)General[_currentPoketchApp]; set => General[_currentPoketchApp] = (byte)Math.Min(24, value); /* Alarm Clock */ }
        protected int _currentPoketchApp;

        // Honey Trees
        protected int OFS_HONEY;
        protected const int HONEY_SIZE = 8;

        public HoneyTree GetHoneyTree(int index)
        {
            if ((uint)index > 21)
                return null;
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

        public int OFS_PoffinCase { get; protected set; }

        //Underground Scores
        protected int OFS_UG_Stats;
        public int UG_PlayersMet { get => BitConverter.ToInt32(General, OFS_UG_Stats); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats); }
        public int UG_Gifts { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0x4); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x4); }
        public int UG_Spheres { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0xC); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0xC); }
        public int UG_Fossils { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0x10); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x10); }
        public int UG_TrapsAvoided { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0x18); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x18); }
        public int UG_TrapsTriggered { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0x1C); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x1C); }
        public int UG_Flags { get => BitConverter.ToInt32(General, OFS_UG_Stats + 0x34); set => SetData(General, BitConverter.GetBytes(value), OFS_UG_Stats + 0x34); }
    }
}