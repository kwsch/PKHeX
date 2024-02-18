namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.XD"/> encounter area
/// </summary>
public sealed record EncounterArea3XD : IVersion, IEncounterArea<EncounterSlot3XD>, IAreaLocation
{
    public EncounterSlot3XD[] Slots { get; }
    public GameVersion Version => GameVersion.XD;
    public readonly byte Location;

    public bool IsMatchLocation(ushort location) => location == Location;

    public EncounterArea3XD(byte loc, ushort s0, byte l0, ushort s1, byte l1, ushort s2, byte l2)
    {
        Location = loc;
        Slots =
        [
            new EncounterSlot3XD(this, s0, 10, l0, 0),
            new EncounterSlot3XD(this, s1, 10, l1, 1),
            new EncounterSlot3XD(this, s2, 10, l2, 2),
        ];
    }
}
