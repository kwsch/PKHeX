namespace PKHeX.Core
{
    public abstract class SaveBlock
    {
        protected readonly byte[] Data;
        protected SaveBlock(SaveFile SAV) => Data = SAV.Data;
    }
}