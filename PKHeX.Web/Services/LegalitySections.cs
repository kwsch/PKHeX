using PKHeX.Core;

namespace PKHeX.Web.Services;

/// <summary>A section of the inspector (see <see cref="EntityInspection"/>), which a legality finding can point to.</summary>
public enum InspectorArea
{
    /// <summary>Species, form, nickname, egg state, gender, shininess, language, level, nature, ability, held item and friendship.</summary>
    Identity,

    /// <summary>Stats, IVs and EVs.</summary>
    Stats,

    /// <summary>Current moves.</summary>
    Moves,

    /// <summary>Trainer, origin game, met data, ball, fateful flag and handler.</summary>
    Origin,

    /// <summary>PID, encryption constant, ribbons, markings and format details.</summary>
    Advanced,
}

/// <summary>
/// Where the inspector shows the values a Core legality check is about, so a finding can link to them (WEB-LEGAL-002).
/// </summary>
/// <remarks>
/// The mapping follows what each inspector section shows today. A check about something the inspector does not show (relearn moves, memories,
/// super training, generation-specific values) maps to nothing rather than to a guess. The switch has no default arm, so an identifier Core adds
/// later throws until it is mapped, which the unit tests catch.
/// </remarks>
public static class LegalitySections
{
    /// <summary>The inspector section that shows what <paramref name="identifier"/> checks, or null when none does.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The identifier is not known to this release.</exception>
    public static InspectorArea? For(CheckIdentifier identifier) => identifier switch
    {
        CheckIdentifier.CurrentMove => InspectorArea.Moves,
        CheckIdentifier.Shiny or CheckIdentifier.Gender or CheckIdentifier.Language or CheckIdentifier.Nickname or CheckIdentifier.Level
            or CheckIdentifier.Form or CheckIdentifier.Egg or CheckIdentifier.Ability or CheckIdentifier.Evolution or CheckIdentifier.Nature
            or CheckIdentifier.HeldItem => InspectorArea.Identity,
        CheckIdentifier.EVs or CheckIdentifier.IVs => InspectorArea.Stats,
        CheckIdentifier.Encounter or CheckIdentifier.Trainer or CheckIdentifier.Ball or CheckIdentifier.Fateful or CheckIdentifier.GameOrigin
            or CheckIdentifier.Handler => InspectorArea.Origin,
        CheckIdentifier.EC or CheckIdentifier.PID or CheckIdentifier.Ribbon or CheckIdentifier.RibbonMark
            or CheckIdentifier.Marking => InspectorArea.Advanced,
        CheckIdentifier.RelearnMove or CheckIdentifier.Memory or CheckIdentifier.Geography or CheckIdentifier.Misc or CheckIdentifier.Training
            or CheckIdentifier.GVs or CheckIdentifier.AVs or CheckIdentifier.TrashBytes or CheckIdentifier.SlotType => null,
        _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "No inspector section is mapped for this check."),
    };
}
