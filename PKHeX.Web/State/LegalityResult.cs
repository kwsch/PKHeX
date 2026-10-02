using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>What Core concluded about a draft.</summary>
public enum LegalityVerdict
{
    /// <summary>Core parsed the entity and found no invalid check.</summary>
    Valid,

    /// <summary>Core parsed the entity and found at least one invalid check.</summary>
    Invalid,

    /// <summary>Core could not complete the analysis (it did not parse the entity, or it threw). Never shown as legal.</summary>
    Unavailable,
}

/// <summary>
/// The draft state an analysis ran on: the draft (which pins its session and source revision) and its <see cref="EditorDraft.EditRevision"/>.
/// A result is current only while the open draft still has this tag; any accepted edit or a new draft makes it stale.
/// </summary>
/// <param name="Draft">The analysed draft.</param>
/// <param name="EditRevision">The draft's edit revision when it was analysed.</param>
public readonly record struct LegalityTag(EditorDraft Draft, int EditRevision)
{
    /// <summary>The tag of <paramref name="draft"/> as it is now.</summary>
    public static LegalityTag Of(EditorDraft draft) => new(draft, draft.EditRevision);
}

/// <summary>
/// One check Core reported as invalid or suspicious (<see cref="Severity.Fishy"/>), with Core's own text.
/// </summary>
/// <param name="Severity">Core's judgement of the check.</param>
/// <param name="Identifier">What Core checked, used to point at the inspector section that shows it.</param>
/// <param name="Text">Core's localized line for the check.</param>
public sealed record LegalityFinding(Severity Severity, CheckIdentifier Identifier, string Text);

/// <summary>Outcome of a legality analysis run on a draft, tagged with the draft state it describes.</summary>
/// <param name="Tag">The draft state that was analysed.</param>
/// <param name="Verdict">Core's verdict.</param>
/// <param name="Findings">Invalid checks first, then suspicious ones, each group in Core's order. Empty when unavailable.</param>
/// <param name="Report">Core's short report, as <see cref="LegalityFormatting.Report(LegalityAnalysis, bool)"/> gives it; empty when unavailable.</param>
/// <param name="VerboseReport">Core's verbose report, including valid checks and the matched encounter; empty when unavailable.</param>
public sealed record LegalityResult(LegalityTag Tag, LegalityVerdict Verdict, IReadOnlyList<LegalityFinding> Findings, string Report, string VerboseReport)
{
    /// <summary>Number of invalid findings.</summary>
    public int Problems => Findings.Count(f => f.Severity == Severity.Invalid);

    /// <summary>Number of suspicious findings, which do not make the entity invalid.</summary>
    public int Warnings => Findings.Count(f => f.Severity == Severity.Fishy);

    /// <summary>A result for an analysis that could not complete.</summary>
    public static LegalityResult Unavailable(LegalityTag tag) => new(tag, LegalityVerdict.Unavailable, [], "", "");
}
