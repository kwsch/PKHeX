using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Contains logic that calculates the evolution chain of a <see cref="PKM"/>, only considering the generation it originated in.
/// </summary>
public static class EncounterOrigin
{
    /// <summary>
    /// Gets possible evolution details for the input <see cref="pk"/>
    /// </summary>
    /// <param name="pk">Current state of the Pokémon</param>
    /// <returns>Possible origin species-form-levels to match against encounter data.</returns>
    /// <remarks>Use <see cref="GetOriginChain12"/> if the <see cref="pk"/> originated from Generation 1 or 2.</remarks>
    public static EvoCriteria[] GetOriginChain(PKM pk)
    {
        bool hasOriginMet = pk.HasOriginalMetLocation;
        var maxLevel = GetLevelOriginMax(pk, hasOriginMet);
        var minLevel = GetLevelOriginMin(pk, hasOriginMet);
        return GetOriginChain(pk, -1, (byte)maxLevel, (byte)minLevel, hasOriginMet);
    }

    /// <summary>
    /// Gets possible evolution details for the input <see cref="pk"/> originating from Generation 1 or 2.
    /// </summary>
    /// <param name="pk">Current state of the Pokémon</param>
    /// <param name="gameSource">Game/group the <see cref="pk"/> originated from. If <see cref="GameVersion.RBY"/>, it assumes Gen 1, otherwise Gen 2.</param>
    /// <returns>Possible origin species-form-levels to match against encounter data.</returns>
    public static EvoCriteria[] GetOriginChain12(PKM pk, GameVersion gameSource)
    {
        bool rby = gameSource == GameVersion.RBY;
        var maxSpecies = rby ? Legal.MaxSpeciesID_1 : Legal.MaxSpeciesID_2;

        bool hasOriginMet;
        int maxLevel, minLevel;
        if (pk is ICaughtData2 pk2)
        {
            hasOriginMet = pk2.CaughtData != 0;
            maxLevel = rby && Future_LevelUp2.Contains(pk.Species) ? pk.CurrentLevel - 1 : pk.CurrentLevel;
            minLevel = !hasOriginMet ? 2 : pk.IsEgg ? 5 : pk2.Met_Level;
        }
        else if (pk is PK1 pk1)
        {
            hasOriginMet = false;
            maxLevel = pk1.CurrentLevel;
            minLevel = 2;
        }
        else if (rby)
        {
            hasOriginMet = false;
            maxLevel = Future_LevelUp2.Contains(pk.Species) ? pk.CurrentLevel - 1 : GetLevelOriginMaxTransfer(pk, pk.Met_Level, 1);
            minLevel = 2;
        }
        else // GSC
        {
            hasOriginMet = false;
            maxLevel = GetLevelOriginMaxTransfer(pk, pk.Met_Level, 2);
            minLevel = 2;
        }

        return GetOriginChain(pk, maxSpecies, (byte)maxLevel, (byte)minLevel, hasOriginMet);
    }

    private static EvoCriteria[] GetOriginChain(PKM pk, int maxSpecies, byte maxLevel, byte minLevel, bool hasOriginMet)
    {
        if (maxLevel < minLevel)
            return Array.Empty<EvoCriteria>();

        if (hasOriginMet)
            return EvolutionChain.GetValidPreEvolutions(pk, maxSpecies, maxLevel, minLevel);

        // Permit the maximum to be all the way up to Current Level; we'll trim these impossible evolutions out later.
        var tempMax = pk.CurrentLevel;
        var chain = EvolutionChain.GetValidPreEvolutions(pk, maxSpecies, tempMax, minLevel);

        for (var i = 0; i < chain.Length; i++)
            chain[i] = chain[i] with { LevelMax = maxLevel, LevelMin = minLevel };

        return chain;
    }

    private static int GetLevelOriginMin(PKM pk, bool hasMet)
    {
        if (pk.Format == 3)
        {
            if (pk.IsEgg)
                return 5;
            return Math.Max(2, pk.Met_Level);
        }
        if (!hasMet)
            return 1;
        return Math.Max(1, pk.Met_Level);
    }

    private static int GetLevelOriginMax(PKM pk, bool hasMet)
    {
        var met = pk.Met_Level;
        if (hasMet)
            return pk.CurrentLevel;

        int generation = pk.Generation;
        if (generation >= 4)
            return met;

        var downLevel = GetLevelOriginMaxTransfer(pk, pk.CurrentLevel, generation);
        return Math.Min(met, downLevel);
    }

    private static int GetLevelOriginMaxTransfer(PKM pk, int met, int generation)
    {
        var species = pk.Species;

        if (Future_LevelUp.TryGetValue((ushort)(species | (pk.Form << 11)), out var delta))
            return met - delta;

        if (generation < 4 && Future_LevelUp4.Contains(species) && (pk.Format <= 7 || !Future_LevelUp4_Not8.Contains(species)))
            return met - 1;

        return met;
    }

    /// <summary>
    /// Species introduced in Generation 2 that require a level up to evolve into from a specimen that originated in a previous generation.
    /// </summary>
    private static readonly HashSet<ushort> Future_LevelUp2 = new()
    {
        (int)Crobat,
        (int)Espeon,
        (int)Umbreon,
        (int)Blissey,
    };

    /// <summary>
    /// Species introduced in Generation 4 that require a level up to evolve into from a specimen that originated in a previous generation.
    /// </summary>
    private static readonly HashSet<ushort> Future_LevelUp4 = new()
    {
        (int)Ambipom,
        (int)Weavile,
        (int)Magnezone,
        (int)Lickilicky,
        (int)Tangrowth,
        (int)Yanmega,
        (int)Leafeon,
        (int)Glaceon,
        (int)Mamoswine,
        (int)Gliscor,
        (int)Probopass,
    };

    /// <summary>
    /// Species introduced in Generation 4 that used to require a level up to evolve prior to Generation 8.
    /// </summary>
    private static readonly HashSet<ushort> Future_LevelUp4_Not8 = new()
    {
        (int)Magnezone, // Thunder Stone
        (int)Leafeon, // Leaf Stone
        (int)Glaceon, // Ice Stone
    };

    /// <summary>
    /// Species introduced in Generation 6+ that require a level up to evolve into from a specimen that originated in a previous generation.
    /// </summary>
    private static readonly Dictionary<ushort, byte> Future_LevelUp = new()
    {
        // Gen6
        {(int)Sylveon, 1},

        // Gen7
        {(int)Marowak | (1 << 11), 1},

        // Gen8
        {(int)Weezing | (1 << 11), 1},
        {(int)MrMime | (1 << 11), 1},
        {(int)MrRime, 2},
    };
}
