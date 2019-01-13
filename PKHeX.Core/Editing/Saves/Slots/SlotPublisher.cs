using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Pushes slot update notifications out to all subscribers.
    /// </summary>
    public sealed class SlotPublisher
    {
        /// <summary>
        /// All <see cref="ISlotViewer"/> instances that provide a view on individual <see cref="StorageSlotOffset"/> content.
        /// </summary>
        public List<ISlotViewer> Subscribers { get; } = new List<ISlotViewer>();

        private SlotChange Previous;
        private SlotTouchType PreviousType = SlotTouchType.None;

        /// <summary>
        /// Notifies all <see cref="Subscribers"/> with the latest slot change details.
        /// </summary>
        /// <param name="slot">Last interacted slot</param>
        /// <param name="type">Last interacted slot interaction type</param>
        public void NotifySlotChanged(SlotChange slot, SlotTouchType type)
        {
            foreach (var sub in Subscribers)
                ResetView(sub, slot, type);
            Previous = slot;
            PreviousType = type;
        }

        private void ResetView(ISlotViewer sub, SlotChange slot, SlotTouchType type)
        {
            if (Previous != null)
                sub.NotifySlotOld(Previous);

            int index = sub.ViewIndex;
            if (index == slot.Box)
                sub.NotifySlotChanged(slot, type);
        }

        public void ResetView(ISlotViewer sub) => ResetView(sub, Previous, PreviousType);
    }
}
