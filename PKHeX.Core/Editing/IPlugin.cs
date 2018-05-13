namespace PKHeX.Core
{
    public interface IPlugin
    {
        string Name { get; }
        void Initialize(params object[] args);
        void NotifySaveLoaded();
        ISaveFileProvider SaveFileEditor { get; }
    }
}
