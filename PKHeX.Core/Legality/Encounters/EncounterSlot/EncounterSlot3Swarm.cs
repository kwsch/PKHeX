namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.RSE"/>.
/// </summary>
/// <remarks>
/// Handled differently as these slots have fixed moves that are different from their normal level-up moves.
/// </remarks>
/// <inheritdoc cref="EncounterSlot"/>
internal sealed record EncounterSlot3Swarm : EncounterSlot3, IMoveset
{
    public Moveset Moves { get; }

    public EncounterSlot3Swarm(EncounterArea3 area, ushort species, byte min, byte max, byte slot,
        Moveset moves) : base(area, species, 0, min, max, slot, 0, 0, 0, 0) => Moves = moves;

    protected override void SetEncounterMoves(PKM pk, GameVersion version, int level)
    {
        pk.SetMoves(Moves);
        pk.SetMaximumPPCurrent(Moves);
    }
}
