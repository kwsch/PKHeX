using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>Which genders a species (and form) can have, from its personal data's gender ratio.</summary>
public enum GenderRule
{
    /// <summary>Male or female.</summary>
    Either,

    /// <summary>Always male.</summary>
    OnlyMale,

    /// <summary>Always female.</summary>
    OnlyFemale,

    /// <summary>Always genderless.</summary>
    Genderless,
}

/// <summary>The genders an editor offers for a species, as the desktop editor's gender toggle allows them.</summary>
/// <remarks>
/// The desktop toggles male and female only for a species with both (<c>PKMEditor.ClickGender</c>); for any other it sets the one gender
/// the species has, which also corrects a wrong stored value. The values are Core's gender values (<see cref="EntityGender"/>).
/// </remarks>
public static class GenderRules
{
    private static readonly byte[] Either = [EntityGender.Male, EntityGender.Female];
    private static readonly byte[] OnlyMale = [EntityGender.Male];
    private static readonly byte[] OnlyFemale = [EntityGender.Female];
    private static readonly byte[] Genderless = [EntityGender.Genderless];

    /// <summary>The rule for a species with the gender ratio in <paramref name="personal"/>.</summary>
    public static GenderRule Of(IGenderDetail personal) => personal switch
    {
        { Genderless: true } => GenderRule.Genderless,
        { OnlyFemale: true } => GenderRule.OnlyFemale,
        { OnlyMale: true } => GenderRule.OnlyMale,
        _ => GenderRule.Either,
    };

    /// <summary>The gender values allowed by <paramref name="rule"/>, in the order an editor offers them.</summary>
    public static IReadOnlyList<byte> Allowed(GenderRule rule) => rule switch
    {
        GenderRule.Either => Either,
        GenderRule.OnlyMale => OnlyMale,
        GenderRule.OnlyFemale => OnlyFemale,
        GenderRule.Genderless => Genderless,
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null),
    };
}
