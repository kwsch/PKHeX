using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class SlotList : UserControl
    {
        public IReadOnlyList<PictureBox> SlotPictureBoxes => slots;

        private static readonly string[] names = Enum.GetNames(typeof(StorageSlotType));
        private readonly LabelType[] Labels = new LabelType[names.Length];
        private readonly List<PictureBox> slots = new List<PictureBox>();
        private List<StorageSlotOffset> SlotOffsets = new List<StorageSlotOffset>();
        public int SlotCount { get; private set; }
        public SlotChangeManager M { get; set; }

        public SlotList()
        {
            InitializeComponent();
            AddLabels();
        }

        private void AddLabels()
        {
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                Enum.TryParse<StorageSlotType>(name, out var val);
                var label = new LabelType
                {
                    Name = $"L_{name}",
                    Text = name,
                    Type = val,
                    AutoSize = true,
                    Visible = false,
                };
                Labels[i] = label;
                FLP_Slots.Controls.Add(label);
                FLP_Slots.SetFlowBreak(label, true);
            }
        }
        private class LabelType : Label
        {
            public StorageSlotType Type;
        }

        private void SetLabelVisibility()
        {
            foreach (var l in Labels)
            {
                int index = SlotOffsets.FindIndex(z => z.Type == l.Type);
                if (index < 0)
                {
                    l.Visible = false;
                    continue;
                }
                int pos = FLP_Slots.Controls.IndexOf(slots[index]);
                if (pos > FLP_Slots.Controls.IndexOf(l))
                    pos--;
                FLP_Slots.Controls.SetChildIndex(l, pos);
                l.Visible = true;
            }
        }

        public IEnumerable<PictureBox> Initialize(List<StorageSlotOffset> list, Action<Control> enableDragDropContext)
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
                    FLP_Slots.SetFlowBreak(slot, true);
                    yield return slot;
                }
            }
            else
            {
                for (int i = before - 1; i >= after; i--)
                    FLP_Slots.Controls.Remove(slots[i]);
            }
            SetLabelVisibility();
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
