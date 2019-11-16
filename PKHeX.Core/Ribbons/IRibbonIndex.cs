namespace PKHeX.Core
{
    /// <summary>
    /// Provides access for reading and writing ribbon states by ribbon index within the <see cref="PKM"/> structure.
    /// </summary>
    public interface IRibbonIndex
    {
        bool GetRibbon(int index);
        void SetRibbon(int index, bool value = true);
        int GetRibbonByte(int index);
    }
}
