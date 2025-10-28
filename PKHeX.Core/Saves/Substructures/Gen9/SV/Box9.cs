using System;

namespace PKHeX.Core;

public sealed class Box9(SaveFile sav, SCBlock block) : SaveBlock<SaveFile>(sav, block.Raw)
{
    private const int afterBox = BoxLayout9.BoxCount * 30 * PokeCrypto.SIZE_9PARTY;
    public Memory<byte> RideLegend => Raw.Slice(afterBox, PokeCrypto.SIZE_9PARTY);
}
