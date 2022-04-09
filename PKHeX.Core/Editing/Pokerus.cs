namespace PKHeX.Core;

public static class Pokerus
{
    public static int GetMaxDuration(int strain) => (strain & 3) + 1;

    public static bool IsObtainable(PKM pkm) => pkm is not PA8; // don't care about PK1

    public static bool IsStrainValid(PKM pkm, int strain, int days) => IsObtainable(pkm) && IsStrainValid(strain, days);

    public static bool IsStrainValid(int strain, int days) => strain switch
    {
        0 when days is not 0 => false,
        8 => false,
        _ => true,
    };
}
