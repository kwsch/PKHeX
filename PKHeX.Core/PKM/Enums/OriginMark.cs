namespace PKHeX.Core;

public enum OriginMark
{
    None,

    Gen6Pentagon,
    Gen7Clover,
    Gen8Galar,
    Gen8Trio,
    Gen8Arc,

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
            return OriginMark.GameBoy;
        if (pk.GO)
            return OriginMark.GO;
        if (pk.LGPE)
            return OriginMark.LetsGo;

        // Lumped Generations
        if (pk.Gen6)
            return OriginMark.Gen6Pentagon;
        if (pk.Gen7)
            return OriginMark.Gen7Clover;
        if (pk.SWSH)
            return OriginMark.Gen8Galar;
        if (pk.BDSP)
            return OriginMark.Gen8Trio;
        if (pk.LA)
            return OriginMark.Gen8Arc;

        return OriginMark.None;
    }
}
