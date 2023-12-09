using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class ItemInfo6 : SaveBlock<SAV6>
{
    public ItemInfo6(SAV6 sav, int offset) : base(sav) => Offset = offset;

    private const int BoundItemCount = 4;
    private const int RecentItemCount = 12;

    public ushort[] SelectItems
    {
        // UP,RIGHT,DOWN,LEFT
        get
        {
            var span = Data.AsSpan(Offset + 10);
            var result = new ushort[BoundItemCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = ReadUInt16LittleEndian(span[(2 * i)..]);
            return result;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, BoundItemCount);
            var span = Data.AsSpan(Offset + 10);
            for (int i = 0; i < value.Length; i++)
                WriteUInt16LittleEndian(span[(2 * i)..], value[i]);
        }
    }

    public ushort[] RecentItems
    {
        // Items recently interacted with (Give, Use)
        get
        {
            var span = Data.AsSpan(Offset + 20);
            var result = new ushort[RecentItemCount];
            for (int i = 0; i < result.Length; i++)
                result[i] = ReadUInt16LittleEndian(span[(2 * i)..]);
            return result;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, RecentItemCount);
            var span = Data.AsSpan(Offset + 20);
            for (int i = 0; i < value.Length; i++)
                WriteUInt16LittleEndian(span[(2 * i)..], value[i]);
        }
    }
}
