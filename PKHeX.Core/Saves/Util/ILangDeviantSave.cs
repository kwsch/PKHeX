namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="SaveFile"/> behaves differently for different languages (different structure layout).
    /// </summary>
    public interface ILangDeviantSave
    {
        bool Japanese { get; }
        bool Korean { get; }
    }
}