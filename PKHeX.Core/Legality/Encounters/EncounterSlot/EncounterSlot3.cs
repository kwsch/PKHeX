namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public record EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot
    {
        public sealed override int Generation => 3;

        public int StaticIndex { get; }
        public int MagnetPullIndex { get; }
        public int StaticCount { get; }
        public int MagnetPullCount { get; }

        public int SlotNumber { get; }

        public EncounterSlot3(EncounterArea3 area, int species, int form, int min, int max, int slot, int mpi, int mpc, int sti, int stc) : base(area, species, form, min, max)
        {
            SlotNumber = slot;

            MagnetPullIndex = mpi;
            MagnetPullCount = mpc;

            StaticIndex = sti;
            StaticCount = stc;
        }

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsDeferredSafari3(pkm.Ball == (int)Ball.Safari))
                return EncounterMatchRating.PartialMatch;
            return base.GetMatchRating(pkm);
        }

        private bool IsDeferredSafari3(bool IsSafariBall) => IsSafariBall != Locations.IsSafariZoneLocation3(Location);
    }
}
