namespace PKHeX.Core;

/// <summary>
/// Evolution Info tracking how an evolution was performed, and the end result species and form.
/// </summary>
public readonly record struct EvoCriteria : ISpeciesForm, ILevelRange
{
    public required ushort Species { get; init; }
    public byte Form { get; init; }

    public byte LevelUpRequired { get; init; }
    public byte LevelMax { get; init; }
    public byte LevelMin { get; init; }

    public EvolutionType Method { get; init; }

    public bool RequiresLvlUp => LevelUpRequired != 0;

    public bool InsideLevelRange(int level) => LevelMin <= level && level <= LevelMax;

    public override string ToString() => $"{(Species) Species}{(Form != 0 ? $"-{Form}" : "")}}} [{LevelMin},{LevelMax}] via {Method}";

    internal const EvolutionType SentinelNotReached = EvolutionType.Invalid;

    public bool IsBetterDevolution(EvoCriteria reference)
    {
        if (reference.Species == 0)
            return true;

        return LevelMin + LevelUpRequired < reference.LevelMin + reference.LevelUpRequired;
    }

    public bool IsBetterEvolution(EvoCriteria reference)
    {
        if (reference.Method == SentinelNotReached)
            return true;

        if (LevelMin + LevelUpRequired > reference.LevelMin + reference.LevelUpRequired)
            return false;

        return LevelMax > reference.LevelMax;
    }
}
