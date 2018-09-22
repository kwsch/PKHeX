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

    /// <summary>
    /// Facilitates interaction with a <see cref="SaveFile"/> or other data location's slot data.
    /// </summary>
    public sealed class SlotEditor
    {
        private readonly SaveFile SAV;
        public SlotPublisher Publisher { get; } = new SlotPublisher();

        private readonly Stack<SlotChange> UndoStack = new Stack<SlotChange>();
        private readonly Stack<SlotChange> RedoStack = new Stack<SlotChange>();

        public SlotEditor(SaveFile sav) => SAV = sav;
        private void NotifySlotChanged(SlotChange slot, SlotTouchType type) => Publisher.NotifySlotChanged(slot, type);

        /// <summary>
        /// Gets data from a slot.
        /// </summary>
        /// <param name="slot">Slot to retrieve from.</param>
        /// <returns>Operation succeeded or not via enum value.</returns>
        public PKM Get(SlotChange slot)
        {
            // Reading from a slot is always allowed.
            var pk = ReadSlot(slot);
            NotifySlotChanged(slot, SlotTouchType.Get);
            return pk;
        }

        /// <summary>
        /// Sets data to a slot.
        /// </summary>
        /// <param name="slot">Slot to be set to.</param>
        /// <param name="pkm">Data to set.</param>
        /// <returns>Operation succeeded or not via enum value.</returns>
        public SlotTouchResult Set(SlotChange slot, PKM pkm)
        {
            if (CantWrite(slot))
                return SlotTouchResult.FailWrite;

            WriteSlot(slot, pkm);
            NotifySlotChanged(slot, SlotTouchType.Set);

            return SlotTouchResult.Success;
        }

        /// <summary>
        /// Deletes a slot.
        /// </summary>
        /// <param name="slot">Slot to be deleted.</param>
        /// <returns>Operation succeeded or not via enum value.</returns>
        public SlotTouchResult Delete(SlotChange slot)
        {
            if (CantWrite(slot))
                return SlotTouchResult.FailDelete;

            DeleteSlot(slot);
            NotifySlotChanged(slot, SlotTouchType.Delete);

            return SlotTouchResult.Success;
        }

        /// <summary>
        /// Swaps two slots.
        /// </summary>
        /// <param name="source">Source slot to be switched with <see cref="dest"/>.</param>
        /// <param name="dest">Destination slot to be switched with <see cref="source"/>.</param>
        /// <returns>Operation succeeded or not via enum value.</returns>
        public SlotTouchResult Swap(SlotChange source, SlotChange dest)
        {
            if (CantWrite(source))
                return SlotTouchResult.FailSource;
            if (CantWrite(dest))
                return SlotTouchResult.FailDestination;

            NotifySlotChanged(source, SlotTouchType.None);
            NotifySlotChanged(dest, SlotTouchType.Swap);

            return SlotTouchResult.Success;
        }

        public bool CantWrite(SlotChange c)
        {
            if (c.Type > StorageSlotType.Party)
                return true;
            return SAV.IsSlotLocked(c.Box, c.Slot);
        }

        private PKM ReadSlot(StorageSlotOffset slot) => SAV.GetPKM(slot);

        private void WriteSlot(SlotChange slot, PKM pkm)
        {
            if (slot.IsParty)
            {
                int count = SAV.PartyCount;
                if (slot.Slot > count)
                    slot.Slot = count;
                SAV.SetPartySlot(pkm, slot.Offset);
            }
            else
            {
                AddUndo(slot);
                SAV.SetStoredSlot(pkm, slot.Offset);
            }
        }

        private void DeleteSlot(SlotChange slot)
        {
            var pkm = slot.PKM;
            if (slot.IsParty)
            {
                SAV.SetPartySlot(pkm, slot.Offset);
                slot.Slot = SAV.PartyCount;
            }
            else
            {
                AddUndo(slot);
                SAV.SetStoredSlot(pkm, slot.Offset);
            }
        }

        public bool CanUndo => UndoStack.Count != 0;
        public bool CanRedo => RedoStack.Count != 0;

        public void Undo()
        {
            if (UndoStack.Count == 0)
                return;

            var change = UndoStack.Pop();
            if (change.Box < 0)
                return;
            AddRedo(change);
            NotifySlotChanged(change, SlotTouchType.Set);
        }

        public void Redo()
        {
            if (RedoStack.Count == 0)
                return;

            var change = RedoStack.Pop();
            if (change.Box < 0)
                return;
            AddUndo(change);
            NotifySlotChanged(change, SlotTouchType.Set);
        }

        private void AddRedo(SlotChange change)
        {
            var slotChange = change.GetInverseData(SAV);
            RedoStack.Push(slotChange);
        }

        private void AddUndo(SlotChange change)
        {
            var slotChange = change.GetInverseData(SAV);
            UndoStack.Push(slotChange);
            RedoStack.Clear();
        }
    }
}
