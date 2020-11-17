namespace PKHeX.Core
{
    /// <summary>
    /// Stores details about <see cref="GameVersion.GO"/> encounters relevant for legality.
    /// </summary>
    public interface IPogoSlot
    {
        /// <summary> Start date the encounter became available. If zero, no date specified (unbounded start). </summary>
        int Start { get; }

        /// <summary> Last day the encounter was available. If zero, no date specified (unbounded finish). </summary>
        int End { get; }

        /// <summary> Possibility of shiny for the encounter. </summary>
        Shiny Shiny { get; }

        /// <summary> Method the Pokémon may be encountered with. </summary>
        PogoType Type { get; }
    }
}
