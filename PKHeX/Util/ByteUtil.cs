using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public partial class Util
    {
        /// <summary>
        /// Swaps the Endianness of an int
        /// <param name="val">Value to swap endianness of.</param>
        /// <returns>The endianness-swapped value.</returns>
        internal static int SwapEndianness(int val)
        {
            return (int) SwapEndianness((uint) val);
        }

        /// <summary>
        /// Swaps the Endianness of a uint
        /// <param name="val">Value to swap endianness of.</param>
        /// <returns>The endianness-swapped value.</returns>
        internal static uint SwapEndianness(uint val)
        {
            return ((val & 0x000000FF) << 24)
                   | ((val & 0x0000FF00) << 8)
                   | ((val & 0x00FF0000) >> 8)
                   | ((val & 0xFF000000) >> 24);
        }

        /// <summary>
        /// Swaps the Endianness of a short
        /// <param name="val">Value to swap endianness of.</param>
        /// <returns>The endianness-swapped value.</returns>
        internal static int SwapEndianness(short val)
        {
            return (short)SwapEndianness((ushort)val);
        }

        /// <summary>
        /// Swaps the Endianness of a ushort
        /// <param name="val">Value to swap endianness of.</param>
        /// <returns>The endianness-swapped value.</returns>
        internal static ushort SwapEndianness(ushort val)
        {
            return (ushort) (((val & 0x00FF) << 8) | ((val & 0xFF00) >> 8));
        }
    }
}
