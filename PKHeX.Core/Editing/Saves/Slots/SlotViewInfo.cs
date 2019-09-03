namespace PKHeX.Core
{
    /// <summary>
    /// Tuple containing data for a <see cref="Slot"/> and the originating <see cref="View"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SlotViewInfo<T>
    {
        public ISlotInfo Slot;
        public ISlotViewer<T> View;
    }
}