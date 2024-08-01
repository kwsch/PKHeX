using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Contains logic for Alternate Form information.
/// </summary>
public static class FormInfo
{
    /// <summary>
    /// Checks if the form cannot exist outside a Battle.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="format">Current generation format</param>
    /// <returns>True if it can only exist in a battle, false if it can exist outside of battle.</returns>
    public static bool IsBattleOnlyForm(ushort species, byte form, byte format) => BattleOnly.Contains(species) && species switch
    {
        // Only continue checking if the species is in the list of Battle Only forms.
        // Some species have battle only forms as well as out-of-battle forms (other than base form).
        (ushort)Slowbro => form == 1, // Mega
        (ushort)Darmanitan => (form & 1) == 1, // Zen
        (ushort)Zygarde => form == 4, // Zygarde Complete
        (ushort)Minior => form < 7, // Minior Shields-Down
        (ushort)Mimikyu => (form & 1) == 1, // Busted
        (ushort)Necrozma => form == 3, // Ultra Necrozma
        (ushort)Ogerpon => form >= 4, // Embody Aspect
        _ => form != 0,
    };

    /// <summary>
    /// Reverts the Battle Form to the form it would have outside of Battle.
    /// </summary>
    /// <remarks>Only call this if you've already checked that <see cref="IsBattleOnlyForm"/> returns true.</remarks>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="format">Current generation format</param>
    /// <returns>Suggested alt form value.</returns>
    public static byte GetOutOfBattleForm(ushort species, byte form, byte format) => species switch
    {
        (ushort)Darmanitan => (byte)(form & 2),
        (ushort)Zygarde when format > 6 => 3,
        (ushort)Minior => (byte)(form + 7),
        (ushort)Mimikyu => (byte)(form & 2),
        (ushort)Ogerpon => (byte)(form & 3),
        _ => 0,
    };

    /// <summary>
    /// Indicates if the entity should be prevented from being traded away.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="formArg">Entity form argument</param>
    /// <param name="format">Current generation format</param>
    /// <returns>True if trading should be disallowed.</returns>
    public static bool IsUntradable(ushort species, byte form, uint formArg, byte format) => species switch
    {
        (ushort)Koraidon or (int)Miraidon => formArg == 1, // Ride-able Box Legend
        (ushort)Pikachu => format == 7 && form == 8, // Let's Go Pikachu Starter
        (ushort)Eevee => format == 7 && form == 1, // Let's Go Eevee Starter
        _ => IsFusedForm(species, form, format),
    };

    /// <summary>
    /// Checks if the <see cref="form"/> is a fused form, which indicates it cannot be traded away.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="format">Current generation format</param>
    /// <returns>True if it is a fused species-form, false if it is not fused.</returns>
    public static bool IsFusedForm(ushort species, byte form, byte format) => species switch
    {
        (ushort)Kyurem => form != 0 && format >= 5,
        (ushort)Necrozma => form != 0 && format >= 7,
        (ushort)Calyrex => form != 0 && format >= 8,
        _ => false,
    };

    /// <summary>Checks if the form may be different from the original encounter detail.</summary>
    /// <param name="species">Original species</param>
    /// <param name="oldForm">Original form</param>
    /// <param name="newForm">Current form</param>
    /// <param name="origin">Encounter context</param>
    /// <param name="current">Current context</param>
    public static bool IsFormChangeable(ushort species, byte oldForm, byte newForm, EntityContext origin, EntityContext current)
    {
        if (FormChange.Contains(species))
            return true;

        // Zygarde Form Changing
        // Gen6: Introduced; no form changing.
        // Gen7: Form changing introduced; can only change to Form 2/3 (Power Construct), never to 0/1 (Aura Break). A form-1 can be boosted to form-0.
        // Gen8: Form changing improved; can pick any Form & Ability combination.
        if (species == (int)Zygarde)
        {
            return current switch
            {
                EntityContext.Gen6 => false,
                EntityContext.Gen7 => newForm >= 2 || (oldForm == 1 && newForm == 0),
                _ => true,
            };
        }
        if (species is (int)Deerling or (int)Sawsbuck)
        {
            if (origin == EntityContext.Gen5)
                return true; // B/W or B2/W2 change via seasons
            if (current.Generation() >= 8)
                return true; // Via S/V change via in-game province on startup.
        }
        return false;
    }

    /// <summary>
    /// Checks if the form can be changed for a given (baby) species into a different-typed form.
    /// </summary>
    /// <param name="species">Baby species</param>
    /// <returns>True if the species can change form to another with a different typing, false if it cannot.</returns>
    public static bool IsFormChangeEggTypes(ushort species) => FormChangeEgg.Contains(species);

    private static ReadOnlySpan<ushort> FormChangeEgg =>
    [
        (int)Burmy,
        (int)Furfrou,
        (int)Oricorio,
    ];

    /// <summary>
    /// Species that can change between their forms, regardless of origin.
    /// </summary>
    /// <remarks>Excludes Zygarde as it has special conditions. Check separately.</remarks>
    private static readonly HashSet<ushort> FormChange =
    [
        (int)Burmy,
        (int)Furfrou,
        (int)Oricorio,

        // Sometimes considered for wild encounters
        (int)Rotom,

        (int)Deoxys,
        (int)Dialga,
        (int)Palkia,
        (int)Giratina,
        (int)Shaymin,
        (int)Arceus,
        (int)Tornadus,
        (int)Thundurus,
        (int)Landorus,
        (int)Kyurem,
        (int)Keldeo,
        (int)Genesect,
        (int)Hoopa,
        (int)Silvally,
        (int)Necrozma,
        (int)Calyrex,
        (int)Enamorus,
        (int)Ogerpon,
    ];

    /// <summary>
    /// Species that have an alternate form that cannot exist outside of battle.
    /// </summary>
    private static ReadOnlySpan<ushort> BattleForms =>
    [
        (int)Castform,
        (int)Kyogre,
        (int)Groudon,

        (int)Cherrim,

        (int)Darmanitan,
        (int)Meloetta,

        (int)Aegislash,
        (int)Xerneas,
        (int)Zygarde,

        (int)Wishiwashi,
        (int)Minior,
        (int)Mimikyu,
        (int)Necrozma,

        (int)Cramorant,
        (int)Morpeko,
        (int)Eiscue,
        (int)Zacian,
        (int)Zamazenta,
        (int)Eternatus,

        (int)Palafin,
        (int)Ogerpon,
        (int)Terapagos,
    ];

    /// <summary>
    /// Species that have a mega form that cannot exist outside of battle.
    /// </summary>
    /// <remarks>Using a held item to change form during battle, via an in-battle transformation feature.</remarks>
    private static ReadOnlySpan<ushort> BattleMegas =>
    [
        // X/Y
        (int)Venusaur, (int)Charizard, (int)Blastoise,
        (int)Alakazam, (int)Gengar, (int)Kangaskhan, (int)Pinsir,
        (int)Gyarados, (int)Aerodactyl, (int)Mewtwo,

        (int)Ampharos, (int)Scizor, (int)Heracross, (int)Houndoom, (int)Tyranitar,

        (int)Blaziken, (int)Gardevoir, (int)Mawile, (int)Aggron, (int)Medicham,
        (int)Manectric, (int)Banette, (int)Absol, (int)Latios, (int)Latias,

        (int)Garchomp, (int)Lucario, (int)Abomasnow,

        // OR/AS
        (int)Beedrill, (int)Pidgeot, (int)Slowbro,

        (int)Steelix,

        (int)Sceptile, (int)Swampert, (int)Sableye, (int)Sharpedo, (int)Camerupt,
        (int)Altaria, (int)Glalie, (int)Salamence, (int)Metagross, (int)Rayquaza,

        (int)Lopunny, (int)Gallade,
        (int)Audino, (int)Diancie,
    ];

    private static readonly HashSet<ushort> BattleOnly = GetBattleFormSet();

    private static HashSet<ushort> GetBattleFormSet()
    {
        var reg = BattleForms;
        var mega = BattleMegas;
        var count = reg.Length + mega.Length + 2;
        var hs = new HashSet<ushort>(count);
        foreach (var species in reg)
            hs.Add(species);
        foreach (var species in mega)
            hs.Add(species);
        return hs;
    }

    /// <summary>
    /// Species has a Totem form in Gen7 (S/M &amp; US/UM) that can be captured and owned.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <returns>True if the species exists as a Totem.</returns>
    /// <remarks>Excludes <see cref="Wishiwashi"/> because it cannot be captured.</remarks>
    public static bool HasTotemForm(ushort species) => species switch
    {
        (ushort)Raticate => true,
        (ushort)Marowak => true,
        (ushort)Gumshoos => true,
        (ushort)Vikavolt => true,
        (ushort)Ribombee => true,
        (ushort)Araquanid => true,
        (ushort)Lurantis => true,
        (ushort)Salazzle => true,
        (ushort)Mimikyu => true,
        (ushort)Kommoo => true,
        (ushort)Togedemaru => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="form"/> for the <see cref="species"/> is a Totem form.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="context">Current generation format</param>
    public static bool IsTotemForm(ushort species, byte form, EntityContext context) => context == EntityContext.Gen7 && IsTotemForm(species, form);

    /// <summary>
    /// Checks if the <see cref="form"/> for the <see cref="species"/> is a Totem form.
    /// </summary>
    /// <remarks>Use <see cref="IsTotemForm(ushort,byte,EntityContext)"/> if you aren't 100% sure the format is 7.</remarks>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    public static bool IsTotemForm(ushort species, byte form)
    {
        if (form == 0)
            return false;
        if (!HasTotemForm(species))
            return false;
        if (species == (int)Mimikyu)
            return form is 2 or 3;
        if (species is (int)Raticate or (int)Marowak)
            return form == 2;
        return form == 1;
    }

    /// <summary>
    /// Gets the base <see cref="form"/> for the <see cref="species"/> when the Totem form is reverted (on transfer).
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    public static byte GetTotemBaseForm(ushort species, byte form)
    {
        if (species == (int)Mimikyu)
            return 0;
        return --form;
    }

    /// <summary>
    /// Checks if the <see cref="form"/> for the <see cref="species"/> is a Lord Form from Legends: Arceus.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="context">Current context</param>
    /// <returns>True if the form is a Lord Form, false if it is not.</returns>
    public static bool IsLordForm(ushort species, byte form, EntityContext context)
    {
        if (context != EntityContext.Gen8a)
            return false;
        return IsLordForm(species, form);
    }

    private static bool IsLordForm(ushort species, byte form) => form != 0 && species switch
    {
        (int)Arcanine when form == 2 => true,
        (int)Electrode when form == 2 => true,
        (int)Lilligant when form == 2 => true,
        (int)Avalugg when form == 2 => true,
        (int)Kleavor when form == 1 => true,
        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="form"/> exists for the <see cref="species"/> without having an associated <see cref="PersonalInfo"/> index.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="format">Current generation format</param>
    /// <seealso cref="HasFormValuesNotIndicatedByPersonal"/>
    public static bool IsValidOutOfBoundsForm(ushort species, byte form, byte format) => (Species) species switch
    {
        Unown => form < (format == 2 ? 26 : 28), // A-Z : A-Z?!
        Mothim => form < 3, // Burmy base form is kept

        Scatterbug => form <= Vivillon3DS.MaxWildFormID, // Vivillon Pre-evolutions
        Spewpa => form <= Vivillon3DS.MaxWildFormID, // Vivillon Pre-evolutions

        _ => false,
    };

    /// <summary>
    /// Checks if the <see cref="PKM"/> data should have a drop-down selection visible for the <see cref="PKM.Form"/> value.
    /// </summary>
    /// <param name="pi">Game specific personal info</param>
    /// <param name="species"><see cref="Species"/> ID</param>
    /// <param name="format"><see cref="PKM.Form"/> ID</param>
    /// <returns>True if it has forms that can be provided by <see cref="FormConverter.GetFormList"/>, otherwise false for none.</returns>
    public static bool HasFormSelection(IPersonalFormInfo pi, ushort species, byte format)
    {
        if (format <= 3 && species != (int)Unown)
            return false;

        if (HasFormValuesNotIndicatedByPersonal(species))
            return true;

        int count = pi.FormCount;
        return count > 1;
    }

    /// <summary>
    /// <seealso cref="IsValidOutOfBoundsForm"/>
    /// </summary>
    private static bool HasFormValuesNotIndicatedByPersonal(ushort species) => species switch
    {
        (int)Unown => true,
        (int)Mothim => true, // (Burmy form is not cleared on evolution)
        (int)Scatterbug or (int)Spewpa => true, // Vivillon pre-evos
        _ => false,
    };
}
