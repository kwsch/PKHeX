using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Exposes properties about a Pokémon being capable of Gigantamax as opposed to regular Dynamax.
/// </summary>
public interface IGigantamaxReadOnly
{
    /// <summary>
    /// Indicates if the Pokémon is capable of Gigantamax as opposed to regular Dynamax.
    /// </summary>
    bool CanGigantamax { get; }
}

/// <summary>
/// Permission checks for Gigantamax.
/// </summary>
public static class Gigantamax
{
    /// <summary>
    /// Checks if either of the input Species can consume the Gigantamax soup, toggling the <see cref="IGigantamax.CanGigantamax"/> flag.
    /// </summary>
    /// <param name="currentSpecies">The current species</param>
    /// <param name="currentForm">The current form of the species</param>
    /// <param name="originSpecies">The original species (what species it was encountered as)</param>
    /// <param name="originForm">The original form of the original species</param>
    /// <returns>True if either species can toggle Gigantamax potential</returns>
    public static bool CanToggle(ushort currentSpecies, byte currentForm, ushort originSpecies, byte originForm)
    {
        if (currentSpecies is (int)Meowth or (int)Pikachu)
            return currentForm == 0;

        return CanToggle(currentSpecies) || (currentSpecies != originSpecies && CanToggle(originSpecies));
    }

    /// <summary>
    /// Checks if the input Species can consume the Gigantamax soup, toggling the <see cref="IGigantamax.CanGigantamax"/> flag.
    /// </summary>
    /// <param name="species">The current species</param>
    /// <param name="form">The current form of the species</param>
    /// <returns>True if the species can toggle Gigantamax potential</returns>
    public static bool CanToggle(ushort species, byte form)
    {
        if (species is (int)Meowth or (int)Pikachu)
            return form == 0;
        return CanToggle(species);
    }

    /// <summary>
    /// Checks if the input Species can consume the Gigantamax soup, toggling the <see cref="IGigantamax.CanGigantamax"/> flag.
    /// </summary>
    /// <param name="species">The current species</param>
    /// <remarks>General case, includes Pikachu and Meowth which require Form == 0.</remarks>
    public static bool CanToggle(ushort species) => species switch
    {
        (ushort)Venusaur => true,
        (ushort)Charizard => true,
        (ushort)Blastoise => true,
        (ushort)Butterfree => true,
        (ushort)Pikachu => true,
        (ushort)Meowth => true,
        (ushort)Machamp => true,
        (ushort)Gengar => true,
        (ushort)Kingler => true,
        (ushort)Lapras => true,
        (ushort)Eevee => true,
        (ushort)Snorlax => true,
        (ushort)Garbodor => true,

        (ushort)Rillaboom => true, // DLC 1
        (ushort)Cinderace => true, // DLC 1
        (ushort)Inteleon => true, // DLC 1
        (ushort)Corviknight => true,
        (ushort)Orbeetle => true,
        (ushort)Drednaw => true,
        (ushort)Coalossal => true,
        (ushort)Flapple => true,
        (ushort)Appletun => true,
        (ushort)Sandaconda => true,
        (ushort)Toxtricity => true,
        (ushort)Centiskorch => true,
        (ushort)Hatterene => true,
        (ushort)Grimmsnarl => true,
        (ushort)Alcremie => true,
        (ushort)Copperajah => true,
        (ushort)Duraludon => true,
        (ushort)Urshifu => true, // DLC 1
        _ => false,
    };
}
