using System;

namespace PKHeX.Core;

public sealed class UnionPokemon6(SAV6 sav, Memory<byte> raw) : SaveBlock<SAV6>(sav, raw)
{
    private const int SizeStored = PokeCrypto.SIZE_6STORED;

    public Memory<byte> this[int index] => Raw[..SizeStored];
}
