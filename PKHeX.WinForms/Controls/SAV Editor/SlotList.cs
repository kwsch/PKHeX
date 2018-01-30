using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class SlotList : UserControl
    {
        public IReadOnlyList<PictureBox> SlotPictureBoxes => slots;

        private readonly List<PictureBox> slots = new List<PictureBox>();
        private IList<StorageSlotOffset> SlotOffsets;
        public int SlotCount { get; private set; }
        public SlotChangeManager M { get; set; }

        public SlotList()
        {
            InitializeComponent();
        }

        public IEnumerable<PictureBox> Initialize(IList<StorageSlotOffset> list, Action<Control> enableDragDropContext)
        {
            SlotOffsets = list;
            return LoadSlots(list.Count, enableDragDropContext);
        }

        public IEnumerable<PictureBox> LoadSlots(int after, Action<Control> enableDragDropContext)
        {
            int before = SlotCount;
            SlotCount = after;
            int diff = after - before;
            if (diff > 0)
            {
                AddSlots(diff);
                for (int i = before; i < after; i++)
                {
                    var slot = slots[i];
                    enableDragDropContext(slot);
                    FLP_Slots.Controls.Add(slot);
                    yield return slot;
                }
            }
            else
            {
                for (int i = before - 1; i >= after; i--)
                    FLP_Slots.Controls.Remove(slots[i]);
            }
        }

        public int GetSlot(object sender) => slots.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);
        public void AddSlots(int count)
        {
            for (int i = 0; i < count; i++)
                slots.Add(GetPictureBox(i));
        }
        public int GetSlotOffset(int slot) => SlotOffsets[slot].Offset;


        private const int PadPixels = 2;
        private const int SlotWidth = 40;
        private const int SlotHeight = 30;
        private static PictureBox GetPictureBox(int index)
        {
            return new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = SlotWidth + 2,
                Height = SlotHeight + 2,
                AllowDrop = true,
                Margin = new Padding(PadPixels),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Name = $"bpkm{index}",
            };
        }
    }
}
