using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.State;

/// <summary>
/// What this release can do with one opened save: the intersection of what the concrete Core save type offers and what
/// <see cref="SupportMatrix"/> allows for its family. A generation number alone never decides it.
/// </summary>
/// <remarks>
/// Built once per session, from the save the session was opened with. The lists in <see cref="Lists"/> belong to this session
/// only: they are never assigned to <see cref="GameInfo.FilteredSources"/>, so opening a save changes no global Core state.
/// </remarks>
public sealed class SaveCapabilities
{
    private SaveCapabilities(SupportedFamily family, SaveFile save)
    {
        Family = family;
        HasParty = save.HasParty;
        HasBoxes = save.HasBox;
        CanApplyToParty = family.WritesParty && save.HasParty;
        Editable = family.Editable;
        MaxNicknameLength = save.MaxStringLengthNickname;
        SaveLanguage = save.Language;
        Lists = new FilteredGameDataSource(save, GameInfo.Sources);
        Personal = save.Personal;
    }

    /// <summary>The family the save was opened as.</summary>
    public SupportedFamily Family { get; }

    /// <summary>The exact entity type the save stores, which the draft and inspector are written for.</summary>
    public Type EntityType => Family.EntityType;

    /// <summary>True when the save has a party.</summary>
    public bool HasParty { get; }

    /// <summary>True when the save has boxes.</summary>
    public bool HasBoxes { get; }

    /// <summary>True when drafts of party members can be applied (see <see cref="SupportedFamily.WritesParty"/>).</summary>
    public bool CanApplyToParty { get; }

    /// <summary>Entity fields the user can draft. Fields not listed are shown read-only and kept as stored.</summary>
    public EditableFields Editable { get; }

    /// <summary>Longest nickname the save format can store, in characters.</summary>
    public int MaxNicknameLength { get; }

    /// <summary>The save's language, which decides the font the game shows names in (see <see cref="NameRules.Describe"/>).</summary>
    public int SaveLanguage { get; }

    /// <summary>
    /// Core's species, move, item, ball, ability, game and language lists, filtered to what this save can hold.
    /// The inspector uses them to point out stored values outside them; editors offer their choices from them.
    /// </summary>
    public FilteredGameDataSource Lists { get; }

    /// <summary>
    /// The save's personal table, which decides the species and forms it can hold. A form change (see <see cref="EditorDraft.EditGender"/>)
    /// is checked against it, as the desktop editor checks it against the open save's.
    /// </summary>
    public IPersonalTable Personal { get; }

    /// <summary>True when a draft of <paramref name="slot"/> can be written back to this save.</summary>
    public bool CanApply(SlotRef slot) => slot.IsParty ? CanApplyToParty : HasBoxes;

    /// <summary>Works out the capabilities of <paramref name="save"/>.</summary>
    /// <exception cref="NotSupportedException">
    /// The save's type is not one this release opens, or it stores a different entity type than its family allows.
    /// <see cref="SaveLoader"/> refuses such saves first, so a session never meets this.
    /// </exception>
    public static SaveCapabilities For(SaveFile save)
    {
        var family = SupportMatrix.Find(save) ?? throw new NotSupportedException($"{save.GetType().Name} is not opened by this release.");
        return For(save, family);
    }

    /// <summary>
    /// Works out the capabilities of <paramref name="save"/> as a member of <paramref name="family"/>. Tests use it to cover what a
    /// family allows (such as a family without party writes) independently of <see cref="SupportMatrix"/>.
    /// </summary>
    /// <exception cref="NotSupportedException"><paramref name="save"/> is not of the family's save type or does not store its entity type.</exception>
    internal static SaveCapabilities For(SaveFile save, SupportedFamily family)
    {
        if (save.GetType() != family.SaveType)
        {
            throw new NotSupportedException($"{save.GetType().Name} is not a {family.SaveType.Name}.");
        }
        if (save.BlankPKM.GetType() != family.EntityType)
        {
            throw new NotSupportedException($"{save.GetType().Name} does not store {family.EntityType.Name}.");
        }
        return new SaveCapabilities(family, save);
    }
}
