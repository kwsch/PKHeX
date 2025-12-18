using static PKHeX.Core.Ball;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

internal static class BallUseLegality
{
    /// <summary>
    /// In Sun/Moon, capturing with Heavy Ball is impossible in Sun/Moon for specific hard to catch species.
    /// </summary>
    /// <param name="species">Encounter species.</param>
    /// <remarks>
    /// Catch rate for these species is 3. Due to the heavy ball modifier adding [-20], the catch rate becomes 0.
    /// </remarks>
    /// <returns>True if it is impossible to capture in a <see cref="Heavy"/> ball.</returns>
    public static bool IsAlolanCaptureNoHeavyBall(ushort species) => species is (int)Beldum
        or (int)TapuKoko or (int)TapuLele or (int)TapuBulu or (int)TapuFini;

    public static bool IsBallPermitted(ulong permit, byte ball)
    {
        if (ball >= 64)
            return false;
        return (permit & (1ul << ball)) != 0;
    }

    public static ulong GetWildBalls(byte generation, GameVersion version) => generation switch
    {
        1 => WildPokeBalls1,
        2 => WildPokeBalls2,
        3 => WildPokeBalls3,
        4 => GameVersion.HGSS.Contains(version) ? WildPokeBalls4_HGSS : WildPokeBalls4_DPPt,
        5 => WildPokeBalls5,
        6 => WildPokeballs6,
        7 => GameVersion.Gen7b.Contains(version) ? WildPokeballs7b : WildPokeballs7,
        8 when GameVersion.BDSP.Contains(version) => WildPokeBalls4_HGSS,
        8 when version is GameVersion.PLA => WildPokeBalls8a,
        8 => GameVersion.GO == version ? WildPokeballs8g_WithRaid : WildPokeballs8,
        9 when version is GameVersion.ZA => WildPokeballs9a,
        9 => WildPokeballs9,
        _ => 0,
    };

    private const ulong WildPokeRegular = (1 << (int)Master)
                                        | (1 << (int)Ultra)
                                        | (1 << (int)Great)
                                        | (1 << (int)Poke);

    private const ulong WildPokeKurt4 = (1 << (int)Fast)
                                      | (1 << (int)Level)
                                      | (1 << (int)Lure)
                                      | (1 << (int)Heavy)
                                      | (1 << (int)Love)
                                      | (1 << (int)Friend)
                                      | (1 << (int)Moon);

    private const ulong WildPokeEnhance3 = (1 << (int)Net)
                                         | (1 << (int)Dive)
                                         | (1 << (int)Nest)
                                         | (1 << (int)Repeat)
                                         | (1 << (int)Timer)
                                         | (1 << (int)Luxury)
                                         | (1 << (int)Premier);

    private const ulong WildPokeEnhance4 = (1 << (int)Dusk) | (1 << (int)Heal) | (1 << (int)Quick);
    private const ulong WildPokeEnhance5 = (1 << (int)Dream);
    private const ulong WildPokeEnhance7 = (1 << (int)Beast);
    private const ulong WildPokeEnhance8 = (1 << (int)Dream) | (1 << (int)Safari) | (1 << (int)Sport);

    private const ulong WildPokeBalls8a =
      (1ul << (int)LAPoke)
    | (1ul << (int)LAGreat)
    | (1ul << (int)LAUltra)
    | (1ul << (int)LAFeather)
    | (1ul << (int)LAWing)
    | (1ul << (int)LAJet)
    | (1ul << (int)LAHeavy)
    | (1ul << (int)LALeaden)
    | (1ul << (int)LAGigaton);

    private const ulong WildPokeBalls1 = 1 << (int)Poke;
    private const ulong WildPokeBalls2 = WildPokeBalls1;
    private const ulong WildPokeBalls3 = WildPokeRegular | WildPokeEnhance3;
    private const ulong WildPokeBalls4_DPPt = WildPokeBalls3 | WildPokeEnhance4;
    public const ulong WildPokeBalls4_HGSS = WildPokeBalls4_DPPt | WildPokeKurt4;
    private const ulong WildPokeBalls5 = WildPokeBalls4_DPPt;

    public const ulong DreamWorldBalls = WildPokeBalls5 | WildPokeEnhance5;
    private const ulong WildPokeballs6 = WildPokeBalls5; // Same as Gen5
    private const ulong WildPokeballs7 = WildPokeBalls4_HGSS | WildPokeEnhance7; // Same as HG/SS + Beast
    private const ulong WildPokeballs8 = WildPokeballs7 | WildPokeEnhance8;

    private const ulong WildPokeballs7b = WildPokeRegular | (1 << (int)Premier);
    public const ulong WildPokeballs8g_WithRaid = WildPokeballs7b & ~(1ul << (int)Master); // Ultra Great Poke Premier, no Master
    public const ulong WildPokeballs8g_WithoutRaid = WildPokeRegular & ~(1ul << (int)Master); // Ultra Great Poke, no Premier/Master

    public const ulong WildPokeballs9 = WildPokeballs8;
    public const ulong WildPokeballs9PreDLC2 = WildPokeballs7 | WildPokeEnhance5; // Same as Gen7 + Dream

    public const ulong WildPokeballs9a = WildPokeballs9;
}
