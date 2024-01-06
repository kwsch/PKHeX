using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Properties common to RS &amp; Emerald save files.
/// </summary>
public interface IGen3Hoenn
{
    RTC3 ClockInitial { get; set; }
    RTC3 ClockElapsed { get; set; }
    PokeBlock3Case PokeBlocks { get; set; }
    DecorationInventory3 Decorations { get; }
    Swarm3 Swarm { get; set; }

    IReadOnlyList<Swarm3> DefaultSwarms { get; }
    int SwarmIndex { get; set; }

    RecordMixing3Gift RecordMixingGift { get; set; }

    /** Max RPM for 2, 3 and 4 players. 0 if no record. */
    Span<ushort> BerryBlenderRPMRecords { get; set; }
}
