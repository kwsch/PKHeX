namespace PKHeX.Core
{
    public sealed class EncounterSlot3PokeSpot : EncounterSlot, INumberedSlot
    {
        public override int Generation => 3;

        public int SlotNumber { get; set; }

        public EncounterSlot3PokeSpot(EncounterArea3XD area, int species, int min, int max, int slot) : base(area)
        {
            Species = species;
            LevelMin = min;
            LevelMax = max;
            SlotNumber = slot;
            Version = GameVersion.XD;
        }

        // PokeSpot encounters always have Fateful Encounter set.
        protected override void SetFormatSpecificData(PKM pk) => pk.FatefulEncounter = true;

        protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
        {
            int gender = criteria.GetGender(-1, pk.PersonalInfo);
            int nature = (int)criteria.GetNature(Nature.Random);
            int ability = Util.Rand.Next(2);
            PIDGenerator.SetRandomPokeSpotPID(pk, nature, gender, ability, SlotNumber);
            pk.Gender = gender;
            pk.StatNature = nature;
        }
    }
}
