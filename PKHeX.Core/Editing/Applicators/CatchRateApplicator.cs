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
            case EncounterGift1 { Version: GameVersion.Stadium, Species: (int)Species.Psyduck }:
                return pk.Japanese ? (byte)167 : (byte)168; // Amnesia Psyduck has different catch rates depending on language
            default:
                var pt = GetPersonalTable(sav, enc.Version);
                var pi = pt[enc.Species];
                return (byte)pi.CatchRate;
        }
    }

    private static PersonalTable1 GetPersonalTable(SaveFile sav, GameVersion ver)
    {
        if (sav.Personal is PersonalTable1 pt)
        {
            var other = sav.Version;
            if (other.Contains(ver) || ver.Contains(other))
                return pt;
        }

        if (!GameVersion.RB.Contains(ver))
            return PersonalTable.Y;
        return PersonalTable.RB;
    }
}
