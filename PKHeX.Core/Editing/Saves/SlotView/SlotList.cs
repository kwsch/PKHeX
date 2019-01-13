using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SlotList : SlotView
    {
        private readonly IReadOnlyList<StorageSlotOffset> Slots;

        public SlotList(SaveFile sav, IReadOnlyList<StorageSlotOffset> slots, bool isReadOnly)
            : base(sav, slots.Count, isReadOnly) => Slots = slots;

        protected override int GetOffset(int index) => Slots[index].Offset;
    }
}