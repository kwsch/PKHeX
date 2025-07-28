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
        Moves = GetFilteredMoves(sav, sav.Context, source, HaX);
        Relearn = sav is SAV7SM sm
            ? GetFilteredMoves(sm.Context, source, HaX, Legal.MaxMoveID_7_USUM) // allow for US/UM relearn move limits on S/M
            : Moves.ToList();
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

        Languages = Source.LanguageDataSource(sav.Generation);
        Balls = Source.BallDataSource.Where(b => b.Value <= sav.MaxBallID).ToList();
        Abilities = Source.AbilityDataSource.Where(a => a.Value <= sav.MaxAbilityID).ToList();

        G4GroundTiles = Source.GroundTileDataSource;
        ConsoleRegions = Source.Regions;
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

    private static List<ComboItem> GetFilteredMoves(IGameValueLimit limit, EntityContext context, GameDataSource source, bool HaX = false)
    {
        return GetFilteredMoves(context, source, HaX, limit.MaxMoveID);
    }

    // return a new list every time
    private static List<ComboItem> GetFilteredMoves(EntityContext context, GameDataSource source, bool HaX, ushort max)
    {
        if (HaX)
            return source.HaXMoveDataSource.Where(m => m.Value <= max).ToList();

        var legal = source.LegalMoveDataSource;
        if (context is EntityContext.Gen7b)
            return legal.Where(s => MoveInfo7b.IsAllowedMoveGG((ushort)s.Value)).ToList();

        var dummied = MoveInfo.GetDummiedMovesHashSet(context);
        if (dummied.Length == 0 || context is EntityContext.Gen8) // Gen8 indicates dummied via Yellow Triangle
            return legal.Where(m => m.Value <= max).ToList();

        return GetMovesWithoutDummy(legal, max, dummied);
    }

    private static List<ComboItem> GetMovesWithoutDummy(IReadOnlyList<ComboItem> legal, ushort max, ReadOnlySpan<byte> dummied)
    {
        var result = new List<ComboItem>(legal.Count);
        foreach (var item in legal)
        {
            var value = item.Value;
            if (value > max)
                continue;
            if (MoveInfo.IsDummiedMove(dummied, (ushort)value))
                continue;
            result.Add(item);
        }
        return result;
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
    public readonly IReadOnlyList<ComboItem> ConsoleRegions;

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
