using System;

namespace PKHeX.Core;

public sealed class PlayerGeoLocation7b(SAV7b sav, int offset) : SaveBlock<SAV7b>(sav, offset)
{
    public Epoch1970Value AdventureBegin => new(Data.AsMemory(Offset + 0x48));
}
