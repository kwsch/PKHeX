using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_2 = 251;
        internal const int MaxMoveID_2 = 251;
        internal const int MaxItemID_2 = 255;
        internal const int MaxAbilityID_2 = 0;

        /// <summary>
        /// Generation 2 -> Generation 7 Transfer Location (Johto)
        /// </summary>
        public const int Transfer2 = 30017;

        internal static readonly ushort[] Pouch_Items_GSC = {
            3, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49, 51, 52, 53, 57, 60, 62, 63, 64, 65, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 96, 97, 98, 99, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 117, 118, 119, 121, 122, 123, 124, 125, 126, 131, 132, 138, 139, 140, 143, 144, 146, 150, 151, 152, 156, 158, 163, 167, 168, 169, 170, 172, 173, 174, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189
        };

        internal static readonly ushort[] Pouch_Ball_GSC = {
            1, 2, 4, 5, 157, 159, 160, 161, 164, 165, 166
        };

        internal static readonly ushort[] Pouch_Key_GS = {
            7, 54, 55, 58, 59, 61, 66, 67, 68, 69, 71, 127, 128, 130, 133, 134, 175, 178
        };

        internal static readonly ushort[] Pouch_Key_C = Pouch_Key_GS.Concat(new ushort[]{70, 115, 116, 129}).ToArray();

        internal static readonly ushort[] Pouch_TMHM_GSC = {
            191, 192, 193, 194, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249
        };

        internal static readonly ushort[] HeldItems_GSC = ArrayUtil.ConcatAll(Pouch_Items_GSC, Pouch_Ball_GSC, Pouch_TMHM_GSC);

        internal static readonly byte[] MovePP_GSC =
        {
            00,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 01, 01, 10, 10, 10, 05, 15, 25, 15, 10, 15, 30, 05, 40, 15, 10, 25, 10, 30, 10, 20, 10, 10, 10, 10, 10, 20, 05, 40, 05, 05, 15, 05, 10, 05, 15,
            10, 05, 10, 20, 20, 40, 15, 10, 20, 20, 25, 05, 15, 10, 05, 20, 15, 20, 25, 20, 05, 30, 05, 10, 20, 40, 05, 20, 40, 20, 15, 35, 10, 05, 05, 05, 15, 05, 20, 05, 05, 15, 20, 10, 05, 05, 15, 15, 15, 15,
            10, 00, 00, 00, 00
        };

        internal static readonly int[] TMHM_GSC =
        {
            223, 029, 174, 205, 046, 092, 192, 249, 244, 237,
            241, 230, 173, 059, 063, 196, 182, 240, 202, 203,
            218, 076, 231, 225, 087, 089, 216, 091, 094, 247,
            189, 104, 008, 207, 214, 188, 201, 126, 129, 111,
            009, 138, 197, 156, 213, 168, 211, 007, 210, 171,

            015, 019, 057, 070, 148, 250, 127
        };

        internal static readonly int[] Tutors_GSC = {53, 85, 58}; // Flamethrower, Thunderbolt & Ice Beam
        internal static readonly int[] WildPokeBalls2 = { 4 };

        internal static readonly HashSet<int> FutureEvolutionsGen2 = new HashSet<int>
        {
            424,429,430,461,462,463,464,465,466,467,468,469,470,471,472,473,474,700
        };

        internal static readonly bool[] ReleasedHeldItems_2 = Enumerable.Range(0, MaxItemID_2+1).Select(i => HeldItems_GSC.Contains((ushort)i)).ToArray();

        internal static readonly HashSet<int> TransferSpeciesDefaultAbility_2 = new HashSet<int>
        {
            92, 93, 94, 109, 110, 151, 200, 201, 251,
            // Future Evolutions
            429, // Misdreavus -> Mismagius
        };
    }
}
