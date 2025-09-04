namespace PKHeX.Core;

public sealed class MysteryGiftDatabaseSettings
{
    [LocalizedDescription("Hides gifts if the currently loaded save file cannot (indirectly) receive them.")]
    public bool FilterUnavailableSpecies { get; set; } = true;
}
