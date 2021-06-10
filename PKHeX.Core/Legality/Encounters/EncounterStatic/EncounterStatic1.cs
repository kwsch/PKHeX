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

        private const int LightBallPikachuCatchRate = 0xA3; // 163

        public EncounterStatic1(int species, int level, GameVersion game) : base(game)
        {
            Species = species;
            Level = level;
        }

        protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
        {
            base.ApplyDetails(sav, criteria, pk);

            var pk1 = (PK1) pk;
            if (Species == (int) Core.Species.Pikachu && Version == GameVersion.YW && Level == 5 && Moves.Count == 0)
            {
                pk1.Catch_Rate = LightBallPikachuCatchRate; // Light Ball
                return;
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            var table = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
            pk1.Catch_Rate = table[Species].CatchRate;
        }

        protected override bool IsMatchLevel(PKM pkm, DexLevel evo)
        {
            // Met Level is not stored in the PK1 format.
            // Check if it is at or above the encounter level.
            return Level <= evo.Level;
        }

        protected override bool IsMatchLocation(PKM pkm)
        {
            // Met Location is not stored in the PK1 format.
            return true;
        }

        protected override bool IsMatchPartial(PKM pkm)
        {
            if (pkm is not PK1 {Gen1_NotTradeback: true} pk1)
                return false;
            if (IsCatchRateValid(pk1))
                return false;
            return true;
        }

        private bool IsCatchRateValid(PK1 pk1)
        {
            var catch_rate = pk1.Catch_Rate;

            // Light Ball (Yellow) starter
            if (Version == GameVersion.YW && Species == (int)Core.Species.Pikachu && Level == 5)
            {
                return catch_rate == LightBallPikachuCatchRate;
            }
            if (Version == GameVersion.Stadium)
            {
                // Amnesia Psyduck has different catch rates depending on language
                if (Species == (int)Core.Species.Psyduck)
                    return catch_rate == (pk1.Japanese ? 167 : 168);
                return catch_rate is 167 or 168;
            }

            // Encounters can have different Catch Rates (RBG vs Y)
            return GBRestrictions.RateMatchesEncounter(Species, Version, catch_rate);
        }
    }
}
