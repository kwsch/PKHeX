using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.Gen2"/>.
/// </summary>
/// <remarks>
/// Referenced Area object contains Time data which is used for <see cref="GameVersion.C"/> origin data.
/// </remarks>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot2 : EncounterSlot, INumberedSlot
{
    public override int Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    public byte SlotNumber { get; }
    public override Ball FixedBall => Ball.Poke;
    public bool IsHeadbutt => SlotType == SlotType.Headbutt;

    public EncounterSlot2(EncounterArea2 area, byte species, byte min, byte max, byte slot) : base(area, species, species == 201 ? FormRandom : (byte)0, min, max)
    {
        SlotNumber = slot;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);

        var pk2 = (PK2)pk;

        if (IsHeadbutt)
        {
            var id = pk2.TID16;
            if (!IsTreeAvailable(id))
            {
                // Get a random TID that satisfies this slot.
                do { id = (ushort)Util.Rand.Next(); }
                while (!IsTreeAvailable(id));
                pk2.TID16 = id;
            }
        }

        if (Version == GameVersion.C)
            pk2.Met_TimeOfDay = ((EncounterArea2)Area).Time.RandomValidTime();
    }

    private static ReadOnlySpan<byte> TreeIndexes => new byte[]
    {
        02, 04, 05, 08, 11, 12, 14, 15, 18, 20, 21, 25, 26, 34, 37, 38, 39, 91, 92,
    };

    private static ReadOnlySpan<int> Trees => new[]
    {
        0x3FF_3FF, // Route 29
        0x0FF_3FF, // Route 30
        0x3FE_3FF, // Route 31
        0x3EE_3FF, // Route 32
        0x240_3FF, // Route 33
        0x37F_3FF, // Azalea Town
        0x3FF_3FF, // Ilex Forest
        0x001_3FE, // Route 34
        0x261_3FF, // Route 35
        0x3FF_3FF, // Route 36
        0x2B9_3FF, // Route 37
        0x3FF_3FF, // Route 38
        0x184_3FF, // Route 39
        0x3FF_3FF, // Route 42
        0x3FF_3FF, // Route 43
        0x3FF_3FF, // Lake of Rage
        0x2FF_3FF, // Route 44
        0x200_1FF, // Route 26
        0x2BB_3FF, // Route 27
    };

    public bool IsTreeAvailable(ushort trainerID)
    {
        var treeIndex = TreeIndexes.BinarySearch((byte)Location);
        if (treeIndex < 0)
            return false;
        var permissions = Trees[treeIndex];

        var pivot = trainerID % 10;
        var type = Area.Type;
        return type switch
        {
            SlotType.Headbutt => (permissions & (1 << pivot)) != 0,
            /*special*/ _ => (permissions & (1 << (pivot + 12))) != 0,
        };
    }

    // we have "Special" bitflag. Strip it out.
    public SlotType SlotType => Area.Type & (SlotType)0xF;
}
