using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Tracks the state of displaying selectable movesets, and indicates if they are ordered.
/// </summary>
public sealed class LegalMoveComboSource : ILegalMoveDisplaySource<ComboItem>
{
    private readonly bool[] IsMoveBoxOrdered = new bool[4];
    private ComboItem[] MoveDataAllowed = [];

    public IReadOnlyList<ComboItem> DataSource => [.. MoveDataAllowed]; // copy

    /// <summary>
    /// Resets the <see cref="MoveDataAllowed"/> data source with an updated collection.
    /// </summary>
    public void ReloadMoves(IReadOnlyList<ComboItem> moves)
    {
        MoveDataAllowed = [.. moves]; // copy
        ClearUpdateCheck();
    }

    public bool GetIsMoveBoxOrdered(int index) => IsMoveBoxOrdered[index];
    public void SetIsMoveBoxOrdered(int index, bool value) => IsMoveBoxOrdered[index] = value;

    public void ReloadMoves(LegalMoveInfo info)
    {
        ClearUpdateCheck();
        SortMoves(info);
    }

    private void SortMoves(LegalMoveInfo info) => Array.Sort(MoveDataAllowed, (i1, i2) => Compare(i1, i2, info.CanLearn));

    // defer re-population until dropdown is opened; handled by dropdown event
    private void ClearUpdateCheck() => IsMoveBoxOrdered.AsSpan().Clear();

    private static int Compare(ComboItem i1, ComboItem i2, Func<ushort, bool> check)
    {
        // split into 2 groups: Allowed & Not, and sort each sublist
        var (strA, value1) = i1;
        var (strB, value2) = i2;
        var c1 = check((ushort)value1);
        var c2 = check((ushort)value2);
        if (c1)
            return c2 ? string.CompareOrdinal(strA, strB) : -1;
        return c2 ? 1 : string.CompareOrdinal(strA, strB);
    }
}
