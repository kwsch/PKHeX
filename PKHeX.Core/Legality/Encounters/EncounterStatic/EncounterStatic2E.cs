namespace PKHeX.Core
{
    /// <summary>
    /// Event data for Generation 2
    /// </summary>
    public sealed class EncounterStatic2E : EncounterStatic2
    {
        public EncounterGBLanguage Language { get; set; } = EncounterGBLanguage.Japanese;

        /// <summary> Trainer name for the event. </summary>
        public string OT_Name { get; set; } = string.Empty;

        /// <summary> Trainer ID for the event. </summary>
        public int TID { get; set; } = -1;

        public EncounterStatic2E(int species, int level, GameVersion ver) : base(species, level, ver)
        {
        }

        public override bool IsMatch(PKM pkm, DexLevel evo)
        {
            if (!base.IsMatch(pkm, evo))
                return false;

            if (Language != EncounterGBLanguage.Any && pkm.Japanese != (Language == EncounterGBLanguage.Japanese))
                return false;

            if (OT_Name.Length != 0 && pkm.OT_Name != OT_Name)
                return false;

            if (TID != -1 && pkm.TID != TID)
                return false;

            return true;
        }

        public override bool IsMatchDeferred(PKM pkm)
        {
            if (base.IsMatchDeferred(pkm))
                return true;
            return !ParseSettings.AllowGBCartEra;
        }
    }
}
