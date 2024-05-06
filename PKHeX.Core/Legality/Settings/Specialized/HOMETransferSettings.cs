using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class HOMETransferSettings
{
    [LocalizedDescription("Severity to flag a Legality Check if the HOME Tracker is Missing")]
    public Severity HOMETransferTrackerNotPresent { get; set; } = Severity.Invalid;

    [LocalizedDescription("Severity to flag a Legality Check if Pokémon has a zero value for both Height and Weight.")]
    public Severity ZeroHeightWeight { get; set; } = Severity.Fishy;

    public void Disable()
    {
        HOMETransferTrackerNotPresent = Severity.Fishy;
        ZeroHeightWeight = Severity.Fishy;
    }
}
