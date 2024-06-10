namespace PKHeX.Core;

public enum Backdrop4 : byte
{
    DressUp,
    Ranch,
    CityatNight,
    SnowyTown,
    Fiery,
    OuterSpace,
    Desert,
    CumulusCloud,
    FlowerPatch,
    FutureRoom,
    OpenSea,
    TotalDarkness,
    TatamiRoom,
    GingerbreadRoom,
    Seafloor,
    Underground,
    Sky,

    // Unreleased
    Theater,

    Unset,
}

public static class BackdropInfo
{
    public const int Count = (int)Backdrop4.Unset;
    public const Backdrop4 MaxLegal = Backdrop4.Sky;
    public static bool IsUnset(this Backdrop4 backdrop) => (uint)backdrop >= (uint)Backdrop4.Unset;
}
