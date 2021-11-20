using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Structure that manages the 3 event variable storage arrays.
    /// </summary>
    /// <remarks>Comprised of 3 fixed-sized arrays, we do our read/write in-place. Each element in an array is 4 bytes. Total size: 0x55F0 </remarks>
    public sealed class FlagWork8b : SaveBlock, IEventFlag, ISystemFlag, IEventWork<int>
    {
        public const int COUNT_WORK = 500;
        public const int COUNT_FLAG = 4000;
        public const int COUNT_SYSTEM = 1000;

        public const int OFS_WORK = 0;
        public const int OFS_FLAG = OFS_WORK + (COUNT_WORK * 4);
        public const int OFS_SYSTEM = OFS_FLAG + (COUNT_FLAG * 4);

        public const int FH_START = 0;
        public const int FH_END = 63;
        public const int FE_FLAG_START = 63;
        public const int FE_END = 314;
        public const int FV_FLAG_START = 314;
        public const int FV_END = 603;
        public const int FT_START = 603;
        public const int FT_END = 859;
        public const int FV_FLD_START = 859;
        public const int FV_FLD_END = 1115;
        public const int TMFLG_START = 1115;

        private const int BASE_VANISH = FV_FLAG_START;
        private const int END_VANISH = FV_END - 1;
        private const int COUNT_VANISH = END_VANISH - BASE_VANISH; // 0x120

        public FlagWork8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;
        public int CountFlag => COUNT_FLAG;
        public int CountSystem => COUNT_SYSTEM;
        public int CountWork => COUNT_WORK;

        private int GetOffsetFlag(int flagNo)
        {
            if ((uint)flagNo >= COUNT_FLAG)
                throw new ArgumentOutOfRangeException(nameof(flagNo), $"Expected a number below {COUNT_FLAG}, not {flagNo}.");
            return Offset + OFS_FLAG + (4 * flagNo);
        }
        private int GetOffsetSystem(int flagNo)
        {
            if ((uint)flagNo >= COUNT_SYSTEM)
                throw new ArgumentOutOfRangeException(nameof(flagNo), $"Expected a number below {COUNT_SYSTEM}, not {flagNo}.");
            return Offset + OFS_SYSTEM + (4 * flagNo);
        }

        private int GetOffsetWork(int flagNo)
        {
            if ((uint)flagNo >= COUNT_WORK)
                throw new ArgumentOutOfRangeException(nameof(flagNo), $"Expected a number below {COUNT_WORK}, not {flagNo}.");
            return Offset + OFS_WORK + (4 * flagNo);
        }

        public bool GetFlag      (int flagNo) => BitConverter.ToInt32(Data, GetOffsetFlag(flagNo)) == 1;
        public bool GetSystemFlag(int flagNo) => BitConverter.ToInt32(Data, GetOffsetSystem(flagNo)) == 1;
        public int GetWork       (int workNo) => BitConverter.ToInt32(Data, GetOffsetWork(workNo));
        public float GetFloatWork(int workNo) => BitConverter.ToSingle(Data, GetOffsetWork(workNo));

        public void SetFlag      (int flagNo,  bool value) => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, GetOffsetFlag(flagNo));
        public void SetSystemFlag(int flagNo,  bool value) => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, GetOffsetSystem(flagNo));
        public void SetWork      (int workNo,   int value) => BitConverter.GetBytes(value).CopyTo(Data, GetOffsetWork(workNo));
        public void SetFloatWork (int workNo, float value) => BitConverter.GetBytes(value).CopyTo(Data, GetOffsetWork(workNo));

        public void ResetFlag      (int flagNo) => SetFlag(flagNo, false);
        public void ResetVanishFlag(int flagNo) => SetVanishFlag(flagNo, false);
        public void ResetSystemFlag(int flagNo) => SetSystemFlag(flagNo, false);
        public void ResetWork      (int workNo) => SetWork(workNo, 0);

        public bool GetVanishFlag(int flagNo)
        {
            if ((uint)flagNo >= COUNT_VANISH)
                throw new ArgumentOutOfRangeException(nameof(flagNo), $"Expected a number below {COUNT_VANISH}, not {flagNo}.");
            return GetFlag(BASE_VANISH + flagNo);
        }

        public void SetVanishFlag(int flagNo, bool value)
        {
            if ((uint)flagNo >= COUNT_VANISH)
                throw new ArgumentOutOfRangeException(nameof(flagNo), $"Expected a number below {COUNT_VANISH}, not {flagNo}.");
            SetFlag(BASE_VANISH + flagNo, value);
        }

        public int BadgeCount()
        {
            // system flags 124-131
            int ctr = 0;
            for (int i = 0; i < 8; i++)
                ctr += GetSystemFlag(124 + i) ? 1 : 0;
            return ctr;
        }
    }
}
