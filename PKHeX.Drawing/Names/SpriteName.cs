using System.Collections.Generic;
using System.Text;
using PKHeX.Core;

namespace PKHeX.Drawing
{
    public static class SpriteName
    {
        public static bool AllowShinySprite { get; set; }

        private const char Separator = '_';
        private const char Cosplay = 'c';
        private const char Shiny = 's';
        private const char GGStarter = 'p';

        /// <summary>
        /// Gets the resource name of the <see cref="Ball"/> sprite.
        /// </summary>
        public static string GetResourceStringBall(int ball) => $"_ball{ball}";

        /// <summary>
        /// Gets the resource name of the Pokémon sprite.
        /// </summary>
        public static string GetResourceStringSprite(int species, int form, int gender, uint formarg, int generation = PKX.Generation, bool shiny = false)
        {
            if (SpeciesDefaultFormSprite.Contains(species)) // Species who show their default sprite regardless of Form
                form = 0;

            var sb = new StringBuilder(12); // longest expected string result
            sb.Append(Separator).Append(species);

            if (form != 0)
            {
                sb.Append(Separator).Append(form);

                if (species == (int) Species.Pikachu)
                {
                    if (generation == 6)
                    {
                        sb.Append(Cosplay);
                        gender = 1; // Cosplay Pikachu gift can only be Female, but personal entries are set to be either Gender
                    }
                    else if (form == 8)
                    {
                        sb.Append(GGStarter);
                    }
                }
                else if (species == (int) Species.Eevee)
                {
                    if (form == 1)
                        sb.Append(GGStarter);
                }
            }
            if (gender == 1 && SpeciesGenderedSprite.Contains(species))
            {
                sb.Append('f');
            }

            if (species == (int) Species.Alcremie)
            {
                if (form == 0)
                    sb.Append(Separator).Append(form);
                sb.Append(Separator).Append(formarg);
            }

            if (shiny && AllowShinySprite)
                sb.Append(Shiny);
            return sb.ToString();
        }

        /// <summary>
        /// Species that show their default Species sprite regardless of current <see cref="PKM.Form"/>
        /// </summary>
        private static readonly HashSet<int> SpeciesDefaultFormSprite = new()
        {
            (int)Species.Mothim,
            (int)Species.Arceus,
            (int)Species.Scatterbug,
            (int)Species.Spewpa,
            (int)Species.Rockruff,
            (int)Species.Silvally,
            (int)Species.Mimikyu,
            (int)Species.Sinistea,
            (int)Species.Polteageist,
            (int)Species.Urshifu,
        };

        /// <summary>
        /// Species that show a <see cref="PKM.Gender"/> specific Sprite
        /// </summary>
        private static readonly HashSet<int> SpeciesGenderedSprite = new()
        {
            (int)Species.Pikachu,
            (int)Species.Hippopotas,
            (int)Species.Hippowdon,
            (int)Species.Unfezant,
            (int)Species.Frillish,
            (int)Species.Jellicent,
            (int)Species.Pyroar,
        };
    }
}
