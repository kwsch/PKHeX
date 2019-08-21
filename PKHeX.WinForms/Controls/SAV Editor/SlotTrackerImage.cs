using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public class SlotTrackerImage : SlotTracker<PictureBox>
    {
        public Image InteractionColor { get; private set; }
        public Image OriginalBackground { get; set; }
        public Image CurrentBackground { get; set; }

        public void Reset()
        {
            Box = Slot = -1;
            OriginalBackground = CurrentBackground = null;
        }

        public void SetInteraction(int box, int slot, Image img)
        {
            Box = box;
            Slot = slot;
            InteractionColor = img;
            OriginalBackground = img;
        }
    }
}