namespace PKHeX.Core
{
    /// <summary>
    /// Logic for applying a <see cref="PK1.Catch_Rate"/> value.
    /// </summary>
    public static class CatchRateApplicator
    {
        public static int GetSuggestedCatchRate(PK1 pk1, SaveFile sav)
        {
            var la = new LegalityAnalysis(pk1);
            return GetSuggestedCatchRate(pk1, sav, la);
        }

        public static int GetSuggestedCatchRate(PK1 pk1, SaveFile sav, LegalityAnalysis la)
        {
            if (la.Valid)
                return pk1.Catch_Rate;

            if (la.Info.Generation == 2)
                return 0;

            var v = la.EncounterMatch;
            switch (v)
            {
                case EncounterTrade1 c:
                    return c.GetInitialCatchRate();
                case EncounterStatic1E { Version: GameVersion.Stadium, Species: (int)Species.Psyduck}:
                    return pk1.Japanese ? 167 : 168; // Amnesia Psyduck has different catch rates depending on language
                default:
                {
                    if (sav.Version.Contains(v.Version) || v.Version.Contains(sav.Version))
                        return sav.Personal[v.Species].CatchRate;
                    if (!GameVersion.RB.Contains(v.Version))
                        return PersonalTable.Y[v.Species].CatchRate;
                    return PersonalTable.RB[v.Species].CatchRate;
                }
            }
        }
    }
}
