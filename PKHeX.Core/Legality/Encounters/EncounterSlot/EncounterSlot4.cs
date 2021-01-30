namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen4"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot4 : EncounterSlot, IMagnetStatic, INumberedSlot, IEncounterTypeTile
    {
        public override int Generation => 4;
        public EncounterType TypeEncounter => ((EncounterArea4)Area).TypeEncounter;

        public int StaticIndex { get; }
        public int MagnetPullIndex { get; }
        public int StaticCount { get; }
        public int MagnetPullCount { get; }

        public int SlotNumber { get; }

        public EncounterSlot4(EncounterArea4 area, int species, int form, int min, int max, int slot, int mpi, int mpc, int sti, int stc) : base(area, species, form, min, max)
        {
            SlotNumber = slot;

            MagnetPullIndex = mpi;
            MagnetPullCount = mpc;

            StaticIndex = sti;
            StaticCount = stc;
        }

        protected override void SetFormatSpecificData(PKM pk) => ((PK4)pk).EncounterType = TypeEncounter.GetIndex();

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if (IsDeferredWurmple(pkm))
                return EncounterMatchRating.PartialMatch;
            if ((pkm.Ball == (int)Ball.Safari) != Locations.IsSafariZoneLocation4(Location))
                return EncounterMatchRating.PartialMatch;
            if ((pkm.Ball == (int)Ball.Sport) != (Area.Type == SlotType.BugContest))
            {
                // Nincada => Shedinja can wipe the ball back to Poke
                if (pkm.Species != (int)Core.Species.Shedinja || pkm.Ball != (int)Ball.Poke)
                    return EncounterMatchRating.PartialMatch;
            }
            return EncounterMatchRating.Match;
        }
    }
}
