using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public static class StorageUtil
{
    public static bool CompressStorage(this SaveFile sav, Span<byte> storage, out int storedCount, Span<int> slotPointers)
    {
        // keep track of empty slots, and only write them at the end if slots were shifted (no need otherwise).
        var empty = new List<byte[]>();
        bool shiftedSlots = false;

        ushort ctr = 0;
        int size = sav.SIZE_BOXSLOT;
        int count = sav.BoxSlotCount * sav.BoxCount;
        for (int i = 0; i < count; i++)
        {
            int offset = sav.GetBoxSlotOffset(i);
            if (sav.IsPKMPresent(storage[offset..]))
            {
                if (ctr != i) // copy required
                {
                    shiftedSlots = true; // appending empty slots afterward is now required since a rewrite was done
                    int destOfs = sav.GetBoxSlotOffset(ctr);
                    storage[offset..(offset + size)].CopyTo(storage[destOfs..(destOfs + size)]);
                    SlotPointerUtil.UpdateRepointFrom(ctr, i, slotPointers);
                }

                ctr++;
                continue;
            }

            // pop out an empty slot; save all unused data & preserve order
            var data = storage.Slice(offset, size).ToArray();
            empty.Add(data);
        }

        storedCount = ctr;

        if (!shiftedSlots)
            return false;

        for (int i = ctr; i < count; i++)
        {
            var data = empty[i - ctr];
            int offset = sav.GetBoxSlotOffset(i);
            data.CopyTo(storage[offset..]);
        }

        return true;
    }

    public static int FindSlotIndex(this SaveFile sav, Func<PKM, bool> method, int maxCount)
    {
        for (int i = 0; i < maxCount; i++)
        {
            var pk = sav.GetBoxSlotAtIndex(i);
            if (method(pk))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Copies a <see cref="T"/> list to the destination list, with an option to copy to a starting point.
    /// </summary>
    /// <param name="list">Source list to copy from</param>
    /// <param name="dest">Destination list/array</param>
    /// <param name="skip">Criteria for skipping a slot</param>
    /// <param name="start">Starting point to copy to</param>
    /// <returns>Count of <see cref="T"/> copied.</returns>
    public static int CopyTo<T>(this IEnumerable<T> list, Span<T> dest, Func<int, bool> skip, int start = 0)
    {
        int ctr = start;
        int skipped = 0;
        foreach (var z in list)
        {
            // seek forward to next open slot
            int next = FindNextValidIndex(dest, skip, ctr);
            if (next == -1)
                break;
            skipped += next - ctr;
            ctr = next;
            dest[ctr++] = z;
        }
        return ctr - start - skipped;
    }

    public static int FindNextValidIndex<T>(Span<T> dest, Func<int, bool> skip, int ctr)
    {
        while (true)
        {
            if ((uint)ctr >= dest.Length)
                return -1;
            var exist = dest[ctr];
            if (exist == null || !skip(ctr))
                return ctr;
            ctr++;
        }
    }
}
