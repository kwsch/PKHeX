using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EvolutionType;

namespace PKHeX.Core
{
    /// <summary>
    /// Criteria for evolving to this branch in the <see cref="EvolutionTree"/>
    /// </summary>
    public sealed class EvolutionMethod
    {
        public int Method;
        public int Species;
        public int Argument;
        public int Form = -1;
        public int Level;

        // Not stored in binary data
        public bool RequiresLevelUp; // tracks if this method requires a Level Up
        internal IReadOnlyCollection<GameVersion> Banlist = Array.Empty<GameVersion>();

        internal static readonly IReadOnlyCollection<GameVersion> BanSM = new[] {GameVersion.SN, GameVersion.MN};

        /// <summary>
        /// Checks the <see cref="EvolutionMethod"/> for validity by comparing against the <see cref="PKM"/> data.
        /// </summary>
        /// <param name="pkm">Entity to check</param>
        /// <param name="lvl">Current level</param>
        /// <param name="skipChecks">Option to skip some comparisons to return a 'possible' evolution.</param>
        /// <returns></returns>
        public bool Valid(PKM pkm, int lvl, bool skipChecks)
        {
            RequiresLevelUp = false;
            if (Form > -1)
            {
                if (!skipChecks && pkm.AltForm != Form)
                    return false;
            }

            // Check for unavailable evolution methods for an un-traded specimen.
            // Example: Sun/Moon lack Ultra's Kantonian evolution methods.
            if (!skipChecks && Banlist.Count > 0 && Banlist.Contains((GameVersion)pkm.Version) && pkm.IsUntraded)
                return false;

            switch ((EvolutionType)Method)
            {
                case UseItem:
                case UseItemWormhole:
                    return true;
                case UseItemMale:
                    return pkm.Gender == 0;
                case UseItemFemale:
                    return pkm.Gender == 1;

                case Trade:
                case TradeHeldItem:
                case TradeSpecies:
                    return !pkm.IsUntraded || skipChecks;

                // Special Level Up Cases -- return false if invalid
                case LevelUpBeauty when !(pkm is IContestStats s) || s.CNT_Beauty < Argument:
                    return skipChecks;
                case LevelUpMale when pkm.Gender != 0:
                    return false;
                case LevelUpFemale when pkm.Gender != 1:
                    return false;
                case LevelUpFormFemale1 when pkm.Gender != 1 || pkm.AltForm != 1:
                    return false;

                case LevelUpVersion when ((pkm.Version & 1) != (Argument & 1) && pkm.IsUntraded) || skipChecks:
                case LevelUpVersionDay when ((pkm.Version & 1) != (Argument & 1) && pkm.IsUntraded) || skipChecks:
                case LevelUpVersionNight when ((pkm.Version & 1) != (Argument & 1) && pkm.IsUntraded) || skipChecks:
                    return skipChecks; // Version checks come in pairs, check for any pair match

                // Level Up (any); the above Level Up (with condition) cases will reach here if they were valid
                default:
                    if (Level == 0 && lvl < 2)
                        return false;
                    if (lvl < Level)
                        return false;

                    RequiresLevelUp = true;
                    if (skipChecks)
                        return lvl >= Level;

                    // Check Met Level for extra validity
                    return HasMetLevelIncreased(pkm, lvl);
            }
        }

        private bool HasMetLevelIncreased(PKM pkm, int lvl)
        {
            int origin = pkm.GenNumber;
            switch (origin)
            {
                case 1: // No met data in RBY
                case 2: // No met data in GS, Crystal met data can be reset
                    return true;
                case 3:
                case 4:
                    if (pkm.Format > origin) // Pal Park / PokeTransfer updates Met Level
                        return true;
                    return pkm.Met_Level < lvl;

                case 5: // Bank keeps current level
                case 6:
                case 7:
                    return lvl >= Level && (!pkm.IsNative || pkm.Met_Level < lvl);

                default: return false;
            }
        }

        public EvoCriteria GetEvoCriteria(int species, int lvl)
        {
            return new EvoCriteria
            {
                Species = species,
                Level = lvl,
                Form = Form,
                Method = Method,
            };
        }

        public EvolutionMethod Copy(int species)
        {
            return new EvolutionMethod
            {
                Method = Method,
                Species = species,
                Argument = Argument,
                Form = Form,
                Level = Level
            };
        }
    }
}