using PKHeX.Core;

namespace PKHeX.WinForms
{
    public class SlotChangeInfo
    {
        public bool LeftMouseIsDown { get; set; }
        public bool RightMouseIsDown { get; set; }
        public bool DragDropInProgress { get; set; }

        public object Cursor { get; set; }
        public string CurrentPath { get; set; }

        public SlotChange Source { get; private set; }
        public SlotChange Destination { get; private set; }

        private readonly byte[] BlankData;

        public SlotChangeInfo(SaveFile sav)
        {
            BlankData = sav.BlankPKM.EncryptedPartyData;
            Reset();
        }

        public bool SameSlot => Source.Slot == Destination.Slot && Source.Box == Destination.Box;
        public void Reset()
        {
            LeftMouseIsDown = RightMouseIsDown = DragDropInProgress = false;
            Source = new SlotChange {OriginalData = BlankData};
            Destination = new SlotChange();
            Cursor = CurrentPath = null;
        }
    }
}
