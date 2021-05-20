using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> sensitive provider for <see cref="ComboItem"/> data sources.
    /// </summary>
    public sealed class FilteredGameDataSource
    {
        public FilteredGameDataSource(SaveFile sav, GameDataSource source, bool HaX = false)
        {
            Source = source;
            Species = GetFilteredSpecies(sav, source, HaX).ToList();
            Moves = GetFilteredMoves(sav, source, HaX).ToList();
            if (sav.Generation > 1)
            {
                var items = Source.GetItemDataSource(sav.Version, sav.Generation, sav.HeldItems, HaX);
                items.RemoveAll(i => i.Value > sav.MaxItemID);
                Items = items;
            }
            else
            {
                Items = Array.Empty<ComboItem>();
            }

            var gamelist = GameUtil.GetVersionsWithinRange(sav, sav.Generation).ToList();
            Games = Source.VersionDataSource.Where(g => gamelist.Contains((GameVersion)g.Value)).ToList();

            Languages = GameDataSource.LanguageDataSource(sav.Generation);
            Balls = Source.BallDataSource.Where(b => b.Value <= sav.MaxBallID).ToList();
            Abilities = Source.AbilityDataSource.Where(a => a.Value <= sav.MaxAbilityID).ToList();

            G4EncounterTypes = Source.EncounterTypeDataSource;
            Natures = Source.NatureDataSource;
        }

        private static IEnumerable<ComboItem> GetFilteredSpecies(IGameValueLimit sav, GameDataSource source, bool HaX = false)
        {
            if (HaX)
                return source.SpeciesDataSource.Where(s => s.Value <= sav.MaxSpeciesID);

            // Some games cannot acquire every Species that exists. Some can only acquire a subset.
            return sav switch
            {
                SAV7b => source.SpeciesDataSource // LGPE: Kanto 151, Meltan/Melmetal
                    .Where(s => s.Value is <= (int)Core.Species.Mew or (int)Core.Species.Meltan or (int)Core.Species.Melmetal),
                _ => source.SpeciesDataSource.Where(s => s.Value <= sav.MaxSpeciesID)
            };
        }

        private static IEnumerable<ComboItem> GetFilteredMoves(IGameValueLimit sav, GameDataSource source, bool HaX = false)
        {
            if (HaX)
                return source.HaXMoveDataSource.Where(m => m.Value <= sav.MaxMoveID);

            var legal = source.LegalMoveDataSource;
            return sav switch
            {
                SAV7b => legal.Where(s => Legal.AllowedMovesGG.Contains((short) s.Value)), // LGPE: Not all moves are available
                _ => legal.Where(m => m.Value <= sav.MaxMoveID)
            };
        }

        public readonly GameDataSource Source;

        public readonly IReadOnlyList<ComboItem> Moves;
        public readonly IReadOnlyList<ComboItem> Balls;
        public readonly IReadOnlyList<ComboItem> Games;
        public readonly IReadOnlyList<ComboItem> Items;
        public readonly IReadOnlyList<ComboItem> Species;
        public readonly IReadOnlyList<ComboItem> Languages;
        public readonly IReadOnlyList<ComboItem> Abilities;
        public readonly IReadOnlyList<ComboItem> Natures;
        public readonly IReadOnlyList<ComboItem> G4EncounterTypes;
        public readonly IReadOnlyList<ComboItem> ConsoleRegions = GameDataSource.Regions;

        public IReadOnlyList<ComboItem> GetAbilityList(PKM pkm)
        {
            var abilities = pkm.PersonalInfo.Abilities;
            int format = pkm.Format;
            return GetAbilityList(abilities, format);
        }

        public IReadOnlyList<ComboItem> GetAbilityList(IReadOnlyList<int> abilities, int format)
        {
            var count = format == 3 && (abilities[1] == 0 || abilities[1] == abilities[0]) ? 1 : abilities.Count;
            var list = new ComboItem[count];

            var alist = Source.Strings.Ability;
            var suffix = AbilityIndexSuffixes;
            for (int i = 0; i < list.Length; i++)
            {
                var ability = abilities[i];
                list[i] = new ComboItem(alist[ability] + suffix[i], ability);
            }

            return list;
        }

        private static readonly string[] AbilityIndexSuffixes = { " (1)", " (2)", " (H)" };
    }
}
