namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen3"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public record EncounterSlot3 : EncounterSlot, IMagnetStatic, INumberedSlot, ISlotRNGType
    {
        public sealed override int Generation => 3;

        public byte StaticIndex { get; }
        public byte MagnetPullIndex { get; }
        public byte StaticCount { get; }
        public byte MagnetPullCount { get; }
        public SlotType Type => Area.Type;

        public byte SlotNumber { get; }
        public override Ball FixedBall => Locations.IsSafariZoneLocation3(Location) ? Ball.Safari : Ball.None;

        public EncounterSlot3(EncounterArea3 area, int species, int form, int min, int max, byte slot, byte mpi, byte mpc, byte sti, byte stc) : base(area, species, form, min, max)
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
