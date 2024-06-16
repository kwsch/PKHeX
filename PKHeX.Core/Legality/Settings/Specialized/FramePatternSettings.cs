using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FramePatternSettings
{
    [LocalizedDescription("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match for Generation 3 encounters.")]
    public Severity RNGFrameNotFound3 { get; set; } = Severity.Fishy;

    [LocalizedDescription("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match for Generation 4 encounters.")]
    public Severity RNGFrameNotFound4 { get; set; } = Severity.Invalid;

    public Severity GetSeverity(byte generation) => generation == 3 ? RNGFrameNotFound3 : RNGFrameNotFound4;
}
