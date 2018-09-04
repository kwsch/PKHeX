using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface IBasicStrings
    {
        IReadOnlyList<string> Species { get; }
        IReadOnlyList<string> Item { get; }
        IReadOnlyList<string> Move { get; }
        IReadOnlyList<string> Ability { get; }
        IReadOnlyList<string> Types { get; }
        IReadOnlyList<string> Natures { get; }
        string EggName { get; }
    }
}
