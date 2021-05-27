namespace PKHeX.Core
{
    /// <summary>
    /// Complex filter of data based on a string value.
    /// </summary>
    public interface IComplexFilter
    {
        bool IsMatch(string prop);
        bool IsFiltered(PKM pkm, StringInstruction value);
        bool IsFiltered(BatchInfo info, StringInstruction value);
    }
}
