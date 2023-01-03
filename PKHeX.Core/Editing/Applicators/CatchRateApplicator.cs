namespace PKHeX.Core;

/// <summary>
/// Logic for applying a <see cref="PK1.Catch_Rate"/> value.
/// </summary>
public static class CatchRateApplicator
{
    /// <summary>
    /// Gets the suggested <see cref="PK1.Catch_Rate"/> for the entity.
    /// </summary>
    public static int GetSuggestedCatchRate(PK1 pk, SaveFile sav)
    {
        var la = new LegalityAnalysis(pk);
        return GetSuggestedCatchRate(pk, sav, la);
    }

    /// <summary>
    /// Gets the suggested <see cref="PK1.Catch_Rate"/> for the entity.
    /// </summary>
    public static byte GetSuggestedCatchRate(PK1 pk, SaveFile sav, LegalityAnalysis la)
    {
        // If it is already valid, just use the current value.
        if (la.Valid)
            return pk.Catch_Rate;

        // If it has ever visited generation 2, the Held Item can be removed prior to trade back.
        if (la.Info.Generation == 2)
            return 0;

        // Return the encounter's original value.
        var enc = la.EncounterMatch;
        switch (enc)
        {
            case EncounterTrade1 c:
                return c.GetInitialCatchRate();
            case EncounterStatic1E { Version: GameVersion.Stadium, Species: (int)Species.Psyduck}:
                return pk.Japanese ? (byte)167 : (byte)168; // Amnesia Psyduck has different catch rates depending on language
            default:
                var pt = GetPersonalTable(sav, enc);
                var pi = pt[enc.Species];
                return (byte)pi.CatchRate;
        }
    }

    private static PersonalTable1 GetPersonalTable(SaveFile sav, IEncounterable v)
    {
        if (sav.Personal is PersonalTable1 pt && (sav.Version.Contains(v.Version) || v.Version.Contains(sav.Version)))
            return pt;
        if (!GameVersion.RB.Contains(v.Version))
            return PersonalTable.Y;
        return PersonalTable.RB;
    }
}
