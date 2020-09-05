namespace PKHeX.Core
{
    public sealed class EncounterStatic1 : EncounterStatic
    {
        public override int Generation => 1;

        public EncounterStatic1(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public EncounterStatic1(int species, int level, GameVersion ver)
        {
            Species = species;
            Level = level;
            Version = ver;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            return true;
        }

        public override bool IsMatchDeferred(PKM pkm)
        {
            if (pkm is PK1 pk1 && pk1.Gen1_NotTradeback && !IsCatchRateValid(pk1))
                return true;

            return !ParseSettings.AllowGBCartEra && GameVersion.GBCartEraOnly.Contains(Version);
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
                return catch_rate == 167 || catch_rate == 168;
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = Version == GameVersion.Y ? PersonalTable.Y : PersonalTable.RB;
            var rate = table[Species].CatchRate;
            return catch_rate == rate;
        }
    }
}
