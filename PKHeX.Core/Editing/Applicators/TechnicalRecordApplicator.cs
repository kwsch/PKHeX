using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for modifying the Technical Record flags of a <see cref="PK8"/>.
    /// </summary>
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
            var permit = pk8.PersonalInfo.TMHM.AsSpan(PersonalInfoSWSH.CountTM);
            var moveIDs = Legal.TMHM_SWSH.AsSpan(PersonalInfoSWSH.CountTM);
            foreach (var m in moves)
            {
                var index = moveIDs.IndexOf(m);
                if (index == -1)
                    continue;
                if (permit[index])
                    pk8.SetMoveRecordFlag(index, true);
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
            var permit = pk8.PersonalInfo.TMHM.AsSpan(PersonalInfoSWSH.CountTM); // tm[100], tr[100]
            for (int i = 0; i < permit.Length; i++)
            {
                if (permit[i])
                    pk8.SetMoveRecordFlag(i, true);
            }
        }
    }
}
