using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public static class SlotUtil
    {
        public static Image GetTouchTypeBackground(SlotTouchType t)
        {
            switch (t)
            {
                case SlotTouchType.None: return Resources.slotTrans;
                case SlotTouchType.Get: return Resources.slotView;
                case SlotTouchType.Set: return Resources.slotSet;
                case SlotTouchType.Delete: return Resources.slotDel;
                case SlotTouchType.Swap: return Resources.slotSet;
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }

        public static DropModifier GetDropModifier()
        {
            switch (Control.ModifierKeys)
            {
                case Keys.Shift: return DropModifier.Clone;
                case Keys.Alt: return DropModifier.Overwrite;
                default: return DropModifier.None;
            }
        }
    }
}