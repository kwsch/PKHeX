using System;

namespace PKHeX.Core;

public interface ITechRecord8
{
    ReadOnlySpan<bool> TechRecordPermitFlags { get; }
    ReadOnlySpan<int> TechRecordPermitIndexes { get; }
    bool GetMoveRecordFlag(int index);
    void SetMoveRecordFlag(int index, bool value = true);
    bool GetMoveRecordFlagAny();
}
