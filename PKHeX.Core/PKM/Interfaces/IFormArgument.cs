using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Alternate form data has an associated value.
/// </summary>
/// <remarks>
/// <see cref="Furfrou"/> How long (days) the form can last before reverting to Form-0 (5 days max)
/// <see cref="Hoopa"/>: How long (days) the form can last before reverting to Form-0 (3 days max)
/// <see cref="Alcremie"/>: Topping (Strawberry, Star, etc.); [0,7]
/// <see cref="Yamask"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
/// <see cref="Runerigus"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
/// <see cref="Stantler"/> How many times the Pokémon has used Psyshield Bash in the Agile Style [0,9999].
/// <see cref="Qwilfish"/> How many times the Pokémon has used Barb Barrage in the Strong Style as Qwilfish-1 [0,9999].
/// <see cref="Basculin"/> How much damage the Pokémon has taken through recoil as Basculin-2 [0,9999].
/// <see cref="Primeape"/> How many times the Pokémon has used Rage Fist [0,9999].
/// <see cref="Bisharp"/> How many Bisharp that head up a group of Pawniard have been KOed [0,9999].
/// <see cref="Gimmighoul"/> How many Gimmighoul Coins were in the player's Bag after last leveling up [0,998].
/// <see cref="Gholdengo"/> How many Gimmighoul Coins were used on Gimmighoul to evolve into this Pokémon.
/// <see cref="Koraidon"/> Flags whether this Pokémon was originally in its Ride Form (0/1).
/// <see cref="Miraidon"/> Flags whether this Pokémon was originally in its Ride Form (0/1).
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
/// Logic for mutating <see cref="IFormArgument"/> objects.
/// </summary>
public static class FormArgumentUtil
{
    /// <summary>
    /// Sets the suggested Form Argument to the <see cref="pk"/>.
    /// </summary>
    public static void SetSuggestedFormArgument(this PKM pk, ushort originalSpecies = 0)
    {
        if (pk is not IFormArgument)
            return;
        uint value = IsFormArgumentTypeDatePair(pk.Species, pk.Form)
            ? GetFormArgumentMax(pk.Species, pk.Form, pk.Context)
            : GetFormArgumentMinEvolution(pk.Species, originalSpecies);
        if (pk.Species is (int)Hoopa && pk.Format >= 8)
            value = 0; // S/V does not set the argument for Hoopa
        pk.ChangeFormArgument(value);
    }

    /// <summary>
    /// Modifies the <see cref="IFormArgument"/> values for the provided <see cref="pk"/> to the requested <see cref="value"/>.
    /// </summary>
    public static void ChangeFormArgument(this PKM pk, uint value)
    {
        if (pk is not IFormArgument f)
            return;
        f.ChangeFormArgument(pk.Species, pk.Form, pk.Context, value);
    }

    /// <summary>
    /// Modifies the <see cref="IFormArgument"/> values for the provided inputs to the requested <see cref="value"/>.
    /// </summary>
    /// <param name="f">Form Argument object</param>
    /// <param name="species">Entity Species</param>
    /// <param name="form">Entity Species</param>
    /// <param name="context">Entity current context</param>
    /// <param name="value">Value to apply</param>
    public static void ChangeFormArgument(this IFormArgument f, ushort species, byte form, EntityContext context, uint value)
    {
        if (!IsFormArgumentTypeDatePair(species, form))
        {
            f.FormArgument = value;
            return;
        }

        var max = GetFormArgumentMax(species, form, context);
        f.FormArgumentRemain = (byte)value;
        if (value == max || (value == 0 && species is (int)Hoopa && form == 1 && context.Generation() >= 8))
        {
            f.FormArgumentElapsed = f.FormArgumentMaximum = 0;
            return;
        }

        byte elapsed = max < value ? (byte)0 : (byte)(max - value);
        f.FormArgumentElapsed = elapsed;
        if (species == (int)Furfrou)
            f.FormArgumentMaximum = Math.Max(f.FormArgumentMaximum, elapsed);
    }

    /// <summary>
    /// Gets the maximum value the <see cref="IFormArgument.FormArgument"/> can be.
    /// </summary>
    /// <param name="species">Entity Species</param>
    /// <param name="form">Entity Form</param>
    /// <param name="context">Context to check with.</param>
    public static uint GetFormArgumentMax(ushort species, byte form, EntityContext context)
    {
        var gen = context.Generation();
        if (gen <= 5)
            return 0;

        return species switch
        {
            (int)Furfrou when form != 0 => 5,
            (int)Hoopa when form == 1 => 3,
            (int)Yamask when form == 1 => 9999,
            (int)Runerigus when form == 0 => 9999,
            (int)Alcremie => (uint)AlcremieDecoration.Ribbon,
            (int)Qwilfish when form == 1 && gen >= 8 => 9999,
            (int)Overqwil => 9999, // 20
            (int)Stantler or (int)Wyrdeer when gen >= 8 => 9999,
            (int)Basculin when form == 2 => 9999, // 294
            (int)Basculegion => 9999, // 294
            (int)Primeape or (int)Annihilape when gen >= 8 => 9999,
            (int)Bisharp or (int)Kingambit when gen >= 8 => 9999,
            (int)Gimmighoul => 998,
            (int)Gholdengo => 999,
            (int)Koraidon or (int)Miraidon => 1,
            _ => 0,
        };
    }

    /// <summary>
    /// Gets the minimum value the <see cref="IFormArgument.FormArgument"/> value can be to satisfy an evolution requirement.
    /// </summary>
    /// <param name="currentSpecies">Current state species</param>
    /// <param name="originalSpecies">Initial species</param>
    public static uint GetFormArgumentMinEvolution(ushort currentSpecies, ushort originalSpecies) => originalSpecies switch
    {
        (int)Yamask when currentSpecies == (int)Runerigus => 49u,
        (int)Qwilfish when currentSpecies == (int)Overqwil => 20u,
        (int)Stantler when currentSpecies == (int)Wyrdeer => 20u,
        (int)Basculin when currentSpecies == (int)Basculegion => 294u,
        (int)Mankey or (int)Primeape when currentSpecies == (int)Annihilape => 20u,
        (int)Pawniard or (int)Bisharp when currentSpecies == (int)Kingambit => 3u,
        (int)Gimmighoul when currentSpecies == (int)Gholdengo => 999u,
        _ => 0u,
    };

    /// <summary>
    /// Checks if the <see cref="IFormArgument.FormArgument"/> value is stored as a days-elapsed / days-remaining pair.
    /// </summary>
    public static bool IsFormArgumentTypeDatePair(ushort species, byte form) => species switch
    {
        (int)Furfrou when form != 0 => true,
        (int)Hoopa when form == 1 => true,
        _ => false,
    };
}
