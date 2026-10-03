using PKHeX.Core;
using PKHeX.Web.State;

namespace PKHeX.Web.Services;

/// <summary>
/// Runs Core legality analysis on a copy of a draft and turns Core's output into a <see cref="LegalityResult"/>.
/// A small wrapper: every verdict, severity and line of text comes from Core.
/// </summary>
/// <remarks>
/// <para>
/// Core is synchronous and runs on the calling thread; this class does not move work anywhere else.
/// </para>
/// <para>
/// Core catches exceptions inside the analysis itself (<see cref="LegalityAnalysis"/> is compiled with <c>SUPPRESS</c>) and reports the
/// entity as not parsed, writing the cause only to its debug output; this class reports that as unavailable, with a stand-in exception for the
/// console. An exception outside Core's guard (the personal-table lookup, formatting the reports) is caught here and becomes unavailable too.
/// Neither is ever shown as a legal result.
/// </para>
/// <para>
/// Some Core checks compare an entity with the trainer of the save it is in (for example, the current-handler state), and only run when
/// Core's active trainer is set. The desktop editor sets it when a save is loaded (<see cref="ParseSettings.InitFromSaveFileData"/>); this
/// class sets it to the session's save for each analysis and clears it afterwards (<see cref="InTrainerContext{T}"/>), so verdicts match the
/// desktop and no other save's trainer can leak into a result.
/// </para>
/// </remarks>
public sealed class LegalityService
{
    /// <summary>Creates Core's analysis of an entity, with the save's personal table and the slot's storage type.</summary>
    internal delegate LegalityAnalysis Analyzer(PKM entity, IPersonalTable table, StorageSlotType slotType);

    private readonly Analyzer analyze;

    /// <summary>The service that calls Core directly.</summary>
    public static LegalityService Default { get; } = new((pk, table, type) => new LegalityAnalysis(pk, table, type));

    internal LegalityService(Analyzer analyze) => this.analyze = analyze;


    /// <summary>Serialises use of Core's global active trainer, so concurrent callers (tests) cannot see each other's trainer.</summary>
    private static readonly Lock TrainerContext = new();

    /// <summary>
    /// Runs <paramref name="work"/> with Core's active trainer set to <paramref name="save"/>, as the desktop editor has it for a loaded save,
    /// and clears it afterwards. Native comparisons in the tests use the same method, so they analyse exactly as the app does.
    /// </summary>
    /// <param name="save">The save whose trainer is active, or null to run with none (only tests do, to show the difference).</param>
    /// <param name="work">The analysis and any formatting of it.</param>
    internal static T InTrainerContext<T>(SaveFile? save, Func<T> work)
    {
        lock (TrainerContext)
        {
            if (save is null)
            {
                ParseSettings.ClearActiveTrainer();
            }
            else
            {
                ParseSettings.InitFromSaveFileData(save);
            }
            try
            {
                return work();
            }
            finally
            {
                ParseSettings.ClearActiveTrainer();
            }
        }
    }

    /// <summary>
    /// Analyses the drafted entity as it is now, in the context of <paramref name="session"/>'s save and the draft's slot type (party or box).
    /// The draft and the session are not changed.
    /// </summary>
    /// <param name="session">The session the draft was taken from.</param>
    /// <param name="draft">The draft to analyse.</param>
    /// <param name="failure">
    /// Why the analysis is unavailable, for the browser console only: the exception, or a stand-in when Core caught it itself; null otherwise.
    /// </param>
    /// <exception cref="SessionException">The draft is foreign or stale.</exception>
    public LegalityResult Analyze(SaveSession session, EditorDraft draft, out Exception? failure)
    {
        session.EnsureOwns(draft);
        session.EnsureCurrent(draft);
        var tag = LegalityTag.Of(draft);
        var save = session.Working;
        Exception? caught = null;
        var result = InTrainerContext(save, () =>
        {
            try
            {
                var entity = draft.ToStoredEntity();
                var analysis = analyze(entity, save.Personal, draft.Slot.ToSlotInfo(save).Type);
                if (!analysis.Parsed)
                {
                    caught = new LegalityNotParsedException();
                    return LegalityResult.Unavailable(tag);
                }
                var context = LegalityLocalizationContext.Create(analysis);
                var verdict = analysis.Valid ? LegalityVerdict.Valid : LegalityVerdict.Invalid;
                return new LegalityResult(tag, verdict, Findings(context, entity), context.Report(false), context.Report(true));
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                caught = ex;
                return LegalityResult.Unavailable(tag);
            }
        });
        failure = caught;
        return result;
    }

    /// <summary>
    /// The checks Core reports as invalid (the lines of its short report, in the same order) followed by those it judges suspicious.
    /// </summary>
    /// <param name="context">Core's formatter for the analysis.</param>
    /// <param name="entity">The analysed entity.</param>
    private static List<LegalityFinding> Findings(LegalityLocalizationContext context, PKM entity)
    {
        var analysis = context.Analysis;
        var findings = new List<LegalityFinding>();
        var moves = analysis.Info.Moves;
        for (var i = 0; i < moves.Length; i++)
        {
            if (!moves[i].Valid)
            {
                findings.Add(new(Severity.Invalid, CheckIdentifier.CurrentMove, context.FormatMove(moves[i], i + 1, entity.Context)));
            }
        }
        // Core reports relearn moves from Generation 6 on, as its short report does.
        if (entity.Format >= 6)
        {
            var relearn = analysis.Info.Relearn;
            for (var i = 0; i < relearn.Length; i++)
            {
                if (!relearn[i].Valid)
                {
                    findings.Add(new(Severity.Invalid, CheckIdentifier.RelearnMove, context.FormatRelearn(relearn[i], i + 1)));
                }
            }
        }
        foreach (var check in analysis.Results)
        {
            if (!check.Valid)
            {
                findings.Add(new(check.Judgement, check.Identifier, context.Humanize(check)));
            }
        }
        foreach (var check in analysis.Results)
        {
            if (check.Judgement == Severity.Fishy)
            {
                findings.Add(new(check.Judgement, check.Identifier, context.Humanize(check)));
            }
        }
        return findings;
    }
}

/// <summary>
/// Reported when Core returns an analysis as not parsed. Core has already caught the cause and keeps it to its debug output, so the type name is
/// all a diagnostic report can say about it.
/// </summary>
public sealed class LegalityNotParsedException() : InvalidOperationException("PKHeX.Core could not complete the legality analysis (the entity was not parsed).");
