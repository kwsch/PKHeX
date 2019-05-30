namespace PKHeX.Core
{
    /// <summary>
    /// Indicates the Availability of the Generation 2 Headbutt Tree
    /// </summary>
    public enum TreeEncounterAvailable : byte
    {
        /// <summary>
        /// Encounter is possible a reachable tree
        /// </summary>
        ValidTree,

        /// <summary>
        /// Encounter is only possible a tree reachable only with walk-through walls cheats
        /// </summary>
        InvalidTree,

        /// <summary>
        /// Encounter is not possible in any tree
        /// </summary>
        Impossible
    }
}