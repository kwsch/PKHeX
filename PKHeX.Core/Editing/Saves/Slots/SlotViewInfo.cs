namespace PKHeX.Core
{
    /// <summary>
    /// Tuple containing data for a <see cref="Slot"/> and the originating <see cref="View"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SlotViewInfo<T>
    {
        public readonly ISlotInfo Slot;
        public readonly ISlotViewer<T> View;

        public PKM ReadCurrent() => Slot.Read(View.SAV);
        public bool CanWriteTo() => Slot.CanWriteTo(View.SAV);
        public WriteBlockedMessage CanWriteTo(PKM pkm) => Slot.CanWriteTo(View.SAV, pkm);

        public SlotViewInfo(ISlotInfo slot, ISlotViewer<T> view)
        {
            Slot = slot;
            View = view;
        }
    }
}