using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class BoxEditor : UserControl, ISlotViewer<PictureBox>
    {
        public IList<PictureBox> SlotPictureBoxes { get; private set; }
        public SaveFile SAV => M?.SE.SAV;

        public int BoxSlotCount { get; private set; }
        public SlotChangeManager M { get; set; }
        public bool FlagIllegal { get; set; }
        public bool CanSetCurrentBox { get; set; }

        public BoxEditor()
        {
            InitializeComponent();
        }

        internal bool InitializeGrid()
        {
            var count = SAV.BoxSlotCount;
            var width = count / 5;
            var height = count / width;
            if (!BoxPokeGrid.InitializeGrid(width, height, SpriteUtil.Spriter))
                return false;
            RecenterControls();
            InitializeSlots();
            return true;
        }

        public void RecenterControls()
        {
            if (Width < BoxPokeGrid.Width)
                Width = BoxPokeGrid.Width;
            BoxPokeGrid.HorizontallyCenter(this);
            int p1 = CB_BoxSelect.Location.X;
            CB_BoxSelect.HorizontallyCenter(this);
            int p2 = CB_BoxSelect.Location.X;
            if (p1 == p2)
                return;

            B_BoxLeft.Location = new Point(B_BoxLeft.Location.X + p2 - p1, B_BoxLeft.Location.Y);
            B_BoxRight.Location = new Point(B_BoxRight.Location.X + p2 - p1, B_BoxRight.Location.Y);
        }

        private void InitializeSlots()
        {
            SlotPictureBoxes = BoxPokeGrid.Entries;
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

        public void NotifySlotOld(ISlotInfo previous)
        {
            if (!(previous is SlotInfoBox b) || b.Box != CurrentBox)
                return;

            var pb = SlotPictureBoxes[previous.Slot];
            pb.BackgroundImage = null;
        }

        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            int index = GetViewIndex(slot);
            if (index < 0)
                return;

            var pb = SlotPictureBoxes[index];
            SlotUtil.UpdateSlot(pb, slot, pkm, SAV, FlagIllegal, type);
        }

        public int GetViewIndex(ISlotInfo slot)
        {
            if (!(slot is SlotInfoBox b) || b.Box != CurrentBox)
                return -1;
            return slot.Slot;
        }

        public ISlotInfo GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            return new SlotInfoBox(ViewIndex, slot);
        }

        private int GetSlot(PictureBox sender) => SlotPictureBoxes.IndexOf(sender);
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

        public void Setup(SlotChangeManager m)
        {
            M = m;
            M.Boxes.Add(this);
            FlagIllegal = M.SE.FlagIllegal;
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

            if (box < 0 && (uint)SAV.CurrentBox < CB_BoxSelect.Items.Count)
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
            Editor.Reload();
            int box = CurrentBox;
            BoxPokeGrid.SetBackground(SAV.WallpaperImage(box));
            M.Hover.Stop();

            int index = box * SAV.BoxSlotCount;
            for (int i = 0; i < BoxSlotCount; i++)
            {
                var pb = SlotPictureBoxes[i];
                if (i >= SAV.BoxSlotCount || index + i >= SAV.SlotCount)
                {
                    pb.Visible = false;
                    continue;
                }
                pb.Visible = true;
                SlotUtil.UpdateSlot(pb, (SlotInfoBox) GetSlotData(pb), Editor[i], SAV, FlagIllegal);
            }

            if (M.Env.Slots.Publisher.Previous is SlotInfoBox b && b.Box == CurrentBox)
                SlotPictureBoxes[b.Slot].BackgroundImage = SlotUtil.GetTouchTypeBackground(M.Env.Slots.Publisher.PreviousType);
        }

        public bool SaveBoxBinary()
        {
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                MsgSaveBoxExportYes + Environment.NewLine +
                string.Format(MsgSaveBoxExportNo, CurrentBoxName, CurrentBox + 1) + Environment.NewLine +
                MsgSaveBoxExportCancel);

            if (dr == DialogResult.Yes)
            {
                using var sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = "pcdata.bin" };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return false;
                File.WriteAllBytes(sfd.FileName, SAV.GetPCBinary());
                return true;
            }
            if (dr == DialogResult.No)
            {
                using var sfd = new SaveFileDialog { Filter = "Box Data|*.bin", FileName = $"boxdata {CurrentBoxName}.bin" };
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

        public bool InitializeFromSAV(SaveFile sav)
        {
            Editor = new BoxEdit(sav);
            bool result = InitializeGrid();

            int box = sav.CurrentBox;
            if ((uint)box >= sav.BoxCount)
                box = 0;
            Editor.LoadBox(box);
            ResetBoxNames();   // Display the Box Names
            return result;
        }
    }
}
