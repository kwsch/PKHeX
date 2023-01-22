namespace PKHeX.Core;

public interface IObedienceLevel : IObedienceLevelReadOnly
{
    new byte Obedience_Level { get; set; }
}

public interface IObedienceLevelReadOnly
{
    byte Obedience_Level { get; } // no setter, use for Encounters
}

public static class ObedienceExtensions
{
    public static byte GetSuggestedObedienceLevel(this IObedienceLevelReadOnly _, PKM entity, int originalMet)
    {
        if (entity.Species is (int)Species.Koraidon or (int)Species.Miraidon && entity is PK9 { FormArgument: not 0 })
            return 0; // Box Legend ride-able is default 0. Everything else is met level!
        if (entity.Version is not (int)GameVersion.SL or (int)GameVersion.VL)
            return (byte)entity.CurrentLevel; // foreign, play it safe.
        // Can just assume min-level
        return (byte)originalMet;
    }
}
