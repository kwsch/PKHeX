using System;
using System.Linq;

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

        var chain = EvolutionChain.GetValidPreEvolutions(pk, maxLevel: 100, skipChecks: true);
        var w = EncounterSlotGenerator.GetCaptureLocation(pk, chain);
        var s = EncounterStaticGenerator.GetStaticLocation(pk, chain);
        if (w is null)
            return s is null ? null : GetSuggestedEncounter(pk, s, loc);
        if (s is null)
            return GetSuggestedEncounter(pk, w, loc);

        bool isDefinitelySlot = chain.Any(z => z.Species == w.Species && z.Form == w.Form);
        bool isDefinitelyStatic = chain.Any(z => z.Species == s.Species && z.Form == s.Form);
        IEncounterable obj = (isDefinitelySlot || !isDefinitelyStatic) ? w : s;
        return GetSuggestedEncounter(pk, obj, loc);
    }

    private static EncounterSuggestionData GetSuggestedEncounterEgg(PKM pk, int loc = -1)
    {
        int lvl = GetSuggestedEncounterEggMetLevel(pk);
        var met = loc != -1 ? loc : GetSuggestedEggMetLocation(pk);
        return new EncounterSuggestionData(pk, met, (byte)lvl);
    }

    public static int GetSuggestedEncounterEggMetLevel(PKM pk)
    {
        if (!pk.IsNative && pk.Generation < 5)
            return pk.CurrentLevel; // be generous with transfer conditions
        if (pk.Format < 5) // and native
            return pk.Format == 2 && pk.Met_Location != 0 ? 1 : 0;
        return 1; // gen5+
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
        8 when BDSP.Contains(version)=> traded ? Locations.LinkTrade6NPC : Locations.Daycare8b,
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
            return Legal.GetTransfer45MetLocation(pk);
        return -1;
    }

    public static int GetLowestLevel(PKM pk, byte startLevel)
    {
        if (startLevel >= 100)
            startLevel = 100;

        var table = EvolutionTree.GetEvolutionTree(pk.Context);
        int count = 1;
        for (byte i = 100; i >= startLevel; i--)
        {
            var evos = table.GetValidPreEvolutions(pk, levelMax: i, skipChecks: true, levelMin: startLevel);
            if (evos.Length < count) // lost an evolution, prior level was minimum current level
                return evos.Max(evo => evo.LevelMax) + 1;
            count = evos.Length;
        }
        return startLevel;
    }

    public static bool IterateMinimumCurrentLevel(PKM pk, bool isLegal, int max = 100)
    {
        var original = pk.CurrentLevel;
        var originalEXP = pk.EXP;
        if (isLegal)
        {
            if (original == 1)
                return false;
            max = original - 1;
        }

        for (int i = max; i != 0; i--)
        {
            pk.CurrentLevel = i;
            var la = new LegalityAnalysis(pk);
            var valid = la.Valid;
            if (valid)
                continue;

            var revert = Math.Min(100, i + 1);
            if (revert == original)
            {
                pk.EXP = originalEXP;
                return false;
            }

            pk.CurrentLevel = revert;
            return true;
        }
        return true; // 1
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
