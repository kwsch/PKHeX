using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Dynamax Level used by <see cref="GameVersion.SWSH"/> format entity data.
    /// </summary>
    public interface IDynamaxLevel
    {
        byte DynamaxLevel { get; set; }
    }

    public static class DynamaxLevelExtensions
    {
        /// <summary>
        /// Checks if the species is allowed to have a non-zero value for <see cref="IDynamaxLevel.DynamaxLevel"/>.
        /// </summary>
        public static bool CanHaveDynamaxLevel(this IDynamaxLevel _, PKM pkm)
        {
            if (pkm.IsEgg)
                return false;
            if (pkm.BDSP || pkm.LA)
                return false;
            return CanHaveDynamaxLevel(pkm.Species);
        }

        /// <summary>
        /// Checks if the species is prevented from gaining any <see cref="IDynamaxLevel.DynamaxLevel"/> via candy in <see cref="GameVersion.SWSH"/>.
        /// </summary>
        private static bool CanHaveDynamaxLevel(int species)
        {
            return species is not ((int)Zacian or (int)Zamazenta or (int)Eternatus);
        }
    }
}
