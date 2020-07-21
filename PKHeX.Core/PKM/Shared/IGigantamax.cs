using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Interface that exposes an indication if the Pokémon can Gigantamax.
    /// </summary>
    public interface IGigantamax
    {
        /// <summary>
        /// Indicates if the Pokémon is capable of Gigantamax as opposed to regular Dynamax.
        /// </summary>
        bool CanGigantamax { get; set; }
    }

    public static class GigantamaxExtensions
    {
        /// <summary>
        /// Checks if either of the input Species can consume the Gigantamax soup, toggling the <see cref="IGigantamax.CanGigantamax"/> flag.
        /// </summary>
        /// <param name="_">Unnecessary, just needed for extension method usage.</param>
        /// <param name="currentSpecies">The current species</param>
        /// <param name="originSpecies">The original species (what species it was encountered as)</param>
        /// <returns>True if either species can toggle Gigantamax potential</returns>
        public static bool CanToggleGigantamax(this IGigantamax _, int currentSpecies, int originSpecies)
        {
            var soup = CanEatMaxSoup;
            return soup.Contains(currentSpecies) || (currentSpecies != originSpecies && soup.Contains(originSpecies));
        }

        /// <summary>
        /// Don't use this method. Use the other overload with multi-species input.
        /// </summary>
        /// <param name="_">Unnecessary, just needed for extension method usage.</param>
        /// <param name="currentSpecies">The current species</param>
        /// <returns>True if the species can toggle Gigantamax potential</returns>
        public static bool CanToggleGigantamax(this IGigantamax _, int currentSpecies)
        {
            var soup = CanEatMaxSoup;
            return soup.Contains(currentSpecies);
        }

        private static readonly HashSet<int> CanEatMaxSoup = new HashSet<int>
        {
            (int)Species.Venusaur,
            (int)Species.Charizard,
            (int)Species.Blastoise,
            (int)Species.Rillaboom,
            (int)Species.Cinderace,
            (int)Species.Inteleon,
            (int)Species.Butterfree,
            (int)Species.Pikachu,
            (int)Species.Meowth,
            (int)Species.Machamp,
            (int)Species.Gengar,
            (int)Species.Lapras,
            (int)Species.Eevee,
            (int)Species.Snorlax,
            (int)Species.Garbodor,
            (int)Species.Drednaw,
            (int)Species.Corviknight,
            (int)Species.Toxtricity,
            (int)Species.Alcremie,
            (int)Species.Duraludon,
            (int)Species.Orbeetle,
            (int)Species.Coalossal,
            (int)Species.Sandaconda,
            (int)Species.Grimmsnarl,
            (int)Species.Flapple,
            (int)Species.Appletun,
            (int)Species.Hatterene,
            (int)Species.Copperajah,
            (int)Species.Kingler,
            (int)Species.Centiskorch,
            (int)Species.Urshifu
        };
    }
}
