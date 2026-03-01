using static PKHeX.Core.PogoType;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Encounter Type for various <see cref="GameVersion.GO"/> encounters.
/// </summary>
public enum PogoType : byte
{
    None, // Don't use this.

    // Pokémon captured in the wild.
    Wild,

    // Pokémon hatched from Eggs.
    Egg,
    Egg12km,

    // Pokémon captured after completing Raid Battles. IV, Level, and Poké Ball permissions may vary depending on the Pokémon.
    Raid = 10,
    RaidMythical,
    RaidUltraBeast,
    RaidShadow,
    RaidShadowMythical,
    RaidGOWA,
    RaidMythicalGOWA,
    RaidUltraBeastGOWA,
    RaidShadowGOWA,
    RaidShadowMythicalGOWA,

    // Pokémon captured after completing various types of Field Research.
    FieldResearch = 20,
    FieldResearchLevelRange,
    ResearchBreakthrough,
    SpecialResearch,
    TimedResearch,
    CollectionChallenge,
    VivillonCollector,
    PartyPlay,
    StampRally,
    GOPass,
    ReferralBonus,

    // Pokémon captured after completing Special Research. IV, Level, and Poké Ball permissions may vary depending on the Pokémon.
    SpecialMythical = 40,
    SpecialMythicalPoke,
    SpecialUltraBeast,
    SpecialGigantamax,
    SpecialPoke,
    SpecialLastBall,
    SpecialNoHUD,
    SpecialLevel10,
    SpecialLevel20,
    SpecialLevelRange,
    SpecialMythicalLevel10,
    SpecialMythicalLevel20,
    SpecialMythicalLevelRange,
    SpecialUltraBeastLevel10,
    SpecialUltraBeastLevel20,
    SpecialUltraBeastLevelRange,
    SpecialGigantamaxLevel10,
    SpecialGigantamaxLevel20,
    SpecialGigantamaxLevelRange,

    // Pokémon captured after completing Timed Research or GO Passes. IV, Level, and Poké Ball permissions may vary depending on the Pokémon.
    TimedMythical = 60,
    TimedMythicalPoke,
    TimedUltraBeast,
    TimedGigantamax,
    TimedPoke,
    TimedLastBall,
    TimedNoHUD,
    TimedLevel10,
    TimedLevel20,
    TimedLevelRange,
    TimedMythicalLevel10,
    TimedMythicalLevel20,
    TimedMythicalLevelRange,
    TimedUltraBeastLevel10,
    TimedUltraBeastLevel20,
    TimedUltraBeastLevelRange,
    TimedGigantamaxLevel10,
    TimedGigantamaxLevel20,
    TimedGigantamaxLevelRange,
    TimedShadow,
    TimedShadowLevel10,
    TimedShadowLevel20,
    TimedShadowLevelRange,
    TimedShadowMythical,
    TimedShadowMythicalLevel10,
    TimedShadowMythicalLevel20,
    TimedShadowMythicalLevelRange,

    // Pokémon captured after winning Trainer Battles in the GO Battle League.
    GBL = 90,
    GBLMythical,
    GBLEvent,

    // Shadow Pokémon captured after defeating members of Team GO Rocket.
    Shadow = 100,
    ShadowMythical,
    ShadowUltraBeast,

    // Pokémon captured after completing Max Battles.
    MaxBattle = 110,
    MaxBattleMythical,
    MaxBattleUltraBeast,
    MaxBattleGigantamax,
    MaxBattleGOWA,
    MaxBattleMythicalGOWA,
    MaxBattleUltraBeastGOWA,
    MaxBattleGigantamaxGOWA,

    /// <summary> Pokémon captured from Special Research or Timed Research with a Premier Ball. </summary>
    /// <remarks>
    /// Niantic released version 0.269.0 on April 22, 2023, which contained an issue with the Remember Last-Used Poké Ball setting.
    /// This allowed for Premier Balls obtained from Raid Battles to be remembered on all future encounters.
    /// The moment the Premier Ball touched the floor or a wild Pokémon, the encounter would end, except if it was from a Special Research, Timed Research, or Collection Challenge encounter.
    /// This made it possible for over 300 species of Pokémon to be obtainable in a Poké Ball they were never meant to be captured in.
    /// This bug was fixed with the release of version 0.269.2.
    /// </remarks>
    PremierBallBug = 254,
    PremierBallBugMythical,
}

/// <summary>
/// Extension methods for <see cref="PogoType"/>.
/// </summary>
public static class PogoTypeExtensions
{
    extension(PogoType encounterType)
    {
        /// <summary>
        /// Gets the minimum level (relative to GO's 1-<see cref="EncountersGO.MAX_LEVEL"/>) the <see cref="encounterType"/> must have.
        /// </summary>
        public byte LevelMin => encounterType switch
        {
            Wild => 1,
            Egg => 1,
            Egg12km => 8,
            Raid => 20,
            RaidMythical => 20,
            RaidUltraBeast => 20,
            RaidShadow => 20,
            RaidShadowMythical => 20,
            RaidGOWA => 20,
            RaidMythicalGOWA => 20,
            RaidUltraBeastGOWA => 20,
            RaidShadowGOWA => 20,
            RaidShadowMythicalGOWA => 20,
            FieldResearchLevelRange => 1,
            SpecialLevel10 => 10,
            SpecialLevel20 => 20,
            SpecialLevelRange => 1,
            SpecialMythicalLevel10 => 10,
            SpecialMythicalLevel20 => 20,
            SpecialMythicalLevelRange => 1,
            SpecialUltraBeastLevel10 => 10,
            SpecialUltraBeastLevel20 => 20,
            SpecialUltraBeastLevelRange => 1,
            SpecialGigantamaxLevel10 => 10,
            SpecialGigantamaxLevel20 => 20,
            SpecialGigantamaxLevelRange => 1,
            TimedLevel10 => 10,
            TimedLevel20 => 20,
            TimedLevelRange => 1,
            TimedMythicalLevel10 => 10,
            TimedMythicalLevel20 => 20,
            TimedMythicalLevelRange => 1,
            TimedUltraBeastLevel10 => 10,
            TimedUltraBeastLevel20 => 20,
            TimedUltraBeastLevelRange => 1,
            TimedGigantamaxLevel10 => 10,
            TimedGigantamaxLevel20 => 20,
            TimedGigantamaxLevelRange => 1,
            TimedShadowLevel10 => 10,
            TimedShadowLevel20 => 20,
            TimedShadowLevelRange => 1,
            TimedShadowMythicalLevel10 => 10,
            TimedShadowMythicalLevel20 => 20,
            TimedShadowMythicalLevelRange => 1,
            GBL => 20,
            GBLMythical => 20,
            GBLEvent => 20,
            Shadow => 8,
            ShadowMythical => 25,
            ShadowUltraBeast => 8,
            MaxBattle => 20,
            MaxBattleMythical => 20,
            MaxBattleUltraBeast => 20,
            MaxBattleGigantamax => 20,
            MaxBattleGOWA => 20,
            MaxBattleMythicalGOWA => 20,
            MaxBattleUltraBeastGOWA => 20,
            MaxBattleGigantamaxGOWA => 20,
            _ => 15,
        };

        /// <summary>
        /// Gets the minimum IVs (relative to GO's 0-15) the <see cref="encounterType"/> must have.
        /// </summary>
        /// <returns>Required minimum IV (0-15)</returns>
        public int MinimumIV => encounterType switch
        {
            Wild => 0,
            RaidMythical => 10,
            RaidShadowMythical => 8,
            RaidShadowMythicalGOWA => 8,
            SpecialMythical => 10,
            SpecialMythicalPoke => 10,
            SpecialLastBall => 10,
            SpecialNoHUD => 10,
            SpecialMythicalLevel10 => 10,
            SpecialMythicalLevel20 => 10,
            SpecialMythicalLevelRange => 10,
            TimedMythical => 10,
            TimedMythicalPoke => 10,
            TimedLastBall => 10,
            TimedNoHUD => 10,
            TimedMythicalLevel10 => 10,
            TimedMythicalLevel20 => 10,
            TimedMythicalLevelRange => 10,
            TimedShadowMythical => 10,
            TimedShadowMythicalLevel10 => 10,
            TimedShadowMythicalLevel20 => 10,
            TimedShadowMythicalLevelRange => 10,
            GBLMythical => 10,
            GBLEvent => 0,
            ShadowMythical => 2,
            MaxBattleMythical => 10,
            MaxBattleMythicalGOWA => 10,
            PremierBallBugMythical => 10,
            _ => 1,
        };

        /// <summary>
        /// Checks if the <see cref="ball"/> is valid for the <see cref="encounterType"/>.
        /// </summary>
        /// <param name="ball">Current <see cref="Ball"/> the Pokémon is in.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool IsBallValid(Ball ball)
        {
            var req = encounterType.GetValidBall();
            if (req == Ball.None)
                return (uint)(ball - 2) <= 2; // Poké, Great, Ultra
            return ball == req;
        }

        /// <summary>
        /// Checks if <see cref="Ball.Master"/> can be used for the <see cref="encounterType"/>.
        /// </summary>
        /// <returns>True if valid, false if invalid.</returns>
        public bool IsMasterBallUsable => encounterType switch
        {
            Egg or Egg12km => false,
            SpecialMythicalPoke or SpecialUltraBeast or SpecialPoke or SpecialLastBall or SpecialNoHUD or SpecialUltraBeastLevel10 or SpecialUltraBeastLevel20 or SpecialUltraBeastLevelRange => false,
            TimedMythicalPoke or TimedUltraBeast or TimedPoke or TimedLastBall or TimedNoHUD or TimedUltraBeastLevel10 or TimedUltraBeastLevel20 or TimedUltraBeastLevelRange => false,
            PremierBallBug or PremierBallBugMythical => false,
            _ => true,
        };

        /// <summary>
        /// Checks if <see cref="Ball.Safari"/> can be used for the <see cref="encounterType"/>.
        /// </summary>
        /// <returns>True if valid, false if invalid.</returns>
        public bool IsSafariBallUsable(ushort species) => encounterType switch
        {
            Egg or Egg12km => false,
            Raid or RaidMythical or RaidUltraBeast or RaidShadow or RaidShadowMythical => false,
            ResearchBreakthrough or VivillonCollector or ReferralBonus => false,
            TimedMythicalPoke or TimedUltraBeast or TimedPoke or TimedLastBall or TimedNoHUD or TimedUltraBeastLevel10 or TimedUltraBeastLevel20 or TimedUltraBeastLevelRange => false,
            GBL or GBLMythical or GBLEvent => false,
            Shadow or ShadowMythical or ShadowUltraBeast => false,
            MaxBattle or MaxBattleMythical or MaxBattleUltraBeast or MaxBattleGigantamax => false,
            PremierBallBug or PremierBallBugMythical => false,
            _ when encounterType.IsSpecialResearch && SpeciesCategory.IsSpecialPokemon(species) => false,
            _ => true,
        };

        /// <summary>
        /// Gets a valid ball that the <see cref="encounterType"/> can have based on the type of capture in Pokémon GO.
        /// </summary>
        /// <returns><see cref="Ball.None"/> if no specific ball is required, otherwise returns the required ball.</returns>
        public Ball GetValidBall() => encounterType switch
        {
            Egg => Poke,
            Egg12km => Poke,
            Raid => Premier,
            RaidMythical => Premier,
            RaidUltraBeast => Beast,
            RaidShadow => Premier,
            RaidShadowMythical => Premier,
            RaidGOWA => Premier,
            RaidMythicalGOWA => Premier,
            RaidUltraBeastGOWA => Beast,
            RaidShadowGOWA => Premier,
            RaidShadowMythicalGOWA => Premier,
            SpecialMythicalPoke => Poke,
            SpecialUltraBeast => Beast,
            SpecialPoke => Poke,
            SpecialUltraBeastLevel10 => Beast,
            SpecialUltraBeastLevel20 => Beast,
            SpecialUltraBeastLevelRange => Beast,
            TimedMythicalPoke => Poke,
            TimedUltraBeast => Beast,
            TimedPoke => Poke,
            TimedUltraBeastLevel10 => Beast,
            TimedUltraBeastLevel20 => Beast,
            TimedUltraBeastLevelRange => Beast,
            PremierBallBug => Premier,
            PremierBallBugMythical => Premier,
            Shadow => Premier,
            ShadowMythical => Premier,
            ShadowUltraBeast => Beast,
            MaxBattle => Premier,
            MaxBattleMythical => Premier,
            MaxBattleUltraBeast => Beast,
            MaxBattleGigantamax => Premier,
            MaxBattleGOWA => Premier,
            MaxBattleMythicalGOWA => Premier,
            MaxBattleUltraBeastGOWA => Beast,
            MaxBattleGigantamaxGOWA => Premier,
            _ => Ball.None, // Poké, Great, Ultra
        };

        public bool IsSpecialResearch => encounterType is >= SpecialMythical and < TimedMythical;
    }
}
