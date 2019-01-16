namespace PKHeX.Core
{
    public interface ISlotViewer
    {
        /// <summary>
        /// Current index the viewer is viewing.
        /// </summary>
        int ViewIndex { get; }

        /// <summary>
        /// Notification that the <see cref="previous"/> slot is no longer the last interacted slot.
        /// </summary>
        /// <param name="previous">Last interacted slot</param>
        void NotifySlotOld(SlotChange previous);

        /// <summary>
        /// Notification that the <see cref="slot"/> has just been interacted with.
        /// </summary>
        /// <param name="slot">Last interacted slot</param>
        /// <param name="type">Last interacted slot interaction type</param>
        void NotifySlotChanged(SlotChange slot, SlotTouchType type);
    }
}