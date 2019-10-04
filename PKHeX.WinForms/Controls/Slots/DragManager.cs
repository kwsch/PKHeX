using System.Drawing;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls
{
    public sealed class DragManager
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
}