using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface IPokeGroup
    {
        IEnumerable<PKM> Contents { get; }
    }
}
