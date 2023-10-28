namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.RSE"/>.
/// </summary>
/// <remarks>
/// Handled differently as these slots have fixed moves that are different from their normal level-up moves.
/// </remarks>
internal sealed record EncounterSlot3Swarm(EncounterArea3 Parent, ushort Species, byte LevelMin, byte LevelMax, byte SlotNumber, Moveset Moves)
    : EncounterSlot3(Parent, Species, 0, LevelMin, LevelMax, SlotNumber, 0, 0, 0, 0), IMoveset
{
    protected override void SetEncounterMoves(PKM pk) => pk.SetMoves(Moves);
}
