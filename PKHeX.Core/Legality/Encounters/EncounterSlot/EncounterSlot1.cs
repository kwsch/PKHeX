namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Wild Encounter Slot data
    /// </summary>
    public sealed class EncounterSlot1 : EncounterSlot, INumberedSlot
    {
        public override int Generation => 1;
        public int SlotNumber { get; set; }

        public EncounterSlot1(EncounterArea1 area, int species, int min, int max, int slot, GameVersion game)
        {
            Area = area;
            Species = species;
            LevelMin = min;
            LevelMax = max;
            SlotNumber = slot;
            Version = game;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk1 = (PK1)pk;
            if (Version == GameVersion.YW)
            {
                pk1.Catch_Rate = Species switch
                {
                    (int) Core.Species.Kadabra => 96,
                    (int) Core.Species.Dragonair => 27,
                    _ => PersonalTable.RB[Species].CatchRate
                };
            }
            else
            {
                pk1.Catch_Rate = PersonalTable.RB[Species].CatchRate; // RB
            }
        }
    }
}
