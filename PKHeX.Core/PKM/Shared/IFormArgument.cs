using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    /// <summary>
    /// Alternate form data has an associated value.
    /// </summary>
    /// <remarks>
    /// <see cref="Species.Furfrou"/> How long (days) the form can last before reverting to Form-0 (5 days max)
    /// <see cref="Species.Hoopa"/>: How long (days) the form can last before reverting to Form-0 (3 days max)
    /// <see cref="Species.Alcremie"/>: Topping (Strawberry, Star, etc); [0,7]
    /// <see cref="Species.Yamask"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
    /// <see cref="Species.Runerigus"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
    /// </remarks>
    public interface IFormArgument
    {
        /// <summary>
        /// Argument for the associated <see cref="PKM.Form"/>
        /// </summary>
        uint FormArgument { get; set; }

        /// <summary>
        /// Amount of days the timed <see cref="PKM.Form"/> will remain active for.
        /// </summary>
        byte FormArgumentRemain { get; set; }

        /// <summary>
        /// Amount of days the timed <see cref="PKM.Form"/> has been active for.
        /// </summary>
        byte FormArgumentElapsed { get; set; }

        /// <summary>
        /// Maximum amount of days the <see cref="Species.Furfrou"/> has maintained a <see cref="PKM.Form"/> without reverting to its base form.
        /// </summary>
        byte FormArgumentMaximum { get; set; }
    }

    /// <summary>
    /// Logic for mutating an <see cref="IFormArgument"/> object.
    /// </summary>
    public static class FormArgumentUtil
    {
        /// <summary>
        /// Sets the suggested Form Argument to the <see cref="pkm"/>.
        /// </summary>
        public static void SetSuggestedFormArgument(this PKM pkm, int originalSpecies = 0)
        {
            if (pkm is not IFormArgument)
                return;
            if (!IsFormArgumentTypeDatePair(pkm.Species, pkm.Form))
            {
                var suggest = originalSpecies switch
                {
                    (int) Yamask when pkm.Species == (int) Runerigus => 49u,
                    _ => 0u,
                };
                pkm.ChangeFormArgument(suggest);
                return;
            }
            var max = GetFormArgumentMax(pkm.Species, pkm.Form, pkm.Format);
            pkm.ChangeFormArgument(max);
        }

        /// <summary>
        /// Modifies the <see cref="IFormArgument"/> values for the provided <see cref="pkm"/> to the requested <see cref="value"/>.
        /// </summary>
        public static void ChangeFormArgument(this PKM pkm, uint value)
        {
            if (pkm is not IFormArgument f)
                return;
            f.ChangeFormArgument(pkm.Species, pkm.Form, pkm.Format, value);
        }

        /// <summary>
        /// Modifies the <see cref="IFormArgument"/> values for the provided inputs to the requested <see cref="value"/>.
        /// </summary>
        /// <param name="f">Form Argument object</param>
        /// <param name="species">Entity Species</param>
        /// <param name="form">Entity Species</param>
        /// <param name="generation">Entity current format generation</param>
        /// <param name="value">Value to apply</param>
        public static void ChangeFormArgument(this IFormArgument f, int species, int form, int generation, uint value)
        {
            if (!IsFormArgumentTypeDatePair(species, form))
            {
                f.FormArgument = value;
                return;
            }

            var max = GetFormArgumentMax(species, form, generation);
            f.FormArgumentRemain = (byte)value;
            if (value == max)
            {
                f.FormArgumentElapsed = f.FormArgumentMaximum = 0;
                return;
            }

            byte elapsed = max < value ? (byte)0 : (byte)(max - value);
            f.FormArgumentElapsed = elapsed;
            if (species == (int)Furfrou)
                f.FormArgumentMaximum = Math.Max(f.FormArgumentMaximum, elapsed);
        }

        public static uint GetFormArgumentMax(int species, int form, int generation)
        {
            if (generation <= 5)
                return 0;

            return species switch
            {
                (int)Furfrou when form != 0 => 5,
                (int)Hoopa when form == 1 => 3,
                (int)Yamask when form == 1 => 9999,
                (int)Runerigus when form == 0 => 9999,
                (int)Alcremie => (uint)AlcremieDecoration.Ribbon,
                _ => 0,
            };
        }

        public static bool IsFormArgumentTypeDatePair(int species, int form)
        {
            return species switch
            {
                (int)Furfrou when form != 0 => true,
                (int)Hoopa when form == 1 => true,
                _ => false,
            };
        }
    }
}
