using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Bundles raw string inputs into lists that can be used in data binding.
    /// </summary>
    public sealed class GameDataSource
    {
        public static readonly IReadOnlyList<ComboItem> Regions = Util.GetCSVUnsortedCBList("regions3ds");
        public static readonly IReadOnlyList<ComboItem> LanguageList = Util.GetCSVUnsortedCBList("languages");

        // ignores Poke/Great/Ultra
        private static readonly ushort[] ball_nums = { 007, 576, 013, 492, 497, 014, 495, 493, 496, 494, 011, 498, 008, 006, 012, 015, 009, 005, 499, 010, 001, 016, 851 };
        private static readonly byte[] ball_vals = { 007, 025, 013, 017, 022, 014, 020, 018, 021, 019, 011, 023, 008, 006, 012, 015, 009, 005, 024, 010, 001, 016, 026 };
        private static readonly byte[] Gen4EncounterTypes = { 0, 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };

        public GameDataSource(GameStrings s)
        {
            Source = s;
            BallDataSource = Util.GetVariedCBListBall(s.itemlist, ball_nums, ball_vals);
            SpeciesDataSource = Util.GetCBList(s.specieslist);
            NatureDataSource = Util.GetCBList(s.natures);
            AbilityDataSource = Util.GetCBList(s.abilitylist);
            EncounterTypeDataSource = Util.GetUnsortedCBList(s.encountertypelist, Gen4EncounterTypes);

            HaXMoveDataSource = Util.GetCBList(s.movelist);
            LegalMoveDataSource = HaXMoveDataSource.Where(m => !Legal.Z_Moves.Contains(m.Value)).ToList();

            VersionDataSource = GetVersionList(s);

            MetGen2 = CreateGen2(s);
            MetGen3 = CreateGen3(s);
            MetGen3CXD = CreateGen3CXD(s);
            MetGen4 = CreateGen4(s);
            MetGen5 = CreateGen5(s);
            MetGen6 = CreateGen6(s);
            MetGen7 = CreateGen7(s);
            MetGen7GG = CreateGen7GG(s);
            MetGen8 = CreateGen8(s);

            Empty = new ComboItem(s.Species[0], 0);
        }

        public readonly GameStrings Source;

        public readonly IReadOnlyList<ComboItem> SpeciesDataSource;
        public readonly IReadOnlyList<ComboItem> BallDataSource;
        public readonly IReadOnlyList<ComboItem> NatureDataSource;
        public readonly IReadOnlyList<ComboItem> AbilityDataSource;
        public readonly IReadOnlyList<ComboItem> VersionDataSource;
        public readonly IReadOnlyList<ComboItem> LegalMoveDataSource;
        public readonly IReadOnlyList<ComboItem> HaXMoveDataSource;
        public readonly IReadOnlyList<ComboItem> EncounterTypeDataSource;

        private readonly IReadOnlyList<ComboItem> MetGen2;
        private readonly IReadOnlyList<ComboItem> MetGen3;
        private readonly IReadOnlyList<ComboItem> MetGen3CXD;
        private readonly IReadOnlyList<ComboItem> MetGen4;
        private readonly IReadOnlyList<ComboItem> MetGen5;
        private readonly IReadOnlyList<ComboItem> MetGen6;
        private readonly IReadOnlyList<ComboItem> MetGen7;
        private readonly IReadOnlyList<ComboItem> MetGen7GG;
        private readonly IReadOnlyList<ComboItem> MetGen8;

        public readonly ComboItem Empty;

        private static IReadOnlyList<ComboItem> GetVersionList(GameStrings s)
        {
            var list = s.gamelist;
            var games = new byte[]
            {
                44, 45, // 8 swsh
                42, 43, // 7 gg
                30, 31, // 7 sm
                32, 33, // 7 usum
                24, 25, // 6 xy
                27, 26, // 6 oras
                21, 20, // 5 bw
                23, 22, // 5 b2w2
                10, 11, 12, // 4 dppt
                07, 08, // 4 hgss
                02, 01, 03, // 3 rse
                04, 05, // 3 frlg
                15,     // 3 cxd

                39, 40, 41, // 7vc2
                35, 36, 37, 38, // 7vc1
                34, // 7go
            };

            return Util.GetUnsortedCBList(list, games);
        }

        private static List<ComboItem> CreateGen2(GameStrings s)
        {
            var locations = Util.GetCBList(s.metGSC_00000, Enumerable.Range(0, 0x5F).ToArray());
            Util.AddCBWithOffset(locations, s.metGSC_00000, 00000, 0x7E, 0x7F);
            return locations;
        }

        private static List<ComboItem> CreateGen3(GameStrings s)
        {
            var locations = Util.GetCBList(s.metRSEFRLG_00000, Enumerable.Range(0, 213).ToArray());
            Util.AddCBWithOffset(locations, s.metRSEFRLG_00000, 00000, 253, 254, 255);
            return locations;
        }

        private static List<ComboItem> CreateGen3CXD(GameStrings s)
        {
            return Util.GetCBList(s.metCXD_00000, Enumerable.Range(0, s.metCXD_00000.Length).ToArray()).Where(c => c.Text.Length > 0).ToList();
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

        public List<ComboItem> GetItemDataSource(GameVersion game, int generation, IReadOnlyList<ushort> allowed, bool HaX = false)
        {
            var items = Source.GetItemStrings(generation, game);
            return HaX ? Util.GetCBList(items) : Util.GetCBList(items, allowed);
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
                R or S   when currentGen == 3 => Partition1(MetGen3, z => z is <= 87), // Ferry
                E        when currentGen == 3 => Partition1(MetGen3, z => z is <= 87 or >= 197 and <= 212), // Trainer Hill
                FR or LG when currentGen == 3 => Partition1(MetGen3, z => z is > 87 and < 197), // Celadon Dept.
                D or P   when currentGen == 4 => Partition2(MetGen4, z => z is <= 111, 4), // Battle Park
                Pt       when currentGen == 4 => Partition2(MetGen4, z => z is <= 125, 4), // Rock Peak Ruins
                HG or SS when currentGen == 4 => Partition2(MetGen4, z => z is > 125 and < 234, 4), // Celadon Dept.

                B  or W  => MetGen5,
                B2 or W2 => Partition2(MetGen5, z => z is <= 116), // Abyssal Ruins
                X  or Y  => Partition2(MetGen6, z => z is <= 168), // Unknown Dungeon
                OR or AS => Partition2(MetGen6, z => z is > 168 and <= 354), // Secret Base
                SN or MN => Partition2(MetGen7, z => z is < 200), // Outer Cape
                US or UM
                   or RD or BU or GN or YW
                   or GD or SV or C => Partition2(MetGen7, z => z < 234), // Dividing Peak Tunnel
                GP or GE or GO => Partition2(MetGen7GG, z => z <= 54), // Pokémon League
                SW or SH => Partition2(MetGen8, z => z < 400),
                _ => GetLocationListModified(version, currentGen),
            };

            static IReadOnlyList<ComboItem> Partition1(IEnumerable<ComboItem> list, Func<int, bool> criteria)
            {
                return list.OrderByDescending(loc => criteria(loc.Value)).ToList();
            }

            static IReadOnlyList<ComboItem> Partition2(IReadOnlyList<ComboItem> list, Func<int, bool> criteria, int keepFirst = 3)
            {
                return list.Take(keepFirst).Concat(list.Skip(keepFirst).OrderByDescending(loc => criteria(loc.Value))).ToList();
            }
        }

        /// <summary>
        /// Fetches a Met Location list for a <see cref="version"/> that has been transferred away from and overwritten.
        /// </summary>
        /// <param name="version">Origin version</param>
        /// <param name="currentGen">Current save file generation</param>
        /// <returns>Met location list</returns>
        private IReadOnlyList<ComboItem> GetLocationListModified(GameVersion version, int currentGen)
        {
            if (version <= CXD && currentGen == 4)
            {
                return MetGen4.Where(loc => loc.Value == Locations.Transfer3) // Pal Park to front
                    .Concat(MetGen4.Take(4))
                    .Concat(MetGen4.Skip(4).Where(loc => loc.Value != Locations.Transfer3)).ToList();
            }

            if (version < X && currentGen >= 5) // PokéTransfer to front
            {
                return MetGen5.Where(loc => loc.Value == Locations.Transfer4)
                    .Concat(MetGen5.Take(3))
                    .Concat(MetGen5.Skip(3).Where(loc => loc.Value != Locations.Transfer4)).ToList();
            }

            return Array.Empty<ComboItem>();
        }

        public static IReadOnlyList<ComboItem> LanguageDataSource(int gen)
        {
            var languages = LanguageList.ToList();
            if (gen == 3)
                languages.RemoveAll(l => l.Value >= (int)LanguageID.Korean);
            else if (gen < 7)
                languages.RemoveAll(l => l.Value > (int)LanguageID.Korean);
            return languages;
        }
    }
}
