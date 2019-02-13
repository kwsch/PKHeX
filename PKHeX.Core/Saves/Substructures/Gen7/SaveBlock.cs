namespace PKHeX.Core
{
    /// <summary>
    /// Base class for a savegame data reader.
    /// </summary>
    public abstract class SaveBlock
    {
        public int Offset { get; protected set; }
        protected readonly byte[] Data;
        protected SaveBlock(SaveFile SAV) => Data = SAV.Data;
    }
}