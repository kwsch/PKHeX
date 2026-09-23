using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Encounter Type for various <see cref="GameVersion.GO"/> encounters.
/// </summary>
public enum PogoType : byte
{
    None = 0, // Don't use this.

    // Pokémon captured in the wild.
    Wild,

    // Pokémon hatched from Eggs.
    Egg,
    Egg12km,

    // Pokémon captured after completing Raid Battles.
    Raid = 10,
    RaidMythical,
    RaidUltraBeast,
    RaidShadow,
    RaidShadowMythical,
    RaidShadowUltraBeast,

    // Pokémon captured after completing various types of Field Research or other in-game features.
    FieldResearch = 20,
    ResearchBreakthrough,
    SpecialResearch,
    TimedResearch,
    CollectionChallenge,
    VivillonCollector,
    PartyPlay,
    StampRally,
    GOPass,
    ReferralBonus,

    // Pokémon captured after winning Trainer Battles in the GO Battle League.
    GBL = 40,
    GBLMythical,
    GBLEvent,

    // Shadow Pokémon captured after defeating members of Team GO Rocket.
    Shadow = 50,
    ShadowMythical,
    ShadowUltraBeast,

    // Pokémon captured after completing Max Battles.
    MaxBattle = 60,
    MaxBattleMythical,
    MaxBattleUltraBeast,
    MaxBattleGigantamax,

    /// <summary> Pokémon captured from Special Research or Timed Research with a Premier Ball. </summary>
    /// <remarks>
    /// Niantic released version 0.269.0 on April 22, 2023, which contained an issue with the Remember Last-Used Poké Ball setting.
    /// This allowed for Premier Balls obtained from Raid Battles to be remembered on all future encounters.
    /// The moment the Premier Ball touched the floor or a wild Pokémon, the encounter would end, except if it was from a Special Research, Timed Research, or Collection Challenge encounter.
    /// This made it possible for over 300 species of Pokémon to be obtainable in a Poké Ball they were never meant to be captured in.
    /// This bug was fixed with the release of version 0.269.2.
    /// </remarks>
    PremierBallBug = 254,
    PremierBallBugMythical = 255,
}

public static class PogoExtensions
{
    private const Ball TransferSafariBall = Strange;

    extension(PogoType type)
    {
        public bool IsRaid => type is PogoType.Raid
            or PogoType.RaidMythical
            or PogoType.RaidUltraBeast
            or PogoType.RaidShadow
            or PogoType.RaidShadowMythical
            or PogoType.RaidShadowUltraBeast;

        public bool IsMaxBattle => type is PogoType.MaxBattle
            or PogoType.MaxBattleMythical
            or PogoType.MaxBattleUltraBeast
            or PogoType.MaxBattleGigantamax;
    }

    extension(PogoBallRestriction ball)
    {
        public Ball GetFixedBall() => ball switch
        {
            PogoBallRestriction.OnlyPoke => Poke,
            PogoBallRestriction.OnlyPremier => Premier,
            PogoBallRestriction.OnlyBeast => Beast,
            PogoBallRestriction.OnlySafari => TransferSafariBall,
            _ => None,
        };

        public bool IsValidBall(Ball current, PogoType type, PogoFlags flags)
        {
            if (current is Master)
                return ball.IsMasterBallUsable(type);
            if (current is TransferSafariBall)
                return ball.IsSafariBallUsable(type, flags);

            return ball switch
            {
                PogoBallRestriction.None => true,
                PogoBallRestriction.StandardMaster => current is Poke or Great or Ultra,
                PogoBallRestriction.Standard => current is Poke or Great or Ultra,
                PogoBallRestriction.OnlyPoke => current is Poke,
                PogoBallRestriction.OnlyPremier => current is Premier,
                PogoBallRestriction.OnlyBeast => current is Beast,
                _ => false,
            };
        }

        public bool IsMasterBallUsable(PogoType type) => ball is not (PogoBallRestriction.Standard or PogoBallRestriction.OnlyPoke or PogoBallRestriction.OnlyBeast);

        public bool IsSafariBallUsable(PogoType type, PogoFlags flags)
        {
            if (ball is PogoBallRestriction.OnlySafari)
                return true;
            if (!flags.HasFlag(PogoFlags.FeaturedWildArea) && (type.IsRaid || type.IsMaxBattle))
                return false;

            return type switch
            {
                PogoType.Egg or PogoType.Egg12km => false,
                PogoType.ResearchBreakthrough or PogoType.VivillonCollector or PogoType.ReferralBonus => false,
                PogoType.GBL or PogoType.GBLMythical or PogoType.GBLEvent => false,
                PogoType.Shadow or PogoType.ShadowMythical or PogoType.ShadowUltraBeast => false,
                _ => true,
            };
        }
    }
}

public enum PogoBallRestriction : byte
{
    None = 0,
    StandardMaster = 1,
    Standard = 2,
    OnlyPoke = 3,
    OnlyPremier = 4,
    OnlyBeast = 5,
    OnlySafari = 6,
}
