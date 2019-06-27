using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> sensitive provider for <see cref="ComboItem"/> data sources.
    /// </summary>
    public class FilteredGameDataSource
    {
        public FilteredGameDataSource(SaveFile sav, GameDataSource source, bool HaX = false)
        {
            Source = source;
            if (sav.Generation > 1)
            {
                var items = Source.GetItemDataSource(sav.Version, sav.Generation, sav.MaxItemID, sav.HeldItems, HaX);
                Items = items.Where(i => i.Value <= sav.MaxItemID).ToList();
            }
            else
            {
                Items = Array.Empty<ComboItem>();
            }

            var gamelist = GameUtil.GetVersionsWithinRange(sav, sav.Generation).ToList();
            Games = Source.VersionDataSource.Where(g => gamelist.Contains((GameVersion)g.Value)).ToList();

            Languages = GameDataSource.LanguageDataSource(sav.Generation);
            Balls = Source.BallDataSource.Where(b => b.Value <= sav.MaxBallID).ToList();
            Species = Source.SpeciesDataSource.Where(s => s.Value <= sav.MaxSpeciesID).ToList();
            Abilities = Source.AbilityDataSource.Where(a => a.Value <= sav.MaxAbilityID).ToList();

            var moves = HaX ? Source.HaXMoveDataSource : Source.LegalMoveDataSource; // Filter Z-Moves if appropriate
            Moves = moves.Where(m => m.Value <= sav.MaxMoveID).ToList();
            G4EncounterTypes = Source.EncounterTypeDataSource;
            Natures = Source.NatureDataSource;
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
            var abils = pkm.PersonalInfo.Abilities;
            int format = pkm.Format;
            return GetAbilityList(abils, format);
        }

        public IReadOnlyList<ComboItem> GetAbilityList(int[] abils, int format)
        {
            var count = format == 3 && (abils[1] == 0 || abils[1] == abils[0]) ? 1 : abils.Length;
            var list = new ComboItem[count];
            for (int i = 0; i < list.Length; i++)
            {
                var ability = abils[i];
                list[i] = new ComboItem(Source.Source.Ability[ability] + abilIdentifier[i], ability);
            }

            return list;
        }

        private static readonly string[] abilIdentifier = { " (1)", " (2)", " (H)" };
    }
}
