using static PKHeX.Core.EvolutionType;

namespace PKHeX.Core
{
    /// <summary>
    /// Criteria for evolving to this branch in the <see cref="EvolutionTree"/>
    /// </summary>
    public sealed class EvolutionMethod
    {
        /// <summary>
        /// Evolution Method
        /// </summary>
        public readonly int Method;

        /// <summary>
        /// Evolve to Species
        /// </summary>
        public readonly int Species;

        /// <summary>
        /// Conditional Argument (different from <see cref="Level"/>)
        /// </summary>
        public readonly int Argument;

        /// <summary>
        /// Conditional Argument (different from <see cref="Argument"/>)
        /// </summary>
        public readonly int Level;

        /// <summary>
        /// Destination Form
        /// </summary>
        /// <remarks>Is <see cref="AnyForm"/> if the evolved form isn't modified. Special consideration for <see cref="LevelUpFormFemale1"/>, which forces 1.</remarks>
        public readonly int Form;

        private const int AnyForm = -1;

        // Not stored in binary data
        public bool RequiresLevelUp; // tracks if this method requires a Level Up, lazily set

        public EvolutionMethod(int method, int species, int argument = 0, int level = 0, int form = AnyForm)
        {
            Method = method;
            Species = species;
            Argument = argument;
            Form = form;
            Level = level;
        }

        /// <summary>
        /// Returns the form that the Pokémon will have after evolution.
        /// </summary>
        /// <param name="form">Un-evolved Form ID</param>
        public int GetDestinationForm(int form)
        {
            if (Method == (int)LevelUpFormFemale1)
                return 1;
            if (Form == AnyForm)
                return form;
            return Form;
        }

        /// <summary>
        /// Checks the <see cref="EvolutionMethod"/> for validity by comparing against the <see cref="PKM"/> data.
        /// </summary>
        /// <param name="pkm">Entity to check</param>
        /// <param name="lvl">Current level</param>
        /// <param name="skipChecks">Option to skip some comparisons to return a 'possible' evolution.</param>
        /// <returns>True if a evolution criteria is valid.</returns>
        public bool Valid(PKM pkm, int lvl, bool skipChecks)
        {
            RequiresLevelUp = false;
            switch ((EvolutionType)Method)
            {
                case UseItem or UseItemWormhole:
                case Crit3 or HPDownBy49 or SpinType:
                case TowerOfDarkness or TowerOfWaters:
                    return true;
                case UseItemMale:
                    return pkm.Gender == 0;
                case UseItemFemale:
                    return pkm.Gender == 1;

                case Trade or TradeHeldItem or TradeSpecies:
                    return !pkm.IsUntraded || skipChecks;

                // Special Level Up Cases -- return false if invalid
                case LevelUpNatureAmped or LevelUpNatureLowKey when GetAmpLowKeyResult(pkm.Nature) != pkm.Form && !skipChecks:
                    return false;

                case LevelUpBeauty when pkm is not IContestStats s || s.CNT_Beauty < Argument:
                    return skipChecks;
                case LevelUpMale when pkm.Gender != 0:
                    return false;
                case LevelUpFemale when pkm.Gender != 1:
                    return false;
                case LevelUpFormFemale1 when pkm.Gender != 1 || pkm.Form != 1:
                    return false;

                case LevelUpVersion or LevelUpVersionDay or LevelUpVersionNight when ((pkm.Version & 1) != (Argument & 1) && pkm.IsUntraded) || skipChecks:
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
            int origin = pkm.Generation;
            return origin switch
            {
                // No met data in RBY; No met data in GS, Crystal met data can be reset
                1 or 2 => true,

                // Pal Park / PokeTransfer updates Met Level
                3 or 4 => pkm.Format > origin || pkm.Met_Level < lvl,

                // 5=>6 and later transfers keep current level
                >=5 => lvl >= Level && (!pkm.IsNative || pkm.Met_Level < lvl),

                _ => false,
            };
        }

        public EvoCriteria GetEvoCriteria(int species, int form, int lvl)
        {
            return new(species, form)
            {
                Level = lvl,
                Method = Method,
            };
        }

        public static int GetAmpLowKeyResult(int n)
        {
            if ((uint)(n - 1) > 22)
                return 0;
            return (0x5BCA51 >> (n - 1)) & 1;
        }
    }
}