using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// The save families this release opens, as shown in the About panel, and what this release allows on each. It is also the allowlist
/// <see cref="SaveLoader"/> enforces and the release half of <see cref="SaveCapabilities"/>, so the panel cannot list a family the
/// loader refuses, or miss one it opens.
/// </summary>
/// <remarks>
/// Being listed means the family is opened, not that it is supported: no family is qualified until its published-app evidence exists.
/// </remarks>
public static class SupportMatrix
{
    /// <summary>Families this release opens, in display order.</summary>
    public static IReadOnlyList<SupportedFamily> Families { get; } =
    [
        new("Pokémon X and Y", typeof(SAV6XY), typeof(PK6), PK6Fields, WritesParty: true),
        new("Pokémon Omega Ruby and Alpha Sapphire", typeof(SAV6AO), typeof(PK6), PK6Fields, WritesParty: true),
    ];

    /// <summary>The PK6 fields this release lets the user draft.</summary>
    private const EditableFields PK6Fields = EditableFields.Nickname | EditableFields.Language | EditableFields.Friendship
        | EditableFields.Level | EditableFields.Nature | EditableFields.Ivs | EditableFields.Evs;

    /// <summary>True if <paramref name="save"/> is of a type this release opens. Related types, such as the ORAS demo, are not included.</summary>
    public static bool IsEnabled(SaveFile save) => Find(save) is not null;

    /// <summary>The family <paramref name="save"/> is opened as, or null when this release does not open its type.</summary>
    public static SupportedFamily? Find(SaveFile save) => Families.FirstOrDefault(f => f.SaveType == save.GetType());
}

/// <summary>One save family that this release opens, and what this release allows on it.</summary>
/// <param name="Games">Display name of the games.</param>
/// <param name="SaveType">The exact Core save type.</param>
/// <param name="EntityType">The exact Core entity type the family stores. The editor and inspector are written for this type only.</param>
/// <param name="Editable">Entity fields this release lets the user draft for the family.</param>
/// <param name="WritesParty">
/// True when drafts of party members can be applied. Set only for a family whose entity type has a party-stat policy (stored stats, HP
/// and status; <see cref="PartyStatPolicy"/> for PK6); a family without one has its party members inspected only.
/// </param>
public sealed record SupportedFamily(string Games, Type SaveType, Type EntityType, EditableFields Editable, bool WritesParty);
