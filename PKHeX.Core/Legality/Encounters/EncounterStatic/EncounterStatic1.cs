namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Static Encounter
    /// </summary>
    /// <inheritdoc cref="EncounterStatic"/>
    public record EncounterStatic1 : EncounterStatic
    {
        public override int Generation => 1;
        public sealed override int Level { get; init; }

        public EncounterStatic1(int species, int level, GameVersion game) : base(game)
        {
            Species = species;
            Level = level;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            return true;
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (pkm is not PK1 pk1)
                return false;
            if (!pk1.Gen1_NotTradeback)
                return false;
            if (IsCatchRateValid(pk1))
                return false;
            return true;
        }

        private bool IsCatchRateValid(PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;
            if (Species == (int)Core.Species.Pikachu)
            {
                if (catch_rate == 190) // Red Blue Pikachu is not a static encounter
                    return false;
                if (catch_rate == 163 && Level == 5) // Light Ball (Yellow) starter
                    return true;
            }

            if (Version == GameVersion.Stadium)
            {
                // Amnesia Psyduck has different catch rates depending on language
                if (Species == (int)Core.Species.Psyduck)
                    return catch_rate == (pk1.Japanese ? 167 : 168);
                return catch_rate is 167 or 168;
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = Version == GameVersion.Y ? PersonalTable.Y : PersonalTable.RB;
            var rate = table[Species].CatchRate;
            return catch_rate == rate;
        }
    }
}
