using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_8 = 809;
        internal const int MaxMoveID_8 = 719;
        internal const int MaxItemID_8 = 920;
        internal const int MaxAbilityID_8 = 232;
        internal const int MaxBallID_8 = 0x1A; // 26
        internal const int MaxGameID_8 = 45;

        #region Met Locations

        internal static readonly int[] Met_SWSH_0 =
        {
        };

        internal static readonly int[] Met_SWSH_3 =
        {
        };

        internal static readonly int[] Met_SWSH_4 =
        {
        };

        internal static readonly int[] Met_SWSH_6 =
        {
        };

        public const int StandardHatchLocation8 = 50; // todo

        #endregion

        internal static readonly ushort[] Pouch_Regular_SWSH = // 00
        {
        };

        internal static readonly ushort[] Pouch_Ball_SWSH = { // 08
        };

        internal static readonly ushort[] Pouch_Battle_SWSH = { // 16
        };

        internal static readonly ushort[] Pouch_Items_SWSH = Pouch_Regular_SWSH.Concat(Pouch_Ball_SWSH).Concat(Pouch_Battle_SWSH).ToArray();

        internal static readonly ushort[] Pouch_Key_SWSH = {
        };

        internal static readonly ushort[] Pouch_TMHM_SWSH = { // 02
        };

        internal static readonly ushort[] Pouch_Medicine_SWSH = { // 32
        };

        internal static readonly ushort[] Pouch_Berries_SWSH = {
        };

        internal static readonly ushort[] HeldItems_SWSH = new ushort[1].Concat(Pouch_Items_SWSH).Concat(Pouch_Berries_SWSH).Concat(Pouch_Medicine_SWSH).ToArray();

        internal static readonly HashSet<int> WildPokeballs8 = new HashSet<int> {
            (int)Ball.Poke,
            (int)Ball.Great,
            (int)Ball.Ultra,
            (int)Ball.Master,
            (int)Ball.Net,
            (int)Ball.Dive,
            (int)Ball.Nest,
            (int)Ball.Repeat,
            (int)Ball.Timer,
            (int)Ball.Luxury,
            (int)Ball.Premier,
            (int)Ball.Dusk,
            (int)Ball.Heal,
            (int)Ball.Quick,
        };

        internal static readonly HashSet<int> GalarOriginForms = new HashSet<int>
        {
        };

        internal static readonly HashSet<int> GalarVariantFormEvolutions = new HashSet<int>
        {
        };

        internal static readonly HashSet<int> EvolveToGalarForms = new HashSet<int>(GalarVariantFormEvolutions.Concat(GalarOriginForms));

        internal static readonly int[] EggLocations8 = {Locations.Daycare5, Locations.LinkTrade6};

        internal static readonly HashSet<int> ValidMet_SWSH = new HashSet<int>
        {
        };

        internal static readonly int[] TMHM_SWSH =
        {
        };

        internal static readonly byte[] MovePP_SWSH =
        {
        };

        internal static readonly HashSet<int> Ban_NoHidden8 = new HashSet<int>
        {
        };

        #region Unreleased Items
        internal static readonly HashSet<int> UnreleasedHeldItems_8 = new HashSet<int>
        {
        };
        #endregion
        internal static readonly bool[] ReleasedHeldItems_8 = Enumerable.Range(0, MaxItemID_8+1).Select(i => HeldItems_SWSH.Contains((ushort)i) && !UnreleasedHeldItems_8.Contains(i)).ToArray();
    }
}
