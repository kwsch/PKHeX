namespace PKHeX.Core
{
    public sealed class EncounterSlot6XY : EncounterSlot
    {
        public override int Generation => 6;
        public bool Pressure { get; set; }

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