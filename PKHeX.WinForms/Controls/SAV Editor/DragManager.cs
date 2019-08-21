using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public class DragManager
    {
        public SlotChangeInfo<Cursor> Info;
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

        public void Initialize(SaveFile SAV)
        {
            Info = new SlotChangeInfo<Cursor>(SAV);
        }

        public void Reset() => Info.Reset();

        public Point MouseDownPosition { get; set; }
        public bool CanStartDrag => Info.LeftMouseIsDown && !Cursor.Position.Equals(MouseDownPosition);
    }
}