namespace PKHeX.Core
{
    public class SlotChangeInfo
    {
        public bool LeftMouseIsDown { get; set; }
        public bool RightMouseIsDown { get; set; }
        public bool DragDropInProgress { get; set; }

        public object Cursor { get; set; }
        public string CurrentPath { get; set; }

        public SlotChange Source { get; set; }
        public SlotChange Destination { get; set; }

        private readonly PKM Blank;

        public SlotChangeInfo(SaveFile sav)
        {
            Blank = sav.BlankPKM;
            Reset();
        }

        public bool SameSlot => Source.Slot == Destination.Slot && Source.Box == Destination.Box;

        public void Reset()
        {
            LeftMouseIsDown = RightMouseIsDown = DragDropInProgress = false;
            Source = new SlotChange { PKM = Blank };
            Destination = new SlotChange();
            Cursor = CurrentPath = null;
        }
    }
}
