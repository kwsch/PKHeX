using System;

namespace PKHeX.Core;

public sealed class UnionPokemon7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw)
{
    private const int EntitySize = PokeCrypto.SIZE_6PARTY;

    // 0 - Kyurem (Zekron/Reshiram)
    // 1 - Necrozma (Solgaleo)
    // 2 - Necrozma (Lunala)

    // 1 for S/M, 3 for US/UM.
    // Bounds checks will throw exceptions based on the size of the Raw data this block object is provided.
    public Memory<byte> GetSlot(int index) => Raw.Slice(index * EntitySize, EntitySize);

    public Memory<byte> this[int index] => GetSlot(index);
}
