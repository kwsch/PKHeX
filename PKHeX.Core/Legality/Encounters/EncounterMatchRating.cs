namespace PKHeX.Core
{
    public enum EncounterMatchRating
    {
        /// <summary> Unused </summary>
        None,

        /// <summary> Matches all data, no other matches will be better. </summary>
        Match,

        /// <summary> Matches most data, might have a better match later. </summary>
        Deferred,

        /// <summary> Matches some data, but will likely have a better match later. </summary>
        PartialMatch,
    }
}
