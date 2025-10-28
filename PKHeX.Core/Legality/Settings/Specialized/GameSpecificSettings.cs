using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class GameSpecificSettings
{
    public GameSpecificSettings7 Gen7 { get; set; } = new();
    public GameSpecificSettings8 Gen8 { get; set; } = new();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class GameSpecificSettings7
{
    [LocalizedDescription("Severity to flag a Legality Check if Pokémon from Gen1/2 has a Star Shiny PID.")]
    public Severity Gen7TransferStarPID { get; set; } = Severity.Fishy;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class GameSpecificSettings8
{
    [LocalizedDescription("Severity to flag a Legality Check if a Gen8 Memory is missing for the Handling Trainer.")]
    public Severity Gen8MemoryMissingHT { get; set; } = Severity.Fishy;
}
