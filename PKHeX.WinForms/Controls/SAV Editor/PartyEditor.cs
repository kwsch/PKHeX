using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public partial class PartyEditor : UserControl, ISlotViewer<PictureBox>
    {
        public IList<PictureBox> SlotPictureBoxes { get; private set; }
        public SaveFile SAV => M?.SE.SAV;

        public int BoxSlotCount { get; private set; }
        public SlotChangeManager M { get; set; }
        public bool FlagIllegal { get; set; }

        public PartyEditor()
        {
            InitializeComponent();
        }

        internal bool InitializeGrid()
        {
            const int width = 2;
            const int height = 3;
            if (PartyPokeGrid.InitializeGrid(width, height, SpriteUtil.Spriter))
            {
                PartyPokeGrid.HorizontallyCenter(this);
                InitializeSlots();
                return true;
            }
            return false;
        }

        private void InitializeSlots()
        {
            SlotPictureBoxes = PartyPokeGrid.Entries;
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
            if (!(previous is SlotInfoParty p))
                return;

            var pb = SlotPictureBoxes[p.Slot];
            pb.BackgroundImage = null;
        }

        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            int index = GetViewIndex(slot);
            if (index < 0)
                return;

            if (type == SlotTouchType.Delete)
            {
                ResetSlots();
                return;
            }

            var pb = SlotPictureBoxes[index];
            SlotUtil.UpdateSlot(pb, slot, pkm, SAV, FlagIllegal, type);
        }

        public int GetViewIndex(ISlotInfo slot)
        {
            if (!(slot is SlotInfoParty p))
                return -1;
            return p.Slot;
        }

        public ISlotInfo GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            return new SlotInfoParty(slot);
        }

        private int GetSlot(PictureBox sender) => SlotPictureBoxes.IndexOf(sender);
        public int ViewIndex => -999;

        public void Setup(SlotChangeManager m)
        {
            M = m;
            FlagIllegal = M.SE.FlagIllegal;
        }

        public void ResetSlots()
        {
            //pokeGrid1.SetBackground(SAV.WallpaperImage(0));
            M.Hover.Stop();

            foreach (var pb in SlotPictureBoxes)
            {
                var slot = (SlotInfoParty) GetSlotData(pb);
                SlotUtil.UpdateSlot(pb, slot, slot.Read(SAV), SAV, FlagIllegal);
            }

            if (M.Env.Slots.Publisher.Previous is SlotInfoParty p)
                SlotPictureBoxes[p.Slot].BackgroundImage = SlotUtil.GetTouchTypeBackground(M.Env.Slots.Publisher.PreviousType);
        }

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

        public bool InitializeFromSAV(SaveFile sav) => InitializeGrid();
    }
}
