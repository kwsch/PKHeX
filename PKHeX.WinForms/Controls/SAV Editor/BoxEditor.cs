using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class BoxEditor : UserControl, ISlotViewer<PictureBox>
    {
        private SaveFile SAV => M?.SE.SAV;

        public IList<PictureBox> SlotPictureBoxes { get; }
        public int BoxSlotCount { get; }
        public SlotChangeManager M { get; set; }
        public bool FlagIllegal { get; set; }
        public bool CanSetCurrentBox { get; set; }

        public BoxEditor()
        {
            InitializeComponent();
            SlotPictureBoxes = new List<PictureBox>
            {
                bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,
            };
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

        public SlotChange GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            return new SlotChange
            {
                Slot = GetSlot(view),
                Box = ViewIndex,
                Offset = GetSlotOffset(slot),
                Type = StorageSlotType.Box,
                IsPartyFormat = false,
                Editable = true,
                Parent = FindForm(),
            };
        }

        private int GetSlot(PictureBox sender) => SlotPictureBoxes.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);
        public int GetSlotOffset(int box, int slot) => GetOffset(slot, box);
        public int GetSlotOffset(int slot) => GetSlotOffset(CurrentBox, slot);
        public int ViewIndex => CurrentBox;

        public bool ControlsVisible
        {
            get => CB_BoxSelect.Enabled;
            set => CB_BoxSelect.Enabled = CB_BoxSelect.Visible = B_BoxLeft.Visible = B_BoxRight.Visible = value;
        }

        public bool ControlsEnabled
        {
            get => CB_BoxSelect.Enabled;
            set => CB_BoxSelect.Enabled = B_BoxLeft.Enabled = B_BoxRight.Enabled = value;
        }

        public int CurrentBox
        {
            get => CB_BoxSelect.SelectedIndex;
            set
            {
                CB_BoxSelect.SelectedIndex = value;
                if (value < 0)
                    return;
                Editor.LoadBox(value);
            }
        }

        public string CurrentBoxName => CB_BoxSelect.Text;

        public int GetOffset(int slot, int box)
        {
            if (box < 0)
                box = CurrentBox;
            return SAV.GetBoxOffset(box) + (slot * SAV.SIZE_STORED);
        }

        public void Setup(SlotChangeManager m)
        {
            M = m;
            M.Boxes.Add(this);
            FlagIllegal = M.SE.FlagIllegal;
        }

        public void SetSlotFiller(PKM p, int box = -1, int slot = -1, PictureBox pb = null)
        {
            if (pb == null)
                pb = SlotPictureBoxes[slot];

            if (p.Species == 0) // Nothing in slot
            {
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                pb.Visible = true;
                return;
            }
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

            if (M != null && M.LastSlot.Box == box && M.LastSlot.Slot == slot)
                pb.BackgroundImage = M.LastSlot.InteractionColor;
        }

        public void ResetBoxNames(int box = -1)
        {
            if (!SAV.HasBox)
                return;
            if (!SAV.Exportable)
            {
                getBoxNamesDefault();
            }
            else
            {
                try { getBoxNamesFromSave(); }
                catch { getBoxNamesDefault(); }
            }

            if (box < 0 && SAV.CurrentBox < CB_BoxSelect.Items.Count)
                CurrentBox = SAV.CurrentBox; // restore selected box
            else
                CurrentBox = box;

            void getBoxNamesFromSave()
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add(SAV.GetBoxName(i));
            }
            void getBoxNamesDefault()
            {
                CB_BoxSelect.Items.Clear();
                for (int i = 0; i < SAV.BoxCount; i++)
                    CB_BoxSelect.Items.Add($"Box {i+1}");
            }
        }

        public void ResetSlots()
        {
            int box = CurrentBox;
            PAN_Box.BackgroundImage = SAV.WallpaperImage(box);
            M?.Hover.Stop();

            int slot = M?.LastSlot.Box == box ? M.LastSlot.Slot : -1;

            int index = box * SAV.BoxSlotCount;
            for (int i = 0; i < BoxSlotCount; i++)
            {
                var pb = SlotPictureBoxes[i];
                if (i < SAV.BoxSlotCount && index + i < SAV.SlotCount)
                    SetSlotFiller(Editor[i], box, slot, pb);
                else
                    pb.Visible = false;
                pb.BackgroundImage = slot == i ? M?.LastSlot.InteractionColor : null;
            }
        }

        public bool SaveBoxBinary()
        {
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                MsgSaveBoxExportYes + Environment.NewLine +
                string.Format(MsgSaveBoxExportNo, CurrentBoxName, CurrentBox + 1) + Environment.NewLine +
                MsgSaveBoxExportCancel);

            if (dr == DialogResult.Yes)
            {
                var sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = "pcdata.bin" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return false;
                File.WriteAllBytes(sfd.FileName, SAV.PCBinary);
                return true;
            }
            if (dr == DialogResult.No)
            {
                var sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = $"boxdata {CurrentBoxName}.bin" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return false;
                File.WriteAllBytes(sfd.FileName, SAV.GetBoxBinary(CurrentBox));
                return true;
            }
            return false;
        }

        public void ClearEvents()
        {
            B_BoxRight.Click -= ClickBoxRight;
            B_BoxLeft.Click -= ClickBoxLeft;
            CB_BoxSelect.SelectedIndexChanged -= GetBox;
        }

        public void Reset()
        {
            ResetBoxNames();
            ResetSlots();
        }

        private void GetBox(object sender, EventArgs e)
        {
            CurrentBox = CB_BoxSelect.SelectedIndex;
            if (SAV.CurrentBox != CurrentBox && CanSetCurrentBox)
                SAV.CurrentBox = CurrentBox;
            ResetSlots();
            M?.Hover.Stop();
        }

        private void ClickBoxLeft(object sender, EventArgs e) => CurrentBox = Editor.MoveLeft(ModifierKeys == Keys.Control);
        private void ClickBoxRight(object sender, EventArgs e) => CurrentBox = Editor.MoveRight(ModifierKeys == Keys.Control);

        public BoxEdit Editor { get; set; }

        // Drag & Drop Handling
        private void BoxSlot_MouseEnter(object sender, EventArgs e) => M?.MouseEnter(sender, e);
        private void BoxSlot_MouseLeave(object sender, EventArgs e) => M?.MouseLeave(sender, e);
        private void BoxSlot_MouseClick(object sender, MouseEventArgs e) => M?.MouseClick(sender, e);
        private void BoxSlot_MouseUp(object sender, MouseEventArgs e) => M?.MouseUp(sender, e);
        private void BoxSlot_MouseDown(object sender, MouseEventArgs e) => M?.MouseDown(sender, e);
        private void BoxSlot_MouseMove(object sender, MouseEventArgs e) => M?.MouseMove(sender, e);
        private void BoxSlot_DragEnter(object sender, DragEventArgs e) => M?.DragEnter(sender, e);
        private void BoxSlot_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) => M?.QueryContinueDrag(sender, e);
        private void BoxSlot_DragDrop(object sender, DragEventArgs e) => M?.DragDrop(sender, e);

        public void InitializeFromSAV(SaveFile sav)
        {
            Editor = new BoxEdit(sav);
            Editor.LoadBox(sav.CurrentBox);
            ResetBoxNames();   // Display the Box Names
        }
    }
}
