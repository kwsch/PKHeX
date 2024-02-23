using System;

using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Species;
using static PKHeX.Core.PotentialGBOrigin;
using static PKHeX.Core.TimeCapsuleEvaluation;

namespace PKHeX.Core;

/// <summary>
/// Miscellaneous GB Era restriction logic for legality checking
/// </summary>
internal static class GBRestrictions
{
    private static bool IsStadiumGiftSpecies(byte species) => species switch
    {
        (int)Bulbasaur => true,
        (int)Charmander => true,
        (int)Squirtle => true,
        (int)Psyduck => true,
        (int)Hitmonlee => true,
        (int)Hitmonchan => true,
        (int)Eevee => true,
        (int)Omanyte => true,
        (int)Kabuto => true,
        _ => false,
    };

    /// <summary>
    /// Species that have a catch rate value that is different from their pre-evolutions, and cannot be obtained directly.
    /// </summary>
    internal static bool IsSpeciesNotAvailableCatchRate(byte species) => species switch
    {
        (int)Butterfree => true,
        (int)Pidgeot => true,
        (int)Nidoqueen => true,
        (int)Nidoking => true,
        (int)Ninetales => true,
        (int)Vileplume => true,
        (int)Persian => true,
        (int)Arcanine => true,
        (int)Poliwrath => true,
        (int)Alakazam => true,
        (int)Machamp => true,
        (int)Victreebel => true,
        (int)Rapidash => true,
        (int)Cloyster => true,
        (int)Exeggutor => true,
        (int)Starmie => true,
        (int)Dragonite => true,
        _ => false,
    };

    private static bool IsEvolvedFromGen1Species(ushort species) => species switch
    {
        (int)Crobat => true,
        (int)Bellossom => true,
        (int)Politoed => true,
        (int)Espeon => true,
        (int)Umbreon => true,
        (int)Slowking => true,
        (int)Steelix => true,
        (int)Scizor => true,
        (int)Kingdra => true,
        (int)Porygon2 => true,
        (int)Blissey => true,
        (int)Magnezone => true,
        (int)Lickilicky => true,
        (int)Rhyperior => true,
        (int)Tangrowth => true,
        (int)Electivire => true,
        (int)Magmortar => true,
        (int)Leafeon => true,
        (int)Glaceon => true,
        (int)PorygonZ => true,
        (int)Sylveon => true,
        (int)Kleavor => true,
        _ => false,
    };

    internal static bool IsTradeEvolution1(ushort species) => species is (int)Kadabra or (int)Machoke or (int)Graveler or (int)Haunter;

    public static bool RateMatchesEncounter(ushort species, GameVersion version, byte rate)
    {
        if (version.Contains(YW))
        {
            if (rate == PersonalTable.Y[species].CatchRate)
                return true;
            if (version == YW) // no RB
                return false;
        }
        return rate == PersonalTable.RB[species].CatchRate;
    }

    private static bool RateMatchesEither(byte rate, ushort species)
    {
        return rate == PersonalTable.RB[species].CatchRate || rate == PersonalTable.Y[species].CatchRate;
    }

    private static bool GetCatchRateMatchesPreEvolution(PK1 pk, byte rate)
    {
        // For species catch rate, discard any species that has no valid encounters and a different catch rate than their pre-evolutions
        var head = new EvoCriteria { Species = pk.Species, Form = pk.Form, LevelMax = pk.CurrentLevel }; // as struct to avoid boxing
        do
        {
            var species = head.Species;
            if (!IsSpeciesNotAvailableCatchRate((byte)species) && RateMatchesEither(rate, species))
                return true;
        }
        while (EvolutionGroup1.Instance.TryDevolve(head, pk, head.LevelMax, 2, false, out head));

        // Account for oddities via special catch rate encounters
        if (rate is 167 or 168 && IsStadiumGiftSpecies((byte)head.Species))
            return true;
        return false;
    }

    /// <summary>
    /// Checks if the <see cref="pk"/> can inhabit <see cref="Gen1"></see>
    /// </summary>
    /// <param name="pk">Data to check</param>
    /// <returns>true if it can inhabit, false if it can not.</returns>
    internal static bool CanInhabitGen1(this PKM pk)
    {
        // Korean Gen2 games can't trade-back because there are no Gen1 Korean games released
        if (pk.Korean || pk.IsEgg)
            return false;

        // Gen2 format with met data can't receive Gen1 moves, unless Stadium 2 is used (Oak's PC).
        // If you put a Pokémon in the N64 box, the met info is retained, even if you switch over to a Gen1 game to teach it TMs
        // You can use rare candies from within the lab, so level-up moves from RBY context can be learned this way as well
        // Stadium 2 is GB Cart Era only (not 3DS Virtual Console).
        if (pk is ICaughtData2 {CaughtData: not 0} && !ParseSettings.AllowGBStadium2)
            return false;

        // Sanity check species, if it could have existed as a pre-evolution.
        ushort species = pk.Species;
        if (species <= MaxSpeciesID_1)
            return true;
        return IsEvolvedFromGen1Species(species);
    }

    /// <summary>
    /// Gets the Tradeback status depending on various values.
    /// </summary>
    /// <param name="pk">Pokémon to guess the Tradeback status from.</param>
    internal static PotentialGBOrigin GetTradebackStatusInitial(PKM pk)
    {
        if (pk is PK1 pk1)
            return GetTradebackStatusRBY(pk1);

        if (pk.Format == 2 || pk.VC2) // Check for impossible Tradeback scenarios
            return !pk.CanInhabitGen1() ? Gen2Only : Either;

        // VC2 is released, we can assume it will be TradebackType.Any.
        // Is impossible to differentiate a VC1 Pokémon traded to Gen7 after VC2 is available.
        // Met Date cannot be used definitively as the player can change their system clock.
        return Either;
    }

    /// <summary>
    /// Gets the Tradeback status depending on the <see cref="PK1.CatchRate"/>
    /// </summary>
    /// <param name="pk">Pokémon to guess the Tradeback status from.</param>
    private static PotentialGBOrigin GetTradebackStatusRBY(PK1 pk)
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return Gen1Only;

        // Detect Tradeback status by comparing the catch rate(Gen1)/held item(Gen2) to the species in the Pokémon's evolution chain.
        var catch_rate = pk.CatchRate;
        if (catch_rate == 0)
            return Either;

        bool matchAny = GetCatchRateMatchesPreEvolution(pk, catch_rate);
        if (!matchAny)
            return Either;

        if (IsTradebackCatchRate(catch_rate))
            return Either;

        return Gen1Only;
    }

    public static TimeCapsuleEvaluation IsTimeCapsuleTransferred(PKM pk, ReadOnlySpan<MoveResult> moves, IEncounterTemplate enc)
    {
        foreach (var z in moves)
        {
            if (z.Generation == enc.Generation || z.Generation is not (1 or 2))
                continue;
            if (pk is PK1 {CatchRate: not 0} g1 && !IsTradebackCatchRate(g1.CatchRate))
                return BadCatchRate;
            return enc.Generation == 2 ? Transferred21 : Transferred12;
        }

        if (pk is not GBPKM gb)
        {
            return enc.Generation switch
            {
                1 when pk.VC2 => Transferred12,
                2 when pk.VC1 => Transferred21,
                _ => NotTransferred,
            };
        }

        if (gb is ICaughtData2 pk2)
        {
            if (enc.Generation == 1)
                return Transferred12;
            if (pk2.CaughtData != 0)
                return NotTransferred;
            if (enc.Version == C)
                return Transferred21;
            return Indeterminate;
        }

        if (gb is PK1 pk1)
        {
            var rate = pk1.CatchRate;
            if (rate == 0)
                return Transferred12;

            bool isTradebackItem = IsTradebackCatchRate(rate);
            if (IsCatchRateMatchEncounter(enc, pk1))
                return isTradebackItem ? Indeterminate : NotTransferred;
            return isTradebackItem ? Transferred12 : BadCatchRate;
        }
        return Indeterminate;
    }

    private static bool IsCatchRateMatchEncounter(IEncounterTemplate enc, PK1 pk1) => enc switch
    {
        EncounterGift1 g when g.GetMatchRating(pk1) < EncounterMatchRating.PartialMatch => true,
        EncounterStatic1 s when s.GetMatchRating(pk1) < EncounterMatchRating.PartialMatch => true,
        EncounterTrade1 => true,
        _ => RateMatchesEncounter(enc.Species, enc.Version, pk1.CatchRate),
    };

    public static bool IsTradebackCatchRate(byte rate) => Array.IndexOf(HeldItems_GSC, rate) != -1;
}

/// <summary>
/// Guess of the origin of a GB Pokémon.
/// </summary>
public enum PotentialGBOrigin
{
    /// <summary>
    /// Pokémon is possible from either generation.
    /// </summary>
    Either,

    /// <summary>
    /// Pokémon is only possible in Generation 1.
    /// </summary>
    Gen1Only,

    /// <summary>
    /// Pokémon is only possible in Generation 2.
    /// </summary>
    Gen2Only,
}

/// <summary>
/// Indicates if the entity has been transferred between Generation 1-2 games via the Time Capsule.
/// </summary>
public enum TimeCapsuleEvaluation
{
    /// <summary>
    /// Transferring via Time Capsule cannot be inferred.
    /// </summary>
    Indeterminate,

    /// <summary>
    /// Indicates that the entity was transferred from Generation 2 to Generation 1.
    /// </summary>
    Transferred21,

    /// <summary>
    /// Indicates that the entity was transferred from Generation 1 to Generation 2, but the catch rate is not a valid tradeback item.
    /// </summary>
    Transferred12,

    /// <summary>
    /// Was not transferred via the Time Capsule.
    /// </summary>
    NotTransferred,

    /// <summary>
    /// Has a catch rate that does not match a held item or the original catch rate value for any progenitor species.
    /// </summary>
    BadCatchRate,
}

/// <summary>
/// Extension methods for <see cref="TimeCapsuleEvaluation"/>.
/// </summary>
public static class TimeCapsuleEvlautationExtensions
{
    /// <summary>
    /// Indicates if the <see cref="eval"/> definitely transferred via Time Capsule.
    /// </summary>
    public static bool WasTimeCapsuleTransferred(this TimeCapsuleEvaluation eval) => eval is not (Indeterminate or NotTransferred or BadCatchRate);
}
