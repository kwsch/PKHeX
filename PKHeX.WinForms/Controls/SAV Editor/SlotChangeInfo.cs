using PKHeX.Core;

namespace PKHeX.WinForms
{
    public class SlotChangeInfo
    {
        public bool LeftMouseIsDown;
        public bool RightMouseIsDown;
        public bool DragDropInProgress;

        public object Cursor;
        public string CurrentPath;

        public SlotChange Source;
        public SlotChange Destination;

        public readonly byte[] BlankData;

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
