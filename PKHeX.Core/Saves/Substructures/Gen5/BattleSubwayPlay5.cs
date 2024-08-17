using System;

namespace PKHeX.Core;

public sealed class BattleSubwayPlay5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public int CurrentType   { get => Data[0x00]; set => Data[0x00] = (byte)value; }
    public int CurrentBattle { get => Data[0x02]; set => Data[0x02] = (byte)value; }
}
