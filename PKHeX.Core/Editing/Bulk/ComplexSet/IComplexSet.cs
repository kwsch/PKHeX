namespace PKHeX.Core
{
    /// <summary>
    /// Complex modification of data to a string value.
    /// </summary>
    public interface IComplexSet
    {
        bool IsMatch(string name, string value);
        void Modify(PKM pk, StringInstruction instr);
    }
}
