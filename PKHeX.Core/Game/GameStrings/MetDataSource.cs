using System;
using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Cached copies of Met Location lists
    /// </summary>
    public sealed class MetDataSource
    {
        private readonly List<ComboItem> MetGen2;
        private readonly List<ComboItem> MetGen3;
        private readonly List<ComboItem> MetGen3CXD;
        private readonly List<ComboItem> MetGen4;
        private readonly List<ComboItem> MetGen5;
        private readonly List<ComboItem> MetGen6;
        private readonly List<ComboItem> MetGen7;
        private readonly List<ComboItem> MetGen7GG;
        private readonly List<ComboItem> MetGen8;

        private IReadOnlyList<ComboItem>? MetGen4Transfer;
        private IReadOnlyList<ComboItem>? MetGen5Transfer;

        public MetDataSource(GameStrings s)
        {
            MetGen2 = CreateGen2(s);
            MetGen3 = CreateGen3(s);
            MetGen3CXD = CreateGen3CXD(s);
            MetGen4 = CreateGen4(s);
            MetGen5 = CreateGen5(s);
            MetGen6 = CreateGen6(s);
            MetGen7 = CreateGen7(s);
            MetGen7GG = CreateGen7GG(s);
            MetGen8 = CreateGen8(s);
        }

        private static List<ComboItem> CreateGen2(GameStrings s)
        {
            var locations = Util.GetCBList(s.metGSC_00000.AsSpan(0, 0x5F));
            Util.AddCBWithOffset(locations, s.metGSC_00000.AsSpan(0x7E, 2), 0x7E);
            return locations;
        }

        private static List<ComboItem> CreateGen3(GameStrings s)
        {
            var locations = Util.GetCBList(s.metRSEFRLG_00000.AsSpan(0, 213));
            Util.AddCBWithOffset(locations, s.metRSEFRLG_00000.AsSpan(253, 3), 253);
            return locations;
        }

        private static List<ComboItem> CreateGen3CXD(GameStrings s)
        {
            var list = Util.GetCBList(s.metCXD_00000);
            list.RemoveAll(z => z.Text.Length == 0);
            return list;
        }

        private static List<ComboItem> CreateGen4(GameStrings s)
        {
            var locations = Util.GetCBList(s.metHGSS_00000, 0);
            Util.AddCBWithOffset(locations, s.metHGSS_02000, 2000, Locations.Daycare4);
            Util.AddCBWithOffset(locations, s.metHGSS_02000, 2000, Locations.LinkTrade4);
            Util.AddCBWithOffset(locations, s.metHGSS_03000, 3000, Locations.Ranger4);
            Util.AddCBWithOffset(locations, s.metHGSS_00000, 0000, Legal.Met_HGSS_0);
            Util.AddCBWithOffset(locations, s.metHGSS_02000, 2000, Legal.Met_HGSS_2);
            Util.AddCBWithOffset(locations, s.metHGSS_03000, 3000, Legal.Met_HGSS_3);
            return locations;
        }

        private IReadOnlyList<ComboItem> CreateGen4Transfer()
        {
            // Pal Park to front
            var met = MetGen4.ToArray();
            var index = Array.FindIndex(met, z => z.Value == Locations.Transfer3);
            var pal = met[index];
            Array.Copy(met, 0, met, 1, index);
            met[0] = pal;
            return met;
        }

        private static List<ComboItem> CreateGen5(GameStrings s)
        {
            var locations = Util.GetCBList(s.metBW2_00000, 0);
            Util.AddCBWithOffset(locations, s.metBW2_60000, 60001, Locations.Daycare5);
            Util.AddCBWithOffset(locations, s.metBW2_30000, 30001, Locations.LinkTrade5);
            Util.AddCBWithOffset(locations, s.metBW2_00000, 00000, Legal.Met_BW2_0);
            Util.AddCBWithOffset(locations, s.metBW2_30000, 30001, Legal.Met_BW2_3);
            Util.AddCBWithOffset(locations, s.metBW2_40000, 40001, Legal.Met_BW2_4);
            Util.AddCBWithOffset(locations, s.metBW2_60000, 60001, Legal.Met_BW2_6);
            return locations;
        }

        private IReadOnlyList<ComboItem> CreateGen5Transfer()
        {
            // PokéTransfer to front
            var met = MetGen5.ToArray();
            var index = Array.FindIndex(met, z => z.Value == Locations.Transfer4);
            var xfr = met[index];
            Array.Copy(met, 0, met, 1, index);
            met[0] = xfr;
            return met;
        }

        private static List<ComboItem> CreateGen6(GameStrings s)
        {
            var locations = Util.GetCBList(s.metXY_00000, 0);
            Util.AddCBWithOffset(locations, s.metXY_60000, 60001, Locations.Daycare5);
            Util.AddCBWithOffset(locations, s.metXY_30000, 30001, Locations.LinkTrade6);
            Util.AddCBWithOffset(locations, s.metXY_00000, 00000, Legal.Met_XY_0);
            Util.AddCBWithOffset(locations, s.metXY_30000, 30001, Legal.Met_XY_3);
            Util.AddCBWithOffset(locations, s.metXY_40000, 40001, Legal.Met_XY_4);
            Util.AddCBWithOffset(locations, s.metXY_60000, 60001, Legal.Met_XY_6);
            return locations;
        }

        private static List<ComboItem> CreateGen7(GameStrings s)
        {
            var locations = Util.GetCBList(s.metSM_00000, 0);
            Util.AddCBWithOffset(locations, s.metSM_60000, 60001, Locations.Daycare5);
            Util.AddCBWithOffset(locations, s.metSM_30000, 30001, Locations.LinkTrade6);
            Util.AddCBWithOffset(locations, s.metSM_00000, 00000, Legal.Met_SM_0);
            Util.AddCBWithOffset(locations, s.metSM_30000, 30001, Legal.Met_SM_3);
            Util.AddCBWithOffset(locations, s.metSM_40000, 40001, Legal.Met_SM_4);
            Util.AddCBWithOffset(locations, s.metSM_60000, 60001, Legal.Met_SM_6);
            return locations;
        }

        private static List<ComboItem> CreateGen7GG(GameStrings s)
        {
            var locations = Util.GetCBList(s.metGG_00000, 0);
            Util.AddCBWithOffset(locations, s.metGG_60000, 60001, 60002);
            Util.AddCBWithOffset(locations, s.metGG_30000, 30001, Locations.LinkTrade6);
            Util.AddCBWithOffset(locations, s.metGG_00000, 00000, Legal.Met_GG_0);
            Util.AddCBWithOffset(locations, s.metGG_30000, 30001, Legal.Met_GG_3);
            Util.AddCBWithOffset(locations, s.metGG_40000, 40001, Legal.Met_GG_4);
            Util.AddCBWithOffset(locations, s.metGG_60000, 60001, Legal.Met_GG_6);
            return locations;
        }

        private static List<ComboItem> CreateGen8(GameStrings s)
        {
            var locations = Util.GetCBList(s.metSWSH_00000, 0);
            Util.AddCBWithOffset(locations, s.metSWSH_60000, 60001, 60002);
            Util.AddCBWithOffset(locations, s.metSWSH_30000, 30001, Locations.LinkTrade6);
            Util.AddCBWithOffset(locations, s.metSWSH_00000, 00000, Legal.Met_SWSH_0);
            Util.AddCBWithOffset(locations, s.metSWSH_30000, 30001, Legal.Met_SWSH_3);
            Util.AddCBWithOffset(locations, s.metSWSH_40000, 40001, Legal.Met_SWSH_4);
            Util.AddCBWithOffset(locations, s.metSWSH_60000, 60001, Legal.Met_SWSH_6);
            return locations;
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current save file generation</param>
        /// <param name="egg">True if an egg location list, false if a regular met location list</param>
        /// <returns>Met location list</returns>
        public IReadOnlyList<ComboItem> GetLocationList(GameVersion version, int currentGen, bool egg = false)
        {
            if (currentGen == 2)
                return MetGen2;

            if (egg && version < W && currentGen >= 5)
                return MetGen4;

            return version switch
            {
                CXD      when currentGen == 3 => MetGen3CXD,
                R or S   when currentGen == 3 => Partition1(MetGen3, z => z    <= 87), // Ferry
                E        when currentGen == 3 => Partition1(MetGen3, z => z is <= 87 or >= 197 and <= 212), // Trainer Hill
                FR or LG when currentGen == 3 => Partition1(MetGen3, z => z is > 87 and < 197), // Celadon Dept.
                D or P   when currentGen == 4 => Partition2(MetGen4, z => z    <= 111, 4), // Battle Park
                Pt       when currentGen == 4 => Partition2(MetGen4, z => z    <= 125, 4), // Rock Peak Ruins
                HG or SS when currentGen == 4 => Partition2(MetGen4, z => z is > 125 and < 234, 4), // Celadon Dept.

                B  or W  => MetGen5,
                B2 or W2 => Partition2(MetGen5, z => z    <= 116), // Abyssal Ruins
                X  or Y  => Partition2(MetGen6, z => z    <= 168), // Unknown Dungeon
                OR or AS => Partition2(MetGen6, z => z is > 168 and <= 354), // Secret Base
                SN or MN => Partition2(MetGen7, z => z    < 200), // Outer Cape
                US or UM
                   or RD or BU or GN or YW
                   or GD or SV or C => Partition2(MetGen7, z => z < 234), // Dividing Peak Tunnel
                GP or GE or GO => Partition2(MetGen7GG, z => z <= 54), // Pokémon League
                SW or SH => Partition2(MetGen8, z => z < 400),
                _ => GetLocationListModified(version, currentGen),
            };

            static IReadOnlyList<ComboItem> Partition1(IReadOnlyList<ComboItem> list, Func<int, bool> criteria)
            {
                var result = new ComboItem[list.Count];
                return GetOrderedList(list, result, criteria);
            }

            static IReadOnlyList<ComboItem> GetOrderedList(IReadOnlyList<ComboItem> list, ComboItem[] result, Func<int, bool> criteria, int start = 0)
            {
                // store values that match criteria at the next available position of the array
                // store non-matches starting at the end. reverse before returning
                int end = list.Count - 1;
                for (var index = start; index < list.Count; index++)
                {
                    var item = list[index];
                    if (criteria(item.Value))
                        result[start++] = item;
                    else
                        result[end--] = item;
                }
                // since the non-matches are reversed in order, we swap them back since we know where they end up at.
                Array.Reverse(result, start, list.Count - start);
                return result;
            }

            static IReadOnlyList<ComboItem> Partition2(IReadOnlyList<ComboItem> list, Func<int, bool> criteria, int keepFirst = 3)
            {
                var result = new ComboItem[list.Count];
                for (int i = 0; i < keepFirst; i++)
                    result[i] = list[i];
                return GetOrderedList(list, result, criteria, keepFirst);
            }
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current save file generation</param>
        /// <returns>Met location list</returns>
        private IReadOnlyList<ComboItem> GetLocationListModified(GameVersion version, int currentGen) => version switch
        {
            <= CXD when currentGen == 4 => MetGen4Transfer ??= CreateGen4Transfer(),
            < X when currentGen >= 5 => MetGen5Transfer ??= CreateGen5Transfer(),
            _ => Array.Empty<ComboItem>()
        };
    }
}
