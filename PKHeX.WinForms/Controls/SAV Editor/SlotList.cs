using System.Collections.Generic;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls
{
    public partial class SlotList : UserControl
    {
        public IReadOnlyList<PictureBox> SlotPictureBoxes => slots;

        private readonly List<PictureBox> slots = new List<PictureBox>();
        private int SlotCount => slots.Count;

        public SlotList()
        {
            InitializeComponent();
        }

        public int GetSlot(object sender) => slots.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);
        public void AddSlots(int count)
        {
            for (int i = SlotCount; i < count; i++)
                slots.Add(GetPictureBox(i));
        }

        private int _slotsperrow;
        public int SlotsPerRow
        {
            get => _slotsperrow;
            set
            {
                _slotsperrow = value;
                Width = value * (SlotWidth + 2 + PadPixels) - PadPixels;
                Height = (SlotCount / value + 1) * (SlotHeight + PadPixels) - PadPixels;
            }
        }

        private const int PadPixels = 2;
        private const int SlotWidth = 30;
        private const int SlotHeight = 40;
        private static PictureBox GetPictureBox(int index)
        {
            return new PictureBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Width = SlotWidth + 2,
                Height = SlotHeight + 2,
                AllowDrop = true,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Name = $"bpkm{index}",
            };
        }
    }
}
