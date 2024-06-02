using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Logic for rearranging pointers for Box Storage utility
/// </summary>
public static class SlotPointerUtil
{
    private static bool WithinRange(int slot, int min, int max) => min <= slot && slot < max;

    public static void UpdateSwap(int b1, int b2, int slotsPerBox, params IList<int>[] ptrset)
    {
        int diff = (b1 - b2) * slotsPerBox;

        int b1s = b1 * slotsPerBox;
        int b1e = b1s + slotsPerBox;
        int b12 = b1 > b2 ? -diff : diff;

        int b2s = b2 * slotsPerBox;
        int b2e = b2s + slotsPerBox;
        int b21 = b2 > b1 ? -diff : diff;
        foreach (var list in ptrset)
        {
            for (int s = 0; s < list.Count; s++)
            {
                if (WithinRange(b1s, b1e, list[s]))
                    list[s] += b12;
                else if (WithinRange(b2s, b2e, list[s]))
                    list[s] += b21;
            }
        }
    }

    public static void UpdateRepointFrom(IList<PKM> result, IList<PKM> bd, int start, params IList<int>[] slotPointers)
    {
        foreach (var p in slotPointers)
        {
            for (int i = 0; i < p.Count; i++)
            {
                var index = p[i];
                if ((uint)index >= bd.Count)
                    continue;
                var pk = bd[index];
                var newIndex = result.IndexOf(pk);
                if (newIndex < 0)
                    continue;

                Debug.WriteLine($"Re-pointing {pk.Nickname} from {index} to {newIndex}");
                Debug.WriteLine($"{result[newIndex]}");
                p[i] = start + newIndex;
            }
        }
    }

    public static void UpdateRepointFrom(int newIndex, int oldIndex, Span<int> slotPointers)
    {
        // Don't return on first match; assume multiple pointers can point to the same slot
        slotPointers.Replace(oldIndex, newIndex);
    }

    public static void UpdateMove(int bMove, int cMove, int slotsPerBox, params IList<int>[] ptrset)
    {
        int bStart = bMove * slotsPerBox;
        int cStart = cMove * slotsPerBox;
        int bEnd = bStart + slotsPerBox;
        int cEnd = cStart + slotsPerBox;
        int cShift;
        int bShift;
        if (bMove < cMove) // shift chunk down, shift box up
        {
            cShift = -slotsPerBox;
            bShift = (cStart - bStart);
        }
        else // shift box down, shift chunk up
        {
            cShift = slotsPerBox;
            bShift = -(bStart - cStart);
        }
        foreach (var list in ptrset)
        {
            for (int s = 0; s < list.Count; s++)
            {
                if (WithinRange(cStart, cEnd, list[s]))
                    list[s] += cShift;
                if (WithinRange(bStart, bEnd, list[s]))
                    list[s] += bShift;
            }
        }
    }
}
