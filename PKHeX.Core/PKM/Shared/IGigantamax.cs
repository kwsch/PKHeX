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
        public static bool CanToggleGigantamax(this IGigantamax _, int currentSpecies, int originSpecies)
        {
            var soup = Legal.CanEatMaxSoup;
            return soup.Contains(currentSpecies) || soup.Contains(originSpecies);
        }
    }
}
