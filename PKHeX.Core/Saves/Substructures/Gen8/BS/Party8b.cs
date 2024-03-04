using System;

namespace PKHeX.Core;

/// <summary>
/// Party data storage and metadata
/// </summary>
public sealed class Party8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    private const int SizeSingle = PokeCrypto.SIZE_8PARTY;
    private const int TeamSize = 6 * SizeSingle;

    public const int SIZE = TeamSize + 2; // 0x812

    public int PartyCount
    {
        get => Data[TeamSize];
        set => Data[TeamSize] = (byte)value;
    }

    public int MarkingIndex
    {
        get => Data[TeamSize + 1];
        set => Data[TeamSize + 1] = (byte)value;
    }
}
