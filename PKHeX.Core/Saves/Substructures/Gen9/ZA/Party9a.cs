using System;

namespace PKHeX.Core;

public sealed class Party9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    private const int MaxCount = 6;
    public const int SlotSize = PokeCrypto.SIZE_9PARTY + 0x40 + 0x48;

    public Memory<byte> GetSlot(int slot) => block.Raw.Slice(SlotSize * slot, SlotSize);

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
                GetSlot(i).Span.Clear(); // probably should fill with Empty pkm data...
        }
    }
}
