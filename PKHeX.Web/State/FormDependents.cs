namespace PKHeX.Web.State;

/// <summary>
/// The drafted values that follow the form: the form itself, the ability and its slot number, and the experience points, which Core sets
/// to the start of the level on a form change. Taken before and after a gender edit, they tell what the edit changed besides the gender.
/// </summary>
/// <param name="Form">The form.</param>
/// <param name="Ability">The ability.</param>
/// <param name="AbilityNumber">The stored ability slot number (1, 2 or 4).</param>
/// <param name="Experience">The experience points.</param>
public readonly record struct FormDependents(byte Form, int Ability, int AbilityNumber, uint Experience);

/// <summary>
/// What a gender edit changed besides the gender, for a species whose form is its gender (Meowstic), so the editor can say so.
/// </summary>
/// <param name="Before">The values before the edit.</param>
/// <param name="After">The values after the edit.</param>
public sealed record GenderChange(FormDependents Before, FormDependents After)
{
    /// <summary>True when the form changed.</summary>
    public bool FormChanged => Before.Form != After.Form;

    /// <summary>True when the ability or its slot number changed.</summary>
    public bool AbilityChanged => Before.Ability != After.Ability || Before.AbilityNumber != After.AbilityNumber;

    /// <summary>True when the experience points changed.</summary>
    public bool ExperienceChanged => Before.Experience != After.Experience;

    /// <summary>The change from <paramref name="before"/> to <paramref name="after"/>, or null when nothing but the gender changed.</summary>
    public static GenderChange? Between(FormDependents before, FormDependents after) => before == after ? null : new GenderChange(before, after);
}
