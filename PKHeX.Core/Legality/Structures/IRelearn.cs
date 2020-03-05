using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface IRelearn
    {
        IReadOnlyList<int> Relearn { get; }
    }
}