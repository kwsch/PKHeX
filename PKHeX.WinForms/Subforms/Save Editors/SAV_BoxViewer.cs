using System;
using System.Windows.Forms;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public sealed partial class SAV_BoxViewer : Form
    {
        private readonly SAVEditor parent;

        public SAV_BoxViewer(SAVEditor p, SlotChangeManager m)
        {
            parent = p;
            InitializeComponent();
            Box.Setup(m);
            Box.Reset();
            CenterToParent();

            AllowDrop = true;
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            DragEnter += Main_DragEnter;
            DragDrop += (sender, e) =>
            {
                Cursor = DefaultCursor;
                System.Media.SystemSounds.Asterisk.Play();
            };
            Owner = p.ParentForm;

            MouseWheel += (s, e) =>
            {
                if (parent.menu.mnuVSD.Visible)
                    return;
                if (e.Delta > 1)
                    Box.MoveLeft();
                else
                    Box.MoveRight();
            };

            foreach (PictureBox pb in Box.SlotPictureBoxes)
                pb.ContextMenuStrip = parent.SlotPictureBoxes[0].ContextMenuStrip;
            Box.ResetBoxNames(); // fix box names
            Box.ResetSlots(); // refresh box background
        }

        public int CurrentBox => Box.CurrentBox;
        private void PB_BoxSwap_Click(object sender, EventArgs e) => Box.CurrentBox = parent.SwapBoxesViewer(Box.CurrentBox);
        public void SetPKMBoxes() => Box.ResetSlots();

        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }

        private void SAV_BoxViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Remove viewer from manager list
            Box.M.Boxes.Remove(Box);
        }
    }
}
