namespace PKHeX.Core;

public sealed class SetImportSettings
{
    [LocalizedDescription("Apply StatNature to Nature on Import")]
    public bool ApplyNature { get; set; } = true;
    [LocalizedDescription("Apply Markings on Import")]
    public bool ApplyMarkings { get; set; } = true;
}
