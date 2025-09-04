namespace PKHeX.Core;

public sealed class EncounterDatabaseSettings
{
    [LocalizedDescription("Skips searching if the user forgot to enter Species / Move(s) into the search criteria.")]
    public bool ReturnNoneIfEmptySearch { get; set; } = true;

    [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;

    [LocalizedDescription("Use properties from the PKM Editor tabs to specify criteria like Gender and Nature when generating an encounter.")]
    public bool UseTabsAsCriteria { get; set; } = true;

    [LocalizedDescription("Use properties from the PKM Editor tabs even if the new encounter isn't the same evolution chain.")]
    public bool UseTabsAsCriteriaAnySpecies { get; set; } = true;
}
