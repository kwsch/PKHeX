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
        var origin = new EvolutionOrigin(pk.Species, (byte)pk.Version, generation, minLevel, maxLevel);
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
        byte ver = rby ? (byte)GameVersion.RBY : (byte)GameVersion.GSC;
        byte gen = rby ? (byte)1 : (byte)2;
        var origin = new EvolutionOrigin(pk.Species, ver, gen, minLevel, maxLevel);
        return EvolutionChain.GetOriginChain(pk, origin);
    }

    private static (byte minLevel, byte maxLevel) GetMinMax(PKM pk, byte generation)
    {
        byte maxLevel = (byte)pk.CurrentLevel;
        byte minLevel = GetLevelOriginMin(pk, generation);
        return (minLevel, maxLevel);
    }

    private static (byte minLevel, byte maxLevel) GetMinMaxGB(PKM pk)
    {
        byte maxLevel = (byte)pk.CurrentLevel;
        byte minLevel = GetLevelOriginMinGB(pk);
        return (minLevel, maxLevel);
    }

    private static byte GetLevelOriginMin(PKM pk, byte generation) => generation switch
    {
        3 when pk.Format != generation =>             pk.Egg_Location != 0 ? (byte)5 : (byte)2,
        4 when pk.Format != generation =>             pk.Egg_Location != 0 ? (byte)1 : (byte)2,
        3 when pk.Format == generation => pk.IsEgg || pk.Egg_Location != 0 ? (byte)5 : (byte)pk.Met_Level,
        4 when pk.Format == generation => pk.IsEgg || pk.Egg_Location != 0 ? (byte)1 : (byte)pk.Met_Level,
        _ => Math.Max((byte)1, (byte)pk.Met_Level),
    };

    private static byte GetLevelOriginMinGB(PKM pk) => pk switch
    {
        { IsEgg: true } => 5,
        ICaughtData2 { CaughtData: not 0 } pk2 => (byte)pk2.Met_Level,
        _ => 2,
    };
}
