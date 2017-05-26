using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Manager class for moving slots.
    /// </summary>
    public class SlotChangeManager
    {
        public readonly SAVEditor SE;
        private SaveFile SAV => SE.SAV;
        private Image OriginalBackground;
        private Image CurrentBackground;
        public SlotChangeInfo DragInfo;
        public readonly List<BoxEditor> Boxes = new List<BoxEditor>();
        public int colorizedbox = -1;
        public int colorizedslot = -1;
        public Image colorizedcolor;

        public SlotChangeManager(SAVEditor se)
        {
            SE = se;
            Reset();
        }
        public void Reset() { DragInfo = new SlotChangeInfo(SAV); colorizedbox = colorizedslot = -1; }
        public bool DragActive => DragInfo.DragDropInProgress || !DragInfo.LeftMouseIsDown;
        public void SetCursor(Cursor z, object sender)
        {
            if (SE != null)
                DragInfo.Cursor = (sender as Control).FindForm().Cursor = z;
        }
        public void MouseEnter(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            if (pb.Image == null)
                return;
            OriginalBackground = pb.BackgroundImage;
            pb.BackgroundImage = CurrentBackground = Resources.slotHover;
            if (!DragActive)
                SetCursor(Cursors.Hand, sender);
        }
        public void MouseLeave(object sender, EventArgs e)
        {
            var pb = (PictureBox)sender;
            if (pb.BackgroundImage != CurrentBackground)
                return;
            pb.BackgroundImage = OriginalBackground;
            if (!DragActive)
                SetCursor(Cursors.Default, sender);
        }
        public void MouseClick(object sender, MouseEventArgs e)
        {
            if (!DragInfo.DragDropInProgress)
                SE.clickSlot(sender, e);
        }
        public void MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DragInfo.LeftMouseIsDown = false;
            if (e.Button == MouseButtons.Right)
                DragInfo.RightMouseIsDown = false;
        }
        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DragInfo.LeftMouseIsDown = true;
            if (e.Button == MouseButtons.Right)
                DragInfo.RightMouseIsDown = true;
        }
        public void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action != DragAction.Cancel && e.Action != DragAction.Drop)
                return;
            DragInfo.LeftMouseIsDown = false;
            DragInfo.RightMouseIsDown = false;
            DragInfo.DragDropInProgress = false;
        }
        public void DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;

            if (DragInfo.DragDropInProgress)
                SetCursor((Cursor)DragInfo.Cursor, sender);
        }
        
        public void HandleMovePKM(PictureBox pb, int slot, int box, bool encrypt)
        {
            // Create a temporary PKM file to perform a drag drop operation.

            // Set flag to prevent re-entering.
            DragInfo.DragDropInProgress = true;

            DragInfo.Source.Parent = pb.Parent;
            DragInfo.Source.Slot = slot;
            DragInfo.Source.Box = box;
            DragInfo.Source.Offset = SE.getPKXOffset(DragInfo.Source.Slot, DragInfo.Source.Box);

            // Prepare Data
            DragInfo.Source.OriginalData = SAV.getData(DragInfo.Source.Offset, SAV.SIZE_STORED);

            // Make a new file name based off the PID
            string newfile = CreateDragDropPKM(pb, box, encrypt, out bool external);
            DragInfo.Reset();
            SetCursor(SE.GetDefaultCursor, pb);

            // Browser apps need time to load data since the file isn't moved to a location on the user's local storage.
            // Tested 10ms -> too quick, 100ms was fine. 500ms should be safe?
            if (!external)
            {
                if (File.Exists(newfile) && DragInfo.CurrentPath == null)
                    File.Delete(newfile);
            }
            else
            {
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    if (File.Exists(newfile) && DragInfo.CurrentPath == null)
                        File.Delete(newfile);
                }).Start();
            }
            if (DragInfo.Source.IsParty || DragInfo.Destination.IsParty)
                SE.setParty();
        }
        private string CreateDragDropPKM(PictureBox pb, int box, bool encrypt, out bool external)
        {
            byte[] dragdata = SAV.decryptPKM(DragInfo.Source.OriginalData);
            Array.Resize(ref dragdata, SAV.SIZE_STORED);
            PKM pkx = SAV.getPKM(dragdata);
            string fn = pkx.FileName; fn = fn.Substring(0, fn.LastIndexOf('.'));
            string filename = $"{fn}{(encrypt ? ".ek" + pkx.Format : "." + pkx.Extension)}";

            // Make File
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                TryMakeDragDropPKM(pb, encrypt, pkx, newfile, out external);
            }
            catch (Exception x)
            {
                WinFormsUtil.Error("Drag & Drop Error", x);
                external = false;
            }

            return newfile;
        }
        private void TryMakeDragDropPKM(PictureBox pb, bool encrypt, PKM pkx, string newfile, out bool external)
        {
            File.WriteAllBytes(newfile, encrypt ? pkx.EncryptedBoxData : pkx.DecryptedBoxData);
            var img = (Bitmap)pb.Image;
            SetCursor(new Cursor(img.GetHicon()), pb);
            pb.Image = null;
            pb.BackgroundImage = Resources.slotDrag;
            // Thread Blocks on DoDragDrop
            DragInfo.CurrentPath = newfile;
            DragDropEffects result = pb.DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            external = !DragInfo.Source.IsValid || result != DragDropEffects.Link;
             if (external || DragInfo.SameSlot || result != DragDropEffects.Link) // not dropped to another box slot, restore img
            {
                pb.Image = img;
                pb.BackgroundImage = OriginalBackground;
            }

            if (result == DragDropEffects.Copy) // viewed in tabs or cloned
            {
                if (!DragInfo.Destination.IsValid) // apply 'view' highlight
                    SetColor(DragInfo.Source.Box, DragInfo.Source.Slot, Resources.slotView);
                external = false;
            }
        }

        private void SetSlotSprite(SlotChange loc, PKM pk, BoxEditor x = null) => (x ?? SE.Box).setSlotFiller(pk, loc.Box, loc.Slot);
        
        public void HandleDropPKM(object sender, DragEventArgs e, bool overwrite, bool clone)
        {
            DragInfo.Destination.Offset = SE.getPKXOffset(DragInfo.Destination.Slot, DragInfo.Destination.Box);
            // Check for In-Dropped files (PKX,SAV,ETC)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Directory.Exists(files[0])) { SE.LoadBoxes(out string _, files[0]); return; }
            if (DragInfo.SameSlot)
            {
                e.Effect = DragDropEffects.Link;
                return;
            }
            if (SAV.getIsSlotLocked(DragInfo.Destination.Box, DragInfo.Destination.Slot))
            {
                DragInfo.Destination.Slot = -1; // Invalidate
                WinFormsUtil.Alert("Unable to set to locked slot.");
                return;
            }
            if (DragInfo.Source.Offset < 0) // external source
            {
                TryLoadFiles(files);
                return;
            }

            TrySetPKMDestination(sender, e, overwrite, clone);

            if (DragInfo.Source.Parent == null) // internal file
                DragInfo.Reset();
        }
        private void TryLoadFiles(string[] files)
        {
            if (files.Length <= 0)
                return;
            string file = files[0];
            FileInfo fi = new FileInfo(file);
            if (!PKX.getIsPKM(fi.Length) && !MysteryGift.getIsMysteryGift(fi.Length))
            {
                SE.ParentForm?.DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { file }), DragDropEffects.Move);
                return;
            }

            byte[] data = File.ReadAllBytes(file);
            MysteryGift mg = MysteryGift.getMysteryGift(data, fi.Extension);
            PKM temp = mg?.convertToPKM(SAV) ?? PKMConverter.getPKMfromBytes(data,
                           prefer: fi.Extension.Length > 0 ? (fi.Extension.Last() - 0x30) & 7 : SAV.Generation);

            PKM pk = PKMConverter.convertToFormat(temp, SAV.PKMType, out string c);
            if (pk == null)
            {
                WinFormsUtil.Error(c);
                Console.WriteLine(c);
                return;
            }

            string[] errata = SAV.IsPKMCompatible(pk);
            if (errata.Length > 0)
            {
                string concat = string.Join(Environment.NewLine, errata);
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, concat, "Continue?"))
                {
                    Console.WriteLine(c);
                    Console.WriteLine(concat);
                    return;
                }
            }

            SetPKM(pk, false, Resources.slotSet);
            Console.WriteLine(c);
        }
        private void TrySetPKMDestination(object sender, DragEventArgs e, bool overwrite, bool clone)
        {
            PKM pkz = GetPKM(true);
            if (DragInfo.Source.IsValid)
                TrySetPKMSource(sender, overwrite, clone);

            // Copy from temp to destination slot.
            SetPKM(pkz, false, null);

            e.Effect = clone ? DragDropEffects.Copy : DragDropEffects.Link;
            SetCursor(SE.GetDefaultCursor, sender);
        }
        private void TrySetPKMSource(object sender, bool overwrite, bool clone)
        {
            if (overwrite && DragInfo.Destination.IsValid) // overwrite delete old slot
            {
                // Clear from slot
                SetPKM(SAV.BlankPKM, true, null);
            }
            else if (!clone && DragInfo.Destination.IsValid)
            {
                // Load data from destination
                PKM pk = ((PictureBox)sender).Image != null
                    ? GetPKM(false)
                    : SAV.BlankPKM;
                
                // Set destination pokemon data to source slot
                SetPKM(pk, true, null);
            }
        }

        public void SetColor(int box, int slot, Image img)
        {
            // Update SubViews
            foreach (var boxview in Boxes)
            {
                if (boxview.CurrentBox != box && boxview.SlotPictureBoxes.Count == boxview.BoxSlotCount) continue;
                var slots = boxview.SlotPictureBoxes;
                for (int i = 0; i < slots.Count; i++)
                    slots[i].BackgroundImage = slot == i ? img : null;
            }
            colorizedbox = box;
            colorizedslot = slot;
            colorizedcolor = img;
        }

        // PKM Get Set
        public PKM GetPKM(bool src) => GetPKM(src ? DragInfo.Source : DragInfo.Destination);
        public PKM GetPKM(SlotChange slot)
        {
            int o = slot.Offset;
            if (o < 0)
                return slot.PKM;
            return slot.IsParty ? SAV.getPartySlot(o) : SAV.getStoredSlot(o);
        }
        public void SetPKM(PKM pk, bool src, Image img) => SetPKM(pk, src ? DragInfo.Source : DragInfo.Destination, src, img);
        public void SetPKM(PKM pk, SlotChange slot, bool src, Image img)
        {
            if (slot.IsParty)
            {
                SetPKMParty(pk, src, slot);
                if (img == Resources.slotDel)
                    slot.Slot = 30 + SAV.PartyCount;
                SetColor(slot.Box, slot.Slot, img ?? Resources.slotSet);
                return;
            }

            int o = slot.Offset;
            SAV.setStoredSlot(pk, o);
            if (slot.Slot >= 30)
            {
                SetSlotSprite(slot, pk);
                return;
            }

            // Update SubViews
            foreach (var boxview in Boxes)
            {
                if (boxview.CurrentBox == slot.Box)
                {
                    Console.WriteLine($"Setting to {boxview.Parent.Name}'s [{boxview.CurrentBox+1:d2}]|{boxview.CurrentBoxName} at Slot {slot.Slot+1}.");
                    SetSlotSprite(slot, pk, boxview);
                }
            }
            SetColor(slot.Box, slot.Slot, img ?? Resources.slotSet);
        }
        private void SetPKMParty(PKM pk, bool src, SlotChange slot)
        {
            int o = slot.Offset;
            if (src)
            {
                if (pk.Species == 0) // Empty Slot
                {
                    SAV.deletePartySlot(slot.Slot - 30);
                    SE.setParty();
                    return;
                }
            }
            else
            {
                if (30 + SAV.PartyCount < slot.Slot)
                {
                    o = SAV.getPartyOffset(SAV.PartyCount);
                    slot.Slot = 30 + SAV.PartyCount;
                }
            }

            if (pk.Stat_HPMax == 0) // Without Stats (Box)
            {
                pk.setStats(pk.getStats(SAV.Personal.getFormeEntry(pk.Species, pk.AltForm)));
                pk.Stat_Level = pk.CurrentLevel;
            }
            SAV.setPartySlot(pk, o);
            SE.setParty();
        }
    }
}
