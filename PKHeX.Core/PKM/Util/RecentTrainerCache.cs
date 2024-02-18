namespace PKHeX.Core;

/// <summary>
/// Caches a reference to the most recently loaded trainer data.
/// </summary>
/// <remarks>Useful for sourcing trainer info for the <see cref="PKM"/> ConvertTo methods.</remarks>
public static class RecentTrainerCache
{
    private static ITrainerInfo Trainer = new SimpleTrainerInfo();
    private static IRegionOrigin Trainer67 = new SimpleTrainerInfo(GameVersion.SN);

    private static IRegionOrigin GetTrainer3DS(ITrainerInfo tr) => tr as IRegionOrigin ?? Trainer67;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.OT"/>. </summary>
    public static string OriginalTrainerName => Trainer.OT;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.Gender"/>. </summary>
    public static byte OriginalTrainerGender => Trainer.Gender;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.Language"/>. </summary>
    public static int Language => Trainer.Language;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.Generation"/>. </summary>
    public static byte Format => Trainer.Generation;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.Version"/>. </summary>
    public static GameVersion Version => Trainer.Version;

    /// <summary> Most recently loaded <see cref="ITrainerInfo.Context"/>. </summary>
    public static EntityContext Context => Trainer.Context;

    /// <summary>
    /// Updates the cache with the most recently loaded trainer reference.
    /// </summary>
    /// <param name="trainer"></param>
    public static void SetRecentTrainer(ITrainerInfo trainer)
    {
        Trainer = trainer;
        if (trainer is IRegionOrigin g67)
            Trainer67 = g67;
    }

    /// <inheritdoc cref="SetConsoleRegionData3DS(IRegionOrigin, ITrainerInfo)"/>
    public static void SetConsoleRegionData3DS(IRegionOrigin pk) => SetConsoleRegionData3DS(pk, Trainer);

    /// <summary>
    /// Fetches a trainer to apply details to the input <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to apply details to.</param>
    /// <param name="trainer">Trainer that is receiving the entity.</param>
    public static void SetConsoleRegionData3DS(IRegionOrigin pk, ITrainerInfo trainer)
    {
        var tr = GetTrainer3DS(trainer);
        pk.ConsoleRegion = tr.ConsoleRegion;
        pk.Country = tr.Country;
        pk.Region = tr.Region;
    }

    /// <summary>
    /// Fetches a trainer to apply details to the input <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to apply details to.</param>
    public static void SetFirstCountryRegion(IGeoTrack pk)
    {
        pk.Geo1_Country = pk.Country;
        pk.Geo1_Region = pk.Region;
    }
}
