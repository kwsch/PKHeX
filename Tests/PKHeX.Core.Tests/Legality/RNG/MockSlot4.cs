namespace PKHeX.Core.Tests;

public record MockSlot4 : IEncounterSlot4
{
    public MockSlot4() { }

    public MockSlot4(byte slotNumber, byte levelMin = 5, byte levelMax = 5, SlotType4 type = SlotType4.Grass)
    {
        SlotNumber = slotNumber;
        LevelMin = levelMin;
        LevelMax = levelMax;
        Type = type;
    }

    public byte SlotNumber { get; init; }
    public byte LevelMin { get; init; }
    public byte LevelMax { get; init; }
    public SlotType4 Type { get; init; }

    public byte StaticIndex { get; init; }
    public byte MagnetPullIndex { get; init; }
    public byte StaticCount { get; init; }
    public byte MagnetPullCount { get; init; }
    public byte AreaRate { get; init; }

    public byte PressureLevel => LevelMax;
    public ushort Species => 0;
    public byte Form => 0;
}
