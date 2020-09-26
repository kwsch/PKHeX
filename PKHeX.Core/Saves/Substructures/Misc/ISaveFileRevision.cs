namespace PKHeX.Core
{
    public interface ISaveFileRevision
    {
        public int SaveRevision { get; }
        string SaveRevisionString { get; }
    }
}
