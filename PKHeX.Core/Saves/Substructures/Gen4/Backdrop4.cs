namespace PKHeX.Core;

public enum Backdrop4 : byte
{
    DressUp = 0,
    Ranch = 1,
    CityatNight = 2,
    SnowyTown = 3,
    Fiery = 4,
    OuterSpace = 5,
    Desert = 6,
    CumulusCloud = 7,
    FlowerPatch = 8,
    FutureRoom = 9,
    OpenSea = 10,
    TotalDarkness = 11,
    TatamiRoom = 12,
    GingerbreadRoom = 13,
    Seafloor = 14,
    Underground = 15,
    Sky = 16,

    // Unreleased
    Theater = 17,

    Unset = 18,
}

public static class BackdropInfo
{
    public const int Count = (int)Backdrop4.Unset;
    public const Backdrop4 MaxLegal = Backdrop4.Sky;
    public static bool IsUnset(this Backdrop4 backdrop) => (uint)backdrop >= (uint)Backdrop4.Unset;
}
