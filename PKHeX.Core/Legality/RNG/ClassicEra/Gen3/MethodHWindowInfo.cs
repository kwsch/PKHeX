namespace PKHeX.Core;

/// <summary>
/// Cache-able representation of the Method H window information.
/// </summary>
/// <param name="CountRegular">Count of reversals allowed for no specific lead (not requiring cute charm).</param>
/// <param name="Type">Type of Method H logic.</param>
/// <param name="Gender">Gender Ratio of the encountered Pokémon.</param>
/// <param name="CountCute">Count of reversals allowed for a matching cute charm lead.</param>
public readonly record struct MethodHWindowInfo(ushort CountRegular, MethodHCondition Type, byte Gender = 0, ushort CountCute = 0)
{
    private bool IsUninitialized => Type == MethodHCondition.Empty;
    private bool IsGenderRatioDifferentCuteCharm(ushort Species) => Type == MethodHCondition.Emerald && PersonalTable.E[Species].Gender != Gender;

    public bool ShouldRevise(ushort Species) => IsUninitialized || IsGenderRatioDifferentCuteCharm(Species);
}
