using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public sealed class SlotChangeInfo<TCursor, TImageSource>
    {
        public bool LeftMouseIsDown { get; set; }
        public bool DragDropInProgress { get; set; }

        public TCursor Cursor { get; set; }
        public string CurrentPath { get; set; }

        public SlotViewInfo<TImageSource> Source { get; set; }
        public SlotViewInfo<TImageSource> Destination { get; set; }

        public SlotChangeInfo()
        {
            Reset();
        }

        public void Reset()
        {
            LeftMouseIsDown = DragDropInProgress = false;
            CurrentPath = null;
            Cursor = default;
        }

        public bool SameLocation => Source?.Equals(Destination) ?? false;
        public bool DragIsParty => Source?.Slot is SlotInfoParty || Destination?.Slot is SlotInfoParty;
    }
}