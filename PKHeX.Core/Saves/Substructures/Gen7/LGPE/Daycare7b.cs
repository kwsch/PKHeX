using System;

namespace PKHeX.Core;

public sealed class Daycare7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    private const int StoredSize = PokeCrypto.SIZE_6STORED;
    public Memory<byte> Stored => Raw.Slice(8, StoredSize);
}
