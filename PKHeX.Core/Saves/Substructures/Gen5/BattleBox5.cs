using System;

namespace PKHeX.Core;

public sealed class BattleBox5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    private const int SizeStored = PokeCrypto.SIZE_5STORED;
    public const int Count = 6;

    public Memory<byte> GetSlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, Count);
        return Raw.Slice(index * SizeStored, SizeStored);
    }

    public Memory<byte> this [int index] => GetSlot(index);

    public bool BattleBoxLocked
    {
        get => Data[0x358] != 0; // Wi-Fi/Live Tournament Active
        set => Data[0x358] = value ? (byte)1 : (byte)0;
    }
}
