using System;

namespace PKHeX.Core
{
    public enum RNGType
    {
        /// <summary> No RNG type specified </summary>
        None,

        /// <summary> <see cref="RNG.LCRNG"/> </summary>
        LCRNG,

        /// <summary> <see cref="RNG.XDRNG"/> </summary>
        XDRNG,

        /// <summary> <see cref="RNG.ARNG"/> </summary>
        ARNG,
    }

    public static class RNGTypeUtil
    {
        public static LCRNG GetRNG(this RNGType type) => type switch
        {
            RNGType.LCRNG => RNG.LCRNG,
            RNGType.XDRNG => RNG.XDRNG,
            RNGType.ARNG => RNG.ARNG,
            _ => throw new ArgumentException(nameof(type))
        };
    }
}
