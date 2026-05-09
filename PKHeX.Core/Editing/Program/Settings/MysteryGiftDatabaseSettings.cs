using System;

namespace PKHeX.Core;

public sealed class MysteryGiftDatabaseSettings
{
    private const int ResultsGridRowCountMin = 5;
    private const int ResultsGridRowCountMax = 20;

    [LocalizedDescription("Hides gifts if the currently loaded save file cannot (indirectly) receive them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;

    [LocalizedDescription("Visible row count for the Mystery Gift Database sprite grid. Clamped from 5 to 20.")]
    public int ResultsGridRowCount
    {
        get;
        set => field = Math.Clamp(value, ResultsGridRowCountMin, ResultsGridRowCountMax);
    } = 9;
}
