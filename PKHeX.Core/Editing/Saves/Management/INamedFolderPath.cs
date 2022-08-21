namespace PKHeX.Core;

/// <summary>
/// Describes details about a folder and other special metadata.
/// </summary>
public interface INamedFolderPath
{
    string Path { get; }
    string DisplayText { get; }
    bool Custom { get; }
}
