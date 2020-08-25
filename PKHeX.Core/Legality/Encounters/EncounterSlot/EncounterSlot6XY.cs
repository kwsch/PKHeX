namespace PKHeX.Core
{
    public sealed class EncounterSlot6XY : EncounterSlot
    {
        public override int Generation => 6;
        public bool Pressure { get; set; }

        public EncounterSlot6XY(EncounterArea6XY area, int species, int form, int min, int max, GameVersion game)
        {
            Area = area;
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            Version = game;
        }

        public EncounterSlot6XY(EncounterArea6XYFriendSafari area, int species, int form, int min, int max, GameVersion game)
        {
            Area = area;
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            Version = game;
        }

        protected override void SetFormatSpecificData(PKM pk)
        {
            var pk6 = (PK6)pk;
            pk6.SetRandomMemory6();
        }

        public override string GetConditionString(out bool valid)
        {
            valid = true;
            return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
        }
    }
}