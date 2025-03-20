using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FramePatternSettings
{
    [LocalizedDescription("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match for Generation 3 encounters.")]
    public Severity RNGFrameNotFound3 { get; set; } = Severity.Fishy;

    [LocalizedDescription("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match for Generation 4 encounters.")]
    public Severity RNGFrameNotFound4 { get; set; } = Severity.Invalid;

    [LocalizedDescription("Allow Generation 3 bred eggs to have any PID/IV type by assuming they were RNG abused to be collisions instead of hacked.")]
    public bool EggRandomAnyType3 { get; set; }

    [LocalizedDescription("Allow Generation 4 bred eggs to have any PID/IV type by assuming they were RNG abused to be collisions instead of hacked.")]
    public bool EggRandomAnyType4 { get; set; }

    public Severity GetSeverity(byte generation) => generation == 3 ? RNGFrameNotFound3 : RNGFrameNotFound4;
}
