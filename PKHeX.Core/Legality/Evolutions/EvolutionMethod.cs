using static PKHeX.Core.EvolutionType;

namespace PKHeX.Core;

/// <summary>
/// Criteria for evolving to this branch in the <see cref="EvolutionTree"/>
/// </summary>
public sealed class EvolutionMethod
{
    /// <summary>
    /// Evolution Method
    /// </summary>
    public readonly EvolutionType Method;

    /// <summary>
    /// Evolve to Species
    /// </summary>
    public readonly ushort Species;

    /// <summary>
    /// Conditional Argument (different from <see cref="Level"/>)
    /// </summary>
    public readonly ushort Argument;

    /// <summary>
    /// Conditional Argument (different from <see cref="Argument"/>)
    /// </summary>
    public readonly byte Level;

    /// <summary>
    /// Destination Form
    /// </summary>
    /// <remarks>Is <see cref="AnyForm"/> if the evolved form isn't modified. Special consideration for <see cref="LevelUpFormFemale1"/>, which forces 1.</remarks>
    public readonly sbyte Form;

    private const sbyte AnyForm = -1;

    // Not stored in binary data
    public bool RequiresLevelUp; // tracks if this method requires a Level Up, lazily set

    public EvolutionMethod(EvolutionType method, ushort species, ushort argument = 0, byte level = 0, sbyte form = AnyForm)
    {
        Method = method;
        Species = species;
        Argument = argument;
        Form = form;
        Level = level;
    }

    public override string ToString() => $"{(Species) Species}-{Form} [{Argument}] @ {Level}{(RequiresLevelUp ? "X" : "")}";

    /// <summary>
    /// Returns the form that the Pokémon will have after evolution.
    /// </summary>
    /// <param name="form">Un-evolved Form ID</param>
    public int GetDestinationForm(int form)
    {
        if (Method == LevelUpFormFemale1)
            return 1;
        if (Form == AnyForm)
            return form;
        return Form;
    }

    /// <summary>
    /// Checks the <see cref="EvolutionMethod"/> for validity by comparing against the <see cref="PKM"/> data.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="lvl">Current level</param>
    /// <param name="skipChecks">Option to skip some comparisons to return a 'possible' evolution.</param>
    /// <param name="game">Game environment in which the evolution occurs.</param>
    /// <returns>True if a evolution criteria is valid.</returns>
    public bool Valid(PKM pk, in byte lvl, in bool skipChecks, in GameVersion game)
    {
        RequiresLevelUp = false;
        switch (Method)
        {
            case UseItem or UseItemWormhole or UseItemFullMoon:
            case CriticalHitsInBattle or HitPointsLostInBattle or Spin:
            case UseAgileStyleMoves or UseStrongStyleMoves:
            case TowerOfDarkness or TowerOfWaters:
                return true;
            case UseItemMale or RecoilDamageMale:
                return pk.Gender == 0;
            case UseItemFemale or RecoilDamageFemale:
                return pk.Gender == 1;

            case Trade or TradeHeldItem or TradeShelmetKarrablast:
                return !pk.IsUntraded || skipChecks;

            // Special Level Up Cases -- return false if invalid
            case LevelUpNatureAmped or LevelUpNatureLowKey when GetAmpLowKeyResult(pk.Nature) != pk.Form && !skipChecks:
                return false;

            case LevelUpBeauty when pk is not IContestStats s || s.CNT_Beauty < Argument:
                return skipChecks;
            case LevelUpMale when pk.Gender != 0:
                return false;
            case LevelUpFemale when pk.Gender != 1:
                return false;
            case LevelUpFormFemale1 when pk.Gender != 1 || pk.Form != 1:
                return false;

            case LevelUpVersion or LevelUpVersionDay or LevelUpVersionNight when ((pk.Version & 1) != (Argument & 1) && pk.IsUntraded) || skipChecks:
                return skipChecks; // Version checks come in pairs, check for any pair match

            // Level Up (any); the above Level Up (with condition) cases will reach here if they were valid
            default:
                if (IsThresholdCheckMode(game))
                    return lvl >= Level;

                if (Level == 0 && lvl < 2)
                    return false;
                if (lvl < Level)
                    return false;

                RequiresLevelUp = true;
                if (skipChecks)
                    return lvl >= Level;

                // Check Met Level for extra validity
                return HasMetLevelIncreased(pk, lvl);
        }
    }

    private static bool IsThresholdCheckMode(GameVersion game)
    {
        // Starting in Legends: Arceus, level-up evolutions can be triggered if the current level is >= criteria.
        // This allows for evolving over-leveled captures immediately without leveling up from capture level.
        return game is GameVersion.PLA;
    }

    private bool HasMetLevelIncreased(PKM pk, int lvl)
    {
        int origin = pk.Generation;
        return origin switch
        {
            // No met data in RBY; No met data in GS, Crystal met data can be reset
            1 or 2 => true,

            // Pal Park / PokeTransfer updates Met Level
            3 or 4 => pk.Format > origin || pk.Met_Level < lvl,

            // 5=>6 and later transfers keep current level
            >=5 => lvl >= Level && (!pk.IsNative || pk.Met_Level < lvl),

            _ => false,
        };
    }

    public EvoCriteria GetEvoCriteria(ushort species, byte form, byte lvl) => new()
    {
        Species = species,
        Form = form,
        LevelMax = lvl,
        LevelMin = 0,
        Method = Method,
    };

    public static int GetAmpLowKeyResult(int n)
    {
        var index = n - 1;
        if ((uint)index > 22)
            return 0;
        return (0b_0101_1011_1100_1010_0101_0001 >> index) & 1;
    }
}
