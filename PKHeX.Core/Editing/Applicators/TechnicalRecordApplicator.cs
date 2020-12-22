using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class TechnicalRecordApplicator
    {
        /// <summary>
        /// Sets the Technical Record flags for the <see cref="pk"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="value">Value to set for the record.</param>
        /// <param name="max">Max record to set.</param>
        public static void SetRecordFlags(this PKM pk, bool value, int max = 100)
        {
            if (pk is not PK8 pk8)
                return;
            for (int i = 0; i < max; i++)
                pk8.SetMoveRecordFlag(i, value);
        }

        /// <summary>
        /// Clears the Technical Record flags for the <see cref="pk"/>.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void ClearRecordFlags(this PKM pk) => pk.SetRecordFlags(false, 112);

        /// <summary>
        /// Sets the Technical Record flags for the <see cref="pk"/> based on the current moves.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        /// <param name="moves">Moves to set flags for. If a move is not a Technical Record, it is skipped.</param>
        public static void SetRecordFlags(this PKM pk, IEnumerable<int> moves)
        {
            if (pk is not PK8 pk8)
                return;
            var permit = pk8.PersonalInfo.TMHM;
            foreach (var m in moves)
            {
                var index = Array.IndexOf(Legal.TMHM_SWSH, m, 100);
                if (index < 100)
                    continue;
                if (permit[index])
                    pk8.SetMoveRecordFlag(index - 100, true);
            }
        }

        /// <summary>
        /// Sets all the Technical Record flags for the <see cref="pk"/> if they are permitted to be learned in-game.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetRecordFlags(this PKM pk)
        {
            if (pk is not PK8 pk8)
                return;
            var permit = pk8.PersonalInfo.TMHM;
            for (int i = 100; i < permit.Length; i++)
            {
                if (permit[i])
                    pk8.SetMoveRecordFlag(i - 100, true);
            }
        }
    }
}