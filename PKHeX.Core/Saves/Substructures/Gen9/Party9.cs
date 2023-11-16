namespace PKHeX.Core;

public sealed class Party9(SAV9SV sav, SCBlock block) : SaveBlock<SAV9SV>(sav, block.Data)
{
    public int PartyCount
    {
        get => Data[6 * PokeCrypto.SIZE_9PARTY];
        set => Data[6 * PokeCrypto.SIZE_9PARTY] = (byte)value;
    }
}
