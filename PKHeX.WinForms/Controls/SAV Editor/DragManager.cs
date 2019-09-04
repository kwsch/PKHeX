using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public class DragManager
    {
        public SlotChangeInfo<Cursor, PictureBox> Info { get; private set; }
        public event DragEventHandler RequestExternalDragDrop;
        public void RequestDD(object sender, DragEventArgs e) => RequestExternalDragDrop?.Invoke(sender, e);

        public void SetCursor(Form f, Cursor z)
        {
            Info.Cursor = f.Cursor = z;
        }

        public void ResetCursor(Form sender)
        {
            SetCursor(sender, Cursors.Default);
        }

        public void Initialize()
        {
            Info = new SlotChangeInfo<Cursor, PictureBox>();
        }

        public void Reset() => Info.Reset();

        public Point MouseDownPosition { private get; set; }
        public bool CanStartDrag => Info.LeftMouseIsDown && !Cursor.Position.Equals(MouseDownPosition);
    }

    public class SlotChangeInfo<TCursor, TImageSource>
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