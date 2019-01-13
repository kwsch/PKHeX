namespace PKHeX.Core
{
    public class SlotBoxes : SlotView
    {
        public SlotBoxes(SaveFile sav)
            : base(sav, sav.SlotCount, false) { }

        protected override int GetOffset(int index) => SAV.GetBoxSlotOffset(index);

        protected override void ShiftDown(int startIndex) { }
        protected override void ShiftUp(int startIndex) { }
    }
}