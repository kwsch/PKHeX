using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc />
    /// <summary>
    /// Fake encounter area used to mock data
    /// </summary>
    public sealed class EncounterAreaFake : EncounterArea
    {
        protected override IEnumerable<EncounterSlot> GetMatchFromEvoLevel(PKM pkm, IEnumerable<EvoCriteria> vs, int minLevel)
            => Enumerable.Empty<EncounterSlot>();

        protected override IEnumerable<EncounterSlot> GetFilteredSlots(PKM pkm, IEnumerable<EncounterSlot> slots, int minLevel)
            => Enumerable.Empty<EncounterSlot>();
    }
}