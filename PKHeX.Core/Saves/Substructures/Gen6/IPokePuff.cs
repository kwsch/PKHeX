namespace PKHeX.Core
{
    public interface IPokePuff
    {
        byte[] Puffs { get; set; }
        int PuffCount { get; set; }
    }

    public interface ILink
    {
        byte[] LinkBlock { get; set; }
    }
}