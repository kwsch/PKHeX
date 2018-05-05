namespace PKHeX.Core
{
    /// <summary>
    /// Specification for <see cref="PKM.IsShiny"/>, used for creating and validating.
    /// </summary>
    public enum Shiny : byte
    {
        /// <summary>
        /// PID is fixed to a specified value.
        /// </summary>
        FixedValue = 0,

        /// <summary>
        /// PID is purely random; can be shiny or not shiny.
        /// </summary>
        Random = 1,

        /// <summary>
        /// PID is randomly created and forced to be shiny.
        /// </summary>
        Always = 2,

        /// <summary>
        /// PID is randomly created and forced to be not shiny.
        /// </summary>
        Never = 3,
    }

    public static partial class Extensions
    {
        public static bool IsValid(this Shiny s, PKM pkm)
        {
            switch (s)
            {
                case Shiny.Always: return pkm.IsShiny;
                case Shiny.Never:  return !pkm.IsShiny;
                default:
                    return true;
            }
        }
    }
}
