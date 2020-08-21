using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Fake encounter area used to mock data
    /// </summary>
    public sealed class EncounterAreaFake : EncounterArea
    {
        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain) =>
            Array.Empty<EncounterSlot>();
    }
}