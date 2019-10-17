using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface ISaveBlockAccessor<out T> where T : BlockInfo
    {
        IReadOnlyList<T> BlockInfo { get; }
    }
}