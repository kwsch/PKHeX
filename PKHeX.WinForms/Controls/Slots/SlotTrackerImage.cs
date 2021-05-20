using System.Drawing;

namespace PKHeX.WinForms.Controls
{
    public sealed class SlotTrackerImage
    {
        public Image? OriginalBackground { get; set; }
        public Image? CurrentBackground { get; set; }

        public void Reset()
        {
            OriginalBackground = CurrentBackground = null;
        }
    }
}