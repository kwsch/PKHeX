namespace PKHeX.Core;

/// <summary>
/// Rejected Encounter Data containing a reason why the encounter was rejected (not compatible).
/// </summary>
public sealed record EncounterRejected(IEncounterable Encounter, CheckResult Check)
{
    public string Reason => Check.Comment;
}
