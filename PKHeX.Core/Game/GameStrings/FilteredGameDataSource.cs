using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

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
        Relearn = GetFilteredMoves(sav.BlankPKM, source, HaX).ToList(); // allow for US/UM relearn move limits on S/M
        if (sav.Generation > 1)
        {
            var items = Source.GetItemDataSource(sav.Version, sav.Context, sav.HeldItems, HaX);
            items.RemoveAll(i => i.Value > sav.MaxItemID);
            Items = items;
        }
        else
        {
            Items = [];
        }

        var gamelist = GameUtil.GetVersionsWithinRange(sav, sav.Generation).ToList();
        Games = Source.VersionDataSource.Where(g => gamelist.Contains((GameVersion)g.Value) || g.Value == 0).ToList();

        Languages = GameDataSource.LanguageDataSource(sav.Generation);
        Balls = Source.BallDataSource.Where(b => b.Value <= sav.MaxBallID).ToList();
        Abilities = Source.AbilityDataSource.Where(a => a.Value <= sav.MaxAbilityID).ToList();

        G4GroundTiles = Source.GroundTileDataSource;
        Natures = Source.NatureDataSource;
    }

    private static IEnumerable<ComboItem> GetFilteredSpecies(IGameValueLimit sav, GameDataSource source, bool HaX = false)
    {
        var all = source.SpeciesDataSource;
        if (HaX)
            return FilterAbove(all, sav.MaxSpeciesID);

        // Some games cannot acquire every Species that exists. Some can only acquire a subset.
        return sav switch
        {
            SAV7b gg => FilterUnavailable(all, gg.Personal),
            SAV8LA la => FilterUnavailable(all, la.Personal),
#if !DEBUG // Mainline games can be useful to show all for testing. Only filter out unavailable species in release builds.
            SAV8SWSH swsh => FilterUnavailable(all, swsh.Personal),
            SAV9SV sv => FilterUnavailable(all, sv.Personal),
#endif
            _ => FilterAbove(all, sav.MaxSpeciesID),
        };

        static IEnumerable<ComboItem> FilterAbove(IReadOnlyList<ComboItem> species, int limit)
            => species.Where(s => s.Value <= limit);

        static IEnumerable<ComboItem> FilterUnavailable<T>(IReadOnlyList<ComboItem> source, T table) where T : IPersonalTable
            => source.Where(s => table.IsSpeciesInGame((ushort)s.Value));
    }

    private static IEnumerable<ComboItem> GetFilteredMoves(IGameValueLimit limit, GameDataSource source, bool HaX = false)
    {
        if (HaX)
            return source.HaXMoveDataSource.Where(m => m.Value <= limit.MaxMoveID);

        var legal = source.LegalMoveDataSource;
        return limit switch
        {
            // LGPE: Not all moves are available
            SAV7b or PB7 => legal.Where(s => MoveInfo7b.IsAllowedMoveGG((ushort)s.Value)),
            _ => legal.Where(m => m.Value <= limit.MaxMoveID),
        };
    }

    public readonly GameDataSource Source;

    public readonly IReadOnlyList<ComboItem> Moves;
    public readonly IReadOnlyList<ComboItem> Relearn;
    public readonly IReadOnlyList<ComboItem> Balls;
    public readonly IReadOnlyList<ComboItem> Games;
    public readonly IReadOnlyList<ComboItem> Items;
    public readonly IReadOnlyList<ComboItem> Species;
    public readonly IReadOnlyList<ComboItem> Languages;
    public readonly IReadOnlyList<ComboItem> Abilities;
    public readonly IReadOnlyList<ComboItem> Natures;
    public readonly IReadOnlyList<ComboItem> G4GroundTiles;
    public readonly IReadOnlyList<ComboItem> ConsoleRegions = GameDataSource.Regions;

    private const char HiddenAbilitySuffix = 'H';
    private const char AbilityIndexSuffix = '1';

    public IReadOnlyList<ComboItem> GetAbilityList(PKM pk) => GetAbilityList(pk.PersonalInfo);

    public IReadOnlyList<ComboItem> GetAbilityList(IPersonalAbility pi)
    {
        var list = new ComboItem[pi.AbilityCount];
        LoadAbilityList(pi, list, Source.Strings.abilitylist);
        return list;
    }

    private static void LoadAbilityList(IPersonalAbility pi, Span<ComboItem> list, ReadOnlySpan<string> names)
    {
        for (int i = 0; i < list.Length; i++)
        {
            var value = pi.GetAbilityAtIndex(i);
            var name = names[value];
            char suffix = i == 2 ? HiddenAbilitySuffix : (char)(AbilityIndexSuffix + i);
            var display = $"{name} ({suffix})";
            list[i] = new ComboItem(display, value);
        }
    }
}
