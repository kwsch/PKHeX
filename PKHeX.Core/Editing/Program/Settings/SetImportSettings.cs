namespace PKHeX.Core;

public sealed class SetImportSettings
{
    [LocalizedDescription("Apply Stat Alignment to Nature on Import")]
    public bool ApplyStatAlignment { get; set; } = true;

    [LocalizedDescription("Apply Markings on Import")]
    public bool ApplyMarkings { get; set; } = true;
}
