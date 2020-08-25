namespace PKHeX.Core
{
    public sealed class EncounterSlot6AO : EncounterSlot
    {
        public override int Generation => 6;

        public EncounterSlot6AO(EncounterArea6AO area, int species, int form, int min, int max, GameVersion game) : base(area)
        {
            Species = species;
            Form = form;
            LevelMin = min;
            LevelMax = max;
            Version = game;
        }

        public bool Pressure { get; set; }
        public bool AllowDexNav { get; set; }
        public bool DexNav { get; set; }
        public bool WhiteFlute { get; set; }
        public bool BlackFlute { get; set; }

        private bool IsDexNav => AllowDexNav && DexNav;

        protected override void SetFormatSpecificData(PKM pk)
        {
            var pk6 = (PK6)pk;
            if (IsDexNav)
            {
                var eggMoves = MoveEgg.GetEggMoves(pk, Species, Form, Version);
                if (eggMoves.Length > 0)
                    pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
            }
            pk6.SetRandomMemory6();
        }

        public override string GetConditionString(out bool valid)
        {
            valid = true;
            if (WhiteFlute) // Decreased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionWhiteLead : LegalityCheckStrings.LEncConditionWhite;
            if (BlackFlute) // Increased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionBlackLead : LegalityCheckStrings.LEncConditionBlack;
            if (DexNav)
                return LegalityCheckStrings.LEncConditionDexNav;

            return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
        }
    }
}