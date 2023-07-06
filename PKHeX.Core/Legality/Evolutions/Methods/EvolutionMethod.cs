using static PKHeX.Core.EvolutionType;
using static PKHeX.Core.EvolutionCheckResult;

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
public readonly record struct EvolutionMethod(EvolutionType Method, ushort Species, byte Form = EvolutionMethod.AnyForm, ushort Argument = 0, byte Level = 0, byte LevelUp = 0) : ISpeciesForm
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
    /// <param name="levelMin">Minimum level to sanity check with</param>
    /// <param name="skipChecks">Option to skip some comparisons to return a 'possible' evolution.</param>
    /// <returns>True if a evolution criteria is valid.</returns>
    public EvolutionCheckResult Check(PKM pk, byte lvl, byte levelMin, bool skipChecks)
    {
        if (!Method.IsLevelUpRequired())
            return ValidNotLevelUp(pk, skipChecks);

        var chk = IsLevelUpMethodSecondarySatisfied(pk, skipChecks);
        if (chk != Valid)
            return chk;

        // Level Up (any); the above Level Up (with condition) cases will reach here if they were valid
        if (lvl < Level)
            return InsufficientLevel;
        if (!RequiresLevelUp)
            return Valid;
        if (Level == 0 && lvl < 2)
            return InsufficientLevel;
        if (lvl < levelMin + LevelUp && !skipChecks)
            return InsufficientLevel;

        return Valid;
    }

    private EvolutionCheckResult IsLevelUpMethodSecondarySatisfied(PKM pk, bool skipChecks) => Method switch
    {
        // Special Level Up Cases -- return false if invalid
        LevelUpMale when pk.Gender != 0 => BadGender,
        LevelUpFemale when pk.Gender != 1 => BadGender,
        LevelUpFormFemale1 when pk.Gender != 1 => BadGender,
        LevelUpFormFemale1 when pk.Form != 1 => BadForm,

        // Permit the evolution if we're exploring for mistakes.
        LevelUpBeauty when pk is IContestStatsReadOnly s && s.CNT_Beauty < Argument => skipChecks ? Valid : LowContestStat,
        LevelUpNatureAmped or LevelUpNatureLowKey when GetAmpLowKeyResult(pk.Nature) != pk.Form => skipChecks ? Valid : BadForm,

        // Version checks come in pairs, check for any pair match
        LevelUpVersion or LevelUpVersionDay or LevelUpVersionNight when ((pk.Version & 1) != (Argument & 1) && pk.IsUntraded) => skipChecks ? Valid : VisitVersion,

        LevelUpKnowMoveEC100  when pk.EncryptionConstant % 100 != 0 => skipChecks ? Valid : WrongEC,
        LevelUpKnowMoveECElse when pk.EncryptionConstant % 100 == 0 => skipChecks ? Valid : WrongEC,
        LevelUpInBattleEC100  when pk.EncryptionConstant % 100 != 0 => skipChecks ? Valid : WrongEC,
        LevelUpInBattleECElse when pk.EncryptionConstant % 100 == 0 => skipChecks ? Valid : WrongEC,

        _ => Valid,
    };

    private EvolutionCheckResult ValidNotLevelUp(PKM pk, bool skipChecks) => Method switch
    {
        UseItemMale or LevelUpRecoilDamageMale => pk.Gender == 0 ? Valid : BadGender,
        UseItemFemale or LevelUpRecoilDamageFemale => pk.Gender == 1 ? Valid : BadGender,

        Trade or TradeHeldItem or TradeShelmetKarrablast => !pk.IsUntraded || skipChecks ? Valid : Untraded,
        _ => Valid, // no conditions
    };

    public static int GetAmpLowKeyResult(int n)
    {
        var index = n - 1;
        if ((uint)index > 22)
            return 0;
        return (0b_0101_1011_1100_1010_0101_0001 >> index) & 1;
    }
}
