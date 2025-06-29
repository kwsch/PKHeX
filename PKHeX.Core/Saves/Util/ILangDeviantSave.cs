namespace PKHeX.Core;

/// <summary>
/// <see cref="SaveFile"/> behaves differently for different languages (different structure layout).
/// </summary>
/// <remarks>
/// Save files that behave differently based on language are best to have an entirely separate class implementation,
/// but lack of complete information necessitates sharing implementations.
/// </remarks>
public interface ILangDeviantSave : ISaveFileRevision
{
    bool Japanese { get; }
    bool Korean { get; }
}
