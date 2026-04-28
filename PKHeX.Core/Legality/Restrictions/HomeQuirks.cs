namespace PKHeX.Core;

/// <summary>
/// Logic to centralize some of the behaviors of HOME that cause changes to the data when you move it in, and how to determine if those changes have occurred.
/// </summary>
/// <remarks>
/// <seealso cref="EncounterStatic8a.IsAlpha127"/>
/// <seealso cref="WB7.IsHeightWeightFixed"/>
/// <seealso cref="WC8.IsHOMEGift"/>
/// <seealso cref="WA8.IsHOMEGift"/>
/// </remarks>
public static class HomeQuirks
{
    /// <summary>
    /// Checks if the entity has entered HOME after 3.0.0, which started setting the Alpha mark.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <returns>True if the entity has the Alpha mark that indicates it was definitely ingested by HOME 3.0.0 or later, false otherwise.</returns>
    public static bool HasEnteredSetAlphaMark(PKM pk)
    {
        // Mark is only set by HOME ingesting the data for the first time.
        // Before HOME 3.0.0, this mark was never set.
        // Could be okay as a Gen8* format -- don't bother checking for "must have visited HOME 3.0.0+".
        if (pk.LA && pk is PK8 or PB8 or PA8)
            return false; // Could have been moved prior to the HOME 3.0.0 update.

        // Before HOME 4.0.0, this mark was only set when you moved it in for the first time.
        // In HOME 4.0.0, the mark is set by HOME opening your save data and saving, modifying properties without you touching them.
        if (pk.ZA && pk is IScaledSize { HeightScalar: 0 }) // Alphas would update to 255-255-255 scale.
            return false; // Might not have touched HOME yet.
        return true;
    }

    public static bool HasEnteredReachingZA(PA9 pk, EntityContext originalContext)
    {
        if (originalContext != EntityContext.Gen9a)
            return true;

        var scale = pk.Scale;
        if (pk.HeightScalar == scale && pk.WeightScalar == scale)
            return true;
        return false;
    }

    public static bool HasEnteredSetZeroScale(ushort species, byte form)
    {
        // Entering HOME 4.0.0 from a format that exposes IScaledSize3 with a Scale of 0 will clear Height and Weight to 0.
        // Originally noticed on entities with the Mini mark, but isn't required.

        // Check if the species can enter any of the games with format inheriting IScaledSize3.
        if (PersonalTable.SV.IsPresentInGame(species, form))
            return true; // PK9
        if (PersonalTable.ZA.IsPresentInGame(species, form))
            return true; // PA9
        if (PersonalTable.LA.IsPresentInGame(species, form))
            return true; // PA8
        return false;
    }

    /// <summary>
    /// When HOME opens a game, it converts entities into <see cref="PKH"/> entities. When saved, it converts them back to their original format.
    /// During this process, it will copy the Scale value into the <see cref="IScaledSize.HeightScalar"/> value.
    /// If the entity has a HOME tracker, we can be sure that it was modified by HOME.
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="value"></param>
    /// <param name="s2"></param>
    /// <param name="s3"></param>
    /// <returns></returns>
    public static bool IsTouchedScaleCopiedOrUntouched(PKM pk, byte value, IScaledSize s2, IScaledSize3 s3)
    {
        // HOME copies Scale to Height. Untouched by HOME must match the value.
        // Viewing the save file in HOME will alter it too. Tracker definitely indicates it was viewed.
        if (s2.HeightScalar == s3.Scale)
            return true;
        if (pk is IHomeTrack { HasTracker: true })
            return false;
        return s2.HeightScalar == value;
    }


    /// <summary>
    /// Checks if the <see cref="WC9"/> Hisuian Zoroark was erroneously updated by HOME to have 255-255 scale values.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <param name="s">Scale accessor of the entity to check.</param>
    /// <param name="cardID">WC9 card ID of the entity to check.</param>
    /// <returns>>True if the entity has the scale values that indicate it was updated by HOME, false otherwise.</returns>
    public static bool IsGlitchedHisuianZoroarkSV(PKM pk, IScaledSize s, int cardID)
    {
        if (pk is IHomeTrack { HasTracker: false })
            return false;

        if (cardID != 1513)
            return false;

        // Hisiuan Zoroark was locked to 128 but could be bumped to 255 via HOME misapplying PLA's fix for Cavern alphas.
        // This is an OK case of mismatch.
        // This was fixed on HOME 4.0.0 update (2026, a few years after this event distribution ended), and samples could be deposited afterward (thus not requiring 255-all).

        // Check for the forced-max scale values.
        if (s is { HeightScalar: 255, WeightScalar: 255 })
        {
            // if Scale is also present, should be 255.
            return pk is not IScaledSize3 { Scale: not 255 }; 
        }
        return false;
    }

    public static bool HasEnteredSetZeroScale(PKM pk)
    {
        // preconditions: height & weight are both 0; only call this method if 0-0 violates correlations AND we need to check if this activated.

        if (pk is IHomeTrack { HasTracker: false })
            return false;

        if (pk is IScaledSize3 { Scale: not 0 })
            return false;

        // On HOME 4.0.0, entering a game with scale properties without having a scale assigned (on origin game):
        // To populate Scale, HOME copies the HeightScalar value. If it is 0, it destructively overwrites the WeightScalar to 0 as well, even if it was not 0 on the origin game.

        // Ensure we are able to enter a game that can zero out the scale. If not, then we don't need to worry about this quirk.
        if (!HasEnteredSetZeroScale(pk.Species, pk.Form))
            return false;

        // Quirk occurred.
        return true;
    }
}
