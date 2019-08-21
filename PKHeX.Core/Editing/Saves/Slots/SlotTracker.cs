namespace PKHeX.Core
{
    public class SlotTracker<T>
    {
        public int Box { get; set; } = -1;
        public int Slot { get; set; } = -1;
        public T View { get; private set; }
        public SlotTouchType Interaction { get; set; }
    }
}