using System;
using System.Collections.Generic;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Logic for providing suggested property values with respect to the input data.
/// </summary>
public static class EncounterSuggestion
{
    /// <summary>
    /// Gets an object containing met data properties that might be legal.
    /// </summary>
    public static EncounterSuggestionData? GetSuggestedMetInfo(PKM pk)
    {
        int loc = GetSuggestedTransferLocation(pk);

        if (pk.WasEgg)
            return GetSuggestedEncounterEgg(pk, loc);

        Span<EvoCriteria> chain = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        var origin = new EvolutionOrigin(pk.Species, (byte)pk.Version, (byte)pk.Generation, (byte)pk.CurrentLevel, (byte)pk.CurrentLevel, OriginOptions.SkipChecks);
        var count = EvolutionChain.GetOriginChain(chain, pk, origin);
        var ver = (GameVersion)pk.Version;
        var generator = EncounterGenerator.GetGenerator(ver);

        var evos = chain[..count].ToArray();
        var w = EncounterSelection.GetMinByLevel(evos, generator.GetPossible(pk, evos, ver, EncounterTypeGroup.Slot));
        var s = EncounterSelection.GetMinByLevel(evos, generator.GetPossible(pk, evos, ver, EncounterTypeGroup.Static));

        if (w is null)
            return s is null ? null : GetSuggestedEncounter(pk, s, loc);
        if (s is null)
            return GetSuggestedEncounter(pk, w, loc);

        // Prefer the wild slot; fall back to wild slot if none are exact match.
        if (IsSpeciesFormMatch(chain, w))
            return GetSuggestedEncounter(pk, w, loc);
        if (IsSpeciesFormMatch(chain, s))
            return GetSuggestedEncounter(pk, s, loc);
        return GetSuggestedEncounter(pk, w, loc);
    }

    private static bool IsSpeciesFormMatch<T>(ReadOnlySpan<EvoCriteria> evos, T encounter) where T : ISpeciesForm
    {
        foreach (var evo in evos)
        {
            if (evo.Species == encounter.Species && evo.Form == encounter.Form)
                return true;
        }
        return false;
    }

    private static EncounterSuggestionData GetSuggestedEncounterEgg(PKM pk, int loc = -1)
    {
        int lvl = GetSuggestedEncounterEggMetLevel(pk);
        var met = loc != -1 ? loc : GetSuggestedEggMetLocation(pk);
        return new EncounterSuggestionData(pk, met, (byte)lvl);
    }

    public static int GetSuggestedEncounterEggMetLevel(PKM pk)
    {
        if (pk is { IsNative: false, Generation: < 5 })
            return pk.CurrentLevel; // be generous with transfer conditions
        if (pk.Format < 5) // and native
            return pk.Format == 2 && pk.Met_Location != 0 ? 1 : 0;
        return 1; // Gen5+
    }

    public static int GetSuggestedEncounterEggLocationEgg(PKM pk, bool traded = false)
    {
        return GetSuggestedEncounterEggLocationEgg(pk.Generation, (GameVersion)pk.Version, traded);
    }

    public static int GetSuggestedEncounterEggLocationEgg(int generation, GameVersion version, bool traded = false) => generation switch
    {
        1 or 2 or 3 => 0,
        4 => traded ? Locations.LinkTrade4 : Locations.Daycare4,
        5 => traded ? Locations.LinkTrade5 : Locations.Daycare5,
        8 when BDSP.Contains(version) => traded ? Locations.LinkTrade6NPC : Locations.Daycare8b,
        9 => Locations.Picnic9,
        _ => traded ? Locations.LinkTrade6 : Locations.Daycare5,
    };

    private static EncounterSuggestionData GetSuggestedEncounter(PKM pk, IEncounterable enc, int loc = -1)
    {
        var met = loc != -1 ? loc : enc.Location;
        return new EncounterSuggestionData(pk, enc, met);
    }

    /// <inheritdoc cref="EggStateLegality.GetEggHatchLocation"/>
    public static int GetSuggestedEggMetLocation(PKM pk) => EggStateLegality.GetEggHatchLocation((GameVersion)pk.Version, pk.Format);

    /// <summary>
    /// Gets the correct Transfer Met location for the origin game.
    /// </summary>
    /// <param name="pk">Pokémon data to suggest for</param>
    /// <remarks>
    /// Returns -1 if the met location is not overriden with a transfer location
    /// </remarks>
    public static int GetSuggestedTransferLocation(PKM pk)
    {
        if (pk.Version == (int)GO)
            return Locations.GO8;
        if (pk.HasOriginalMetLocation)
            return -1;
        if (pk.VC1)
            return Locations.Transfer1;
        if (pk.VC2)
            return Locations.Transfer2;
        if (pk.Format == 4) // Pal Park
            return Locations.Transfer3;
        if (pk.Format >= 5) // Transporter
            return PK5.GetTransferMetLocation4(pk);
        return -1;
    }

    public static int GetLowestLevel(PKM pk, byte startLevel)
    {
        if (startLevel >= 100)
            startLevel = 100;

        int most = 1;
        Span<EvoCriteria> chain = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        var origin = new EvolutionOrigin(pk.Species, (byte)pk.Version, (byte)pk.Generation, startLevel, 100, OriginOptions.SkipChecks);
        while (true)
        {
            var count = EvolutionChain.GetOriginChain(chain, pk, origin);
            if (count < most) // lost an evolution, prior level was minimum current level
                return GetMaxLevelMax(chain) + 1;
            most = count;
            if (origin.LevelMax == origin.LevelMin)
                return startLevel;
            origin = origin with { LevelMax = (byte)(origin.LevelMax - 1) };
        }
    }

    private static int GetMaxLevelMax(ReadOnlySpan<EvoCriteria> evos)
    {
        int max = 0;
        foreach (var evo in evos)
            max = Math.Max(evo.LevelMax, max);
        return max;
    }

    /// <summary>
    /// Iterates through all possible levels to drop to the lowest level possible.
    /// </summary>
    /// <param name="pk">Entity to modify</param>
    /// <param name="isLegal">Current state is legal or invalid (false)</param>
    /// <param name="level">Maximum level to iterate down from</param>
    /// <returns>True if the level was changed, false if it was already at the lowest level possible or impossible.</returns>
    public static bool IterateMinimumCurrentLevel(PKM pk, bool isLegal, int level = 100)
    {
        // Find the lowest level possible while still remaining legal.
        var growth = pk.PersonalInfo.EXPGrowth;
        var table = Experience.GetTable(growth);

        var originalEXP = pk.EXP;
        var original = Experience.GetLevel(originalEXP, table);
        if (isLegal)
        {
            // If we can't go any lower, we're already at the lowest level possible.
            if (original == 1)
                return false;

            // Skip to original - 1, since all levels [original,max] are already legal.
            level = original - 1;
        }
        // If it's not legal, then we'll first try the max level and abort if it will never be legal.

        // Find the first level that is illegal via searching downwards, and set it to the level above it.
        while (level != 0)
        {
            pk.EXP = Experience.GetEXP(level, table);
            var la = new LegalityAnalysis(pk);
            var valid = la.Valid;
            if (valid)
            {
                --level;
                continue;
            }

            // First illegal level found, revert to the previous level.
            level = Math.Min(100, level + 1);
            if (level == original) // same, revert actual EXP value.
            {
                pk.EXP = originalEXP;
                return false;
            }

            pk.EXP = Experience.GetEXP(level, table);
            return true;
        }

        // Lowest level possible (1) is deemed as legal per above loop.
        return true;
    }

    /// <summary>
    /// Gets the suggested <see cref="PKM.Met_Level"/> based on a baseline <see cref="minLevel"/> and the <see cref="pk"/>'s current moves.
    /// </summary>
    /// <param name="pk">Entity to calculate for</param>
    /// <param name="minLevel">Encounter minimum level to calculate for</param>
    /// <returns>Minimum level the <see cref="pk"/> can have for its <see cref="PKM.Met_Level"/></returns>
    /// <remarks>Brute-forces the value by cloning the <see cref="pk"/> and adjusting the <see cref="PKM.Met_Level"/> and returning the lowest valid value.</remarks>
    public static int GetSuggestedMetLevel(PKM pk, int minLevel)
    {
        var clone = pk.Clone();
        int minMove = -1;
        for (int i = clone.CurrentLevel; i >= minLevel; i--)
        {
            clone.Met_Level = i;
            var la = new LegalityAnalysis(clone);
            if (la.Valid)
                return i;
            var moves = la.Info.Moves;
            if (MoveResult.AllValid(moves))
                minMove = i;
        }
        return Math.Max(minMove, minLevel);
    }
}

internal static class EncounterSelection
{
    internal static T? GetMinByLevel<T>(ReadOnlySpan<EvoCriteria> chain, IEnumerable<T> possible) where T : class, IEncounterTemplate
    {
        // MinBy grading: prefer species-form match, select lowest min level encounter.
        // Minimum allocation :)
        T? result = null;
        int min = int.MaxValue;

        foreach (var enc in possible)
        {
            int m = int.MaxValue;
            foreach (var evo in chain)
            {
                bool specDiff = enc.Species != evo.Species || enc.Form != evo.Form;
                var val = (Convert.ToInt32(specDiff) << 16) | enc.LevelMin;
                if (val < m)
                    m = val;
            }

            if (m >= min)
                continue;
            min = m;
            result = enc;
        }

        return result;
    }
}
