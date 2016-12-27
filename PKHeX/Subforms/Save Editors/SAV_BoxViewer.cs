using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static PKHeX.Main;

namespace PKHeX
{
    public partial class SAV_BoxViewer : Form
    {
        public SAV_BoxViewer(Main p)
        {
            InitializeComponent();
            parent = p;
            CenterToParent();

            AllowDrop = true;
            DragEnter += tabMain_DragEnter;
            DragDrop += (sender, e) =>
            {
                Cursor = DefaultCursor;
                System.Media.SystemSounds.Asterisk.Play();
            };

            SlotPictureBoxes = new[] {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,
                                };
            foreach (PictureBox pb in SlotPictureBoxes)
            {
                pb.AllowDrop = true;
                pb.GiveFeedback += (sender, e) => { e.UseDefaultCursors = false; };
                pb.MouseUp += pbBoxSlot_MouseUp;
                pb.MouseDown += pbBoxSlot_MouseDown;
                pb.MouseMove += pbBoxSlot_MouseMove;
                pb.DragDrop += pbBoxSlot_DragDrop;
                pb.DragEnter += pbBoxSlot_DragEnter;
                pb.QueryContinueDrag += pbBoxSlot_QueryContinueDrag;
            }
            for (int i = SAV.BoxSlotCount; i < SlotPictureBoxes.Length; i++)
                SlotPictureBoxes[i].Visible = false;

            try
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add(SAV.getBoxName(i));
            }
            catch
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 1; i <= SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add($"BOX {i}");
            }
            CB_BoxSelect.SelectedIndex = 0;
        }
        private readonly Main parent;
        private readonly PictureBox[] SlotPictureBoxes;
        public int CurrentBox => CB_BoxSelect.SelectedIndex;

        private void CB_BoxSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            setPKXBoxes();
        }
        private void clickBoxRight(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + 1) % SAV.BoxCount;
        }
        private void clickBoxLeft(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = (CB_BoxSelect.SelectedIndex + SAV.BoxCount - 1) % SAV.BoxCount;
        }
        private void PB_BoxSwap_Click(object sender, EventArgs e)
        {
            CB_BoxSelect.SelectedIndex = parent.swapBoxesViewer(CB_BoxSelect.SelectedIndex);
        }

        // Copied Methods from Main.cs (slightly modified)
        private int getPKXOffset(int slot)
        {
            return SAV.getBoxOffset(CB_BoxSelect.SelectedIndex) + slot * SAV.SIZE_STORED;
        }
        public void setPKXBoxes()
        {
            int boxoffset = SAV.getBoxOffset(CB_BoxSelect.SelectedIndex);
            int boxbgval = SAV.getBoxWallpaper(CB_BoxSelect.SelectedIndex);
            PAN_Box.BackgroundImage = BoxWallpaper.getWallpaper(SAV, boxbgval);

            for (int i = 0; i < SAV.BoxSlotCount; i++)
                getSlotFiller(boxoffset + SAV.SIZE_STORED*i, SlotPictureBoxes[i]);
        }
        private void getSlotFiller(int offset, PictureBox pb)
        {
            if (SAV.getData(offset, SAV.SIZE_STORED).SequenceEqual(new byte[SAV.SIZE_STORED]))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            PKM p = SAV.getStoredSlot(offset);
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }
            // Something stored in slot. Only display if species is valid.
            var sprite = p.Species != 0 ? p.Sprite : null;
            int slot = getSlot(pb);
            bool locked = slot < 30 && SAV.getIsSlotLocked(CB_BoxSelect.SelectedIndex, slot);
            bool team = slot < 30 && SAV.getIsTeamSet(CB_BoxSelect.SelectedIndex, slot);
            if (locked)
                sprite = Util.LayerImage(sprite, Properties.Resources.locked, 26, 0, 1);
            else if (team)
                sprite = Util.LayerImage(sprite, Properties.Resources.team, 21, 0, 1);
            pb.Image = sprite;
            pb.BackColor = Color.Transparent;
        }
        private void getQuickFiller(PictureBox pb, PKM pk)
        {
            var sprite = pk.Species != 0 ? pk.Sprite : null;
            int slot = getSlot(pb);
            bool locked = slot < 30 && SAV.getIsSlotLocked(CB_BoxSelect.SelectedIndex, slot);
            bool team = slot < 30 && SAV.getIsTeamSet(CB_BoxSelect.SelectedIndex, slot);
            if (locked)
                sprite = Util.LayerImage(sprite, Properties.Resources.locked, 26, 0, 1);
            else if (team)
                sprite = Util.LayerImage(sprite, Properties.Resources.team, 21, 0, 1);
            pb.Image = sprite;
            if (pb.BackColor == Color.Red)
                pb.BackColor = Color.Transparent;
        }
        private int getSlot(object sender)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            return Array.IndexOf(SlotPictureBoxes, sender);
        }
        
        // Drag and drop related functions
        private void pbBoxSlot_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DragInfo.slotLeftMouseIsDown = false;
            if (e.Button == MouseButtons.Right)
                DragInfo.slotRightMouseIsDown = false;
        }
        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DragInfo.slotLeftMouseIsDown = true;
            if (e.Button == MouseButtons.Right)
                DragInfo.slotRightMouseIsDown = true;
        }
        private void pbBoxSlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragInfo.slotDragDropInProgress)
                return;

            if (DragInfo.slotLeftMouseIsDown)
            {
                // The goal is to create a temporary PKX file for the underlying Pokemon
                // and use that file to perform a drag drop operation.

                // Abort if there is no Pokemon in the given slot.
                PictureBox pb = (PictureBox)sender;
                if (pb.Image == null)
                    return;

                int slot = getSlot(pb);
                int box = slot >= 30 ? -1 : CB_BoxSelect.SelectedIndex;
                if (SAV.getIsSlotLocked(box, slot))
                    return;

                // Set flag to prevent re-entering.
                DragInfo.slotDragDropInProgress = true;

                DragInfo.slotSource = this;
                DragInfo.slotSourceSlotNumber = slot;
                DragInfo.slotSourceBoxNumber = box;
                DragInfo.slotSourceOffset = getPKXOffset(DragInfo.slotSourceSlotNumber);

                // Prepare Data
                DragInfo.slotPkmSource = SAV.getData(DragInfo.slotSourceOffset, SAV.SIZE_STORED);

                // Make a new file name based off the PID
                byte[] dragdata = SAV.decryptPKM(DragInfo.slotPkmSource);
                Array.Resize(ref dragdata, SAV.SIZE_STORED);
                PKM pkx = SAV.getPKM(dragdata);
                string filename = pkx.FileName;

                // Make File
                string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);
                    var img = (Bitmap)pb.Image;
                    DragInfo.Cursor = Cursor.Current = new Cursor(img.GetHicon());
                    pb.Image = null;
                    pb.BackgroundImage = Properties.Resources.slotDrag;
                    // Thread Blocks on DoDragDrop
                    DragInfo.CurrentPath = newfile;
                    DragDropEffects result = pb.DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
                    if (!DragInfo.SourceValid || result != DragDropEffects.Link) // not dropped to another box slot, restore img
                        pb.Image = img;
                    else // refresh image
                        getQuickFiller(pb, SAV.getStoredSlot(DragInfo.slotSourceOffset));
                    pb.BackgroundImage = null;

                    if (DragInfo.SameBox && DragInfo.DestinationValid)
                        SlotPictureBoxes[DragInfo.slotDestinationSlotNumber].Image = img;
                }
                catch (Exception x)
                {
                    Util.Error("Drag & Drop Error", x);
                }
                parent.notifyBoxViewerRefresh();
                DragInfo.Reset();
                Cursor = DefaultCursor;

                // Browser apps need time to load data since the file isn't moved to a location on the user's local storage.
                // Tested 10ms -> too quick, 100ms was fine. 500ms should be safe?
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    if (File.Exists(newfile) && DragInfo.CurrentPath == null)
                        File.Delete(newfile);
                }).Start();
            }
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            DragInfo.slotDestination = this;
            DragInfo.slotDestinationSlotNumber = getSlot(sender);
            DragInfo.slotDestinationOffset = getPKXOffset(DragInfo.slotDestinationSlotNumber);
            DragInfo.slotDestinationBoxNumber = CB_BoxSelect.SelectedIndex;

            // Check for In-Dropped files (PKX,SAV,ETC)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Directory.Exists(files[0])) { return; }
            if (SAV.getIsSlotLocked(DragInfo.slotDestinationBoxNumber, DragInfo.slotDestinationSlotNumber))
            {
                DragInfo.slotDestinationSlotNumber = -1; // Invalidate
                Util.Alert("Unable to set to locked slot.");
                return;
            }
            if (DragInfo.slotSourceOffset < 0) // file
            {
                if (files.Length <= 0)
                    return;
                string file = files[0];
                FileInfo fi = new FileInfo(file);
                if (!PKX.getIsPKM(fi.Length) && !MysteryGift.getIsMysteryGift(fi.Length))
                { return; }

                byte[] data = File.ReadAllBytes(file);
                MysteryGift mg = MysteryGift.getMysteryGift(data, fi.Extension);
                PKM temp = mg != null ? mg.convertToPKM(SAV) : PKMConverter.getPKMfromBytes(data);
                string c;

                PKM pk = PKMConverter.convertToFormat(temp, SAV.PKMType, out c);
                if (pk == null)
                { Util.Error(c); Console.WriteLine(c); return; }

                string[] errata = verifyPKMtoSAV(pk);
                if (errata.Length > 0)
                {
                    string concat = string.Join(Environment.NewLine, errata);
                    if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, concat, "Continue?"))
                    { Console.WriteLine(c); Console.WriteLine(concat); return; }
                }

                DragInfo.setPKMtoDestination(SAV, pk);
                getQuickFiller(SlotPictureBoxes[DragInfo.slotDestinationSlotNumber], pk);
                Console.WriteLine(c);
            }
            else
            {
                PKM pkz = DragInfo.getPKMfromSource(SAV);
                if (!DragInfo.SourceValid) { } // not overwritable, do nothing
                else if (ModifierKeys == Keys.Alt && DragInfo.DestinationValid) // overwrite
                {
                    // Clear from slot
                    if (DragInfo.SameBox)
                        getQuickFiller(SlotPictureBoxes[DragInfo.slotSourceSlotNumber], SAV.BlankPKM); // picturebox

                    DragInfo.setPKMtoSource(SAV, SAV.BlankPKM);
                }
                else if (ModifierKeys != Keys.Control && DragInfo.DestinationValid) // move
                {
                    // Load data from destination
                    PKM pk = ((PictureBox)sender).Image != null
                        ? DragInfo.getPKMfromDestination(SAV)
                        : SAV.BlankPKM;

                    // Set destination pokemon image to source picture box
                    if (DragInfo.SameBox)
                        getQuickFiller(SlotPictureBoxes[DragInfo.slotSourceSlotNumber], pk);

                    // Set destination pokemon data to source slot
                    DragInfo.setPKMtoSource(SAV, pk);
                }
                else if (DragInfo.SameBox) // clone
                    getQuickFiller(SlotPictureBoxes[DragInfo.slotSourceSlotNumber], pkz);

                // Copy from temp to destination slot.
                DragInfo.setPKMtoDestination(SAV, pkz);
                getQuickFiller(SlotPictureBoxes[DragInfo.slotDestinationSlotNumber], pkz);

                e.Effect = DragDropEffects.Link;
                Cursor = DefaultCursor;
            }
            if (DragInfo.SourceParty || DragInfo.DestinationParty)
                parent.setParty();
            if (DragInfo.slotSource == null) // another instance or file
            {
                parent.notifyBoxViewerRefresh();
                DragInfo.Reset();
            }
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;

            if (DragInfo.slotDragDropInProgress)
                Cursor = DragInfo.Cursor;
        }
        private void pbBoxSlot_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
            {
                DragInfo.slotLeftMouseIsDown = false;
                DragInfo.slotRightMouseIsDown = false;
                DragInfo.slotDragDropInProgress = false;
            }
        }

        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
    }
}
