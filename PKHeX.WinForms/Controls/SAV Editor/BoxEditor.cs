using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class BoxEditor : UserControl
    {
        private SaveFile SAV => M?.SE.SAV;
        public List<PictureBox> SlotPictureBoxes { get; }
        public int BoxSlotCount { get; }
        public SlotChangeManager M { get; set; }
        public bool FlagIllegal { get; set; }

        public BoxEditor()
        {
            InitializeComponent();
            SlotPictureBoxes = new List<PictureBox>();
            SlotPictureBoxes.AddRange(new[]
            {
                bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,
            });
            BoxSlotCount = SlotPictureBoxes.Count;
            foreach (var pb in SlotPictureBoxes)
            {
                pb.MouseEnter += BoxSlot_MouseEnter;
                pb.MouseLeave += BoxSlot_MouseLeave;
                pb.MouseClick += BoxSlot_MouseClick;
                pb.MouseMove += BoxSlot_MouseMove;
                pb.MouseDown += BoxSlot_MouseDown;
                pb.MouseUp += BoxSlot_MouseUp;

                pb.DragEnter += BoxSlot_DragEnter;
                pb.DragDrop += BoxSlot_DragDrop;
                pb.QueryContinueDrag += BoxSlot_QueryContinueDrag;
                pb.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
                pb.AllowDrop = true;
            }
        }

        public int CurrentBox
        {
            get => CB_BoxSelect.SelectedIndex;
            set => CB_BoxSelect.SelectedIndex = value;
        }
        public string CurrentBoxName => CB_BoxSelect.Text;
        public int GetOffset(int slot, int box)
        {
            if (box < 0)
                box = CurrentBox;
            return SAV.GetBoxOffset(box) + slot * SAV.SIZE_STORED;
        }
        public void Setup(SlotChangeManager m)
        {
            M = m;
            M.Boxes.Add(this);
            FlagIllegal = M.SE.FlagIllegal;
            Reset();
        }
        public void SetSlotFiller(PKM p, int box = -1, int slot = -1, PictureBox pb = null)
        {
            if (pb == null)
                pb = SlotPictureBoxes[slot];
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                pb.Visible = true;
                return;
            }

            pb.Image = p.Sprite(SAV, box, slot, FlagIllegal);
            pb.BackColor = Color.Transparent;
            pb.Visible = true;

            if (M != null && M.colorizedbox == box && M.colorizedslot == slot)
                pb.BackgroundImage = M.colorizedcolor;
        }

        public void ResetBoxNames()
        {
            if (!SAV.HasBox)
                return;
            // Build ComboBox Dropdown Items
            try
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add(SAV.GetBoxName(i));
            }
            catch
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 1; i <= SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add($"BOX {i}");
            }
            if (SAV.CurrentBox < CB_BoxSelect.Items.Count)
                CurrentBox = SAV.CurrentBox; // restore selected box
        }
        public void ResetSlots()
        {
            int box = CurrentBox;
            int boxoffset = SAV.GetBoxOffset(box);
            int boxbgval = SAV.GetBoxWallpaper(box);
            PAN_Box.BackgroundImage = SAV.WallpaperImage(boxbgval);

            int slot = M?.colorizedbox == box ? M.colorizedslot : -1;

            for (int i = 0; i < BoxSlotCount; i++)
            {
                var pb = SlotPictureBoxes[i];
                if (i < SAV.BoxSlotCount)
                    GetSlotFiller(boxoffset + SAV.SIZE_STORED * i, pb, box, i);
                else
                    pb.Visible = false;
                pb.BackgroundImage = slot == i ? M?.colorizedcolor : null;
            }
        }
        public bool SaveBoxBinary()
        {
            DialogResult dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                "Yes: Export All Boxes" + Environment.NewLine +
                $"No: Export {CurrentBoxName} (Box {CurrentBox + 1})" + Environment.NewLine +
                "Cancel: Abort");

            if (dr == DialogResult.Yes)
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = "pcdata.bin" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return false;
                File.WriteAllBytes(sfd.FileName, SAV.PCBinary);
                return true;
            }
            if (dr == DialogResult.No)
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = $"boxdata {CurrentBoxName}.bin" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return false;
                File.WriteAllBytes(sfd.FileName, SAV.GetBoxBinary(CurrentBox));
                return true;
            }
            return false;
        }

        public int GetSlot(object sender) => SlotPictureBoxes.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);

        private void Reset()
        {
            ResetBoxNames();
            ResetSlots();
        }
        private void GetBox(object sender, EventArgs e)
        {
            if (SAV.CurrentBox != CurrentBox)
                SAV.CurrentBox = CurrentBox;
            ResetSlots();
        }
        private void ClickBoxLeft(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                CurrentBox = 0;
            else
                CurrentBox = (CurrentBox + SAV.BoxCount - 1) % SAV.BoxCount;
        }
        private void ClickBoxRight(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
                CurrentBox = SAV.BoxCount - 1;
            else
                CurrentBox = (CurrentBox + 1) % SAV.BoxCount;
        }
        private void GetSlotFiller(int offset, PictureBox pb, int box = -1, int slot = -1)
        {
            if (SAV.GetData(offset, SAV.SIZE_STORED).SequenceEqual(new byte[SAV.SIZE_STORED]))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                pb.Visible = true;
                return;
            }
            PKM p = SAV.GetStoredSlot(offset);
            SetSlotFiller(p, box, slot, pb);
        }

        // Drag & Drop Handling
        private void BoxSlot_MouseEnter(object sender, EventArgs e) => M?.MouseEnter(sender, e);
        private void BoxSlot_MouseLeave(object sender, EventArgs e) => M?.MouseLeave(sender, e);
        private void BoxSlot_MouseClick(object sender, MouseEventArgs e) => M?.MouseClick(sender, e);
        private void BoxSlot_MouseUp(object sender, MouseEventArgs e) => M?.MouseUp(sender, e);
        private void BoxSlot_MouseDown(object sender, MouseEventArgs e) => M?.MouseDown(sender, e);
        private void BoxSlot_DragEnter(object sender, DragEventArgs e) => M?.DragEnter(sender, e);
        private void BoxSlot_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) => M?.QueryContinueDrag(sender, e);
        private void BoxSlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (M == null || M.DragActive)
                return;

            // Abort if there is no Pokemon in the given slot.
            PictureBox pb = (PictureBox)sender;
            if (pb.Image == null)
                return;
            int slot = GetSlot(pb);
            int box = slot >= 30 ? -1 : CurrentBox;
            if (SAV.IsSlotLocked(box, slot))
                return;

            bool encrypt = ModifierKeys == Keys.Control;
            M.HandleMovePKM(pb, slot, box, encrypt);
        }
        private void BoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            if (M == null)
                return;

            // Abort if there is no Pokemon in the given slot.
            PictureBox pb = (PictureBox)sender;
            int slot = GetSlot(pb);
            int box = slot >= 30 ? -1 : CurrentBox;
            if (SAV.IsSlotLocked(box, slot) || slot >= 36)
            {
                SystemSounds.Asterisk.Play();
                e.Effect = DragDropEffects.Copy;
                M.DragInfo.Reset();
                return;
            }

            bool overwrite = ModifierKeys == Keys.Alt;
            bool clone = ModifierKeys == Keys.Control;
            M.DragInfo.Destination.Parent = FindForm();
            M.DragInfo.Destination.Slot = GetSlot(sender);
            M.DragInfo.Destination.Box = M.DragInfo.Destination.IsParty ? -1 : CurrentBox;
            M.HandleDropPKM(sender, e, overwrite, clone);
        }
    }
}
