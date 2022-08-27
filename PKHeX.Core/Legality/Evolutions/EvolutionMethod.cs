using static PKHeX.Core.EvolutionType;

namespace PKHeX.Core;

/// <summary>
/// Criteria for evolving to this branch in the <see cref="EvolutionTree"/>
/// </summary>
/// <param name="Method">Evolution Method</param>
/// <param name="Species">Evolve to Species</param>
/// <param name="Form">Destination Form</param>
/// <param name="Argument">Conditional Argument (different from <see cref="Level"/>)</param>
/// <param name="Level">Conditional Argument (different from <see cref="Argument"/>)</param>
/// <param name="LevelUp">Indicates if a level up is required to trigger evolution.</param>
public readonly record struct EvolutionMethod(EvolutionType Method, ushort Species, byte Form = 0, ushort Argument = 0, byte Level = 0, byte LevelUp = 0) : ISpeciesForm
{
    /// <summary>Evolve to Species</summary>
    public ushort Species { get; } = Species;

    /// <summary>Conditional Argument (different from <see cref="Level"/>)</summary>
    public ushort Argument { get; } = Argument;

    /// <summary>Evolution Method</summary>
    public EvolutionType Method { get; } = Method;

    /// <summary>Destination Form</summary>
    public byte Form { get; } = Form;

    /// <summary>Conditional Argument (different from <see cref="Argument"/>)</summary>
    public byte Level { get; } = Level;

    /// <summary>Indicates if a level up is required to trigger evolution.</summary>
    public byte LevelUp { get; } = LevelUp;

    public override string ToString() => $"{(Species)Species}-{Form} [{Argument}] @ {Level}{(RequiresLevelUp ? "X" : "")}";

    /// <summary>Is <see cref="AnyForm"/> if the evolved form isn't modified. Special consideration for <see cref="LevelUpFormFemale1"/>, which forces 1.</summary>
    private const byte AnyForm = byte.MaxValue;

    public bool RequiresLevelUp => LevelUp != 0;

    /// <summary>
    /// Returns the form that the Pokémon will have after evolution.
    /// </summary>
    /// <param name="form">Un-evolved Form ID</param>
    public byte GetDestinationForm(byte form)
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
    /// <returns>True if a evolution criteria is valid.</returns>
    public bool Valid(PKM pk, byte lvl, bool skipChecks)
    {
        if (!Method.IsLevelUpRequired())
            return ValidNotLevelUp(pk, skipChecks);

        if (!IsLevelUpMethodSecondarySatisfied(pk, skipChecks))
            return false;

        // Level Up (any); the above Level Up (with condition) cases will reach here if they were valid
        if (!RequiresLevelUp)
            return lvl >= Level;

        if (Level == 0 && lvl < 2)
            return false;
        if (lvl < Level)
            return false;

        if (skipChecks)
            return lvl >= Level;

        // Check Met Level for extra validity
        return HasMetLevelIncreased(pk, lvl);
    }

    private bool IsLevelUpMethodSecondarySatisfied(PKM pk, bool skipChecks) => Method switch
    {
        // Special Level Up Cases -- return false if invalid
        LevelUpMale when pk.Gender != 0 => false,
        LevelUpFemale when pk.Gender != 1 => false,
        LevelUpFormFemale1 when pk.Gender != 1 || pk.Form != 1 => false,

        // Permit the evolution if we're exploring for mistakes.
        LevelUpBeauty when pk is IContestStats s && s.CNT_Beauty < Argument => skipChecks,
        LevelUpNatureAmped or LevelUpNatureLowKey when GetAmpLowKeyResult(pk.Nature) != pk.Form => skipChecks,

        // Version checks come in pairs, check for any pair match
        LevelUpVersion or LevelUpVersionDay or LevelUpVersionNight when ((pk.Version & 1) != (Argument & 1) && pk.IsUntraded) => skipChecks,

        _ => true,
    };

    private bool ValidNotLevelUp(PKM pk, bool skipChecks) => Method switch
    {
        UseItemMale or RecoilDamageMale => pk.Gender == 0,
        UseItemFemale or RecoilDamageFemale => pk.Gender == 1,

        Trade or TradeHeldItem or TradeShelmetKarrablast => !pk.IsUntraded || skipChecks,
        _ => true, // no conditions
    };

    private bool HasMetLevelIncreased(PKM pk, int lvl)
    {
        int origin = pk.Generation;
        return origin switch
        {
            // No met data in RBY; No met data in GS, Crystal met data can be reset
            1 or 2 => true,

            // Pal Park / PokeTransfer updates Met Level
            3 or 4 => pk.Format > origin || lvl > pk.Met_Level,

            // 5=>6 and later transfers keep current level
            >=5 => lvl >= Level && (!pk.IsNative || lvl > pk.Met_Level),

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
