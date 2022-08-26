namespace PKHeX.Core;

/// <inheritdoc cref="EncounterTrade"/>
public abstract record EncounterTradeGB : EncounterTrade
{
    protected EncounterTradeGB(ushort species, byte level, GameVersion game) : base(game)
    {
        Species = species;
        Level = level;
    }

    public abstract override bool IsMatchExact(PKM pk, EvoCriteria evo);
}
