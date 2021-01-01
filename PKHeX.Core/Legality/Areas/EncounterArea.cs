using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
    /// </summary>
    public abstract record EncounterArea : IVersion
    {
        public GameVersion Version { get; }
        public int Location { get; protected init; }
        public SlotType Type { get; protected init; } = SlotType.Any;
        public EncounterSlot[] Slots { get; internal set; } = Array.Empty<EncounterSlot>();

        protected EncounterArea(GameVersion game) => Version = game;

        /// <summary>
        /// Gets the slots contained in the area that match the provided data.
        /// </summary>
        /// <param name="pkm">Pokémon Data</param>
        /// <param name="chain">Evolution lineage</param>
        /// <returns>Enumerable list of encounters</returns>
        public abstract IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain);

        /// <summary>
        /// Checks if the provided met location ID matches the parameters for the area.
        /// </summary>
        /// <param name="location">Met Location ID</param>
        /// <returns>True if possibly originated from this area, false otherwise.</returns>
        public virtual bool IsMatchLocation(int location) => Location == location;
    }
}
