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

        /// <summary>
        /// PID is randomly created and forced to be shiny as Stars.
        /// </summary>
        AlwaysStar = 5,

        /// <summary>
        /// PID is randomly created and forced to be shiny as Squares.
        /// </summary>
        AlwaysSquare = 6,
    }

    public static partial class Extensions
    {
        public static bool IsValid(this Shiny s, PKM pkm) => s switch
        {
            Shiny.Always => pkm.IsShiny,
            Shiny.Never => !pkm.IsShiny,
            Shiny.AlwaysSquare => pkm.ShinyXor == 0,
            Shiny.AlwaysStar => pkm.ShinyXor == 1,
            _ => true
        };
    }
}
