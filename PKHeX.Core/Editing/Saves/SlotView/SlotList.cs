using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SlotList : SlotView
    {
        private readonly IReadOnlyList<SlotInfoMisc> Slots;

        public SlotList(SaveFile sav, IReadOnlyList<SlotInfoMisc> slots, bool isReadOnly)
            : base(sav, slots.Count, isReadOnly) => Slots = slots;

        protected override int GetOffset(int index) => Slots[index].Offset;
    }
}