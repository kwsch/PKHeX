namespace PKHeX.Core
{
    /// <summary>
    /// Event data for Generation 1
    /// </summary>
    public sealed class EncounterStatic1E : EncounterStatic1
    {
        public EncounterGBLanguage Language { get; set; } = EncounterGBLanguage.Japanese;

        /// <summary> Trainer name for the event. </summary>
        public string OT_Name { get; set; } = string.Empty;

        /// <summary> Trainer ID for the event. </summary>
        public int TID { get; set; } = -1;

        public EncounterStatic1E(int species, int level, GameVersion ver) : base(species, level, ver)
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

    /// <summary>
    /// Generations 1 &amp; 2 cannot communicate between Japanese &amp; International versions.
    /// </summary>
    public enum EncounterGBLanguage
    {
        Japanese,
        International,
        Any,
    }
}
