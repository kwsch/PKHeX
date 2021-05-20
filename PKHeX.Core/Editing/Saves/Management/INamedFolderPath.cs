namespace PKHeX.Core
{
    public interface INamedFolderPath
    {
        string Path { get; }
        string DisplayText { get; }
        bool Custom { get; }
    }
}
