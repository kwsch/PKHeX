using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.Tests;

/// <summary>
/// Native Core's legality analysis of an entity, made the way the desktop editor makes it for a loaded save: with the save's personal table,
/// the slot type, and Core's active trainer set to the save (through <see cref="LegalityService.InTrainerContext{T}"/>). Every comparison
/// with "native Core" in the tests uses this, so an oracle can never agree with the app by both leaving the trainer out.
/// </summary>
/// <param name="Parsed">Core parsed the entity.</param>
/// <param name="Valid">Core's verdict.</param>
/// <param name="Report">Core's short report.</param>
/// <param name="VerboseReport">Core's verbose report.</param>
/// <param name="Warnings">Number of checks Core judged suspicious.</param>
internal sealed record NativeLegality(bool Parsed, bool Valid, string Report, string VerboseReport, int Warnings)
{
    /// <summary>The status word the app shows for this analysis.</summary>
    public string Verdict => Parsed ? Valid ? "Valid" : "Invalid" : "Unavailable";

    /// <summary>Analyses a copy of <paramref name="entity"/> as stored in <paramref name="save"/> at a slot of type <paramref name="type"/>.</summary>
    public static NativeLegality Of(SaveFile save, PKM entity, StorageSlotType type) => LegalityService.InTrainerContext(save, () =>
    {
        var analysis = new LegalityAnalysis(entity.Clone(), save.Personal, type);
        return new NativeLegality(analysis.Parsed, analysis.Valid, analysis.Report(), analysis.Report(verbose: true),
            analysis.Results.Count(r => r.Judgement == Severity.Fishy));
    });
}
