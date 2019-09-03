using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Slot Viewer that shows many slots of <see cref="PKM"/> data.
    /// </summary>
    /// <typeparam name="T">Object that displays the <see cref="PKM"/> slot.</typeparam>
    public interface ISlotViewer<T>
    {
        /// <summary>
        /// Current index the viewer is viewing.
        /// </summary>
        int ViewIndex { get; }

        /// <summary>
        /// Notification that the <see cref="previous"/> slot is no longer the last interacted slot.
        /// </summary>
        /// <param name="previous">Last interacted slot</param>
        void NotifySlotOld(ISlotInfo previous);

        /// <summary>
        /// Notification that the <see cref="slot"/> has just been interacted with.
        /// </summary>
        /// <param name="slot">Last interacted slot</param>
        /// <param name="type">Last interacted slot interaction type</param>
        /// <param name="pkm">Last interacted slot interaction data</param>
        void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm);

        /// <summary>
        /// Gets the information for the requested slot's view picture.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        ISlotInfo GetSlotData(T view);

        /// <summary>
        /// Gets the index of the <see cref="T"/> view within the <see cref="ISlotViewer{T}"/>'s list of slots.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        int GetViewIndex(ISlotInfo slot);

        /// <summary>
        /// List of <see cref="T"/> views the <see cref="ISlotViewer{T}"/> is showing.
        /// </summary>
        IList<T> SlotPictureBoxes { get; }

        /// <summary>
        /// Save data the <see cref="ISlotViewer{T}"/> is showing data from.
        /// </summary>
        SaveFile SAV { get; }
    }
}