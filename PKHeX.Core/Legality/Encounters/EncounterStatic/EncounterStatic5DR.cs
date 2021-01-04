namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Dream Radar gift encounters
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public sealed record EncounterStatic5DR : EncounterStatic5
    {
        public EncounterStatic5DR(int species, int form, int abilityIndex = 4) : base(GameVersion.B2W2)
        {
            Species = species;
            Form = form;
            Ability = abilityIndex;
            Location = 30015;
            Gift = true;
            Ball = 25;
            Level = 5; // to 40
            Shiny = Shiny.Never;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            // Level from 5->40 depends on the number of badges
            var met = pkm.Met_Level;
            if (met % 5 != 0)
                return false;
            return (uint) (met - 5) <= 35; // 5 <= x <= 40
        }
    }
}
