namespace PKHeX.Core
{
    /// <summary>
    /// Exposes simple encounter info and can be converted to a <see cref="PKM"/>.
    /// </summary>
    public interface IEncounterInfo : IEncounterTemplate, IEncounterConvertible
    {
    }
}
