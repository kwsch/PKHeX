namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the Contest stat details.
    /// </summary>
    public sealed class ContestStatVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Memory;
        public override void Verify(LegalityAnalysis data)
        {
            var pkm = data.pkm;
            if (pkm.Format <= 4)
                return; // legal || not present

            if (pkm is IContestStats s && s.HasContestStats() && !CanHaveContestStats(pkm, data.Info.Generation))
                data.AddLine(GetInvalid(LegalityCheckStrings.LContestZero));

            // some encounters have contest stats built in. they're already checked by the initial encounter match.
        }

        private static bool CanHaveContestStats(PKM pkm, int generation) => generation switch
        {
            1 => false,
            2 => false,
            3 => true,
            4 => true,
            5 => pkm.Format >= 6, // ORAS Contests
            6 => !pkm.IsUntraded || pkm.AO,
            _ => false,
        };
    }
}
