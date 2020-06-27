using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Information wrapper used for Bulk Editing to apply suggested values.
    /// </summary>
    internal sealed class PKMInfo
    {
        internal PKM Entity { get; }
        internal PKMInfo(PKM pk) { Entity = pk; }

        private LegalityAnalysis? la;
        internal LegalityAnalysis Legality => la ??= new LegalityAnalysis(Entity);

        public bool Legal => Legality.Valid;
        internal IReadOnlyList<int> SuggestedRelearn => Legality.GetSuggestedRelearnMoves();
    }
}