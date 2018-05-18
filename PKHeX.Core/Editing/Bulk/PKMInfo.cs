namespace PKHeX.Core
{
    internal sealed class PKMInfo
    {
        internal PKM pkm { get; }
        internal PKMInfo(PKM pk) { pkm = pk; }

        private LegalityAnalysis la;
        private LegalityAnalysis Legality => la ?? (la = new LegalityAnalysis(pkm));

        internal bool Legal => Legality.Valid;
        internal int[] SuggestedRelearn => Legality.GetSuggestedRelearn();
        internal int[] SuggestedMoves => Legality.GetSuggestedMoves(tm: true, tutor: true, reminder: false);
        internal EncounterStatic SuggestedEncounter => Legality.GetSuggestedMetInfo();
    }
}