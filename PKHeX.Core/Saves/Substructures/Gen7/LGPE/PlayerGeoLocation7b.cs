using System;

namespace PKHeX.Core;

public sealed class PlayerGeoLocation7b : SaveBlock<SAV7b>
{
    public PlayerGeoLocation7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

    public Epoch1970Value AdventureBegin => new(Data.AsMemory(Offset + 0x48));
}
