using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SlotArray : SlotList
    {
        public SlotArray(SaveFile sav, IReadOnlyList<StorageSlotOffset> slots, bool isReadOnly)
            : base(sav, slots, isReadOnly) { }

        protected override void ShiftDown(int startIndex) { }
        protected override void ShiftUp(int startIndex) { }
    }
}