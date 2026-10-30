using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Entity fields changed by <see cref="SpeciesFormChange.ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable,int)"/>.
/// </summary>
[Flags]
public enum SpeciesFormChangeResult
{
    /// <summary>No dependent field changed.</summary>
    None = 0,

    /// <summary><see cref="PKM.Form"/> changed.</summary>
    Form = 1 << 0,

    /// <summary><see cref="PKM.EXP"/> changed (and therefore possibly the level).</summary>
    EXP = 1 << 1,

    /// <summary><see cref="PKM.Ability"/> or <see cref="PKM.AbilityNumber"/> changed.</summary>
    Ability = 1 << 2,

    /// <summary><see cref="PKM.Gender"/> changed.</summary>
    Gender = 1 << 3,

    /// <summary>
    /// <see cref="PKM.PID"/> changed, and <see cref="PKM.EncryptionConstant"/> if it is bound to the PID.
    /// Values derived from the PID (e.g. Gen 3 nature and shininess) may have changed with it.
    /// </summary>
    PID = 1 << 4,

    /// <summary><see cref="PKM.Nickname"/> or <see cref="PKM.IsNicknamed"/> changed.</summary>
    Nickname = 1 << 5,
}

/// <summary>
/// Changes an entity's species and/or form, updating the fields that depend on them the same way the desktop editor does.
/// </summary>
/// <remarks>
/// <para>
/// EXP is snapped to the minimum for the level it yields on the new growth curve, so the level can change when the growth rate differs.
/// The PID is never rerolled, except for Generation 3 Unown, whose form is derived from the PID; the reroll also changes the other PID-derived values (nature, shininess, and possibly gender).
/// Party stats are not recalculated; callers editing a party entity should refresh them afterward.
/// </para>
/// <para>
/// Differences from the desktop editor: gender forms are identified by species rather than by parsing localized form names (so Gen 9 mega-context Meowstic forms also set the gender), an invalid Gen 6+ <see cref="PKM.AbilityNumber"/> is normalized to the kept slot on a change, eggs keep their nickname, and an entity without a valid language keeps its nickname.
/// </para>
/// <para>
/// Not supported: Generation 1/2 entities (Unown form is derived from IVs; nickname language is derived from the name), and unrestricted (HaX) editing, which allows species/forms this method rejects.
/// Not handled: the Generation 5 Basculin-Blue extra ability entry, form arguments, and anything UI-specific.
/// </para>
/// </remarks>
public static class SpeciesFormChange
{
    extension(PKM pk)
    {
        /// <summary>
        /// Changes the species and form, keeping the ability slot the entity currently has (see <see cref="GetAbilitySlot"/>).
        /// </summary>
        /// <inheritdoc cref="ChangeSpeciesForm(PKM,ushort,byte,IPersonalTable,int)"/>
        public SpeciesFormChangeResult ChangeSpeciesForm(ushort species, byte form, IPersonalTable table)
            => pk.ChangeSpeciesForm(species, form, table, pk.GetAbilitySlot());

        /// <summary>
        /// Changes the species and form, then updates the dependent fields.
        /// </summary>
        /// <remarks>
        /// If neither the species nor the (effective) form differs from the entity's, nothing is changed and <see cref="SpeciesFormChangeResult.None"/> is returned.
        /// </remarks>
        /// <param name="species">Species to change to; 0 (none) is allowed, as in the desktop editor.</param>
        /// <param name="form">
        /// Form to change to, as an index of the entity context's form list (<see cref="FormConverter.GetFormList"/>).
        /// Ignored (reset to 0) if the species has no form selection in the <paramref name="table"/>.
        /// </param>
        /// <param name="table">Personal table of the save file the entity is being edited for; used to validate the species and check if the species has forms.</param>
        /// <param name="abilitySlot">
        /// Ability slot (0/1/2 = first/second/hidden) to keep; values outside the slots the species has are clamped.
        /// Editors carry this across changes, since Gen 4/5 entities do not store the slot when both regular abilities are the same.
        /// </param>
        /// <returns>Flags indicating which dependent fields changed.</returns>
        /// <exception cref="NotSupportedException">The entity is a Generation 1/2 format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The species does not exist in the <paramref name="table"/>, or the form is not selectable for it.</exception>
        public SpeciesFormChangeResult ChangeSpeciesForm(ushort species, byte form, IPersonalTable table, int abilitySlot)
        {
            if (pk.Format < 3)
                throw new NotSupportedException("Generation 1/2 entities are not supported.");
            if (species > table.MaxSpeciesID)
                throw new ArgumentOutOfRangeException(nameof(species), species, "Species does not exist in the personal table.");

            bool hasForms = FormInfo.HasFormSelection(table[species], species, pk.Format);
            if (!hasForms)
                form = 0;
            else if (!IsFormSelectable(species, form, pk.Context, pk.Format))
                throw new ArgumentOutOfRangeException(nameof(form), form, "Form is not selectable for the species.");

            if (species == pk.Species && form == pk.Form)
                return SpeciesFormChangeResult.None;

            var before = new Snapshot(pk);
            bool speciesChanged = pk.Species != species;
            bool nicknamed = pk.IsNicknamed;

            pk.Species = species;
            pk.Form = form; // Gen 3 Unown: rerolls the PID until it yields the form

            // Level is derived from the current EXP on the new growth curve, then EXP is reset to that level's minimum.
            pk.EXP = Experience.GetEXP(pk.CurrentLevel, pk.PersonalInfo.EXPGrowth);

            // Legends: Z-A abilities do not change with species; the desktop editor sets them directly.
            if (pk is not PA9)
                SetAbilitySlot(pk, abilitySlot);

            if (TryGetFormGender(pk.Species, pk.Form, out var gender))
                pk.Gender = gender;
            pk.Gender = pk.GetSaneGender();

            if (speciesChanged && !nicknamed && !pk.IsEgg)
                UpdateDefaultNickname(pk);

            return before.Compare(pk);
        }

        /// <summary>
        /// Gets the ability slot (0/1/2 = first/second/hidden) the entity currently has, as the desktop editor shows it.
        /// </summary>
        /// <remarks>
        /// Gen 4/5 regular slots are derived from the ability value; if both regular abilities are the same, the PID-derived slot is used.
        /// An ability that is not in the species' list (e.g. Gen 5 Basculin-Blue's Reckless) is reported as the first slot.
        /// This is not the inverse of <see cref="CommonEdits.SetAbilityIndex"/>, which rerolls the PID for Gen 3-5 entities.
        /// </remarks>
        public int GetAbilitySlot() => pk switch
        {
            G3PKM pk3 => pk3.AbilityBit ? 1 : 0,
            PK5 { HiddenAbility: true } => 2,
            G4PKM or PK5 => GetAbilitySlot45(pk),
            _ => AbilityVerifier.IsValidAbilityBits(pk.AbilityNumber) ? pk.AbilityNumber >> 1 : 0,
        };
    }

    private static int GetAbilitySlot45(PKM pk)
    {
        var pi = pk.PersonalInfo;
        int index = pi.GetIndexOfAbility(pk.Ability);
        if (index >= 2)
            return 2;
        if (index < 0)
            return 0;
        if (((IPersonalAbility12)pi).IsAbility12Same)
            return pk.PIDAbility;
        return index;
    }

    /// <summary>
    /// Applies the ability slot to the current species/form without altering the PID.
    /// </summary>
    private static void SetAbilitySlot(PKM pk, int index)
    {
        index = Math.Clamp(index, 0, pk.PersonalInfo.AbilityCount - 1);
        if (pk is G3PKM pk3)
            pk3.AbilityBit = index != 0; // stored as-is, even if the species has a single ability
        else
            pk.RefreshAbility(index);
    }

    /// <summary>
    /// Checks if the form is offered for the species in the entity context's form list, or is otherwise valid for it.
    /// </summary>
    /// <remarks>
    /// The list can include forms missing from a save's personal table (e.g. Dusk Lycanroc for a Sun/Moon save), so the context's list is used.
    /// Only its length is needed; form names do not affect it.
    /// </remarks>
    private static bool IsFormSelectable(ushort species, byte form, EntityContext context, byte format)
    {
        var strings = GameInfo.Strings;
        var count = FormConverter.GetFormList(species, strings.types, strings.forms, GameInfo.GenderSymbolUnicode, context).Length;
        return form < Math.Max(1, count) || FormInfo.IsValidOutOfBoundsForm(species, form, format);
    }

    /// <summary>
    /// Gets the gender selected by the form, for species whose forms are Male/Female.
    /// </summary>
    private static bool TryGetFormGender(ushort species, byte form, out byte gender)
    {
        gender = (byte)(form & 1);
        return (Species)species switch
        {
            Meowstic => true, // Male, Female; Gen 9 mega context adds Mega (Male), Mega (Female)
            Indeedee or Basculegion or Oinkologne => form <= 1,
            _ => false,
        };
    }

    /// <summary>
    /// Sets the species name as the nickname, unless the current name is already a default name for the species.
    /// </summary>
    /// <remarks>
    /// Written through <see cref="PKM.Nickname"/> like the desktop editor, which also refreshes the Colosseum/XD display name copy.
    /// </remarks>
    private static void UpdateDefaultNickname(PKM pk)
    {
        if (pk.Species is 0)
            pk.Nickname = string.Empty;
        else if (!IsDefaultNickname(pk))
        {
            var name = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, pk.Format);
            if (name.Length != 0) // no name for an invalid language
                pk.Nickname = name;
        }
    }

    /// <summary>
    /// Checks if the current nickname is already a default species name that should be kept.
    /// </summary>
    private static bool IsDefaultNickname(PKM pk)
    {
        var species = pk.Species;
        var current = pk.Nickname;
        var context = pk.Context;
        if (!SpeciesName.IsNicknamedAnyLanguage(species, current, context))
            return true;

        // Auto-decapitalization did not happen until Gen 6.
        // If transferred from Gen 3/4 to Gen 5, the name can be either ALL-CAPS or decapitalized.
        if (context != EntityContext.Gen5 || species > (ushort)Arceus || pk.Gen5)
            return false;
        return !SpeciesName.IsNicknamedAnyLanguage(species, current, EntityContext.Gen4);
    }

    /// <summary>
    /// Values of the fields that can be changed, captured before the change.
    /// </summary>
    private readonly record struct Snapshot(byte Form, uint EXP, int Ability, int AbilityNumber, byte Gender, uint PID, uint EC, string Nickname, bool IsNicknamed)
    {
        public Snapshot(PKM pk) : this(pk.Form, pk.EXP, pk.Ability, pk.AbilityNumber, pk.Gender, pk.PID, pk.EncryptionConstant, pk.Nickname, pk.IsNicknamed) { }

        public SpeciesFormChangeResult Compare(PKM pk)
        {
            var result = SpeciesFormChangeResult.None;
            if (Form != pk.Form)
                result |= SpeciesFormChangeResult.Form;
            if (EXP != pk.EXP)
                result |= SpeciesFormChangeResult.EXP;
            if (Ability != pk.Ability || AbilityNumber != pk.AbilityNumber)
                result |= SpeciesFormChangeResult.Ability;
            if (Gender != pk.Gender)
                result |= SpeciesFormChangeResult.Gender;
            if (PID != pk.PID || EC != pk.EncryptionConstant)
                result |= SpeciesFormChangeResult.PID;
            if (Nickname != pk.Nickname || IsNicknamed != pk.IsNicknamed)
                result |= SpeciesFormChangeResult.Nickname;
            return result;
        }
    }
}
