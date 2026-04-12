using System;

namespace PKHeX.Core;

public sealed class Party9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    private const int MaxCount = 6;
    public const int SlotSize = PokeCrypto.SIZE_8PARTY + 0x40 + 0x48;

    public Memory<byte> GetSlot(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, MaxCount);
        return block.Raw.Slice(SlotSize * slot, SlotSize);
    }

    public int PartyCount
    {
        get
        {
            for (int i = 0; i < MaxCount; i++)
            {
                var span = GetSlot(i).Span;
                if (!EntityDetection.IsPresent(span))
                    return i;
            }
            return MaxCount;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan<uint>((uint)value, MaxCount);
            var current = PartyCount;
            if (value >= current)
                return;
            for (int i = value; i < current; i++)
                WriteEmpty(i);
        }
    }

    public void WriteEmpty(int slot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)slot, MaxCount);
        var span = GetSlot(slot).Span;
        span.Clear();
        PokeCrypto.Encrypt8(span[..PokeCrypto.SIZE_8STORED]);
        PokeCrypto.CryptArray(span[PokeCrypto.SIZE_8STORED..PokeCrypto.SIZE_8PARTY], 0);
        span[PokeCrypto.SIZE_8PARTY] = 1; // mark as present, even if it is empty, to match game behavior for at-rest save data.
    }
}
