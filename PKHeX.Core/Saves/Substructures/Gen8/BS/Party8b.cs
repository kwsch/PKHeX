using System;

namespace PKHeX.Core;

/// <summary>
/// Party data storage and metadata
/// </summary>
public sealed class Party8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public int PartyCount
    {
        get => Data[(6 * PokeCrypto.SIZE_8PARTY)];
        set => Data[(6 * PokeCrypto.SIZE_8PARTY)] = (byte)value;
    }

    public int MarkingIndex
    {
        get => Data[(6 * PokeCrypto.SIZE_8PARTY) + 1];
        set => Data[(6 * PokeCrypto.SIZE_8PARTY) + 1] = (byte)value;
    }
}
