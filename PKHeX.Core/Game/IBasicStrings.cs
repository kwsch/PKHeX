using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// String providing interface with minimal support for game strings.
    /// </summary>
    public interface IBasicStrings
    {
        IReadOnlyList<string> Species { get; }
        IReadOnlyList<string> Item { get; }
        IReadOnlyList<string> Move { get; }
        IReadOnlyList<string> Ability { get; }
        IReadOnlyList<string> Types { get; }
        IReadOnlyList<string> Natures { get; }

        /// <summary>
        /// Name an Egg has when obtained on this language.
        /// </summary>
        string EggName { get; }
    }
}
