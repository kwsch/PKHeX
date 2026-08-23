using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class GameSpecificSettings
{
    public GameSpecificSettings3 Gen3 { get; set; } = new();
    public GameSpecificSettings7 Gen7 { get; set; } = new();
    public GameSpecificSettings8 Gen8 { get; set; } = new();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class GameSpecificSettings3
{
    /// <summary>
    /// Pokemon Box: Ruby &amp; Sapphire allows swapping of teams at a precise time, which can put Mythicals/Legends/Over-leveled into the team slots before the ribbon award sequence.
    /// </summary>
    /// <remarks>
    /// https://projectpokemon.org/home/forums/topic/68163-how-to-give-ribbons-to-legendary-pok%C3%A9mon-using-pok%C3%A9mon-box-ruby-sapphire/
    /// </remarks>
    [LocalizedDescription("Rule-tweak to allow the Pokemon Box: Ruby && Sapphire exploit to obtain Battle Tower ribbons illegitimately.")]
    public bool AllowBattleTowerTeamSwap { get; set; }
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
