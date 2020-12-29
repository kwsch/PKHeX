using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Bundles raw string inputs into lists that can be used in data binding.
    /// </summary>
    public sealed class GameDataSource
    {
        public static readonly IReadOnlyList<ComboItem> Regions = Util.GetCSVUnsortedCBList("regions3ds");
        private static readonly List<ComboItem> LanguageList = Util.GetCSVUnsortedCBList("languages");

        // ignores Poke/Great/Ultra
        private static readonly ushort[] ball_nums = { 007, 576, 013, 492, 497, 014, 495, 493, 496, 494, 011, 498, 008, 006, 012, 015, 009, 005, 499, 010, 001, 016, 851 };
        private static readonly byte[] ball_vals = { 007, 025, 013, 017, 022, 014, 020, 018, 021, 019, 011, 023, 008, 006, 012, 015, 009, 005, 024, 010, 001, 016, 026 };
        private static readonly byte[] Gen4EncounterTypes = { 0, 1, 2, 4, 5, 7, 9, 10, 12, 23, 24 };

        public GameDataSource(GameStrings s)
        {
            Strings = s;
            BallDataSource = Util.GetVariedCBListBall(s.itemlist, ball_nums, ball_vals);
            SpeciesDataSource = Util.GetCBList(s.specieslist);
            NatureDataSource = Util.GetCBList(s.natures);
            AbilityDataSource = Util.GetCBList(s.abilitylist);
            EncounterTypeDataSource = Util.GetUnsortedCBList(s.encountertypelist, Gen4EncounterTypes);

            var moves = Util.GetCBList(s.movelist);
            HaXMoveDataSource = moves;
            var legal = new List<ComboItem>(moves);
            legal.RemoveAll(m => Legal.Z_Moves.Contains(m.Value));
            LegalMoveDataSource = legal;

            VersionDataSource = GetVersionList(s);

            Met = new MetDataSource(s);

            Empty = new ComboItem(s.Species[0], 0);
        }

        /// <summary> Strings that this object's lists were generated with. </summary>
        public readonly GameStrings Strings;

        /// <summary> Contains Met Data lists to source lists from. </summary>
        public readonly MetDataSource Met;

        /// <summary> Represents "(None)", localized to this object's language strings. </summary>
        public readonly ComboItem Empty;

        public readonly IReadOnlyList<ComboItem> SpeciesDataSource;
        public readonly IReadOnlyList<ComboItem> BallDataSource;
        public readonly IReadOnlyList<ComboItem> NatureDataSource;
        public readonly IReadOnlyList<ComboItem> AbilityDataSource;
        public readonly IReadOnlyList<ComboItem> VersionDataSource;
        public readonly IReadOnlyList<ComboItem> LegalMoveDataSource;
        public readonly IReadOnlyList<ComboItem> HaXMoveDataSource;
        public readonly IReadOnlyList<ComboItem> EncounterTypeDataSource;

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

        public List<ComboItem> GetItemDataSource(GameVersion game, int generation, IReadOnlyList<ushort> allowed, bool HaX = false)
        {
            var items = Strings.GetItemStrings(generation, game);
            return HaX ? Util.GetCBList(items) : Util.GetCBList(items, allowed);
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
