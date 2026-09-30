using PKHeX.Core;

namespace PKHeX.Web.Services;

/// <summary>
/// The save families this release opens, as shown in the About panel. It is also the allowlist <see cref="SaveLoader"/> enforces,
/// so the panel cannot list a family the loader refuses, or miss one it opens.
/// </summary>
/// <remarks>
/// Being listed means the family is opened, not that it is supported: no family is qualified until its published-app evidence exists.
/// </remarks>
public static class SupportMatrix
{
    /// <summary>Families this release opens, in display order.</summary>
    public static IReadOnlyList<SupportedFamily> Families { get; } =
    [
        new("Pokémon X and Y", typeof(SAV6XY)),
        new("Pokémon Omega Ruby and Alpha Sapphire", typeof(SAV6AO)),
    ];

    /// <summary>True if <paramref name="save"/> is of a type this release opens. Related types, such as the ORAS demo, are not included.</summary>
    public static bool IsEnabled(SaveFile save) => Families.Any(f => f.SaveType == save.GetType());
}

/// <summary>One save family that this release opens.</summary>
/// <param name="Games">Display name of the games.</param>
/// <param name="SaveType">The exact Core save type.</param>
public sealed record SupportedFamily(string Games, Type SaveType);
