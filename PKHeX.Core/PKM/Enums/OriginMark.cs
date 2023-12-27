using static PKHeX.Core.OriginMark;

namespace PKHeX.Core;

/// <summary>
/// Displayed origin mark, indicating the group of games it originated from.
/// </summary>
public enum OriginMark
{
    None,

    Gen6Pentagon,
    Gen7Clover,
    Gen8Galar,
    Gen8Trio,
    Gen8Arc,
    Gen9Paldea,

    GameBoy,
    GO,
    LetsGo,
}

public static class OriginMarkUtil
{
    public static OriginMark GetOriginMark(PKM pk)
    {
        // Specific Markings
        if (pk.VC)
            return GameBoy;
        if (pk.GO)
            return GO;
        if (pk.LGPE)
            return LetsGo;

        // Lumped Generations
        if (pk.Gen6)
            return Gen6Pentagon;
        if (pk.Gen7)
            return Gen7Clover;
        if (pk.SWSH)
            return Gen8Galar;
        if (pk.BDSP)
            return Gen8Trio;
        if (pk.LA)
            return Gen8Arc;
        if (pk.SV)
            return Gen9Paldea;

        return None;
    }
}
