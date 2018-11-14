namespace PKHeX.Core
{
    public abstract class SaveBlock
    {
        public int Offset { get; protected set; }
        protected readonly byte[] Data;
        protected SaveBlock(SaveFile SAV) => Data = SAV.Data;
    }
}