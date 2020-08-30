namespace PKHeX.Core
{
    public sealed class EncounterSlot6XY : EncounterSlot
    {
        public override int Generation => 6;
        public bool Pressure { get; set; }

        public EncounterSlot6XY(EncounterArea6XY area, int species, int form, int min, int max) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
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

        public EncounterSlot6XY CreatePressureFormCopy(int evoForm)
        {
            var clone = (EncounterSlot6XY)Clone();
            clone.Form = evoForm;
            clone.Pressure = true;
            return clone;
        }
    }
}
