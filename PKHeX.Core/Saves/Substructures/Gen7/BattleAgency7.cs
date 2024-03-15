using System;

namespace PKHeX.Core;

public sealed class BattleAgency7(SAV7USUM sav, Memory<byte> raw) : SaveBlock<SAV7USUM>(sav, raw)
{
    public static int GetSlotOffset(int slot) => slot switch
    {
        0 => 0,
        1 => PokeCrypto.SIZE_6STORED,
        // 0x30 bytes in between
        2 => 0x220,
        _ => throw new ArgumentOutOfRangeException(nameof(slot)),
    };

    public Memory<byte> this[int i] => Raw.Slice(GetSlotOffset(i), PokeCrypto.SIZE_6STORED);
}
