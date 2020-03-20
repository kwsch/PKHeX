using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="EncounterStatic8"/> with multiple references (used for multiple met locations)
    /// </summary>
    public sealed class EncounterStatic8S : EncounterStatic8
    {
        public override int Location { get => Locations[0]; set { } }
        public IReadOnlyList<int> Locations { get; internal set; } = Array.Empty<int>();
        protected override bool IsMatchLocation(PKM pkm) => Locations.Contains(pkm.Met_Location);
    }
}