namespace PKHeX.Core
{
    /// <summary>
    /// Provides access for reading and writing ribbon states by ribbon index within the <see cref="PKM"/> structure.
    /// </summary>
    public interface IRibbonIndex
    {
        /// <summary>Gets the state of the ribbon at the requested ribbon index.</summary>
        /// <param name="index"><see cref="RibbonIndex"/></param>
        bool GetRibbon(int index);

        /// <summary>Sets the ribbon index to the provided state.</summary>
        /// <param name="index"><see cref="RibbonIndex"/></param>
        /// <param name="value">value to set</param>
        void SetRibbon(int index, bool value = true);

        /// <summary>
        /// Gets the value of the byte that has the ribbon <see cref="index"/>.
        /// </summary>
        /// <param name="index"><see cref="RibbonIndex"/></param>
        int GetRibbonByte(int index);
    }
}
