namespace PKHeX.Core
{
    /// <summary>
    /// Rejected Encounter Data containing a reason why the encounter was rejected (not compatible).
    /// </summary>
    public sealed record EncounterRejected
    {
        public readonly IEncounterable Encounter;
        public readonly CheckResult Check;
        public string Reason => Check.Comment;

        public EncounterRejected(IEncounterable encounter, CheckResult check)
        {
            Encounter = encounter;
            Check = check;
        }
    }
}
