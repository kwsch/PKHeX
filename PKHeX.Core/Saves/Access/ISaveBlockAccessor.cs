using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Interface for Accessing named blocks within a save file.
    /// </summary>
    public interface ISaveBlockAccessor<out T>
    {
        /// <summary>
        /// List of all known block details.
        /// </summary>
        IReadOnlyList<T> BlockInfo { get; }
    }
}
