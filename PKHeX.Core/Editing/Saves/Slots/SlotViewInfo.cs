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

        public PKM ReadCurrent() => Slot.Read(View.SAV);
        public bool CanWriteTo() => Slot.CanWriteTo(View.SAV);
        public WriteBlockedMessage CanWriteTo(PKM pkm) => Slot.CanWriteTo(View.SAV, pkm);
    }
}