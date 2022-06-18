using System;

namespace PKHeX.Core;

public static class GenerationTraversal
{
    /// <summary>
    /// Gets the generation numbers in descending order for iterating over.
    /// </summary>
    public static int[] GetVisitedGenerationOrder(PKM pk, int origin)
    {
        if (pk.Format < 3)
            return GetVisitedGenerationOrderGB(pk, pk.Format);
        if (pk.VC)
            return GetVisitedGenerationOrderVC(pk, origin);
        return GetVisitedGenerationOrder(pk.Format, origin);
    }

    private static int[] GetVisitedGenerationOrderVC(PKM pk, int origin)
    {
        // VC case: check transfer games in reverse order (8, 7..) then past games.
        int[] xfer = GetVisitedGenerationOrder(pk.Format, 7);
        int[] past = GetVisitedGenerationOrderGB(pk, origin);
        int end = xfer.Length;
        Array.Resize(ref xfer, xfer.Length + past.Length);
        past.CopyTo(xfer, end);
        return xfer;
    }

    private static readonly int[] G2 = { 2 };
    private static readonly int[] G12 = { 1, 2 };
    private static readonly int[] G21 = { 2, 1 };

    private static int[] GetVisitedGenerationOrderGB(PKM pk, int originalGeneration)
    {
        if (originalGeneration == 2)
            return pk.Korean ? G2 : G21;
        return G12; // RBY
    }

    private static int[] GetVisitedGenerationOrder(int start, int end)
    {
        if (end < 0)
            return Array.Empty<int>();
        if (start <= end)
            return new[] { start };
        var order = new int[start - end + 1];
        for (int i = 0; i < order.Length; i++)
            order[i] = start - i;
        return order;
    }
}
