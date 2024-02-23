using System;

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
    /// <param name="generation">Original Generation</param>
    /// <returns>Possible origin species-form-levels to match against encounter data.</returns>
    /// <remarks>Use <see cref="GetOriginChain12"/> if the <see cref="pk"/> originated from Generation 1 or 2.</remarks>
    public static EvoCriteria[] GetOriginChain(PKM pk, byte generation)
    {
        var (minLevel, maxLevel) = GetMinMax(pk, generation);
        var origin = new EvolutionOrigin(pk.Species, pk.Version, generation, minLevel, maxLevel);
        return EvolutionChain.GetOriginChain(pk, origin);
    }

    /// <summary>
    /// Gets possible evolution details for the input <see cref="pk"/> originating from Generation 1 or 2.
    /// </summary>
    /// <param name="pk">Current state of the Pokémon</param>
    /// <param name="gameSource">Game/group the <see cref="pk"/> originated from. If <see cref="GameVersion.RBY"/>, it assumes Gen 1, otherwise Gen 2.</param>
    /// <returns>Possible origin species-form-levels to match against encounter data.</returns>
    public static EvoCriteria[] GetOriginChain12(PKM pk, GameVersion gameSource)
    {
        var (minLevel, maxLevel) = GetMinMaxGB(pk);
        bool rby = gameSource == GameVersion.RBY;
        GameVersion version = rby ? GameVersion.RBY : GameVersion.GSC;
        byte gen = rby ? (byte)1 : (byte)2;
        var origin = new EvolutionOrigin(pk.Species, version, gen, minLevel, maxLevel);
        return EvolutionChain.GetOriginChain(pk, origin);
    }

    private static (byte minLevel, byte maxLevel) GetMinMax(PKM pk, byte generation)
    {
        byte maxLevel = pk.CurrentLevel;
        byte minLevel = GetLevelOriginMin(pk, generation);
        return (minLevel, maxLevel);
    }

    private static (byte minLevel, byte maxLevel) GetMinMaxGB(PKM pk)
    {
        byte maxLevel = pk.CurrentLevel;
        byte minLevel = GetLevelOriginMinGB(pk);
        return (minLevel, maxLevel);
    }

    private static byte GetLevelOriginMin(PKM pk, byte generation) => generation switch
    {
        3 => GetLevelOriginMin3(pk),
        4 => GetLevelOriginMin4(pk),
        _ => Math.Max((byte)1, pk.MetLevel),
    };

    private static bool IsEggLocationNonZero(PKM pk) => pk.EggLocation != LocationEdits.GetNoneLocation(pk.Context);

    private static byte GetLevelOriginMinGB(PKM pk)
    {
        const byte EggLevel = 5;
        const byte MinWildLevel = 2;
        if (pk.IsEgg)
            return EggLevel;
        if (pk is not ICaughtData2 { CaughtData: not 0 } pk2)
            return MinWildLevel;
        return pk2.MetLevel;
    }

    private static byte GetLevelOriginMin3(PKM pk)
    {
        const byte EggLevel = 5;
        const byte MinWildLevel = 2;
        if (pk.Format != 3)
            return MinWildLevel;
        if (pk.IsEgg)
            return EggLevel;
        return pk.MetLevel;
    }

    private static byte GetLevelOriginMin4(PKM pk)
    {
        const byte EggLevel = 1;
        const byte MinWildLevel = 2;
        if (pk.Format != 4)
            return IsEggLocationNonZero(pk) ? EggLevel : MinWildLevel;
        if (pk.IsEgg || IsEggLocationNonZero(pk))
            return EggLevel;
        return pk.MetLevel;
    }
}
