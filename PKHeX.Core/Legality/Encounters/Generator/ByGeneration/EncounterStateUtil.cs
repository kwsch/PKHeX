namespace PKHeX.Core;

public static class EncounterStateUtil
{
    public static bool CanBeWildEncounter(PKM pk)
    {
        if (pk.IsEgg)
            return false;
        if (IsMetAsEgg(pk))
            return false;
        return true;
    }

    public static bool IsMetAsEgg(PKM pk) => pk switch
    {
        PA8 or PK8 => pk.Egg_Location is not 0 || pk is { BDSP: true, Egg_Day: not 0 },
        PB8 pb8 => pb8.Egg_Location is not Locations.Default8bNone,
        _ => pk.Egg_Location is not 0,
    };
}
