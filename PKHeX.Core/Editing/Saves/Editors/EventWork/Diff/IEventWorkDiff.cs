using System.Collections.Generic;

namespace PKHeX.Core;

public interface IEventWorkDiff
{
    List<int> SetFlags { get; }
    List<int> ClearedFlags { get; }
    List<int> WorkChanged { get; }
    List<string> WorkDiff { get; }
    EventWorkDiffCompatibility Message { get; }
    IReadOnlyList<string> Summarize();
}
