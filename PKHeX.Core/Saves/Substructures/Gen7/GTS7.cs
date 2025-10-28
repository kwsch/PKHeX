using System;

namespace PKHeX.Core;

public sealed class GTS7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    // 0x00: Stored Upload
    private const int SizeStored = PokeCrypto.SIZE_6STORED;

    public Memory<byte> Upload => Raw[..SizeStored];
}
