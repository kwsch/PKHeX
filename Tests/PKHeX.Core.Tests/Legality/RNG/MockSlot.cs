namespace PKHeX.Core.Tests;

public record MockSlot : IEncounterSlot34
{
    public MockSlot() { }

    public MockSlot(byte slotNumber, byte levelMin = 5, byte levelMax = 5, SlotType type = SlotType.Grass)
    {
        SlotNumber = slotNumber;
        LevelMin = levelMin;
        LevelMax = levelMax;
        Type = type;
    }

    public byte SlotNumber { get; init; }
    public byte LevelMin { get; init; }
    public byte LevelMax { get; init; }
    public SlotType Type { get; init; }

    public byte StaticIndex { get; init; }
    public byte MagnetPullIndex { get; init; }
    public byte StaticCount { get; init; }
    public byte MagnetPullCount { get; init; }
    public byte AreaRate { get; init; }

    public byte PressureLevel => LevelMax;
}
