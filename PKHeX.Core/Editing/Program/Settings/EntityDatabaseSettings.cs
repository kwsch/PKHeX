using System;

namespace PKHeX.Core;

public sealed class EntityDatabaseSettings
{
    private const int ResultsGridRowCountMin = 5;
    private const int ResultsGridRowCountMax = 20;

    [LocalizedDescription("When loading content for the PKM Database, search within backup save files.")]
    public bool SearchBackups { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM Database, search within OtherBackupPaths.")]
    public bool SearchExtraSaves { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM Database, search subfolders within OtherBackupPaths.")]
    public bool SearchExtraSavesDeep { get; set; } = true;

    [LocalizedDescription("When loading content for the PKM database, the list will be ordered by this option.")]
    public DatabaseSortMode InitialSortMode { get; set; }

    [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;

    [LocalizedDescription("Visible row count for the PKM Database sprite grid. Clamped from 5 to 20.")]
    public int ResultsGridRowCount
    {
        get;
        set => field = Math.Clamp(value, ResultsGridRowCountMin, ResultsGridRowCountMax);
    } = 9;
}

public enum DatabaseSortMode
{
    None,
    SpeciesForm,
    SlotIdentity,
}
