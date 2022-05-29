namespace PKHeX.Core;

public static class LocationEdits
{
    public static int GetNoneLocation(PKM pk) => pk switch
    {
        PB8 => Locations.Default8bNone,
        _ => 0,
    };
}
