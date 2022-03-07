namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.Gen4"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot4 : EncounterSlot, IMagnetStatic, INumberedSlot, IGroundTypeTile, ISlotRNGType
    {
        public override int Generation => 4;
        public GroundTilePermission GroundTile => ((EncounterArea4)Area).GroundTile;

        public byte StaticIndex { get; }
        public byte MagnetPullIndex { get; }
        public byte StaticCount { get; }
        public byte MagnetPullCount { get; }
        public SlotType Type => Area.Type;

        public byte SlotNumber { get; }
        public override Ball FixedBall => GetRequiredBallValue();
        public bool CanUseRadar => !GameVersion.HGSS.Contains(Version) && GroundTile.HasFlag(GroundTilePermission.Grass);

        public EncounterSlot4(EncounterArea4 area, ushort species, byte form, byte min, byte max, byte slot, byte mpi, byte mpc, byte sti, byte stc) : base(area, species, form, min, max)
        {
            SlotNumber = slot;

            MagnetPullIndex = mpi;
            MagnetPullCount = mpc;

            StaticIndex = sti;
            StaticCount = stc;
        }

        protected override void SetFormatSpecificData(PKM pk) => ((PK4)pk).GroundTile = GroundTile.GetIndex();

        public override EncounterMatchRating GetMatchRating(PKM pkm)
        {
            if ((pkm.Ball == (int)Ball.Safari) != Locations.IsSafariZoneLocation4(Location))
                return EncounterMatchRating.PartialMatch;
            if ((pkm.Ball == (int)Ball.Sport) != (Type == SlotType.BugContest))
            {
                // Nincada => Shedinja can wipe the ball back to Poke
                if (pkm.Species != (int)Core.Species.Shedinja || pkm.Ball != (int)Ball.Poke)
                    return EncounterMatchRating.PartialMatch;
            }
            return base.GetMatchRating(pkm);
        }

        private Ball GetRequiredBallValue()
        {
            if (Type is SlotType.BugContest)
                return Ball.Sport;
            return Locations.IsSafariZoneLocation4(Location) ? Ball.Safari : Ball.None;
        }
    }
}
