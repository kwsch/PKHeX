using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Interface that exposes a Moveset for the object.
    /// </summary>
    internal interface IMoveset
    {
        IReadOnlyList<int> Moves { get; }
    }
}
