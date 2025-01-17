namespace PKHeX.Core;

public sealed class Party8a(SAV8LA sav, SCBlock block) : SaveBlock<SAV8LA>(sav, block.Raw)
{
    public int PartyCount
    {
        get => Data[6 * PokeCrypto.SIZE_8APARTY];
        set => Data[6 * PokeCrypto.SIZE_8APARTY] = (byte)value;
    }
}
