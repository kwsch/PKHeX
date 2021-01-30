namespace PKHeX.Core
{
    /// <summary>
    /// Common Encounter Properties base interface.
    /// <inheritdoc cref="IEncounterInfo"/>
    /// </summary>
    public interface IEncounterable : IEncounterInfo
    {
        string Name { get; }
        string LongName { get; }
    }
}
