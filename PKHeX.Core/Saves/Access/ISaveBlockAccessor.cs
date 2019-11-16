using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface ISaveBlockAccessor<out T>
    {
        IReadOnlyList<T> BlockInfo { get; }
    }
}