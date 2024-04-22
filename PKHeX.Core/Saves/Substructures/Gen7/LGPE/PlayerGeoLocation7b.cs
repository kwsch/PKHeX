using System;

namespace PKHeX.Core;

public sealed class PlayerGeoLocation7b(SAV7b sav, Memory<byte> raw) : SaveBlock<SAV7b>(sav, raw)
{
    public Epoch1970Value AdventureBegin => new(Raw.Slice(0x48, 8));
}
