using static PKHeX.Core.Species;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_1 = 151;
    internal const int MaxMoveID_1 = 165;
    internal const int MaxItemID_1 = 255;
    internal const int MaxAbilityID_1 = 0;

    internal static readonly ushort[] Pouch_Items_RBY =
    {
        000,001,002,003,004,005,006,            010,011,012,013,014,015,
        016,017,018,019,020,                                029,030,031,
        032,033,034,035,036,037,038,039,040,041,042,043,    045,046,047,
        048,049,    051,052,053,054,055,056,057,058,    060,061,062,063,
        064,065,066,067,068,069,070,071,072,073,074,075,076,077,078,079,
        080,081,082,083,

        // ...

        196,197,198,199,200,201,202,203,204,205,206,207,
        208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,
        224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,
        240,241,242,243,244,245,246,247,248,249,250,
    };

    internal static bool TransferSpeciesDefaultAbilityGen1(ushort species)
    {
        System.Diagnostics.Debug.Assert((uint)species <= MaxSpeciesID_1);
        return species is (int)Gastly or (int)Haunter or (int)Gengar
            or (int)Koffing or (int)Weezing
            or (int)Mew;
    }
}
