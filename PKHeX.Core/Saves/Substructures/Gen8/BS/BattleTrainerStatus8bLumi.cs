using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Defeated Status for all trainers (Dpr.Trainer.TrainerID)
    /// </summary>
    /// <remarks>size: 0x1618</remarks>
    public sealed class BattleTrainerStatus8bLumi : SaveBlock<SAV8BSLuminescent>
    {
        public BattleTrainerStatus8bLumi(SAV8BSLuminescent sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // (bool IsWin1, bool IsBattleSearcher1, bool IsWin2, bool IsBattleSearcher2, bool IsWin3, bool IsBattleSearcher3, bool IsWin4, bool IsBattleSearcher4)[1414];
        private const int COUNT_TRAINER = 5656;
        //private const int SIZE_TRAINER_STRUCT = 1; // bool,bool,bool,bool,bool,bool,bool,bool

        public bool AnyDefeated => Enumerable.Range(0, COUNT_TRAINER).Any(GetIsWin);
        public bool AnyUndefeated => Enumerable.Range(0, COUNT_TRAINER).Any(z => !GetIsWin(z));

        /// <summary>
        /// Don't use this unless you've finished the post-game.
        /// </summary>
        public void DefeatAll()
        {
            for (int i = 0; i < COUNT_TRAINER; i++)
            {
                SetIsWin(i, true);
                SetIsBattleSearcher(i, false);
            }
        }

        /// <summary>
        /// Don't use this unless you've finished the post-game.
        /// </summary>
        public void RebattleAll()
        {
            for (int i = 0; i < COUNT_TRAINER; i++)
            {
                SetIsWin(i, false);
                SetIsBattleSearcher(i, true);
            }
        }

        private int GetTrainerStructOffset(int trainer)
        {
            if ((uint)trainer >= COUNT_TRAINER)
                throw new ArgumentOutOfRangeException(nameof(trainer));
            return Offset + (trainer / 4);
        }

        private void SetBit(ref byte bitFlag, byte bitIndex, bool bitValue)
        {
            bitFlag = (byte)(bitFlag & ~(0xF << bitIndex) | ((bitValue ? 1 : 0) << bitIndex));
        }

        public bool GetIsWin(int trainer) => (Data[GetTrainerStructOffset(trainer)] >> (trainer % 4 * 2) & 1) == 1;
        public bool GetIsBattleSearcher(int trainer) => (Data[GetTrainerStructOffset(trainer)] >> (trainer % 4 * 2 + 1) & 1) == 1;
        public void SetIsWin(int trainer, bool value) => SetBit(ref Data[GetTrainerStructOffset(trainer)], (byte)(trainer % 4 * 2), value);
        public void SetIsBattleSearcher(int trainer, bool value) => SetBit(ref Data[GetTrainerStructOffset(trainer)], (byte)(trainer % 4 * 2 + 1), value);
    }
}
