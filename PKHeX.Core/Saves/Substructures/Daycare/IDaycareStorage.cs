using System;

namespace PKHeX.Core;

public interface IDaycareStorage
{
    int DaycareSlotCount { get; }

    /// <summary>
    /// Gets the segment of memory that holds a stored entity.
    /// </summary>
    Memory<byte> GetDaycareSlot(int index);

    bool IsDaycareOccupied(int index);
    void SetDaycareOccupied(int index, bool occupied);
}
