namespace PKHeX.Core
{
    public class SlotChangeInfo<T>
    {
        public bool LeftMouseIsDown { get; set; }
        public bool DragDropInProgress { get; set; }

        public T Cursor { get; set; }
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
        public bool EitherIsParty => Source.IsParty || Destination.IsParty;

        public void Reset()
        {
            LeftMouseIsDown = DragDropInProgress = false;
            Source = new SlotChange { PKM = Blank };
            Destination = new SlotChange();
            CurrentPath = null;
            Cursor = default;
        }

        public void Invalidate()
        {
            Destination.Slot = -1;
        }
    }
}
