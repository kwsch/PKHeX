using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pushes slot update notifications out to all subscribers.
    /// </summary>
    public sealed class SlotPublisher<T>
    {
        /// <summary>
        /// All <see cref="ISlotViewer{T}"/> instances that provide a view on individual <see cref="ISlotInfo"/> content.
        /// </summary>
        public List<ISlotViewer<T>> Subscribers { get; } = new List<ISlotViewer<T>>();

        public ISlotInfo? Previous { get; private set; }
        public SlotTouchType PreviousType { get; private set; } = SlotTouchType.None;
        public PKM? PreviousPKM { get; private set; }

        /// <summary>
        /// Notifies all <see cref="Subscribers"/> with the latest slot change details.
        /// </summary>
        /// <param name="slot">Last interacted slot</param>
        /// <param name="type">Last interacted slot interaction type</param>
        /// <param name="pkm">Last interacted slot interaction data</param>
        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            foreach (var sub in Subscribers)
                ResetView(sub, slot, type, pkm);
            Previous = slot;
            PreviousType = type;
            PreviousPKM = pkm;
        }

        private void ResetView(ISlotViewer<T> sub, ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            if (Previous != null)
                sub.NotifySlotOld(Previous);

            if (!(slot is SlotInfoBox b) || sub.ViewIndex == b.Box)
                sub.NotifySlotChanged(slot, type, pkm);
        }

        public void ResetView(ISlotViewer<T> sub)
        {
            if (Previous == null || PreviousPKM == null)
                return;
            ResetView(sub, Previous, PreviousType, PreviousPKM);
        }
    }
}
