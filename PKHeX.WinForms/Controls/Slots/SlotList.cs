using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public partial class SlotList : UserControl, ISlotViewer<PictureBox>
    {
        private static readonly string[] names = Enum.GetNames(typeof(StorageSlotType));
        private readonly LabelType[] Labels = new LabelType[names.Length];
        private readonly List<PictureBox> slots = new();
        private List<SlotInfoMisc> SlotOffsets = new();
        public int SlotCount { get; private set; }
        public SaveFile SAV { get; set; } = null!;
        public bool FlagIllegal { get; set; }

        public SlotList()
        {
            InitializeComponent();
            AddLabels();
        }

        /// <summary>
        /// Initializes the extra slot viewers with a list of offsets and sets up event handling.
        /// </summary>
        /// <param name="list">Extra slots to show</param>
        /// <param name="enableDragDropContext">Events to set up</param>
        /// <remarks>Uses an object pool for viewers (only generates as needed)</remarks>
        public void Initialize(List<SlotInfoMisc> list, Action<Control> enableDragDropContext)
        {
            SlotOffsets = list;
            LoadSlots(list.Count, enableDragDropContext);
        }

        /// <summary>
        /// Hides all slots from the <see cref="SlotList"/>.
        /// </summary>
        public void HideAllSlots() => LoadSlots(0, _ => { });

        public void NotifySlotOld(ISlotInfo previous)
        {
            if (previous is not SlotInfoMisc m)
                return;
            var index = SlotOffsets.FindIndex(z => m.Equals(z));
            if (index < 0)
                return;
            var pb = slots[index];
            pb.BackgroundImage = null;
        }

        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            if (slot is not SlotInfoMisc m)
                return;
            var index = GetViewIndex(m);
            if (index < 0)
                return;
            var pb = slots[index];
            SlotUtil.UpdateSlot(pb, slot, pkm, SAV, FlagIllegal, type);
        }

        public int GetViewIndex(ISlotInfo info) => SlotOffsets.FindIndex(info.Equals);

        public ISlotInfo GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            return GetSlotData(slot);
        }

        public ISlotInfo GetSlotData(int slot) => SlotOffsets[slot];

        public IList<PictureBox> SlotPictureBoxes => slots;

        public int GetSlot(PictureBox sender)
        {
            var view = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (view == null)
                return -1;
            return slots.IndexOf(view);
        }

        public int GetSlotOffset(int slot) => SlotOffsets[slot].Offset;
        public int ViewIndex { get; set; } = -1;

        private IEnumerable<PictureBox> LoadSlots(int after, Action<Control> enableDragDropContext)
        {
            var generated = new List<PictureBox>();
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
                    generated.Add(slot);
                }
            }
            else
            {
                for (int i = before - 1; i >= after; i--)
                    FLP_Slots.Controls.Remove(slots[i]);
            }
            SetLabelVisibility();
            return generated;
        }

        private void AddSlots(int count)
        {
            for (int i = 0; i < count; i++)
                slots.Add(GetPictureBox(i, SpriteUtil.Spriter));
        }

        private const int PadPixels = 2;

        private static PictureBox GetPictureBox(int index, SpriteBuilder s)
        {
            return new()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = s.Width + 2,
                Height = s.Height + 2,
                AllowDrop = true,
                Margin = new Padding(PadPixels),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Name = $"bpkm{index}",
            };
        }

        private sealed class LabelType : Label
        {
            public StorageSlotType Type;
        }

        private void AddLabels()
        {
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                bool result = Enum.TryParse<StorageSlotType>(name, out var val);
                if (!result)
                    continue;

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
    }
}
