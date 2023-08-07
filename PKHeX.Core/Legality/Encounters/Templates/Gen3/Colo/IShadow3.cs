using System;

namespace PKHeX.Core;

public interface IShadow3
{
    GameVersion Version { get; }
    ReadOnlyMemory<TeamLock> PartyPrior { get; }
}
