namespace PKHeX.Core
{
    public interface IPokePuff
    {
        bool HasPuffData { get; }
        byte[] Puffs { get; set; }
        int PuffCount { get; set; }
    }
}