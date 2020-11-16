namespace PKHeX.Core
{
    public interface IPogoSlotTime : IPogoSlot
    {
        int Start { get; }
        int End { get; }
    }
}
