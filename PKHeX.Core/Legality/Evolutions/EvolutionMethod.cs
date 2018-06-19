using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Criteria for evolving to this branch in the <see cref="EvolutionTree"/>
    /// </summary>
    public class EvolutionMethod
    {
        public int Method;
        public int Species;
        public int Argument;
        public int Form = -1;
        public int Level;

        public bool RequiresLevelUp;

        internal static readonly HashSet<int> TradeMethods = new HashSet<int> {5, 6, 7};
        private static readonly IReadOnlyCollection<GameVersion> NoBanlist = new GameVersion[0];
        internal static readonly IReadOnlyCollection<GameVersion> BanSM = new[] {GameVersion.SN, GameVersion.MN};
        internal IReadOnlyCollection<GameVersion> Banlist = NoBanlist;

        public bool Valid(PKM pkm, int lvl, bool skipChecks)
        {
            RequiresLevelUp = false;
            if (Form > -1)
                if (!skipChecks && pkm.AltForm != Form)
                    return false;

            if (!skipChecks && Banlist.Contains((GameVersion)pkm.Version) && pkm.IsUntraded) // sm lacks usum kantonian evos
                return false;

            switch (Method)
            {
                case 8: // Use Item
                case 42:
                    return true;
                case 17: // Male
                    return pkm.Gender == 0;
                case 18: // Female
                    return pkm.Gender == 1;

                case 5: // Trade Evolution
                case 6: // Trade while Holding
                case 7: // Trade for Opposite Species
                    return !pkm.IsUntraded || skipChecks;

                // Special Levelup Cases
                case 16:
                    if (!(pkm is IContestStats s) || s.CNT_Beauty < Argument)
                        return skipChecks;
                    goto default;
                case 23: // Gender = Male
                    if (pkm.Gender != 0)
                        return false;
                    goto default;
                case 24: // Gender = Female
                    if (pkm.Gender != 1)
                        return false;
                    goto default;
                case 34: // Gender = Female, out Form1
                    if (pkm.Gender != 1 || pkm.AltForm != 1)
                        return false;
                    goto default;

                case 36: // Any Time on Version
                case 37: // Daytime on Version
                case 38: // Nighttime on Version
                    // Version checks come in pairs, check for any pair match
                    if ((pkm.Version & 1) != (Argument & 1) && pkm.IsUntraded || skipChecks)
                        return skipChecks;
                    goto default;

                default:
                    if (Level == 0 && lvl < 2)
                        return false;
                    if (lvl < Level)
                        return false;

                    RequiresLevelUp = true;
                    if (skipChecks)
                        return lvl >= Level;

                    // Check Met Level for extra validity
                    switch (pkm.GenNumber)
                    {
                        case 1: // No metdata in RBY
                        case 2: // No metdata in GS, Crystal metdata can be reset
                            return true;
                        case 3:
                        case 4:
                            if (pkm.Format > pkm.GenNumber) // Pal Park / PokeTransfer updates Met Level
                                return true;
                            return pkm.Met_Level < lvl;

                        case 5: // Bank keeps current level
                        case 6:
                        case 7:
                            return lvl >= Level && (!pkm.IsNative || pkm.Met_Level < lvl);
                    }
                    return false;
            }
        }

        public EvoCriteria GetEvoCriteria(int species, int lvl)
        {
            return new EvoCriteria
            {
                Species = species,
                Level = lvl,
                Form = Form,
                Flag = Method,
            };
        }

        public EvolutionMethod Copy(int species = -1)
        {
            if (species < 0)
                species = Species;
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